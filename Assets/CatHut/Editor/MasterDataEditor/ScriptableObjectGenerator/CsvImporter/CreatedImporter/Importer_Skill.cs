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
		static void Import_Skill(string path)
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
                    var SkillClassData = new SkillClassDictionary();


					foreach (var name in wsNameList)
					{
						var table = UsingExcelCommon.GetExcelSheetTable(xwb, name);
						ExcelSheetFormat edf = new ExcelSheetFormat();
						UsingExcelCommon.GetExcelDataFormat(xwb, name, ref edf);

						switch (edf.ClassName)
						{

							//エクセルファイル中のクラス定義分追加
                            case "SkillClass":
                                foreach (var row in table.DataRange.Rows())
                                {
                                    string ret = "";
                                    if (row.Cell(0).TryGetValue(out ret) && ret == "#")
                                    {
                                        //FirstColumn == # then continue
                                        continue;
                                    }
                                    var rowData = new SkillClass();
                                    rowData.id = row.Cell(1).GetString();
                                    rowData.name = row.Cell(2).GetString();
                                    rowData.playerlearning = row.Cell(3).GetValue<bool>();
                                    rowData.rarity = row.Cell(4).GetValue<int>();
                                    rowData.ShowCategory = (Skill.SHOW_CATEGORY)Enum.Parse(typeof(Skill.SHOW_CATEGORY), row.Cell(5).GetString());
                                    rowData.SkillCategory = (Skill.SKILL_CATEGORY)Enum.Parse(typeof(Skill.SKILL_CATEGORY), row.Cell(6).GetString());
                                    rowData.category2 = row.Cell(7).GetValue<int>();
                                    rowData.category3 = row.Cell(8).GetValue<int>();
                                    rowData.mp = row.Cell(9).GetValue<int>();
                                    rowData.attr = (Skill.ATTR)Enum.Parse(typeof(Skill.ATTR), row.Cell(10).GetString());
                                    rowData.MainTarget = (Skill.TARGET)Enum.Parse(typeof(Skill.TARGET), row.Cell(11).GetString());
                                    rowData.SubTarget = (Skill.TARGET)Enum.Parse(typeof(Skill.TARGET), row.Cell(12).GetString());
                                    rowData.ContinuationTurn = row.Cell(13).GetValue<int>();
                                    rowData.explain = row.Cell(14).GetString();
                                    rowData.value1 = row.Cell(15).GetValue<float>();
                                    rowData.value2 = row.Cell(16).GetValue<float>();
                                    rowData.value3 = row.Cell(17).GetValue<float>();
                                    rowData.value4 = row.Cell(18).GetValue<float>();
                                    rowData.value5 = row.Cell(19).GetValue<float>();
                                    rowData.value6 = row.Cell(20).GetValue<float>();
                                    rowData.lowlimit = row.Cell(21).GetValue<int>();
                                    rowData.uplimit = row.Cell(22).GetValue<int>();
                                    rowData.VisualEffect = row.Cell(23).GetString();
                                    rowData.SE = row.Cell(24).GetString();
                                    rowData.SeTiming = row.Cell(25).GetValue<float>();
                                    SkillClassData.Add(rowData.id, rowData);
                                }
                                break;

						}
					}

                    var data = ScriptableObject.CreateInstance<Skill>();

                    data.SkillClassData = SkillClassData;


                    if (!Directory.Exists("Assets/MasterData/Excels/CreatedAssets/"))
                    {
                         Directory.CreateDirectory("Assets/MasterData/Excels/CreatedAssets/");
                    }
                    AssetDatabase.CreateAsset(data, "Assets/MasterData/Excels/CreatedAssets/Skill.asset");


				}
			}
		}

	}

}


#endif