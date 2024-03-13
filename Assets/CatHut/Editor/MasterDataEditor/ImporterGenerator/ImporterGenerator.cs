#if UNITY_EDITOR

using System.IO;
using System.Text;
using UnityEditor;
using System;

namespace CatHut
{
    public static class ImporterGenerator
    {

        public static void CreateCsvImporter(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            //ScriptableOjbectの定義ファイルを作成する
            foreach (var dg in dataGroupDic)
            {

                var ClassDataDefineStr = GetClassDataDefineStr(dg.Value);
                var ExcelDataReadStr = GetCsvDataReadStr(dg.Value, dg.Key);
                var ClassDataSetStr = GetClassDataSetStr(dg.Value);
                var SaveAssetStr = GetSaveAssetStr(dg.Key);


                //テンプレートファイルを探す
                var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterTemplate);
                var TemplateFile = "";

                TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

                var FileStr = File.ReadAllText(TemplateFile);

                FileStr = FileStr.Replace("#DataGroupName#", dg.Key);
                FileStr = FileStr.Replace("#ClassDataDefine#", ClassDataDefineStr);
                FileStr = FileStr.Replace("#CsvDataRead#", ExcelDataReadStr);
                FileStr = FileStr.Replace("#ClassDataSet#", ClassDataSetStr);
                FileStr = FileStr.Replace("#ClassDataSave#", SaveAssetStr);

                var CreatedImporterPath = MasterDataEditorConfig.LoadSettings().CreatedImporterPath;
                if (!Directory.Exists(CreatedImporterPath))
                {
                    Directory.CreateDirectory(CreatedImporterPath);
                }

                var fullpath = Path.Combine(CreatedImporterPath, "Importer_" + dg.Key + ".cs");
                File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

            }

        }

        private static string GetClassDataDefineStr(DataGroup dg)
        {
            string str = "";

            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "            var " + temp + "Data = new " + dg.Name + "." + temp + "Dictionary();" + Environment.NewLine;
            }
            return str;
        }

        private static string GetCsvDataReadStr(DataGroup dg, string fileName)
        {
            string str = "";

            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "                    case \"" + temp + "\":" + Environment.NewLine;
                str += "                        {" + Environment.NewLine;
                str += "                            int i = 1;" + Environment.NewLine;
                str += "                            foreach (var row in fc.DataPart.DataWithoutColumnTitle)" + Environment.NewLine;
                str += "                            {" + Environment.NewLine;
                str += "                                bool ret;" + Environment.NewLine;
                str += "                                var rowData = new "+ dg.Name + "." + temp + "();" + Environment.NewLine;
                str += Environment.NewLine;

                foreach (var value in dg.FormatedCsvDic[temp].HeaderPart.VariableDic.Values)
                {

                    if (value.Type.Contains("Tables["))
                    {
                        //TODO更にグローバルテーブルとローカルテーブルの区別必要

                        str += "                            //" + value.Name + Environment.NewLine;
                        str += "                            ret = MasterDataEditorCommon.TryConvert<" + dg.Name + "." + UsingExcelCommon.GetEnumTypeName(value.Type) + ">(row[valDic[\"" + value.Name + "\"].ColumnIndex], out var result_" + value.Name + ");" + Environment.NewLine;
                        str += "                            rowData." + value.Name + " = result_" + value.Name + ";" + Environment.NewLine;
                        str += "                            if (!ret) { Debug.LogWarning($\"Convert Failed row:{i} col:" + value.Name + "\"); }" + Environment.NewLine;
                        str += Environment.NewLine;
                    }
                    else if(value.Type.Contains("Comment"))
                    {
                        //何もしない
                    }
                    else
                    {
                        str += "                            //" + value.Name + Environment.NewLine;
                        str += "                            ret = MasterDataEditorCommon.TryConvert<" + value.Type + ">(row[valDic[\"" + value.Name + "\"].ColumnIndex], out var result_" + value.Name + ");" + Environment.NewLine;
                        str += "                            rowData." + value.Name + " = result_" + value.Name + ";" + Environment.NewLine;
                        str += "                            if (!ret) { Debug.LogWarning($\"Convert Failed row:{i} col:" + value.Name + "\"); }" + Environment.NewLine;
                        str += Environment.NewLine;
                    }
                }

                str += "                            }" + Environment.NewLine;
                str += "                        }" + Environment.NewLine;
                str += "                        break;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetClassDataSetStr(DataGroup dg)
        {
            string str = "";
            foreach (var temp in dg.FormatedCsvDic.Keys)
            {
                str += "            data." + temp + "Data = " + temp + "Data;" + Environment.NewLine;
            }
            return str;
        }

        private static string GetSaveAssetStr(string filename)
        {
            string str = "";

            str += "            var folder = MasterDataEditorConfig.settings.ScriptableObjectInstancePath;" + Environment.NewLine;

            str += "            if (!Directory.Exists(folder))" + Environment.NewLine;
            str += "            {" + Environment.NewLine;
            str += "                Directory.CreateDirectory(folder);" + Environment.NewLine;
            str += "            }" + Environment.NewLine;
            str += Environment.NewLine;
            str += "            AssetDatabase.CreateAsset(data, Path.Combine(folder, \"" + filename +".asset\"));" + Environment.NewLine;
            return str;
        }


        public static void CreateCsvImporterPart(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            //Importerの呼び出し元のスクリプトファイルを作成する
            var SwitchCaseListStr = GetSwitchCaseListStr(dataGroupDic);
            var SwitchCaseListStr2 = GetSwitchCaseListStr2(dataGroupDic);

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.ExcelImporterPartTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);

            FileStr = FileStr.Replace("#SwitchCaseList#", SwitchCaseListStr);
            FileStr = FileStr.Replace("#SwitchCaseList2#", SwitchCaseListStr2);

            var CreatedImporterPath = MasterDataEditorConfig.LoadSettings().CreatedImporterPath;
            if (!Directory.Exists(CreatedImporterPath))
            {
                Directory.CreateDirectory(CreatedImporterPath);
            }

            var fullpath = Path.Combine(CreatedImporterPath, "ExcelImporter_part.cs");
            File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

        }

        private static string GetSwitchCaseListStr(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            string str = "";

            foreach (var temp in dataGroupDic.Keys)
            {
                var file = Path.GetFileNameWithoutExtension(temp);
                str += "                    case \"" + file + "\":" + Environment.NewLine;
                str += "                        Import_" + file + "(_DataGroupDic[temp]);" + Environment.NewLine;
                str += "                        break;" + Environment.NewLine;
            }
            return str;
        }
        private static string GetSwitchCaseListStr2(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            string str = "";

            foreach (var temp in dataGroupDic.Keys)
            {
                var file = Path.GetFileNameWithoutExtension(temp);
                str += "                case \"" + file + "\":" + Environment.NewLine;
                str += "                    Import_" + file + "(_DataGroupDic[DataGroupName]);" + Environment.NewLine;
                str += "                    break;" + Environment.NewLine;
            }
            return str;
        }

    }

}
#endif