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

        public DataGroup(List<string> folders)
        {

            //TableData格納
            _TableData = new TableData();

            _FormatedCsvDic = new SerializableDictionary<string, FormatedCsvData>();

            foreach (var folder in folders)
            {
                //全体共通テーブル取得
                _TableData.AddTableData(folder);

                var searchPattern = "Header_*.csv";
                string[] HeaderFiles = Directory.GetFiles(folder, searchPattern);

                //ヘッダ情報格納
                foreach (string headerFile in HeaderFiles)
                {
                    // DirectoryInfoを使ってディレクトリ情報を取得
                    DirectoryInfo directoryInfo = new DirectoryInfo(headerFile);

                    // ディレクトリの親を取得し、その名前を取得
                    string parentDirectoryName = directoryInfo.Parent.Name;

                    //この段階ではヘッダ情報のみ設定
                    var fcd = new FormatedCsvData(this);
                    fcd.SetHeaderInfo(headerFile);
                    fcd.Enable = false;

                    _FormatedCsvDic.Add(parentDirectoryName, fcd);
                }

            }

            //データ部分取得
            foreach (var folder in folders)
            {
                var searchPattern = "Data_*.csv";
                string[] DataFiles = Directory.GetFiles(folder, searchPattern);

                //ヘッダ情報格納
                foreach (string dataFile in DataFiles)
                {
                    // DirectoryInfoを使ってディレクトリ情報を取得
                    DirectoryInfo directoryInfo = new DirectoryInfo(dataFile);

                    // ディレクトリの親を取得し、その名前を取得
                    string parentDirectoryName = directoryInfo.Parent.Name;

                    if (_FormatedCsvDic.ContainsKey(parentDirectoryName))
                    {
                        _FormatedCsvDic[parentDirectoryName].SetData(dataFile);
                        _FormatedCsvDic[parentDirectoryName].Enable = true;
                    }
                }
            }
        }

    }
}
