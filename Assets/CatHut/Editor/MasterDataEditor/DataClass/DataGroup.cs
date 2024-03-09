using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;

namespace CatHut
{
    public class DataGroup
    {
        private SerializableDictionary<string, FormatedCsvData> _FormatedCsvDic;
        private TableData _TableData;
        bool _Enable = false;

        public string Name { get; set; }

        public SerializableDictionary<string, FormatedCsvData> FormatedCsvDic
        {
            get { return _FormatedCsvDic; }
        }

        public TableData TableData {
            get { return _TableData; }
        }

        public bool Enable{ get { return _Enable; } }


        public DataGroup() { }

        public DataGroup(string folder) {
            
            //Name設定
            Name = new DirectoryInfo(folder).Name;

            //TableData格納
            _TableData = new TableData(folder);


            _FormatedCsvDic = new SerializableDictionary<string, FormatedCsvData>();

            string[] subSubFolders = Directory.GetDirectories(folder);

            //FormatedCsvData格納
            foreach (string subSubFolder in subSubFolders)
            {
                string subSubFolderName = new DirectoryInfo(subSubFolder).Name;

                this._FormatedCsvDic.Add(subSubFolderName, new FormatedCsvData(this, subSubFolder));
            }

            foreach (var kvp in _FormatedCsvDic)
            {
                if (kvp.Value.Enable == true)
                {
                    this._Enable = true;
                }
            }

        }

        public void SetHeaderInfo(string folder)
        {
            //Name設定
            Name = new DirectoryInfo(folder).Name;

            //TableData格納
            _TableData = new TableData(folder);

            _FormatedCsvDic = new SerializableDictionary<string, FormatedCsvData>();

            string[] subSubFolders = Directory.GetDirectories(folder);

            //FormatedCsvData格納
            foreach (string subSubFolder in subSubFolders)
            {
                string subSubFolderName = new DirectoryInfo(subSubFolder).Name;


                //この段階ではヘッダ情報のみ設定
                var fcd = new FormatedCsvData();
                fcd.SetHeaderInfo(this, subSubFolder);
                fcd.Enable = false;
                this._FormatedCsvDic.Add(subSubFolderName, fcd);
            }

        }

        public void SetData(string folder)
        {
            string[] subSubFolders = Directory.GetDirectories(folder);

            //FormatedCsvData格納
            foreach (string subSubFolder in subSubFolders)
            {
                string subSubFolderName = new DirectoryInfo(subSubFolder).Name;

                //データを追加
                this._FormatedCsvDic[subSubFolderName].AddData(subSubFolder);
            }
        }

        public void AdjustDicitonary()
        {
            //要素のないキーは削除する
            foreach (var key in this._FormatedCsvDic.Keys.ToList())
            {
                if (this._FormatedCsvDic[key].DataPart == null || this._FormatedCsvDic[key].HeaderPart == null)
                {
                    this._FormatedCsvDic.Remove(key);
                }
                else
                {
                    _FormatedCsvDic[key].Enable = true;
                }
            }

            if(_FormatedCsvDic.Count > 0)
            {
                _Enable = true;
            }
        }

    }
}
