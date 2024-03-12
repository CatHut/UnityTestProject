﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace CatHut
{
    public static class MasterDataEditorCommon
    {
        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ImportHeaderMultiply(string folderPath, ref SerializableDictionary<string, DataGroup> ret)
        {

            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);

            if (ret == null)
            {
                ret = new SerializableDictionary<string, DataGroup>();
            }

            //一旦ヘッダ情報を生成(重複時は上書きされる)
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                var dg = new DataGroup();
                dg.SetHeaderInfo(subFolder);

                if (ret.ContainsKey(subFolderName) == true)
                {
                    Debug.LogWarning(subFolderName + "header is Already Registered. " + subFolder + " is Skipped.");
                }
                else
                {
                    ret[subFolderName] = dg;
                }
            }

            //要素のないキーは削除する
            foreach (var key in ret.Keys.ToList())
            {
                //格納状況により調整。
                ret[key].AdjustDicitonaryByHeader();

                if (ret[key].FormatedCsvDic.Count == 0)
                {
                    ret.Remove(key);
                }
            }
        }


        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ImportDataMultiply(string folderPath, ref SerializableDictionary<string, DataGroup> ret)
        {
            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);


            //データ読み込み
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                if (ret.ContainsKey(subFolderName))
                {
                    ret[subFolderName].AddData(subFolder);
                }
            }


            //要素のないキーは削除する
            foreach (var key in ret.Keys.ToList())
            {
                //格納状況により調整。
                ret[key].AdjustDicitonaryByData();

                if (ret[key].FormatedCsvDic.Count == 0)
                {
                    ret.Remove(key);
                }
            }
        }


        public static bool TryConvert<T>(string str, out T ret)
        {
            ret = default(T);
            bool success = true;

            switch (typeof(T))
            {
                case Type t when t == typeof(int):
                    success = int.TryParse(str, out var intResult);
                    ret = (T)(object)intResult;
                    break;
                case Type t when t == typeof(uint):
                    success = uint.TryParse(str, out var uintResult);
                    ret = (T)(object)uintResult;
                    break;
                case Type t when t == typeof(short):
                    success = short.TryParse(str, out var shortResult);
                    ret = (T)(object)shortResult;
                    break;
                case Type t when t == typeof(ushort):
                    success = ushort.TryParse(str, out var ushortResult);
                    ret = (T)(object)ushortResult;
                    break;
                case Type t when t == typeof(long):
                    success = long.TryParse(str, out var longResult);
                    ret = (T)(object)longResult;
                    break;
                case Type t when t == typeof(ulong):
                    success = ulong.TryParse(str, out var ulongResult);
                    ret = (T)(object)ulongResult;
                    break;
                case Type t when t == typeof(float):
                    success = float.TryParse(str, out var floatResult);
                    ret = (T)(object)floatResult;
                    break;
                case Type t when t == typeof(double):
                    success = double.TryParse(str, out var doubleResult);
                    ret = (T)(object)doubleResult;
                    break;
                case Type t when t == typeof(char):
                    success = char.TryParse(str, out var charResult);
                    ret = (T)(object)charResult;
                    break;
                case Type t when t == typeof(bool):
                    success = bool.TryParse(str, out var boolResult);
                    ret = (T)(object)boolResult;
                    break;
                case Type t when t == typeof(byte):
                    success = byte.TryParse(str, out var byteResult);
                    ret = (T)(object)byteResult;
                    break;
                case Type t when t == typeof(sbyte):
                    success = sbyte.TryParse(str, out var sbyteResult);
                    ret = (T)(object)sbyteResult;
                    break;
                case Type t when t == typeof(string):
                    ret = (T)(object)str;
                    success = true;
                    break;
                case Type t when t.IsEnum:      //テーブル値用
                    success = Enum.TryParse(t, str, out var enumResult);
                    ret = (T)enumResult;
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
