#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ClosedXML;
using ClosedXML.Excel;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace CatHut
{
    public class ScriptableObjectGenerator
    {

        [MenuItem("Tools/CatHut/MasterDataEditor/ReCreate", false, 1)]
        private static void ReCereateScriptableObjectAndImporter()
        {
            CleanUsingExcelScripts();

            CreateUsingExcelScripts();
        }


        [MenuItem("Tools/CatHut/MasterDataEditor/Create Using Excel Scripts", false, 2)]
        private static void CreateUsingExcelScripts()
        {
            //エクセルをインポートするフォルダリスト取得
            var ExcelFolderList = UsingExcelCommon.GetExcelFolderList();

            //ScriptableObjectを生成するエクセルファイルリスト取得
            var ExcelFileList = UsingExcelCommon.GetMasterDataExcelList();

            //対象のファイルを諸々条件つけて整形（重複等対応）
            var CreateScriptableObjectList = UsingExcelCommon.GetMasterDataExcelPath(ExcelFolderList, ExcelFileList);

            var ExcelDataDic = new Dictionary<string, ExcelDataDictionary>();

            //ここでScriptableobjectを作成
            foreach (var path in CreateScriptableObjectList)
            {
                var dic = AnalysisExcelFile(path.Value);
                ExcelDataDic.Add(path.Value, dic);
            }

            //ScriptableObject出力
            CreateScriptableObject(ExcelDataDic);

            //AssetPostProcessImporter
            CreateAssetPostProcessImporter();

            //ExcelImporterParent
            CreateExcelImporterParent();

            //Importer_partの出力
            CreateExcelImporterPart(ExcelDataDic);

            //Importerの出力
            CreateExcelImporter(ExcelDataDic);

            //MasterDataClassの出力
            CreateMasterDataClass(ExcelDataDic);

            //CustomTreeView出力
            CreateCustomTreeView(ExcelDataDic);

            //MasterDataEditor出力
            CreateEditorWindow(ExcelDataDic);

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/CatHut/MasterDataEditor/Clean", false, 3)]
        private static void CleanUsingExcelScripts()
        {
            //MasterDataEditor削除
            DeleteMasterDataEditor();

            //CreatedScriptableObject削除
            DeleteScriptableObject();

            //Importerの削除
            DeleteExcelImporter();

            AssetDatabase.Refresh();
        }


        [MenuItem("Tools/CatHut/MasterDataEditor/Settings/Open ImportFolderList", false, 4)]
        private static void OpenImportFolderList()
        {
            var temp = UsingExcelCommon.GetFileList(UsingExcelCommon.ImportFolderListFile);

            if (temp.Count <= 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("注意！", "設定ファイルがありません", "OK");
            }

            if (temp.Count > 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("注意！", "2つ以上のファイルが見つかりました。１つ目を開きます", "OK");
            }

            var o = AssetDatabase.LoadAssetAtPath(temp[0], typeof(UnityEngine.Object)) as UnityEngine.Object;
            if (o != null)
            {
                AssetDatabase.OpenAsset(o);
            }

        }

        [MenuItem("Tools/CatHut/MasterDataEditor/Settings/Open MasterDataExcelList", false, 5)]
        private static void OpenMasterDataExcelList()
        {
            var temp = UsingExcelCommon.GetFileList(UsingExcelCommon.MasterDataExcelListFile);

            if (temp.Count <= 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("注意！", "設定ファイルがありません", "OK");
            }

            if (temp.Count > 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("注意！", "2つ以上のファイルが見つかりました。１つ目を開きます", "OK");
            }

            var o = AssetDatabase.LoadAssetAtPath(temp[0], typeof(UnityEngine.Object)) as UnityEngine.Object;
            if (o != null)
            {
                // ファイルを開く(Visual Studioでファイルが開く)
                AssetDatabase.OpenAsset(o);
            }

        }

        private static void CreateMasterDataClass(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //MasterDataのメンバ宣言文字列を作成
            var MasterDataClassDeclareStr = GetMasterDataClassDeclareStr(ExcelDataDic);

            //MasterDataのAssetロード処理の文字列作成
            var MasterDataClassLoadStr = GetMasterDataClassLosdStr(ExcelDataDic);

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.MasterDataTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);


            FileStr = FileStr.Replace("#MasterDataClassDeclare#", MasterDataClassDeclareStr);
            FileStr = FileStr.Replace("#MasterDataClassLoad#", MasterDataClassLoadStr);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedScriptableObject");
            File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.MasterDataTemplate + ".txt", "MasterData.cs"), FileStr, Encoding.UTF8);

        }

        private static void CreateEditorWindow(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.EditorWindowTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);


            //EditorWindowの定義ファイルを作成する
            foreach (var temp in ExcelDataDic)
            {

                var FileName = Path.GetFileNameWithoutExtension(temp.Key);

                foreach (var sheetInfo in temp.Value.ExcelSheetDic.Values)
                {
                    var FileStr = File.ReadAllText(TemplateFile);

                    FileStr = FileStr.Replace("#SheetName#", sheetInfo.SheetName);
                    FileStr = FileStr.Replace("#FileName#", FileName);
                    FileStr = FileStr.Replace("#MasterDataClassName#", sheetInfo.ClassName);

                    var SaveFile = TemplateFile.Replace("/Template", "/CreatedScript");
                    File.WriteAllText(SaveFile.Replace(UsingExcelCommon.EditorWindowTemplate + ".txt", "EditorWindow_" + sheetInfo.ClassName + ".cs"), FileStr, Encoding.UTF8);

                }
            }
        }

        private static void CreateCustomTreeView(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.CustomTreeViewTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var EnumCaseStr = "";
            var cnt = 0;

            //EditorWindowの定義ファイルを作成する
            foreach (var temp in ExcelDataDic)
            {
                var FileName = Path.GetFileNameWithoutExtension(temp.Key);

                foreach (var enumKey in temp.Value.EnumDic.Keys)
                {
                    var enumName = UsingExcelCommon.GetEnumTypeName(enumKey);

                    EnumCaseStr += "                case " + FileName + "." + enumName + " " + "enum" + cnt.ToString("00") + ":" + Environment.NewLine;
                    EnumCaseStr += "                    {" + Environment.NewLine;
                    EnumCaseStr += "                        tempDic[key][column] = (" + FileName + "." + enumName + ")EditorGUI.EnumPopup(cellRect, enum" + cnt.ToString("00") + ");" + Environment.NewLine;
                    EnumCaseStr += "                    }" + Environment.NewLine;
                    EnumCaseStr += "                    break;" + Environment.NewLine;
                    cnt++;
                }
            }

            var FileStr = File.ReadAllText(TemplateFile);

            FileStr = FileStr.Replace("#EnumCaseText#", EnumCaseStr);

            var SaveFile = TemplateFile.Replace("/Template", "/CreatedScript");
            File.WriteAllText(SaveFile.Replace(UsingExcelCommon.CustomTreeViewTemplate + ".txt", "CustomTreeView.cs"), FileStr, Encoding.UTF8);

        }


        private static void CreateExcelImporterParent()
        {

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterParentTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedImporter");

            var CreatedImporterFolder = Path.GetDirectoryName(TemplateFile);
            if (!Directory.Exists(CreatedImporterFolder))
            {
                Directory.CreateDirectory(CreatedImporterFolder);
            }
            File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.ExcelImporterParentTemplate + ".txt", "ExcelImporter.cs"), FileStr, Encoding.UTF8);

        }

        private static void CreateAssetPostProcessImporter()
        {

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.AssetPostProcessImporterTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedImporter");

            var CreatedImporterFolder = Path.GetDirectoryName(TemplateFile);
            if (!Directory.Exists(CreatedImporterFolder))
            {
                Directory.CreateDirectory(CreatedImporterFolder);
            }
            File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.AssetPostProcessImporterTemplate + ".txt", "AssetPostProcessImporter.cs"), FileStr, Encoding.UTF8);

        }

        private static void CreateExcelImporterPart(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //Importerの呼び出し元のスクリプトファイルを作成する
            var SwitchCaseListStr = GetSwitchCaseListStr(ExcelDataDic);

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterPartTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);

            FileStr = FileStr.Replace("#SwitchCaseList#", SwitchCaseListStr);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedImporter");
            File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.ExcelImporterPartTemplate + ".txt", "ExcelImporter_part.cs"), FileStr, Encoding.UTF8);

        }

        private static void CreateExcelImporter(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //ScriptableOjbectの定義ファイルを作成する
            foreach (var temp in ExcelDataDic)
            {

                var FileName = Path.GetFileNameWithoutExtension(temp.Key);


                var ClassDataDefineStr = GetClassDataDefineStr(temp.Value.ExcelSheetDic);
                var ExcelDataReadStr = GetExcelDataReadStr(temp.Value.ExcelSheetDic, FileName);
                var ClassDataSetStr = GetClassDataSetStr(temp.Value.ExcelSheetDic);
                var SaveAssetStr = GetSaveAssetStr(temp.Key);


                //テンプレートファイルを探す
                var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterTemplate);
                var TemplateFile = "";

                TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

                var FileStr = File.ReadAllText(TemplateFile);

                FileStr = FileStr.Replace("#FileName#", FileName);
                FileStr = FileStr.Replace("#ClassDataDefine#", ClassDataDefineStr);
                FileStr = FileStr.Replace("#ExcelDataRead#", ExcelDataReadStr);
                FileStr = FileStr.Replace("#ClassDataSet#", ClassDataSetStr);
                FileStr = FileStr.Replace("#ClassDataSave#", SaveAssetStr);

                TemplateFile = TemplateFile.Replace("/Template", "/CreatedImporter");
                File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.ExcelImporterTemplate + ".txt", "Importer_" + FileName + ".cs"), FileStr, Encoding.UTF8);

            }

        }

        private static void CreateScriptableObject(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            //ScriptableOjbectの定義ファイルを作成する
            foreach (var temp in ExcelDataDic)
            {

                var FileName = Path.GetFileNameWithoutExtension(temp.Key);

                var EnmuDeclareStr = GetEnumDeclareStr(temp.Value);
                var ClassDeclareStr = GetClassDeclareStr(temp.Value, FileName);
                var ClassProtoTypeDeclareStr = GetClsssProtoTypeDeclareStr(temp.Value.ExcelSheetDic);
                var ClassDefinitionsStr = GetClassDefinitionsStr(temp.Value.ExcelSheetDic, FileName);


                //テンプレートファイルを探す
                var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.DictNotDublicatableTemplate);
                var TemplateFile = "";

                TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

                var FileStr = File.ReadAllText(TemplateFile);
                FileStr = FileStr.Replace("#FileName#", FileName);
                FileStr = FileStr.Replace("#EnumDeclare#", EnmuDeclareStr);
                FileStr = FileStr.Replace("#ClsssDeclare#", ClassDeclareStr);
                FileStr = FileStr.Replace("#ClsssProtoTypeDeclare#", ClassProtoTypeDeclareStr);
                FileStr = FileStr.Replace("#ClassDefinitions#", ClassDefinitionsStr);

                TemplateFile = TemplateFile.Replace("/Template", "/CreatedScriptableObject");

                var CreatedScriptableObjectFolder = Path.GetDirectoryName(TemplateFile);
                if (!Directory.Exists(CreatedScriptableObjectFolder))
                {
                    Directory.CreateDirectory(CreatedScriptableObjectFolder);
                }
                File.WriteAllText(TemplateFile.Replace(UsingExcelCommon.DictNotDublicatableTemplate + ".txt", FileName + ".cs"), FileStr, Encoding.UTF8);

            }

        }


        private static string GetMasterDataClassLosdStr(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            string str = "";

            foreach (var temp in ExcelDataDic)
            {
                var file = Path.GetFileNameWithoutExtension(temp.Key);
                var folder = Path.GetDirectoryName(temp.Key);
                folder = folder.Replace("\\", "/");
                str += "        " + file + "Data = Addressables.LoadAssetAsync<" + file + ">(\"" + file + "\").WaitForCompletion();" + Environment.NewLine;

            }
            return str;
        }

        private static string GetMasterDataClassDeclareStr(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            string str = "";

            foreach (var temp in ExcelDataDic)
            {
                var file = Path.GetFileNameWithoutExtension(temp.Key);

                str += "    private " + file + " " + "_" + file + "Data;" + Environment.NewLine;
                str += "    public " + file + " " + file + "Data" + Environment.NewLine;
                str += "    {" + Environment.NewLine;
                str += "        get { return " + "_" + file + "Data; }" + Environment.NewLine;
                str += "        set { _" + file + "Data = value; } " + Environment.NewLine;
                str += "    }" + Environment.NewLine;
                str += Environment.NewLine;

            }
            return str;
        }

        private static string GetMasterDataEnumDeclareStr(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            string str = "";

            foreach (var temp in ExcelDataDic)
            {
                var file = Path.GetFileNameWithoutExtension(temp.Key);

                str += "    public enum" + file + " " + file + "Data;" + Environment.NewLine;

            }
            return str;
        }

        private static string GetMasterDataClassNumStr(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            string str = "";

            str += "    private const int CLASS_NUM = " + ExcelDataDic.Count + ";" + Environment.NewLine;

            return str;
        }

        private static string GetSwitchCaseListStr(Dictionary<string, ExcelDataDictionary> ExcelDataDic)
        {
            string str = "";

            foreach (var temp in ExcelDataDic)
            {
                var file = Path.GetFileNameWithoutExtension(temp.Key);
                str += "                        case \"" + file + "\":" + Environment.NewLine;
                str += "                            Import_" + file + "(path);" + Environment.NewLine;
                str += "                            break;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetClassDataDefineStr(Dictionary<string, ExcelSheetFormat> edfDic)
        {
            string str = "";

            foreach (var temp in edfDic)
            {
                str += "                    var " + temp.Value.ClassName + "Data = new " + temp.Value.ClassName + "Dictionary();" + Environment.NewLine;
            }
            return str;
        }

        private static string GetExcelDataReadStr(Dictionary<string, ExcelSheetFormat> edfDic, string fileName)
        {
            string str = "";

            foreach (var temp in edfDic)
            {
                str += "                            case \"" + temp.Value.ClassName + "\":" + Environment.NewLine;
                str += "                                foreach (var row in table.DataRange.Rows())" + Environment.NewLine;
                str += "                                {" + Environment.NewLine;
                str += "                                    string ret = \"\";" + Environment.NewLine;
                str += "                                    if (row.Cell(0).TryGetValue(out ret) && ret == \"#\")" + Environment.NewLine;
                str += "                                    {" + Environment.NewLine;
                str += "                                        //FirstColumn == # then continue" + Environment.NewLine;
                str += "                                        continue;" + Environment.NewLine;
                str += "                                    }" + Environment.NewLine;
                str += "                                    var rowData = new " + temp.Value.ClassName + "();" + Environment.NewLine;

                int idx = 1;
                foreach (var value in temp.Value.ValuableDefine)
                {
                    switch (value.Value)
                    {
                        case "Comment":
                            idx++; //idxだけ進める
                            break;
                        case "string":
                        case "String":
                            str += "                                    rowData." + value.Key + " = row.Cell(" + idx.ToString() + ").GetString();" + Environment.NewLine;
                            idx++;
                            break;
                        default:

                            var typeStr = value.Value;

                            if (typeStr.Contains(UsingExcelCommon.TableDeclareWorkSheetName + "["))
                            {
                                typeStr = UsingExcelCommon.GetEnumTypeName(typeStr);
                                typeStr = fileName + "." + typeStr;

                                str += "                                    rowData." + value.Key + " = (" + typeStr + ")Enum.Parse(typeof(" + typeStr + "), " + "row.Cell(" + idx.ToString() + ").GetString());" + Environment.NewLine;
                            }
                            else
                            {
                                str += "                                    rowData." + value.Key + " = row.Cell(" + idx.ToString() + ").GetValue<" + value.Value + ">();" + Environment.NewLine;
                            }
                            idx++;
                            break;
                    }
                }

                if (false == temp.Value.IdDuplicatable)
                {
                    str += "                                    " + temp.Value.ClassName + "Data.Add(rowData.id, rowData);" + Environment.NewLine;
                }
                else
                {
                    str += "                                    " + temp.Value.ClassName + "ListClass" + " temp;" + Environment.NewLine;
                    str += "                                    if (!" + temp.Value.ClassName + "Data" + ".ContainsKey(rowData.id))" + Environment.NewLine;
                    str += "                                    {" + Environment.NewLine;
                    str += "                                        temp = new " + temp.Value.ClassName + "ListClass" + "();" + Environment.NewLine;
                    str += "                                        temp." + temp.Value.ClassName + "List = new List<" + temp.Value.ClassName + ">();" + Environment.NewLine;
                    str += "                                        " + temp.Value.ClassName + "Data" + ".Add(rowData.id, temp);" + Environment.NewLine;
                    str += "                                    }" + Environment.NewLine;
                    str += "                                    else" + Environment.NewLine;
                    str += "                                    {" + Environment.NewLine;
                    str += "                                        temp = " + temp.Value.ClassName + "Data" + "[rowData.id];" + Environment.NewLine;
                    str += "                                    }" + Environment.NewLine;
                    str += Environment.NewLine;
                    str += "                                    temp." + temp.Value.ClassName + "List.Add(rowData);" + Environment.NewLine;
                    str += "                                    " + temp.Value.ClassName + "Data" + "[rowData.id] = temp;" + Environment.NewLine;
                }

                str += "                                }" + Environment.NewLine;
                str += "                                break;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetClassDataSetStr(Dictionary<string, ExcelSheetFormat> edfDic)
        {
            string str = "";
            foreach (var temp in edfDic)
            {
                str += "                    data." + temp.Value.ClassName + "Data = " + temp.Value.ClassName + "Data;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetSaveAssetStr(string path)
        {
            string str = "";
            var file = Path.GetFileNameWithoutExtension(path);
            var folder = Path.GetDirectoryName(path);
            folder = folder.Replace("\\", "/");

            str += "                    if (!Directory.Exists(\"" + folder + "/" + UsingExcelCommon.CreatedAssetFoldeName + "/\"))" + Environment.NewLine;
            str += "                    {" + Environment.NewLine;
            str += "                         Directory.CreateDirectory(\"" + folder + "/" + UsingExcelCommon.CreatedAssetFoldeName + "/\");" + Environment.NewLine;
            str += "                    }" + Environment.NewLine;

            str += "                    AssetDatabase.CreateAsset(data, \"" + folder + "/" + UsingExcelCommon.CreatedAssetFoldeName + "/" + file + ".asset\");" + Environment.NewLine;
            return str;
        }


        private static string GetClassDeclareStr(ExcelDataDictionary edfDic, string file)
        {
            string str = "";
            foreach (var classinfo in edfDic.ExcelSheetDic)
            {
                str += "    [SerializeField]" + Environment.NewLine; 
                str += "    private " + classinfo.Value.ClassName + "Dictionary " + "_" + classinfo.Value.ClassName + "Data;" + Environment.NewLine;
                str += "    public " + classinfo.Value.ClassName + "Dictionary " + classinfo.Value.ClassName + "Data" + Environment.NewLine;
                str += "    {" + Environment.NewLine;
                str += "        get { return " + "_" + classinfo.Value.ClassName + "Data; }" + Environment.NewLine;
                str += "        set { _" + classinfo.Value.ClassName + "Data = value; } " + Environment.NewLine;
                str += "    }" + Environment.NewLine;
                str += Environment.NewLine;
            }

            str += "    public object this[string propertyName]" + Environment.NewLine; 
            str += "    {" + Environment.NewLine; 
            str += "        get" + Environment.NewLine; 
            str += "        {" + Environment.NewLine; 
            str += "            return typeof(" + file + ").GetProperty(propertyName).GetValue(this);" + Environment.NewLine; 
            str += "        }" + Environment.NewLine;
            str += Environment.NewLine;
            str += "        set" + Environment.NewLine; 
            str += "        {" + Environment.NewLine; 
            str += "            typeof(" + file + ").GetProperty(propertyName).SetValue(this, value);" + Environment.NewLine; 
            str += "        }" + Environment.NewLine; 
            str += "    }" + Environment.NewLine;
            str += Environment.NewLine;

            return str;
        }



        private static string GetEnumDeclareStr(ExcelDataDictionary edfDic)
        {
            string str = "";
            foreach (var enuminfo in edfDic.EnumDic)
            {

                var enumName = UsingExcelCommon.GetEnumTypeName(enuminfo.Key);


                str += "    public enum " + enumName + "{" + Environment.NewLine;

                var cnt = enuminfo.Value.Count - 1;
                var j = 0;
                foreach (var kvp in enuminfo.Value)
                {
                    if (j < cnt)
                    {
                        str += "        " + kvp.Key + " = " + kvp.Value + "," + Environment.NewLine;
                    }
                    else
                    {
                        str += "        " + kvp.Key + " = " + kvp.Value + Environment.NewLine;
                    }
                    j++;
                }

                str += "    }" + Environment.NewLine + Environment.NewLine;
            }

            return str;
        }

        private static string GetClsssProtoTypeDeclareStr(Dictionary<string, ExcelSheetFormat> edfDic)
        {
            string str = "";
            foreach (var classinfo in edfDic)
            {
                if (false == classinfo.Value.IdDuplicatable)
                {
                    str += "[System.Serializable]" + Environment.NewLine
                         + "public class " + classinfo.Value.ClassName + "Dictionary : SerializableDictionary<" + classinfo.Value.ValuableDefine["id"] + ", " + classinfo.Value.ClassName + "> { }" + Environment.NewLine;
                }
                else
                {
                    str += "[System.Serializable]" + Environment.NewLine
                         + "public class " + classinfo.Value.ClassName + "Dictionary : SerializableDictionary<" + classinfo.Value.ValuableDefine["id"] + ", " + classinfo.Value.ClassName + "ListClass" + "> { }" + Environment.NewLine;
                }
            }
            str += Environment.NewLine;
            foreach (var classinfo in edfDic)
            {
                if (false == classinfo.Value.IdDuplicatable)
                {
                    //処理なし
                }
                else
                {
                    str += "[System.Serializable]" + Environment.NewLine;
                    str += "public class " + classinfo.Value.ClassName + "ListClass" + Environment.NewLine;
                    str += "{" + Environment.NewLine;
                    str += "    public List<" + classinfo.Value.ClassName + "> " + classinfo.Value.ClassName + "List;" + Environment.NewLine;
                    str += "}" + Environment.NewLine;
                }
            }

            return str;
        }

        private static string GetClassDefinitionsStr(Dictionary<string, ExcelSheetFormat> edfDic, string fileName)
        {
            string str = "";


            foreach (var classinfo in edfDic)
            {
                str += "[System.Serializable]" + Environment.NewLine
                     + "public class " + classinfo.Value.ClassName + " : IMasterData" + Environment.NewLine
                     + "{" + Environment.NewLine;

                foreach (var valuables in classinfo.Value.ValuableDefine)
                {
                    //Coment Column Skip
                    if (valuables.Value == "Comment")
                    {
                        continue;
                    }

                    var typeStr = valuables.Value;
                    if (typeStr.Contains(UsingExcelCommon.TableDeclareWorkSheetName + "["))
                    {
                        //Enumの型名取得
                        typeStr = UsingExcelCommon.GetEnumTypeName(typeStr);
                        typeStr = fileName + "." + typeStr;
                    }
                    str += "    [SerializeField]" + Environment.NewLine;

                    if (typeStr == "string")
                    {
                        str += "    private " + typeStr + " _" + valuables.Key + " = \"\";" + Environment.NewLine;
                    }
                    else
                    {
                        str += "    private " + typeStr + " _" + valuables.Key + ";" + Environment.NewLine;
                    }
                    str += "    public " + typeStr + " " + valuables.Key + Environment.NewLine;
                    str += "    {" + Environment.NewLine;
                    str += "        get { return _" + valuables.Key + "; }" + Environment.NewLine;
                    str += "        set { _" + valuables.Key + " = value; }" + Environment.NewLine;
                    str += "    }" + Environment.NewLine;
                    str += Environment.NewLine;
                }


                str += "    public object this[string propertyName]" + Environment.NewLine;
                str += "    {" + Environment.NewLine;
                str += "       get" + Environment.NewLine;
                str += "       {" + Environment.NewLine;
                str += "            return typeof(" + classinfo.Value.ClassName + ").GetProperty(propertyName).GetValue(this);" + Environment.NewLine;
                str += "        }" + Environment.NewLine;
                str += "" + Environment.NewLine;
                str += "       set" + Environment.NewLine;
                str += "        {" + Environment.NewLine;
                str += "            typeof(" + classinfo.Value.ClassName + ").GetProperty(propertyName).SetValue(this, value);" + Environment.NewLine;
                str += "        }" + Environment.NewLine;
                str += "    }" + Environment.NewLine;
                str += Environment.NewLine;
                str += Environment.NewLine;
                str += "    public List<string> PropertyNames" + Environment.NewLine;
                str += "    {" + Environment.NewLine;
                str += "       get" + Environment.NewLine;
                str += "       {" + Environment.NewLine;
                str += "            var ret = new List<string>();" + Environment.NewLine;
                str += "            var properties = this.GetType().GetProperties()" + Environment.NewLine;
                str += "                .Where(p => p.PropertyType != typeof(System.Object) && p.Name != \"PropertyNames\")" + Environment.NewLine;
                str += "                .ToArray();  //インデクサによるItemプロパティ(System.Ojbect)を除外" + Environment.NewLine;
                str += Environment.NewLine;
                str += "            foreach (var property in properties)" + Environment.NewLine;
                str += "            {" + Environment.NewLine;
                str += "                 ret.Add(property.Name);" + Environment.NewLine;
                str += "            }" + Environment.NewLine;
                str += Environment.NewLine;
                str += "        return ret;" + Environment.NewLine;
                str += "        }" + Environment.NewLine;
                str += "    }" + Environment.NewLine;
                str += Environment.NewLine;
                str += Environment.NewLine;
                str += "}";
                str += Environment.NewLine;
                str += Environment.NewLine;


            }

            return str;
        }


        private static ExcelDataDictionary AnalysisExcelFile(string path)
        {
            //処理するワークシートのフォーマット情報を生成
            var ExcelDataFromatDic = new ExcelDataDictionary();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var xwb = new XLWorkbook(fs, XLEventTracking.Disabled))
                {

                    var wsNameList = new List<string>();

                    //処理するワークシート名を取得
                    foreach (var ws in xwb.Worksheets)
                    {
                        if (ws.Name.Contains(UsingExcelCommon.TargetWorkSheetMark))
                        {
                            wsNameList.Add(ws.Name);
                        }


                        if (ws.Name == UsingExcelCommon.TableDeclareWorkSheetName)
                        {
                            UsingExcelCommon.GetEnumDeclare(ws, ref ExcelDataFromatDic.EnumDic);
                        }

                    }

                    foreach (var name in wsNameList)
                    {
                        var edf = new ExcelSheetFormat();

                        UsingExcelCommon.GetExcelDataFormat(xwb, name, ref edf);

                        if (!ExcelDataFromatDic.ExcelSheetDic.ContainsKey(edf.ClassName))
                        {
                            ExcelDataFromatDic.ExcelSheetDic.Add(edf.ClassName, edf);
                        }

                    }


                }
            }

            return ExcelDataFromatDic;

        }


        private static void DeleteExcelImporter()
        {
            //CreatedImporterのフォルダを削除する

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedImporter");

            var CreatedImporterFolder = Path.GetDirectoryName(TemplateFile);

            if (Directory.Exists(CreatedImporterFolder))
            {
                Directory.Delete(CreatedImporterFolder, true);
            }
        }

        private static void DeleteScriptableObject()
        {
            //ScriptableOjbectのフォルダを削除する

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.DictNotDublicatableTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedScriptableObject");

            var CreatedScriptableObjectFolder = Path.GetDirectoryName(TemplateFile);

            if (Directory.Exists(CreatedScriptableObjectFolder))
            {
                Directory.Delete(CreatedScriptableObjectFolder, true);
            }

        }

        private static void DeleteMasterDataEditor()
        {
            //CreatedImporterのフォルダを削除する

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.EditorWindowTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            TemplateFile = TemplateFile.Replace("/Template", "/CreatedScript");

            var CreatedImporterFolder = Path.GetDirectoryName(TemplateFile);

            if (Directory.Exists(CreatedImporterFolder))
            {
                Directory.Delete(CreatedImporterFolder, true);
            }
        }

    }
}
#endif