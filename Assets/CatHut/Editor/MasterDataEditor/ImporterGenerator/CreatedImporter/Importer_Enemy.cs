#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using CatHut;
using static UnityEngine.Rendering.DebugUI.Table;


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

                                // rowDataオブジェクトの各プロパティに値を設定する処理
                                // ID
                                ret = TryConvert<string>(row[valDic["id"].ColumnIndex], out var result_id);
                                rowData.id = result_id;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:id"); }

                                // NAME
                                ret = TryConvert<string>(row[valDic["NAME"].ColumnIndex], out var result_name);
                                rowData.NAME = result_name;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:NAME"); }

                                // LV
                                ret = TryConvert<int>(row[valDic["LV"].ColumnIndex], out var result_lv);
                                rowData.LV = result_lv;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:LV"); }

                                // attr
                                ret = TryConvert<Enemy.ATTR>(row[valDic["attr"].ColumnIndex], out var result_attr);
                                rowData.attr = result_attr;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:attr"); }

                                // HP
                                ret = TryConvert<int>(row[valDic["HP"].ColumnIndex], out var result_hp);
                                rowData.HP = result_hp;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:HP"); }

                                // MP
                                ret = TryConvert<int>(row[valDic["MP"].ColumnIndex], out var result_mp);
                                rowData.MP = result_mp;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:MP"); }

                                // ATK
                                ret = TryConvert<int>(row[valDic["ATK"].ColumnIndex], out var result_atk);
                                rowData.ATK = result_atk;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:ATK"); }

                                // DEF
                                ret = TryConvert<int>(row[valDic["DEF"].ColumnIndex], out var result_def);
                                rowData.DEF = result_def;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:DEF"); }

                                // INT
                                ret = TryConvert<int>(row[valDic["INT"].ColumnIndex], out var result_int);
                                rowData.INT = result_int;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:INT"); }

                                // REG
                                ret = TryConvert<int>(row[valDic["REG"].ColumnIndex], out var result_reg);
                                rowData.REG = result_reg;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:REG"); }

                                // SPD
                                ret = TryConvert<int>(row[valDic["SPD"].ColumnIndex], out var result_spd);
                                rowData.SPD = result_spd;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:SPD"); }

                                // EXP
                                ret = TryConvert<int>(row[valDic["EXP"].ColumnIndex], out var result_exp);
                                rowData.EXP = result_exp;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:EXP"); }

                                // IMAGE
                                ret = TryConvert<string>(row[valDic["IMAGE"].ColumnIndex], out var result_image);
                                rowData.IMAGE = result_image;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:IMAGE"); }

                                // lowlimit
                                ret = TryConvert<int>(row[valDic["lowlimit"].ColumnIndex], out var result_lowlimit);
                                rowData.lowlimit = result_lowlimit;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:lowlimit"); }

                                // uplimit
                                ret = TryConvert<int>(row[valDic["uplimit"].ColumnIndex], out var result_uplimit);
                                rowData.uplimit = result_uplimit;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:uplimit"); }

                                // BOSS
                                ret = TryConvert<bool>(row[valDic["BOSS"].ColumnIndex], out var result_boss);
                                rowData.BOSS = result_boss;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:BOSS"); }

                                // Pattern
                                ret = TryConvert<string>(row[valDic["Pattern"].ColumnIndex], out var result_pattern);
                                rowData.Pattern = result_pattern;
                                if (!ret) { Debug.LogWarning($"Convert Failed row:{i} col:Pattern"); }

                                EnemyParameterData.Add(rowData.id, rowData);

                                i++;
                            }
                        }
                        break;
                    case "SkillPattern":
                        foreach (var row in fc.DataPart.DataWithoutColumnTitle)
                        {
                            var rowData = new Enemy.SkillPattern();
                            //rowData.id = row.Cell(1).GetString();
                            //rowData.Pattern = (Enemy.PATTERN)Enum.Parse(typeof(Enemy.PATTERN), row.Cell(3).GetString());
                            //rowData.skill1 = row.Cell(4).GetValue<int>();
                            //rowData.skill2 = row.Cell(5).GetValue<int>();
                            //rowData.skill3 = row.Cell(6).GetValue<int>();
                            //rowData.skill4 = row.Cell(7).GetValue<int>();
                            //rowData.skill5 = row.Cell(8).GetValue<int>();
                            //rowData.weight1 = row.Cell(9).GetValue<int>();
                            //rowData.weight2 = row.Cell(10).GetValue<int>();
                            //rowData.weight3 = row.Cell(11).GetValue<int>();
                            //rowData.weight4 = row.Cell(12).GetValue<int>();
                            //rowData.weight5 = row.Cell(13).GetValue<int>();
                            SkillPatternData.Add(rowData.id, rowData);
                        }
                        break;

                }
            }

            var data = ScriptableObject.CreateInstance<Enemy>();

            data.EnemyParameterData = EnemyParameterData;
            data.SkillPatternData = SkillPatternData;


            if (!Directory.Exists("/CreatedAssets/"))
            {
                Directory.CreateDirectory("/CreatedAssets/");
            }
            AssetDatabase.CreateAsset(data, "/CreatedAssets/Enemy.asset");


        }


        public static bool TryConvert<T>(string str, out T ret)
        {
            ret = default;
            bool success = true;

            // Tの型に応じて適切なTryParseメソッドを呼び出す
            switch (default(T))
            {
                case int _:
                    success = int.TryParse(str, out var intResult);
                    ret = (T)(object)intResult;
                    break;
                case uint _:
                    success = uint.TryParse(str, out var uintResult);
                    ret = (T)(object)uintResult;
                    break;
                case short _:
                    success = short.TryParse(str, out var shortResult);
                    ret = (T)(object)shortResult;
                    break;
                case ushort _:
                    success = ushort.TryParse(str, out var ushortResult);
                    ret = (T)(object)ushortResult;
                    break;
                case long _:
                    success = long.TryParse(str, out var longResult);
                    ret = (T)(object)longResult;
                    break;
                case ulong _:
                    success = ulong.TryParse(str, out var ulongResult);
                    ret = (T)(object)ulongResult;
                    break;
                case float _:
                    success = float.TryParse(str, out var floatResult);
                    ret = (T)(object)floatResult;
                    break;
                case double _:
                    success = double.TryParse(str, out var doubleResult);
                    ret = (T)(object)doubleResult;
                    break;
                case char _:
                    success = char.TryParse(str, out var charResult);
                    ret = (T)(object)charResult;
                    break;
                case bool _:
                    success = bool.TryParse(str, out var boolResult);
                    ret = (T)(object)boolResult;
                    break;
                case byte _:
                    success = byte.TryParse(str, out var byteResult);
                    ret = (T)(object)byteResult;
                    break;
                case sbyte _:
                    success = sbyte.TryParse(str, out var sbyteResult);
                    ret = (T)(object)sbyteResult;
                    break;
                case string _:
                    ret = (T)(object)str;
                    success = true;
                    break;
                case Enemy.ATTR _:
                    success = Enum.TryParse<Enemy.ATTR>(str, out var enumEnemyAttrResult);
                    ret = (T)(object)enumEnemyAttrResult;
                    break;
                default:
                    success = false;
                    Debug.LogWarning($"Unsupported type: {typeof(T)}");
                    break;
            }

            if (!success)
            {
                Debug.LogWarning($"Conversion failed: Cannot convert \"{str}\" to {typeof(T)}.");
            }

            return success;
        }
    }
}







#endif