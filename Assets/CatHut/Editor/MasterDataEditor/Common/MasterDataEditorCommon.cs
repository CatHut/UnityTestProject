using System;
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




    }
}
