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

		public static void ImportCsvData(List<string> list) 
		{
			foreach(var path in list)
			{
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
}

#endif