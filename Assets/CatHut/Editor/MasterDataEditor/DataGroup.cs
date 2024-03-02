using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

    }
}
