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
    public static partial class ExcellImporter
    {

        [MenuItem("UsingExcel/ReImportExcels", false, 0)]
        private static void ReimportAllExcels()
        {
            var ImportExcelList = UsingExcelCommon.GetAllExcelList();

            ImportExcelData(ImportExcelList);
        }

    }
}

#endif