//#if UNITY_EDITOR
#if false

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using CatHut;


namespace CatHut
{
	public static partial class ExcellImporter
	{
		static void Import_Enemy(string path)
		{

			//処理するワークシートのフォーマット情報を生成
			var ExcelDataFromatDic = new Dictionary<string, ExcelSheetFormat>();

			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (var xwb = new XLWorkbook(fs, XLEventTracking.Disabled))
				{

					var wsNameList = new List<string>();


					//処理するワークシート名を取得
					foreach (var ws in xwb.Worksheets)
					{
						if (ws.Name.Contains(UsingExcelCommon.TargetWorkSheetMark))
						{
							wsNameList.Add(ws.Name);
						}
					}

					//エクセルファイル中のクラス定義分追加
                    var EnemyParameterClassData = new EnemyParameterClassDictionary();
                    var SkillPatternClassData = new SkillPatternClassDictionary();


					foreach (var name in wsNameList)
					{
						var table = UsingExcelCommon.GetExcelSheetTable(xwb, name);
						ExcelSheetFormat edf = new ExcelSheetFormat();
						UsingExcelCommon.GetExcelDataFormat(xwb, name, ref edf);

						switch (edf.ClassName)
						{

							//エクセルファイル中のクラス定義分追加
                            case "EnemyParameterClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new EnemyParameterClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.NAME = row.Cell(2).GetString();
                                    rowData.LV = row.Cell(3).GetValue<int>();
                                    rowData.attr = (Enemy.ATTR)Enum.Parse(typeof(Enemy.ATTR), row.Cell(4).GetString());
                                    rowData.HP = row.Cell(5).GetValue<int>();
                                    rowData.MP = row.Cell(6).GetValue<int>();
                                    rowData.ATK = row.Cell(7).GetValue<int>();
                                    rowData.DEF = row.Cell(8).GetValue<int>();
                                    rowData.INT = row.Cell(9).GetValue<int>();
                                    rowData.REG = row.Cell(10).GetValue<int>();
                                    rowData.SPD = row.Cell(11).GetValue<int>();
                                    rowData.EXP = row.Cell(12).GetValue<int>();
                                    rowData.IMAGE = row.Cell(13).GetString();
                                    rowData.lowlimit = row.Cell(14).GetValue<int>();
                                    rowData.uplimit = row.Cell(15).GetValue<int>();
                                    rowData.BOSS = row.Cell(16).GetValue<bool>();
                                    rowData.Pattern = row.Cell(17).GetString();
                                    EnemyParameterClassData.Add(rowData.id, rowData);
                                }
                                break;
                            case "SkillPatternClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new SkillPatternClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.Pattern = (Enemy.PATTERN)Enum.Parse(typeof(Enemy.PATTERN), row.Cell(3).GetString());
                                    rowData.skill1 = row.Cell(4).GetValue<int>();
                                    rowData.skill2 = row.Cell(5).GetValue<int>();
                                    rowData.skill3 = row.Cell(6).GetValue<int>();
                                    rowData.skill4 = row.Cell(7).GetValue<int>();
                                    rowData.skill5 = row.Cell(8).GetValue<int>();
                                    rowData.weight1 = row.Cell(9).GetValue<int>();
                                    rowData.weight2 = row.Cell(10).GetValue<int>();
                                    rowData.weight3 = row.Cell(11).GetValue<int>();
                                    rowData.weight4 = row.Cell(12).GetValue<int>();
                                    rowData.weight5 = row.Cell(13).GetValue<int>();
                                    SkillPatternClassData.Add(rowData.id, rowData);
                                }
                                break;

						}
					}

                    var data = ScriptableObject.CreateInstance<Enemy>();

                    data.EnemyParameterClassData = EnemyParameterClassData;
                    data.SkillPatternClassData = SkillPatternClassData;


                    if (!Directory.Exists("Assets/MasterData/Excels/CreatedAssets/"))
                    {
                         Directory.CreateDirectory("Assets/MasterData/Excels/CreatedAssets/");
                    }
                    AssetDatabase.CreateAsset(data, "Assets/MasterData/Excels/CreatedAssets/Enemy.asset");


				}
			}
		}

	}

}


#endif