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

		public static void ImportAllCsvData() 
		{

            List<string> folderPathList = MasterDataEditorConfig.settings.CsvMasterDataPathList;

            _GrobalTableData = new TableData();


            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                _GrobalTableData.AddTableData(folderPath);
                MasterDataEditorCommon.ImportHeaderMultiply(folderPath, ref _DataGroupDic);
            }

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                MasterDataEditorCommon.ImportDataMultiply(folderPath, ref _DataGroupDic);
            }


            var file = Path.GetFileNameWithoutExtension(path);

			switch (file)
			{
                    case "Enemy":
                        //Import_Enemy(path);
                        break;
                    case "Skill":
                        //Import_Skill(path);
                        break;
                    case "Player":
                        //Import_Player(path);
                        break;
                    case "Item":
                        //Import_Item(path);
                        break;


				default:
					break;
			}
		}

	}
}

#endif