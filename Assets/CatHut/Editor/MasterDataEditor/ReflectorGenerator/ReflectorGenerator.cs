#if UNITY_EDITOR

using System.IO;
using System.Text;
using UnityEditor;
using System;
using System.Collections.Generic;

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

                if (dg.FormatedCsvDic[temp].HeaderPart.IndexDuplicatable)
                {
                    str += $"                        int j = 1;" + Environment.NewLine;
                    str += $"                        //IndexValuableの要素を一旦全て削除" + Environment.NewLine;
                    str += $"                        foreach (var row in fcd.DataPart.DataWithoutColumnTitle)" + Environment.NewLine;
                    str += $"                        {{" + Environment.NewLine;
                    str += $"                            bool ret;" + Environment.NewLine;
                    str += $"                            ret = MasterDataEditorCommon.TryConvert<string>(row[valDic[\"{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}\"].ColumnIndex], out var idx);" + Environment.NewLine;
                    str += $"                            if (!ret) {{ Debug.LogWarning($\"Convert Failed row:{{j}} col:{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}\"); continue; }}" + Environment.NewLine;
                    str += Environment.NewLine;
                    str += $"                            MasterData.Instance.{dg.Name}Data.{temp}Data.Remove(idx);" + Environment.NewLine;
                    str += Environment.NewLine;
                    str += $"                            j++;" + Environment.NewLine;
                    str += $"                        }}" + Environment.NewLine;
                    str += Environment.NewLine;
                    str += Environment.NewLine;

                }

                str += "                        int i = 1;" + Environment.NewLine;
                str += "                        foreach (var row in fcd.DataPart.DataWithoutColumnTitle)" + Environment.NewLine;
                str += "                        {" + Environment.NewLine;
                str += "                            bool ret;" + Environment.NewLine;
                str += "                            var rowData = new " + dg.Name + "." + temp + "();" + Environment.NewLine;
                str += Environment.NewLine;

                foreach (var value in dg.FormatedCsvDic[temp].HeaderPart.VariableDic.Values)
                {

                    if (value.IsTableType)
                    {
                        //TODO更にグローバルテーブルとローカルテーブルの区別必要

                        str += "                            //" + value.Name + Environment.NewLine;
                        str += "                            ret = MasterDataEditorCommon.TryConvert<" + dg.Name + "." + UsingCsvCommon.GetEnumTypeName(value.Type) + ">(row[valDic[\"" + value.Name + "\"].ColumnIndex], out var result_" + value.Name + ");" + Environment.NewLine;
                        str += "                            rowData." + value.Name + " = result_" + value.Name + ";" + Environment.NewLine;
                        str += "                            if (!ret) { Debug.LogWarning($\"Convert Failed row:{i} col:" + value.Name + "\"); }" + Environment.NewLine;
                        str += Environment.NewLine;
                    }
                    else if (value.IsComment)
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

                if (dg.FormatedCsvDic[temp].HeaderPart.IndexDuplicatable) {
                    str += $"                            if (!MasterData.Instance.{dg.Name}Data.{temp}Data.ContainsKey(rowData.{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}))" + Environment.NewLine;
                    str += $"                            {{" + Environment.NewLine;
                    str += $"                                //未登録だったら新規作成" + Environment.NewLine;
                    str += $"                                var list = new {dg.Name}.{temp}ListClass();" + Environment.NewLine;
                    str += $"                                list.{temp}List = new List<{dg.Name}.{temp}> {{ rowData }};" + Environment.NewLine;
                    str += $"                                MasterData.Instance.{dg.Name}Data.{temp}Data[rowData.{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}] = list;" + Environment.NewLine;
                    str += $"                            }}" + Environment.NewLine;
                    str += $"                            else" + Environment.NewLine;
                    str += $"                            {{" + Environment.NewLine;
                    str += $"                                MasterData.Instance.{dg.Name}Data.{temp}Data[rowData.{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}].{temp}List.Add(rowData);" + Environment.NewLine;
                    str += $"                            }}" + Environment.NewLine;
                }
                else
                {
                    str += $"                            MasterData.Instance.{dg.Name}Data.{temp}Data[rowData.{dg.FormatedCsvDic[temp].HeaderPart.IndexVariable}] = rowData;" + Environment.NewLine;
                }

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