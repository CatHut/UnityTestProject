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
    public static class MasterDataComponentGenerator
    {
        public static void CreateMasterDataClass(RawMasterData rmd)
        {
            //MasterDataのメンバ宣言文字列を作成
            var MasterDataClassDeclareStr = GetMasterDataClassDeclareStr(rmd.DataGroupDic);

            //MasterDataのAssetロード処理の文字列作成
            var MasterDataClassLoadStr = GetMasterDataClassLosdStr(rmd.DataGroupDic);

            //テンプレートファイルを探す
            var TemplateFileGUIDs = AssetDatabase.FindAssets(UsingExcelCommon.MasterDataTemplate);

            var TemplateFile = AssetDatabase.GUIDToAssetPath(TemplateFileGUIDs[0]);

            var FileStr = File.ReadAllText(TemplateFile);


            FileStr = FileStr.Replace("#MasterDataClassDeclare#", MasterDataClassDeclareStr);
            FileStr = FileStr.Replace("#MasterDataClassLoad#", MasterDataClassLoadStr);


            var CreatedMasterDataClassPath = MasterDataEditorConfig.LoadSettings().CreatedMasterDataClassPath;
            if (!Directory.Exists(CreatedMasterDataClassPath))
            {
                Directory.CreateDirectory(CreatedMasterDataClassPath);
            }

            var fullpath = Path.Combine(CreatedMasterDataClassPath, "MasterData.cs");
            File.WriteAllText(fullpath, FileStr, Encoding.UTF8);

            AssetDatabase.ImportAsset(fullpath);

        }


        private static string GetMasterDataClassLosdStr(SerializableDictionary<string, DataGroup> ExcelDataDic)
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

        private static string GetMasterDataClassDeclareStr(SerializableDictionary<string, DataGroup> ExcelDataDic)
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

    }

}
#endif