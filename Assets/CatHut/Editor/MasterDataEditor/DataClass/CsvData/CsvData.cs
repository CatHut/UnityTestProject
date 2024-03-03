using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CatHut
{
    public class CsvData
    {
        /// <summary>
        /// CSVデータ
        /// </summary>
        private List<List<string>> data;

        /// <summary>
        /// 列名の辞書 key:列名 , value:インデックス
        /// </summary>
        private Dictionary<string, int> ColumnDic;

        /// <summary>
        /// 列名のタイトル行
        /// </summary>
        private int ColumnTitleRow = 0;

        /// <summary>
        /// ファイルのフルパス
        /// </summary>
        private string filePath;

        private Encoding FileEncode = Encoding.UTF8;


        public CsvData()
        {
            data = new List<List<string>>();
        }

        public CsvData(string filePath)
        {
            this.filePath = filePath;
            this.Load();
        }

        public CsvData(string filePath, Encoding encoding)
        {
            this.FileEncode = encoding;
            this.filePath = filePath;
            this.Load();
        }


        public CsvData(string filePath, int ColumnTitleRow)
        {
            this.filePath = filePath;
            this.ColumnTitleRow = ColumnTitleRow;
            this.Load();
        }


        public List<List<string>> Data
        {
            get { return this.data; }
        }

        public string FilePath
        {
            set { filePath = value; }
            get { return this.filePath; }
        }

        public void Save()
        {
            CatHutDiag.FunctionCalled();
            using (var writer = new StreamWriter(this.filePath, false, FileEncode))
            {
                foreach (var row in this.data)
                {
                    var quotedRow = new List<string>();
                    foreach (var item in row)
                    {
                        if (item.Contains(",") || item.Contains("\"") || item.Contains("\r") || item.Contains("\n"))
                        {
                            var quotedItem = "\"" + item.Replace("\"", "\"\"") + "\"";
                            quotedRow.Add(quotedItem);
                        }
                        else
                        {
                            quotedRow.Add(item);
                        }
                    }
                    writer.WriteLine(string.Join(",", quotedRow));
                }
            }
        }

        public void Load()
        {
            CatHutDiag.FunctionCalled();
            if (this.data == null)
            {
                using (var reader = new CsvReader(this.filePath, FileEncode))
                {
                    this.data = reader.ReadToEnd();
                }

                ColumnDic = new Dictionary<string, int>();

                int i = 0;
                foreach (string str in data[ColumnTitleRow])
                {
                    ColumnDic.Add(str, i++);
                }
            }
        }

        public void Reload()
        {
            CatHutDiag.FunctionCalled();
            this.data = null;
            this.Load();
        }

        public void AddColumn(string col)
        {
            CatHutDiag.FunctionCalled();
            data[0].Add(col);
            int colIndex = data[0].Count - 1;
            for (int i = 1; i < data.Count; i++)
            {
                data[i].Add("");
            }
            ColumnDic.Add(col, colIndex);
        }

        public void AddRow(List<string> newRow)
        {
            CatHutDiag.FunctionCalled();

            this.data.Add(newRow);
        }

        public void AddRow()
        {
            CatHutDiag.FunctionCalled();
            List<string> row = new List<string>();
            for (int i = 0; i < data[0].Count; i++)
            {
                row.Add("");
            }
            data.Add(row);
        }


        public void SetValue<T>(int rowIndex, int columnIndex, T newValue)
        {
            CatHutDiag.FunctionCalled();
            if (typeof(T) == typeof(double))
            {
                this.data[rowIndex][columnIndex] = ((double)(object)newValue).ToString("0.########");
            }
            else if (typeof(T) == typeof(float))
            {
                this.data[rowIndex][columnIndex] = ((float)(object)newValue).ToString("0.########");
            }
            else
            {
                this.data[rowIndex][columnIndex] = newValue.ToString();
            }
        }

        public void SetValue<T>(int rowIndex, string columnName, T newValue)
        {
            CatHutDiag.FunctionCalled();
            if (!this.ColumnDic.ContainsKey(columnName))
            {
                throw new ArgumentException("The specified column name does not exist.");
            }

            int columnIndex = this.ColumnDic[columnName];

            if (typeof(T) == typeof(double))
            {
                this.data[rowIndex][columnIndex] = ((double)(object)newValue).ToString("0.########");
            }
            else if (typeof(T) == typeof(float))
            {
                this.data[rowIndex][columnIndex] = ((float)(object)newValue).ToString("0.########");
            }
            else
            {
                this.data[rowIndex][columnIndex] = newValue.ToString();
            }
        }



        public bool GetValue<T>(int rowIndex, int columnIndex, out T value)
        {
            CatHutDiag.FunctionCalled();
            value = default(T);
            if (rowIndex >= this.data.Count || columnIndex >= this.data[rowIndex].Count)
            {
                return false;
            }

            return GetValueCommon(this.data[rowIndex][columnIndex], out value);
        }

        public bool GetDataValue<T>(int rowIndex, int columnIndex, out T value)
        {
            CatHutDiag.FunctionCalled();

            int dataRowIndex = rowIndex + ColumnTitleRow + 1;

            value = default(T);
            if (dataRowIndex >= this.data.Count || columnIndex >= this.data[dataRowIndex].Count)
            {
                return false;
            }

            return GetValueCommon(this.data[dataRowIndex][columnIndex], out value);
        }

        public bool GetValue<T>(int rowIndex, string columnName, out T value)
        {
            CatHutDiag.FunctionCalled();
            value = default(T);
            if (rowIndex >= this.data.Count)
            {
                return false;
            }

            int columnIndex = this.data[0].FindIndex(x => x == columnName);
            if (columnIndex < 0)
            {
                return false;
            }

            return GetValueCommon(this.data[rowIndex][columnIndex], out value);
        }


        public bool GetDataValue<T>(int rowIndex, string columnName, out T value)
        {
            CatHutDiag.FunctionCalled();
            value = default(T);
            if (rowIndex >= this.data.Count)
            {
                return false;
            }

            int columnIndex = this.data[0].FindIndex(x => x == columnName);
            if (columnIndex < 0)
            {
                return false;
            }

            int dataRowIndex = rowIndex + ColumnTitleRow + 1;

            return GetValueCommon(this.data[dataRowIndex][columnIndex], out value);
        }

        private bool GetValueCommon<T>(string str, out T value)
        {
            CatHutDiag.FunctionCalled();
            bool success = false;

            if (typeof(T) == typeof(int))
            {
                int intValue;
                success = int.TryParse(str, out intValue);
                value = (T)(object)intValue;
            }
            else if (typeof(T) == typeof(double))
            {
                double doubleValue;
                success = double.TryParse(str, out doubleValue);
                value = (T)(object)doubleValue;
            }
            else if (typeof(T) == typeof(float))
            {
                float floatValue;
                success = float.TryParse(str, out floatValue);
                value = (T)(object)floatValue;
            }
            else if (typeof(T) == typeof(string))
            {
                value = (T)(object)str;
                success = true;
            }
            else
            {
                throw new ArgumentException("The type parameter must be int, double, float, or string.");
            }

            return success;
        }

        public bool GetColumnValues<T>(int columnIndex, out List<T> values)
        {
            CatHutDiag.FunctionCalled();
            values = new List<T>();

            if (columnIndex < 0 || columnIndex >= data[ColumnTitleRow].Count)
            {
                return false;
            }

            for (int i = ColumnTitleRow + 1; i < data.Count; i++)
            {
                T value;
                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)data[i][columnIndex];
                }
                else
                {
                    bool success = false;
                    if (typeof(T) == typeof(int))
                    {
                        success = int.TryParse(data[i][columnIndex], out int intValue);
                        value = (T)(object)intValue;
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        success = double.TryParse(data[i][columnIndex], out double doubleValue);
                        value = (T)(object)doubleValue;
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        success = float.TryParse(data[i][columnIndex], out float floatValue);
                        value = (T)(object)floatValue;
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported type '{typeof(T)}'.");
                    }
                    if (!success)
                    {
                        return false;
                    }
                }
                values.Add(value);
            }

            return true;
        }

        public bool GetColumnValues<T>(string columnName, out List<T> values)
        {
            CatHutDiag.FunctionCalled();
            values = new List<T>();

            if (!this.ColumnDic.ContainsKey(columnName))
            {
                return false;
            }

            int columnIndex = this.ColumnDic[columnName];

            return GetColumnValues(columnIndex, out values);
        }


        public bool GetColumnDataValues<T>(int columnIndex, out List<T> columnData)
        {
            columnData = new List<T>();

            if (columnIndex < 0 || columnIndex >= data[ColumnTitleRow].Count)
            {
                return false;
            }

            for (int i = ColumnTitleRow + 1; i < data.Count; i++)
            {
                T value;
                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)data[i][columnIndex];
                }
                else
                {
                    bool success = false;
                    if (typeof(T) == typeof(int))
                    {
                        success = int.TryParse(data[i][columnIndex], out int intValue);
                        value = (T)(object)intValue;
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        success = double.TryParse(data[i][columnIndex], out double doubleValue);
                        value = (T)(object)doubleValue;
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        success = float.TryParse(data[i][columnIndex], out float floatValue);
                        value = (T)(object)floatValue;
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported type '{typeof(T)}'.");
                    }
                    if (!success)
                    {
                        return false;
                    }
                }
                columnData.Add(value);
            }

            return true;
        }

        public bool GetColumnDataValues<T>(string columnName, out List<T> columnData)
        {
            columnData = new List<T>();

            if (!this.ColumnDic.ContainsKey(columnName))
            {
                return false;
            }

            int columnIndex = this.ColumnDic[columnName];

            return GetColumnDataValues(columnIndex, out columnData);
        }

        public List<List<string>> GetRows(int startIndex)
        {
            return data.Skip(startIndex).ToList();
        }


    }
}
