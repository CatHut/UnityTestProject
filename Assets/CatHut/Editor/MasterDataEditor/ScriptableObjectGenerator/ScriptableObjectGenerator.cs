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
    public static class ScriptableObjectGenerator
    {

        public static void CreateScriptableObject(DataGroup dg)
        {
            //ScriptableOjbectの定義ファイルを作成する
            var FileName = dg.Name;

            var EnmuDeclareStr = GetEnumDeclareStr(dg);
            var ClassDeclareStr = GetClassDeclareStr(dg, FileName);
            var ClassProtoTypeDeclareStr = GetClsssProtoTypeDeclareStr(dg.FormatedCsvDic);
            var ClassDefinitionsStr = GetClassDefinitionsStr(dg.FormatedCsvDic, FileName);

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


            var CreatedScriptableObjectFolder = MasterDataEditorConfig.LoadSettings().CreatedScriptableObjectClassPath;
            if (!Directory.Exists(CreatedScriptableObjectFolder))
            {
                Directory.CreateDirectory(CreatedScriptableObjectFolder);
            }

            var fullpath = Path.Combine(CreatedScriptableObjectFolder, FileName + ".cs");
            File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

            AssetDatabase.ImportAsset(fullpath);

        }

        private static string GetEnumDeclareStr(DataGroup dg)
        {
            string str = "";
            foreach (var enuminfo in dg.TableData.TableDic)
            {

                var enumName = UsingExcelCommon.GetEnumTypeName(enuminfo.Key);


                str += "    public enum " + enumName + "{" + Environment.NewLine;

                var cnt = enuminfo.Value.DataSet.Count - 1;
                var j = 0;
                foreach (var kvp in enuminfo.Value.DataSet)
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

        private static string GetClassDeclareStr(DataGroup dg, string file)
        {
            string str = "";
            foreach (var classinfo in dg.FormatedCsvDic)
            {
                str += "    [SerializeField]" + Environment.NewLine;
                str += "    private " + classinfo.Key + "Dictionary " + "_" + classinfo.Key + "Data;" + Environment.NewLine;
                str += "    public " + classinfo.Key + "Dictionary " + classinfo.Key + "Data" + Environment.NewLine;
                str += "    {" + Environment.NewLine;
                str += "        get { return " + "_" + classinfo.Key + "Data; }" + Environment.NewLine;
                str += "        set { _" + classinfo.Key + "Data = value; } " + Environment.NewLine;
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

        private static string GetClsssProtoTypeDeclareStr(SerializableDictionary<string, FormatedCsvData> fcdDic)
        {
            string str = "";
            foreach (var classinfo in fcdDic)
            {
                if (false == classinfo.Value.HeaderPart.IndexDuplicatable)
                {
                    str += "[System.Serializable]" + Environment.NewLine
                         + "public class " + classinfo.Key + "Dictionary : SerializableDictionary<" + classinfo.Value.HeaderPart.VariableDic[classinfo.Value.HeaderPart.IndexVariable].Type + ", " + classinfo.Key + "> { }" + Environment.NewLine;
                }
                else
                {
                    str += "[System.Serializable]" + Environment.NewLine
                         + "public class " + classinfo.Key + "Dictionary : SerializableDictionary<" + classinfo.Value.HeaderPart.VariableDic[classinfo.Value.HeaderPart.IndexVariable].Type + ", " + classinfo.Key + "ListClass" + "> { }" + Environment.NewLine;
                }
            }

            str += Environment.NewLine;

            foreach (var classinfo in fcdDic)
            {
                if (false == classinfo.Value.HeaderPart.IndexDuplicatable)
                {
                    //処理なし
                }
                else
                {
                    str += "[System.Serializable]" + Environment.NewLine;
                    str += "public class " + classinfo.Key + "ListClass" + Environment.NewLine;
                    str += "{" + Environment.NewLine;
                    str += "    public List<" + classinfo.Key + "> " + classinfo.Key + "List;" + Environment.NewLine;
                    str += "}" + Environment.NewLine;
                }
            }

            return str;
        }

        private static string GetClassDefinitionsStr(SerializableDictionary<string, FormatedCsvData > fcdDic, string fileName)
        {
            string str = "";


            foreach (var classinfo in fcdDic)
            {
                str += "[System.Serializable]" + Environment.NewLine
                     + "public class " + classinfo.Key + " : IMasterData" + Environment.NewLine
                     + "{" + Environment.NewLine;

                foreach (var valuables in classinfo.Value.HeaderPart.VariableDic)
                {
                    //Coment Column Skip
                    if (valuables.Value.Type == "Comment")
                    {
                        continue;
                    }

                    var typeStr = valuables.Value.Type;
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
                str += "            return typeof(" + classinfo.Key + ").GetProperty(propertyName).GetValue(this);" + Environment.NewLine;
                str += "        }" + Environment.NewLine;
                str += "" + Environment.NewLine;
                str += "       set" + Environment.NewLine;
                str += "        {" + Environment.NewLine;
                str += "            typeof(" + classinfo.Key + ").GetProperty(propertyName).SetValue(this, value);" + Environment.NewLine;
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
    }

}
#endif