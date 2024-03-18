#if UNITY_EDITOR

using System.IO;
using System.Text;
using UnityEditor;
using System;

namespace CatHut
{
    public static class ReflectorGenerator
    {

        public static void CreateCsvReflector(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            //ScriptableOjbectの定義ファイルを作成する
            foreach (var dg in dataGroupDic)
            {

                var ClassDataDefineStr = GetClassDataDefineStr(dg.Value);
                var ExcelDataReadStr = GetCsvDataReadStr(dg.Value, dg.Key);


                //テンプレートファイルを探す
                var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingCsvCommon.CsvReflectorTemplate);
                var TemplateFile = "";

                TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

                var FileStr = File.ReadAllText(TemplateFile);

                FileStr = FileStr.Replace("#DataGroupName#", dg.Key);
                FileStr = FileStr.Replace("#ClassDataDefine#", ClassDataDefineStr);
                FileStr = FileStr.Replace("#CsvDataRead#", ExcelDataReadStr);

                var CreatedReflectorPath = MasterDataEditorConfig.LoadSettings().CreatedReflectorPath;
                if (!Directory.Exists(CreatedReflectorPath))
                {
                    Directory.CreateDirectory(CreatedReflectorPath);
                }

                var fullpath = Path.Combine(CreatedReflectorPath, "Reflector_" + dg.Key + ".cs");
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
                str += "                case \"" + temp + "\":" + Environment.NewLine;
                str += "                    {" + Environment.NewLine;
                str += "                        int i = 1;" + Environment.NewLine;
                str += "                        foreach (var row in fcd.DataPart.DataWithoutColumnTitle)" + Environment.NewLine;
                str += "                        {" + Environment.NewLine;
                str += "                            bool ret;" + Environment.NewLine;
                str += "                            var rowData = new " + dg.Name + "." + temp + "();" + Environment.NewLine;
                str += Environment.NewLine;

                foreach (var value in dg.FormatedCsvDic[temp].HeaderPart.VariableDic.Values)
                {

                    if (value.Type.Contains("Tables["))
                    {
                        //TODO更にグローバルテーブルとローカルテーブルの区別必要

                        str += "                            //" + value.Name + Environment.NewLine;
                        str += "                            ret = MasterDataEditorCommon.TryConvert<" + dg.Name + "." + UsingCsvCommon.GetEnumTypeName(value.Type) + ">(row[valDic[\"" + value.Name + "\"].ColumnIndex], out var result_" + value.Name + ");" + Environment.NewLine;
                        str += "                            rowData." + value.Name + " = result_" + value.Name + ";" + Environment.NewLine;
                        str += "                            if (!ret) { Debug.LogWarning($\"Convert Failed row:{i} col:" + value.Name + "\"); }" + Environment.NewLine;
                        str += Environment.NewLine;
                    }
                    else if (value.Type.Contains("Comment"))
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

                var indexValue = dg.FormatedCsvDic[temp].HeaderPart.IndexVariable;

                str += $"                            MasterData.Instance.{dg.Name}Data.{temp}Data[rowData.id] = rowData;" + Environment.NewLine;
                str += "                        }" + Environment.NewLine;
                str += "                    }" + Environment.NewLine;
                str += "                    break;" + Environment.NewLine;
            }
            return str;
        }


        public static void CreateCsvReflectorPart(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            //Importerの呼び出し元のスクリプトファイルを作成する
            var SwitchCaseListStr = GetSwitchCaseListStr(dataGroupDic);

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingCsvCommon.CsvReflectorPartTemplate);
            var TemplateFile = "";

            TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);

            FileStr = FileStr.Replace("#SwitchCaseList#", SwitchCaseListStr);

            var CreatedReflectorPath = MasterDataEditorConfig.LoadSettings().CreatedReflectorPath;
            if (!Directory.Exists(CreatedReflectorPath))
            {
                Directory.CreateDirectory(CreatedReflectorPath);
            }

            var fullpath = Path.Combine(CreatedReflectorPath, "CsvReflector_part.cs");
            File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

        }

        private static string GetSwitchCaseListStr(SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            string str = "";

            foreach (var temp in dataGroupDic.Keys)
            {
                var file = Path.GetFileNameWithoutExtension(temp);
                str += "                case \"" + file + "\":" + Environment.NewLine;
                str += "                    Reflect" + file + "(ChildName, fcd);" + Environment.NewLine;
                str += "                    break;" + Environment.NewLine;
            }
            return str;
        }

    }

}
#endif