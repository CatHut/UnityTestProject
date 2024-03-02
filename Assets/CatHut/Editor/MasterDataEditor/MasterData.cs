using CatHut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CatHut
{
    public class MasterData
    {
        private Dictionary<string, DataGroup> _DataGroupDic;

        public Dictionary<string, DataGroup> DataGroupDic
        {
            get
            {
                return _DataGroupDic;
            }
        }

        public MasterData()
        {

        }

        public MasterData(string folderPath)
        {
            if(!Directory.Exists(folderPath)) { return; }

            ImportMasterData(folderPath);
        }

        public void ImportMasterData(string folderPath)
        {

            var folder = folderPath;

            this._DataGroupDic = new Dictionary<string, DataGroup>();

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

    }

}
