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
    public static partial class CsvReflector
    {

        [MenuItem("Tools/CatHut/MasterDataEditor/Import All Csv", false, 0)]
        private static void ReimportAllExcels()
        {
            ImportAllCsvData();
        }

    }
}

#endif