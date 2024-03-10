#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using static MasterDataEditorConfig;

namespace CatHut
{
    public static class ImporterGenerator
    {

        public static void CreateExcelImporter(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            //ScriptableOjbectの定義ファイルを作成する
            foreach (var dg in dataGroupDic)
            {

                var FileName = Path.GetFileNameWithoutExtension(dg.Value.Name);


                var ClassDataDefineStr = GetClassDataDefineStr(dg.Value);
                var ExcelDataReadStr = GetExcelDataReadStr(dg.Value, FileName);
                var ClassDataSetStr = GetClassDataSetStr(dg.Value);
                var SaveAssetStr = GetSaveAssetStr(dg.Key);


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

                var CreatedImporterPath = MasterDataEditorConfig.LoadSettings().CreatedImporterPath;
                if (!Directory.Exists(CreatedImporterPath))
                {
                    Directory.CreateDirectory(CreatedImporterPath);
                }

                var fullpath = Path.Combine(CreatedImporterPath, "Importer_" + FileName + ".cs");
                File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

            }

        }

        private static string GetClassDataDefineStr(DataGroup dg)
        {
            string str = "";

            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "                    var " + temp + "Data = new " + temp + "Dictionary();" + Environment.NewLine;
            }
            return str;
        }

        private static string GetExcelDataReadStr(DataGroup dg, string fileName)
        {
            string str = "";

            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "                            case \"" + temp + "\":" + Environment.NewLine;
                str += "                                foreach (var row in table.DataRange.Rows())" + Environment.NewLine;
                str += "                                {" + Environment.NewLine;
                str += "                                    string ret = \"\";" + Environment.NewLine;
                str += "                                    if (row.Cell(0).TryGetValue(out ret) && ret == \"#\")" + Environment.NewLine;
                str += "                                    {" + Environment.NewLine;
                str += "                                        //FirstColumn == # then continue" + Environment.NewLine;
                str += "                                        continue;" + Environment.NewLine;
                str += "                                    }" + Environment.NewLine;
                str += "                                    var rowData = new " + temp + "();" + Environment.NewLine;

                int idx = 1;
                foreach (var value in dg.FormatedCsvDic[temp].HeaderPart.VariableDic.Values)
                {
                    switch (value.Type)
                    {
                        case "Comment":
                            idx++; //idxだけ進める
                            break;
                        case "string":
                        case "String":
                            str += "                                    rowData." + value.Name + " = row.Cell(" + idx.ToString() + ").GetString();" + Environment.NewLine;
                            idx++;
                            break;
                        default:

                            var typeStr = value.Type;

                            if (typeStr.Contains(UsingExcelCommon.TableDeclareWorkSheetName + "["))
                            {
                                typeStr = UsingExcelCommon.GetEnumTypeName(typeStr);
                                typeStr = fileName + "." + typeStr;

                                str += "                                    rowData." + value.Name + " = (" + typeStr + ")Enum.Parse(typeof(" + typeStr + "), " + "row.Cell(" + idx.ToString() + ").GetString());" + Environment.NewLine;
                            }
                            else
                            {
                                str += "                                    rowData." + value.Name + " = row.Cell(" + idx.ToString() + ").GetValue<" + value.Type + ">();" + Environment.NewLine;
                            }
                            idx++;
                            break;
                    }
                }

                if (false == dg.FormatedCsvDic[temp].HeaderPart.IndexDuplicatable)
                {
                    str += "                                    " + temp + "Data.Add(rowData.id, rowData);" + Environment.NewLine;
                }
                else
                {
                    str += "                                    " + temp + "List" + " temp;" + Environment.NewLine;
                    str += "                                    if (!" + temp + "Data" + ".ContainsKey(rowData.id))" + Environment.NewLine;
                    str += "                                    {" + Environment.NewLine;
                    str += "                                        temp = new " + temp + "List" + "();" + Environment.NewLine;
                    str += "                                        temp." + temp + "List = new List<" + temp + ">();" + Environment.NewLine;
                    str += "                                        " + temp + "Data" + ".Add(rowData.id, temp);" + Environment.NewLine;
                    str += "                                    }" + Environment.NewLine;
                    str += "                                    else" + Environment.NewLine;
                    str += "                                    {" + Environment.NewLine;
                    str += "                                        temp = " + temp + "Data" + "[rowData.id];" + Environment.NewLine;
                    str += "                                    }" + Environment.NewLine;
                    str += Environment.NewLine;
                    str += "                                    temp." + temp + "List.Add(rowData);" + Environment.NewLine;
                    str += "                                    " + temp + "Data" + "[rowData.id] = temp;" + Environment.NewLine;
                }

                str += "                                }" + Environment.NewLine;
                str += "                                break;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetClassDataSetStr(DataGroup dg)
        {
            string str = "";
            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "                    data." + temp + "Data = " + temp + "Data;" + Environment.NewLine;
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

    }

}
#endif