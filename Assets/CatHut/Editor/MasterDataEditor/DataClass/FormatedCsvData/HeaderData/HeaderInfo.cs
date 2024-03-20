using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CatHut
{
    [Serializable]
    public class VariableInfo
    {
        /// <summary>
        /// 変数名
        /// </summary>
        [SerializeField]
        public string Name;
        /// <summary>
        /// 型名
        /// </summary>
        [SerializeField]
        private string _Type;

        public string Type
        {
            set { _Type = value; }
            
            get { 
                if(_Type == "MLstring")
                {
                    return "string";
                }

                return _Type; 
            }
        }

        public string EditorViewType
        {
            get { return _Type; }
        }

        public bool IsTableType
        {
            get
            {
                return Type.Contains("Tables[");
            }
        }

        public bool IsComment
        {
            get
            {
                return Type.Contains("Comment");
            }
        }

        /// 変数の説明
        /// </summary>
        [SerializeField]
        public string Description;
        /// <summary>
        /// データ列（DataのCsvにおける列）
        /// </summary>
        [SerializeField]
        public int ColumnIndex;

    }

    [Serializable]
    public class HeaderInfo
    {
        //CSV Header識別子
        public string IDENTIFIER_CLASSNAME = "ClassName";
        public string IDENTIFIER_PARENTNAME = "ParentName";
        public string IDENTIFIER_INDEXVARIABLE = "IndexVariable";
        public string IDENTIFIER_INDEXDUPLICATABLE = "IndexDuplicatable";
        public string IDENTIFIER_VARIABLE = "#Valuable";
        public string IDENTIFIER_CUSTOM = "#Custom";

        public string IDENTIFIER_VARUALBENAME = "Name";
        public string IDENTIFIER_VARUALBETYPE = "Type";
        public string IDENTIFIER_VARUALBEDESCRIPTION = "Description";

        /// <summary>
        /// Header情報を保存しているCSVのパス
        /// </summary>
        [SerializeField]
        public string FilePath;

        /// <summary>
        /// このデータクラスのクラス名
        /// </summary>
        [SerializeField]
        public string ClassName;

        /// <summary>
        /// このデータクラスの親クラス名
        /// </summary>
        [SerializeField]
        public string ParentName;

        /// <summary>
        /// このクラスが管理する変数リスト
        /// key:Name
        /// value:VariableInfo
        /// </summary>
        [SerializeField]
        public SerializableDictionary<string, VariableInfo> VariableDic;

        /// <summary>
        /// ユーザが任意に指定できるカスタム値
        /// </summary>
        [SerializeField]
        public SerializableDictionary<string, List<string>> CustomValue;

        /// <summary>
        /// IDなどインデックスとして使用する変数リスト
        /// これをキーにUnity上でDictionaryを作成します。
        /// </summary>
        [SerializeField]
        public string IndexVariable;

        /// <summary>
        /// キーの重複を許容するか。（同じキーに複数のデータを保持するか）
        /// IndexDuplicatableがtrueの場合DictionaryのValueにはListが格納される
        /// </summary>
        [SerializeField]
        public bool IndexDuplicatable;
        /// <summary>
        /// キーの重複を許容するか。（同じキーに複数のデータを保持するか）
        /// IndexDuplicatableがtrueの場合DictionaryのValueにはListが格納される
        /// </summary>
        public string IndexDuplicatableString {
            get { return IndexDuplicatable ? "true" : "false"; }
            set { 
                if(value == "true" )
                { 
                    IndexDuplicatable = true;
                }
                else
                {
                    IndexDuplicatable = false;
                }
            }
        }

        /// <summary>
        /// インポートしたCSVのRawデータ
        /// </summary>
        [SerializeField]
        public CsvData CsvData;


        public HeaderInfo()
        {

            ClassName = "";
            ParentName = "";

            CustomValue = new SerializableDictionary<string, List<string>>();
            VariableDic = new SerializableDictionary<string, VariableInfo>();

            IndexVariable = "";
            IndexDuplicatable = false;
        }

        /// <summary>
        /// Header_ClassName.csvからヘッダ情報を取得する
        /// </summary>
        public void SetHeaderInfo()
        {
            for (int i = 0; i < CsvData.Data.Count; i++)
            {
                if (CsvData.Data[i].Count == 0) continue;

                if (CsvData.Data[i][0] == IDENTIFIER_CLASSNAME)
                {
                    ClassName = CsvData.Data[i][1];
                }
                else if (CsvData.Data[i][0] == IDENTIFIER_PARENTNAME)
                {
                    ParentName = CsvData.Data[i][1];
                }
                else if (CsvData.Data[i][0] == IDENTIFIER_INDEXVARIABLE)
                {
                    IndexVariable = CsvData.Data[i][1];
                }
                else if (CsvData.Data[i][0] == IDENTIFIER_INDEXDUPLICATABLE)
                {
                    string value = CsvData.Data[i][1].Trim().ToLower();
                    if (value == "true")
                    {
                        IndexDuplicatable = true;
                    }
                    else if (value == "false")
                    {
                        IndexDuplicatable = false;
                    }
                }
                else if (CsvData.Data[i][0] == IDENTIFIER_CUSTOM)
                {
                    SetCustomInfo(i);
                }
                else if (CsvData.Data[i][0] == IDENTIFIER_VARIABLE)
                {
                    SetVariableInfo(i);
                }
            }
        }

        /// <summary>
        /// 変数情報を格納する
        /// </summary>
        /// <param name="i">#Variableが見つかった行番号</param>
        private void SetVariableInfo(int VariableRow)
        {
            var VariableInfoColumn = new SerializableDictionary<string, int>();

            var columnNameRow = VariableRow + 1;

            for (int i = 0; i < CsvData.Data[columnNameRow].Count; i++)
            {
                VariableInfoColumn[CsvData.Data[columnNameRow][i]]= i;
            }

            for (int i = VariableRow + 2; i < CsvData.Data.Count; i++)
            {
                if (CsvData.Data[i].Count == 0) continue;
                if (CsvData.Data[i][0] == IDENTIFIER_CUSTOM) return;

                    var variableInfo = new VariableInfo();
                if (CsvData.Data[i][0] != "")
                {
                    variableInfo.Name = CsvData.Data[i][VariableInfoColumn[IDENTIFIER_VARUALBENAME]];
                    variableInfo.Type = CsvData.Data[i][VariableInfoColumn[IDENTIFIER_VARUALBETYPE]];
                    variableInfo.Description = CsvData.Data[i][VariableInfoColumn[IDENTIFIER_VARUALBEDESCRIPTION]];

                    //未登録の場合は追加、登録済みは上書き
                    VariableDic[variableInfo.Name] = variableInfo;
                }
            }
        }

        /// <summary>
        /// カスタム情報を格納する
        /// </summary>
        /// <param name="i">#Customが見つかった行番号</param>
        private void SetCustomInfo(int CustomRow)
        {

            for (int i = CustomRow + 1; i < CsvData.Data.Count; i++)
            {
                if (CsvData.Data[i].Count == 0) continue;
                if (CsvData.Data[i][0] == IDENTIFIER_VARIABLE) return;

                if (CsvData.Data[i][0] != "")
                {
                    var customValue = new List<string>();

                    for(int j = 1; j < CsvData.Data[i].Count; j++)
                    {
                        customValue.Add(CsvData.Data[i][j]);
                    }

                    //未登録の場合は追加、登録済みは上書き
                    CustomValue[CsvData.Data[i][0]] = customValue;
                }
            }
        }

        //保持しているHeaderInfoの情報をCsvDataに設定して保存する
        public void Save()
        {
            var csvData = new CsvData();

            var rowData = new List<string>();

            //ClassName
            rowData.Add(IDENTIFIER_CLASSNAME);
            rowData.Add(ClassName);
            csvData.AddRow(rowData);

            //ParentName
            rowData = new List<string>();
            rowData.Add(IDENTIFIER_PARENTNAME);
            rowData.Add(ParentName);
            csvData.AddRow(rowData);

            //IndexVariable
            rowData = new List<string>();
            rowData.Add(IDENTIFIER_INDEXVARIABLE);
            rowData.Add(IndexVariable);
            csvData.AddRow(rowData);

            //IndexVariable
            rowData = new List<string>();
            rowData.Add(IDENTIFIER_INDEXDUPLICATABLE);
            rowData.Add(IndexDuplicatableString);
            csvData.AddRow(rowData);

            //Custom
            rowData = new List<string>();
            rowData.Add(IDENTIFIER_CUSTOM);
            csvData.AddRow(rowData);

            foreach (var pair in CustomValue)
            {
                rowData = new List<string>();
                rowData.Add(pair.Key);
                foreach(var str in pair.Value)
                {
                    rowData.Add(str);
                }
                csvData.AddRow(rowData);
            }

            //Variable
            rowData = new List<string>();
            rowData.Add(IDENTIFIER_VARIABLE);
            csvData.AddRow(rowData);

            rowData = new List<string>();
            rowData.Add("Name");
            rowData.Add("Type");
            rowData.Add("Description");
            csvData.AddRow(rowData);

            foreach (var variable in VariableDic)
            {
                rowData = new List<string>();
                rowData.Add(variable.Value.Name);
                rowData.Add(variable.Value.EditorViewType);
                rowData.Add(variable.Value.Description);
                csvData.AddRow(rowData);
            }

            csvData.FilePath = FilePath;
            csvData.Save();
            AssetDatabase.ImportAsset(csvData.FilePath);
        }

        public void Update()
        {
            SetHeaderInfo();
        }
    }

}

