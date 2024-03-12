#if UNITY_EDITOR

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
	public static partial class CsvImporter
	{
		static void Import_Player(DataGroup dg)
		{

            var EnemyParameterData = new Player.EnemyParameterDictionary();


            foreach (var key in dg.FormatedCsvDic.Keys)
            {

                var fc = dg.FormatedCsvDic[key];
                var valDic = dg.FormatedCsvDic[key].HeaderPart.VariableDic;

                switch (key)
                {

					//エクセルファイル中のクラス定義分追加
                    case "EnemyParameter":
                        {
                            int i = 1;
                            foreach (var row in fc.DataPart.DataWithoutColumnTitle)
                            {
                                bool ret;
                                var rowData = new Player.EnemyParameter();

                            //id
                            ret = MasterDataEditorCommon.TryConvert<string>(row[valDic["id"].ColumnIndex], out var result_id);
                            rowData.id = result_id;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:id"); }

                            //NAME
                            ret = MasterDataEditorCommon.TryConvert<string>(row[valDic["NAME"].ColumnIndex], out var result_NAME);
                            rowData.NAME = result_NAME;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:NAME"); }

                            //LV
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["LV"].ColumnIndex], out var result_LV);
                            rowData.LV = result_LV;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:LV"); }

                            //attr
                            ret = MasterDataEditorCommon.TryConvert<Player.ATTR>(row[valDic["attr"].ColumnIndex], out var result_attr);
                            rowData.attr = result_attr;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:attr"); }

                            //HP
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["HP"].ColumnIndex], out var result_HP);
                            rowData.HP = result_HP;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:HP"); }

                            //MP
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["MP"].ColumnIndex], out var result_MP);
                            rowData.MP = result_MP;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:MP"); }

                            //ATK
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["ATK"].ColumnIndex], out var result_ATK);
                            rowData.ATK = result_ATK;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:ATK"); }

                            //DEF
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["DEF"].ColumnIndex], out var result_DEF);
                            rowData.DEF = result_DEF;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:DEF"); }

                            //INT
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["INT"].ColumnIndex], out var result_INT);
                            rowData.INT = result_INT;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:INT"); }

                            //REG
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["REG"].ColumnIndex], out var result_REG);
                            rowData.REG = result_REG;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:REG"); }

                            //SPD
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["SPD"].ColumnIndex], out var result_SPD);
                            rowData.SPD = result_SPD;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:SPD"); }

                            //EXP
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["EXP"].ColumnIndex], out var result_EXP);
                            rowData.EXP = result_EXP;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:EXP"); }

                            //IMAGE
                            ret = MasterDataEditorCommon.TryConvert<string>(row[valDic["IMAGE"].ColumnIndex], out var result_IMAGE);
                            rowData.IMAGE = result_IMAGE;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:IMAGE"); }

                            //lowlimit
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["lowlimit"].ColumnIndex], out var result_lowlimit);
                            rowData.lowlimit = result_lowlimit;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:lowlimit"); }

                            //uplimit
                            ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["uplimit"].ColumnIndex], out var result_uplimit);
                            rowData.uplimit = result_uplimit;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:uplimit"); }

                            //BOSS
                            ret = MasterDataEditorCommon.TryConvert<bool>(row[valDic["BOSS"].ColumnIndex], out var result_BOSS);
                            rowData.BOSS = result_BOSS;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:BOSS"); }

                            //Pattern
                            ret = MasterDataEditorCommon.TryConvert<string>(row[valDic["Pattern"].ColumnIndex], out var result_Pattern);
                            rowData.Pattern = result_Pattern;
                            if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:Pattern"); }

                            }
                        }
                        break;

                }
            }

            var data = ScriptableObject.CreateInstance<Player>();

            data.EnemyParameterData = EnemyParameterData;


            var folder = MasterDataEditorConfig.settings.ScriptableObjectInstancePath;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            AssetDatabase.CreateAsset(data, Path.Combine(folder, "Player.asset"));


        }
    }
}


#endif