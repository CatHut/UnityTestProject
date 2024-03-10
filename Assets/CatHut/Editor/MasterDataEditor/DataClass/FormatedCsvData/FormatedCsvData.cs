using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.WSA;

namespace CatHut
{

    [Serializable]
    public class FormatedCsvData
    {
        /// <summary>
        /// データ部
        /// </summary>
        [SerializeField]
        public CsvData DataPart;
        /// <summary>
        /// ヘッダ部
        /// </summary>
        [SerializeField]
        public HeaderInfo HeaderPart;

        [SerializeField]
        public bool Enable;

        public FormatedCsvData() { 
        
        }


        public FormatedCsvData(string folder)
        {
            Enable = true;
            
            HeaderPart = new HeaderInfo();

            string searchPattern = "Header_*.csv";
            string[] csvFiles = Directory.GetFiles(folder, searchPattern);

            if(csvFiles.Length == 0)
            {
                HeaderPart = null;
                Enable = false;
                return;
            }

            HeaderPart.CsvData = new CsvData(csvFiles[0]);
            HeaderPart.FilePath = csvFiles[0];
            HeaderPart.SetHeaderInfo();


            searchPattern = "Data_*.csv";
            csvFiles = Directory.GetFiles(folder, searchPattern);

            if (csvFiles.Length == 0)
            {
                DataPart = null;
                Enable = false;
                return;
            }

            DataPart = new CsvData(csvFiles[0]);

            SetVariableColumnIndex();

            //this.Save();

        }

        public void SetHeaderInfo(string folder)
        {
            Enable = true;

            HeaderPart = new HeaderInfo();

            string searchPattern = "Header_*.csv";
            string[] csvFiles = Directory.GetFiles(folder, searchPattern);

            if (csvFiles.Length == 0)
            {
                HeaderPart = null;
                Enable = false;
                return;
            }

            HeaderPart.CsvData = new CsvData(csvFiles[0]);
            HeaderPart.FilePath = csvFiles[0];
            HeaderPart.SetHeaderInfo();

            SetVariableColumnIndex(folder);
        }

        public void AddData(string folder)
        {
            var searchPattern = "Data_*.csv";
            string[] csvFiles = Directory.GetFiles(folder, searchPattern);

            //データが無ければデータを作成、あれば追加。
            if (csvFiles.Length != 0)
            {
                if (DataPart == null)
                {
                    DataPart = new CsvData(csvFiles[0]);
                }
                else
                {
                    var data = new CsvData(csvFiles[0]);
                    DataPart.Data.AddRange(data.GetRows(1));
                }
            }

            if (DataPart != null)
            {
                if (DataPart.Data.Count == 0)
                {
                    DataPart = null;
                    Enable = false;
                    return;
                }
            }

            SetVariableColumnIndex();
        }



        public void SetData(string folder)
        {
            var searchPattern = "Data_*.csv";
            string[] csvFiles = Directory.GetFiles(folder, searchPattern);

            if (csvFiles.Length != 0)
            {
                DataPart = new CsvData(csvFiles[0]);
            }

            if (DataPart != null)
            {
                if (DataPart.Data.Count == 0)
                {
                    DataPart = null;
                    Enable = false;
                    return;
                }
            }

            SetVariableColumnIndex();
        }

        //インデックス取得するがここでのデータは一旦破棄する。
        private void SetVariableColumnIndex(string folder)
        {
            var searchPattern = "Data_*.csv";
            string[] csvFiles = Directory.GetFiles(folder, searchPattern);

            if (csvFiles.Length == 0)
            {
                DataPart = null;
                Enable = false;
                return;
            }

            DataPart = new CsvData(csvFiles[0]);

            if (DataPart == null) { return; }

            var titleRow = DataPart.Data[0];

            foreach (var title in titleRow)
            {
                if (HeaderPart.VariableDic.ContainsKey(title))
                {
                    HeaderPart.VariableDic[title].ColumnIndex = titleRow.IndexOf(title);
                }
            }

            DataPart = null;
        }

        private void SetVariableColumnIndex()
        {
            if(DataPart == null) { return; }

            var titleRow = DataPart.Data[0];

            foreach (var title in titleRow)
            {
                if (HeaderPart.VariableDic.ContainsKey(title))
                {
                    HeaderPart.VariableDic[title].ColumnIndex = titleRow.IndexOf(title);
                }
            }
        }

        public void Save()
        {
            HeaderPart.Save();
            DataPart.Save();
        }


        /// <summary>
        /// 指定されたフォルダ配下に指定の形式のCSVがあるかチェックする
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool DataCheck(string folder)
        {
            return true;
        }

        /// <summary>
        /// 指定されたフォルダ配下に指定の形式のCSVがあるかチェックする
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool HeaderCheck(HeaderInfo info)
        {
            return true;
        }


    }
}
