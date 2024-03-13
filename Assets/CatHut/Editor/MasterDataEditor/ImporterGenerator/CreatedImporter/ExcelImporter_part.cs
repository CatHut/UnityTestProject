#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CatHut;

namespace CatHut
{
    public static partial class CsvImporter
    {
        private static SerializableDictionary<string, DataGroup> _DataGroupDic;
        private static TableData _GrobalTableData;

        public static void ImportAllCsvData()
        {
            _GrobalTableData = MasterDataEditorCommon.GetGlobalTable();
            _DataGroupDic = MasterDataEditorCommon.GetDataGroupDic();


            foreach (var temp in _DataGroupDic.Keys)
            {

                switch (temp)
				{
                    case "Enemy":
                        Import_Enemy(_DataGroupDic[temp]);
                        break;
                    case "Player":
                        Import_Player(_DataGroupDic[temp]);
                        break;

					default:
						break;
				}
			}
		}

	}
}

#endif