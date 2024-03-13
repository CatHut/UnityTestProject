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
		static void Import_Enemy(DataGroup dg)
		{

            var EnemyParameterData = new Enemy.EnemyParameterDictionary();
            var SkillPatternData = new Enemy.SkillPatternDictionary();


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
                                var rowData = new Enemy.EnemyParameter();

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
                                ret = MasterDataEditorCommon.TryConvert<Enemy.ATTR>(row[valDic["attr"].ColumnIndex], out var result_attr);
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

                                EnemyParameterData.Add(rowData.id, rowData);
                            }
                        }
                        break;
                    case "SkillPattern":
                        {
                            int i = 1;
                            foreach (var row in fc.DataPart.DataWithoutColumnTitle)
                            {
                                bool ret;
                                var rowData = new Enemy.SkillPattern();

                                //id
                                ret = MasterDataEditorCommon.TryConvert<string>(row[valDic["id"].ColumnIndex], out var result_id);
                                rowData.id = result_id;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:id"); }

                                //Pattern
                                ret = MasterDataEditorCommon.TryConvert<Enemy.PATTERN>(row[valDic["Pattern"].ColumnIndex], out var result_Pattern);
                                rowData.Pattern = result_Pattern;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:Pattern"); }

                                //skill1
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["skill1"].ColumnIndex], out var result_skill1);
                                rowData.skill1 = result_skill1;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:skill1"); }

                                //skill2
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["skill2"].ColumnIndex], out var result_skill2);
                                rowData.skill2 = result_skill2;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:skill2"); }

                                //skill3
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["skill3"].ColumnIndex], out var result_skill3);
                                rowData.skill3 = result_skill3;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:skill3"); }

                                //skill4
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["skill4"].ColumnIndex], out var result_skill4);
                                rowData.skill4 = result_skill4;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:skill4"); }

                                //skill5
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["skill5"].ColumnIndex], out var result_skill5);
                                rowData.skill5 = result_skill5;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:skill5"); }

                                //weight1
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["weight1"].ColumnIndex], out var result_weight1);
                                rowData.weight1 = result_weight1;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:weight1"); }

                                //weight2
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["weight2"].ColumnIndex], out var result_weight2);
                                rowData.weight2 = result_weight2;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:weight2"); }

                                //weight3
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["weight3"].ColumnIndex], out var result_weight3);
                                rowData.weight3 = result_weight3;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:weight3"); }

                                //weight4
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["weight4"].ColumnIndex], out var result_weight4);
                                rowData.weight4 = result_weight4;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:weight4"); }

                                //weight5
                                ret = MasterDataEditorCommon.TryConvert<int>(row[valDic["weight5"].ColumnIndex], out var result_weight5);
                                rowData.weight5 = result_weight5;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:weight5"); }

                                SkillPatternData.Add(rowData.id, rowData);
                            }
                        }
                        break;

                }
            }

            var data = ScriptableObject.CreateInstance<Enemy>();

            data.EnemyParameterData = EnemyParameterData;
            data.SkillPatternData = SkillPatternData;


            var folder = MasterDataEditorConfig.settings.ScriptableObjectInstancePath;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            AssetDatabase.CreateAsset(data, Path.Combine(folder, "Enemy.asset"));


        }
    }
}


#endif