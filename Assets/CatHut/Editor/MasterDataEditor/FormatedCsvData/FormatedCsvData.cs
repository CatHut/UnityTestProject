using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CatHut
{

    public class FormatedCsvData
    {
        /// <summary>
        /// データ部
        /// </summary>
        public CsvData DataPart { get; set; }
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public HeaderInfo HeaderPart { get; set; }

        public DataGroup Parent { get; set; }

        public bool Enable { get; set; }

        public FormatedCsvData() { 
        
        }

        public FormatedCsvData(DataGroup dg, string folder)
        {
            Parent = dg;
            Enable = true;
            
            HeaderPart = new HeaderInfo(Parent);

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

        private void SetVariableColumnIndex()
        {
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
