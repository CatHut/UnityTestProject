using CatHut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace CatHut
{
    public class RawMasterData
    {
        private SerializableDictionary<string, DataGroup> _DataGroupDic;    //こっちではヘッダ情報のみを管理
        private SerializableDictionary<string, SerializableDictionary<string, DataGroup>> _EachPathDataGroupDic;    //パスごとのデータ管理
        private TableData _GrobalTableData;

        public SerializableDictionary<string, DataGroup> DataGroupDic
        {
            get
            {
                return _DataGroupDic;
            }
        }
        public SerializableDictionary<string, SerializableDictionary<string, DataGroup>> EachPathDataGroupDic
        {
            get
            {
                return _EachPathDataGroupDic;
            }
        }


        public TableData GrobalTableData
        {
            get { return _GrobalTableData; }
        }

        public RawMasterData()
        {

        }

        public RawMasterData(string folderPath)
        {
            if(!Directory.Exists(folderPath)) { return; }

            ImportMasterData(folderPath);
        }

        public RawMasterData(List<string> folderPathList)
        {
            _GrobalTableData = new TableData();

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                _GrobalTableData.AddTableData(folderPath);
                ImportHeaderMultiply(folderPath);
            }


            this._EachPathDataGroupDic = new SerializableDictionary<string, SerializableDictionary<string, DataGroup>>();

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                var temp = CatHutCommon.DeepCloneJson(_DataGroupDic);
                _EachPathDataGroupDic[folderPath] = temp;
                ImportDataMultiply(folderPath, _EachPathDataGroupDic[folderPath]);
            }

        }

        public void DataReload(string folderPath)
        {
            ImportDataMultiply(folderPath, _EachPathDataGroupDic[folderPath]);
        }

        public void ImportMasterData(string folderPath)
        {

            var folder = folderPath;

            this._DataGroupDic = new SerializableDictionary<string, DataGroup>();

            string[] subFolders = Directory.GetDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                this._DataGroupDic[subFolderName] = new DataGroup(subFolder);

            }

            //要素のないキーは削除する
            foreach (var key in this._DataGroupDic.Keys.ToList())
            {
                if (this._DataGroupDic[key].FormatedCsvDic.Count == 0)
                {
                    this._DataGroupDic.Remove(key);
                }
            }
        }

        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public void ImportHeaderMultiply(string folderPath)
        {

            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);

            this._DataGroupDic = new SerializableDictionary<string, DataGroup>();

            //一旦ヘッダ情報を生成(重複時は上書きされる)
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                var dg = new DataGroup();
                dg.SetHeaderInfo(subFolder);

                if(_DataGroupDic.ContainsKey(subFolderName) == true)
                {
                    Debug.LogWarning(subFolderName + "header is Already Registered. " + "It is overwritten by " + subFolder);
                }
                this._DataGroupDic[subFolderName] = dg;
            }

            //要素のないキーは削除する
            foreach (var key in this._DataGroupDic.Keys.ToList())
            {
                //格納状況により調整。
                _DataGroupDic[key].AdjustDicitonaryByHeader();

                if (this._DataGroupDic[key].FormatedCsvDic.Count == 0)
                {
                    this._DataGroupDic.Remove(key);
                }
            }
        }

        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public void ImportDataMultiply(string folderPath, SerializableDictionary<string, DataGroup> dataGroupDic)
        {
            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);


            //データ読み込み
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                if (dataGroupDic.ContainsKey(subFolderName))
                {
                    dataGroupDic[subFolderName].SetData(subFolder);
                }
            }


            //要素のないキーは削除する
            foreach (var key in dataGroupDic.Keys.ToList())
            {
                //格納状況により調整。
                dataGroupDic[key].AdjustDicitonaryByData();

                if (dataGroupDic[key].FormatedCsvDic.Count == 0)
                {
                    dataGroupDic.Remove(key);
                }
            }
        }

    }

}
