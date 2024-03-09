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

    }

}
#endif