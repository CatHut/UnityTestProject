#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using CatHut;

namespace CatHut
{
    public partial class AssetPostProcessImporter : AssetPostprocessor
    {

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {

            //変更のあったエクセルを抽出
            var assetsList = new List<string>();
            assetsList.AddRange(importedAssets);
            assetsList.AddRange(deletedAssets);

            //エクセルをインポートするフォルダリスト生成
            var ExcelFolderList = UsingExcelCommon.GetExcelFolderList();

            var ImportExcelList = UsingExcelCommon.GetImportExcelList(assetsList, ExcelFolderList);

            ExcellImporter.ImportExcelData(ImportExcelList);

        }

    }
}

#endif