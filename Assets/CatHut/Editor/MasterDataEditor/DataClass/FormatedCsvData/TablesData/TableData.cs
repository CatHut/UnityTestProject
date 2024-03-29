﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using PlasticGui.WorkspaceWindow.QueryViews.Labels;

namespace CatHut
{
    [Serializable]
    public class Table
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public string Name;

        /// <summary>
        /// テーブルが所有するラベルと値のデータセットDictionary
        /// </summary>
        public SerializableDictionary<string, string> DataSet;

        public Table(string name)
        {
            Name = name;
            DataSet = new SerializableDictionary<string, string>();
        }

        public Table() {
            DataSet = new SerializableDictionary<string, string>();
        }

        public List<string> Labels
        {
            get { return DataSet.Keys.ToList(); }
        }

    }


    [Serializable]
    public class TableData
    {

        private const string TABLE_FOLDER_NAME = @"Tables";
        private const string TABLE_FILE_NAME = @"Tables.csv";


        //テーブルデータ
        [SerializeField]
        public SerializableDictionary<string, Table> TableDic;

        /// <summary>
        /// インポートしたCSVのRawデータ
        /// </summary>
        [SerializeField]
        public CsvData CsvData;

        [SerializeField]
        public string Path;

        public TableData() {
            TableDic = new SerializableDictionary<string, Table>();
        }

        public TableData(string folder)
        {

            TableDic = new SerializableDictionary<string, Table>();
            Path = folder + "\\" + TABLE_FOLDER_NAME + "\\" + TABLE_FILE_NAME;

            if (File.Exists(Path))
            {
                CsvData = new CsvData(Path);
                SetTableData();
            }

        }

        public void AddTableData(string folder)
        {
            Path = folder + "\\" + TABLE_FOLDER_NAME + "\\" + TABLE_FILE_NAME;

            if(TableDic == null)
            {
                TableDic = new SerializableDictionary<string, Table>();
            }

            if (File.Exists(Path))
            {
                CsvData = new CsvData(Path);
                SetTableData();
            }
        }

        public void SetTableData()
        {
            Table currentTable = null;
            for (int i = 0; i < CsvData.Data.Count; i++)
            {
                List<string> row = CsvData.Data[i];

                if (row.Count == 0)
                {
                    continue;
                }

                if (row[0] == "#Table")
                {
                    if (row.Count >= 2)
                    {
                        string tableName = row[1];
                        if (!TableDic.ContainsKey(tableName))
                        {
                            currentTable = new Table(tableName);
                            TableDic.Add(tableName, currentTable);
                        }
                        else
                        {
                            currentTable = TableDic[tableName];
                        }
                    }
                }
                else
                {
                    if (currentTable != null && row.Count >= 2)
                    {
                        string label = row[0];
                        string value = row[1];
                        currentTable.DataSet.Add(label, value);
                    }
                }
            }
        }

        public void Update()
        {
            SetTableData();
        }

    }
}
