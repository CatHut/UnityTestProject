#if false

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ClosedXML;
using ClosedXML.Excel;
using System;
using CatHut;


namespace CatHut
{
	public static partial class ExcellImporter
	{
		static void Import_Player(string path)
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
                    var PlayerParameterClassData = new PlayerParameterClassDictionary();
                    var PlayerJobClassData = new PlayerJobClassDictionary();


					foreach (var name in wsNameList)
					{
						var table = UsingExcelCommon.GetExcelSheetTable(xwb, name);
						ExcelSheetFormat edf = new ExcelSheetFormat();
						UsingExcelCommon.GetExcelDataFormat(xwb, name, ref edf);

						switch (edf.ClassName)
						{

							//エクセルファイル中のクラス定義分追加
                            case "PlayerParameterClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new PlayerParameterClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.job = row.Cell(2).GetString();
                                    rowData.LV = row.Cell(3).GetValue<int>();
                                    rowData.HP = row.Cell(4).GetValue<int>();
                                    rowData.MP = row.Cell(5).GetValue<int>();
                                    rowData.ATK = row.Cell(6).GetValue<int>();
                                    rowData.DEF = row.Cell(7).GetValue<int>();
                                    rowData.INT = row.Cell(8).GetValue<int>();
                                    rowData.REG = row.Cell(9).GetValue<int>();
                                    rowData.SPD = row.Cell(10).GetValue<int>();
                                    rowData.EXP = row.Cell(11).GetValue<int>();
                                    rowData.uplimit = row.Cell(12).GetValue<int>();
                                    rowData.lowlimit = row.Cell(13).GetValue<int>();
                                    PlayerParameterClassData.Add(rowData.id, rowData);
                                }
                                break;
                            case "PlayerJobClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new PlayerJobClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.name = row.Cell(2).GetString();
                                    rowData.image = row.Cell(3).GetString();
                                    PlayerJobClassData.Add(rowData.id, rowData);
                                }
                                break;

						}
					}

                    var data = ScriptableObject.CreateInstance<Player>();

                    data.PlayerParameterClassData = PlayerParameterClassData;
                    data.PlayerJobClassData = PlayerJobClassData;


                    if (!Directory.Exists("Assets/MasterData/Excels/CreatedAssets/"))
                    {
                         Directory.CreateDirectory("Assets/MasterData/Excels/CreatedAssets/");
                    }
                    AssetDatabase.CreateAsset(data, "Assets/MasterData/Excels/CreatedAssets/Player.asset");


				}
			}
		}

	}

}


#endif