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
		static void Import_Item(string path)
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
                    var ItemClassData = new ItemClassDictionary();


					foreach (var name in wsNameList)
					{
						var table = UsingExcelCommon.GetExcelSheetTable(xwb, name);
						ExcelSheetFormat edf = new ExcelSheetFormat();
						UsingExcelCommon.GetExcelDataFormat(xwb, name, ref edf);

						switch (edf.ClassName)
						{

							//エクセルファイル中のクラス定義分追加
                            case "ItemClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new ItemClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.name = row.Cell(2).GetString();
                                    rowData.rarity = row.Cell(3).GetValue<int>();
                                    rowData.EffectCategory = (Item.EFFECT_CATEGORY)Enum.Parse(typeof(Item.EFFECT_CATEGORY), row.Cell(4).GetString());
                                    rowData.value1 = row.Cell(5).GetValue<int>();
                                    rowData.value2 = row.Cell(6).GetValue<int>();
                                    rowData.value3 = row.Cell(7).GetValue<int>();
                                    rowData.value4 = row.Cell(8).GetValue<int>();
                                    rowData.value5 = row.Cell(9).GetValue<int>();
                                    rowData.value6 = row.Cell(10).GetValue<int>();
                                    rowData.value7 = row.Cell(11).GetValue<int>();
                                    rowData.explain = row.Cell(12).GetString();
                                    rowData.IMAGE = row.Cell(13).GetString();
                                    rowData.lowlimit = row.Cell(14).GetValue<int>();
                                    rowData.uplimit = row.Cell(15).GetValue<int>();
                                    ItemClassData.Add(rowData.id, rowData);
                                }
                                break;

						}
					}

                    var data = ScriptableObject.CreateInstance<Item>();

                    data.ItemClassData = ItemClassData;


                    if (!Directory.Exists("Assets/MasterData/Excels/CreatedAssets/"))
                    {
                         Directory.CreateDirectory("Assets/MasterData/Excels/CreatedAssets/");
                    }
                    AssetDatabase.CreateAsset(data, "Assets/MasterData/Excels/CreatedAssets/Item.asset");


				}
			}
		}

	}

}


#endif