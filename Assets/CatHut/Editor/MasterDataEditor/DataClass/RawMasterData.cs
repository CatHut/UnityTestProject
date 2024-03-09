using CatHut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.WSA;

namespace CatHut
{
    public class RawMasterData
    {
        private SerializableDictionary<string, DataGroup> _DataGroupDic;
        private TableData _GrobalTableData;

        public SerializableDictionary<string, DataGroup> DataGroupDic
        {
            get
            {
                return _DataGroupDic;
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
                ImportMasterDataMultiply(folderPath);
            }
        }

        public void ImportMasterData(string folderPath)
        {

            var folder = folderPath;

            this._DataGroupDic = new SerializableDictionary<string, DataGroup>();

            string[] subFolders = Directory.GetDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                this._DataGroupDic.Add(subFolderName, new DataGroup(subFolder));

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

        public void ImportMasterDataMultiply(string folderPath)
        {

            var folder = folderPath;

            this._DataGroupDic = new SerializableDictionary<string, DataGroup>();

            string[] subFolders = Directory.GetDirectories(folder);




            //一旦ヘッダ情報を生成(重複時は上書きされる)
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                var dg = new DataGroup();
                dg.SetHeaderInfo(subFolder);

                this._DataGroupDic.Add(subFolderName, dg);
            }

            //データ設定（全て追加）
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                this._DataGroupDic[subFolderName].SetData(subFolder);
            }


            //要素のないキーは削除する
            foreach (var key in this._DataGroupDic.Keys.ToList())
            {
                //格納状況により調整。
                _DataGroupDic[key].AdjustDicitonary();

                if (this._DataGroupDic[key].FormatedCsvDic.Count == 0)
                {
                    this._DataGroupDic.Remove(key);
                }
            }
        }

    }

}
