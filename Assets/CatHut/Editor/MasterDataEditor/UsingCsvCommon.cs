#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace CatHut
{
    public class UsingCsvCommon
    {
        readonly public static string TargetWorkSheetMark = "$";
        readonly public static string TableDeclareWorkSheetName = "Tables";

        readonly static string ClassNameHeading = "$ClassName";
        readonly static string CollectionTypeHeading = "$CollectionType";
        readonly static string IdDuplicatableHeading = "$IdDuplicatable";
        readonly static string TypeHeading = "$Type";
        readonly static string ValuableNameHeading = "$ValuableName";

        readonly public static string DictNotDublicatableTemplate = "$DictNotDublicatableTemplateFile";
        readonly public static string AssetPostProcessImporterTemplate = "$AssetPostProcessImporterTemplate";
        readonly public static string CsvImporterParentTemplate = "$CsvImporterParentTemplate";
        readonly public static string CsvImporterTemplate = "$Importer_Template";
        readonly public static string CsvImporterPartTemplate = "$CsvImporterPartTemplate";
        readonly public static string CsvReflectorTemplate = "$Reflector_Template";
        readonly public static string CsvReflectorPartTemplate = "$CsvReflectorPartTemplate";
        readonly public static string MasterDataTemplate = "$MasterDataTemplate";

        readonly public static string EditorWindowTemplate = "$EditorWindowTemplate";
        readonly public static string CustomTreeViewTemplate = "$CustomTreeViewTemplate";

        readonly public static string InitializeExcelImporter_part = "$InitializeExcelImporter_part";
        readonly public static string InitializeMasterData = "$InitializeMasterData";

        readonly public static string CreatedAssetFoldeName = "CreatedAssets";

        readonly public static List<string> ExclusionTableName = new List<string>() { "CollectionType", "Type" };


        /// <summary>
        /// 指定されたファイルをプロジェクト内から探す
        /// </summary>
        /// <param name="file">ファイル名</param>
        /// <returns>見つかったファイルパスリスト</returns>
        static public List<string> GetFileList(string file)
        {
            //FolderSettingsファイルをすべて探す
            var fileGUIDs = AssetDatabase.FindAssets(file);
            var fileList = new List<string>();

            foreach (string guid in fileGUIDs)
            {
                fileList.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            return fileList;
        }

        /// <summary>
        /// 設定ファイルから探索対象のフォルダリストを取得する
        /// </summary>
        /// <returns></returns>
        static public List<string> GetExcelFolderList()
        {
            //FolderSettingsファイルをすべて探す
            //var FolderSettingFile = GetFileList(ImportFolderListFile);
            List<string> FolderSettingFile = new List<string>();

            List<string> ExcelFolderList = new List<string>();
            //foreach (string files in FolderSettingFile)
            //{
            //    var temp = File.ReadAllLines(files);
            //    ExcelFolderList.AddRange(temp);
            //}

            //必要あればここで整形する

            return ExcelFolderList;

        }

        /// <summary>
        /// 設定ファイルからエクセルリストを取得
        /// </summary>
        /// <returns>対象ファイルリスト</returns>
        static public List<string> GetMasterDataExcelList()
        {
            //FolderSettingsファイルをすべて探す
            //var MasterDataExcelList = GetFileList(MasterDataExcelListFile);
            List<string> MasterDataExcelList = new List<string>();

            List<string> ExcelFileList = new List<string>();
            foreach (string files in MasterDataExcelList)
            {
                var temp = File.ReadAllLines(files);
                ExcelFileList.AddRange(temp);
            }

            //必要あればここで整形する

            return ExcelFileList;

        }

        /// <summary>
        /// マスターデータのエクセルディクショナリを取得
        /// </summary>
        /// <param name="folders">探索フォルダリスト</param>
        /// <param name="files">探索ファイル名リスト</param>
        /// <returns>探索結果</returns>
        static public Dictionary<string, string> GetMasterDataExcelPath(List<string> folders, List<string> files)
        {
            var MasterDataExcelGUIDs = new List<string>();
            foreach (var file in files)
            {
                MasterDataExcelGUIDs.AddRange(AssetDatabase.FindAssets(file));
            }

            //FolderSettingsファイルをすべて探す
            var MasterDataExcelPath = new List<string>();

            foreach (string guid in MasterDataExcelGUIDs)
            {
                MasterDataExcelPath.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            var MasterDataDic = new Dictionary<string, string>();
            foreach (var path in MasterDataExcelPath)
            {
                foreach (var folder in folders)
                {

                    if (path.Contains(folder))
                    {
                        var file = Path.GetFileName(path);

                        if (!MasterDataDic.ContainsKey(file))
                        {
                            if ((file.Contains("xlsx") || file.Contains("xlsm")) && !file.Contains("~$"))
                                MasterDataDic.Add(file, path);
                        }
                    }
                }

            }

            //必要あればここで整形する

            return MasterDataDic;

        }

        ///// <summary>
        ///// エクセルの内容からデータのフォーマット情報を取得する
        ///// </summary>
        ///// <param name="wb">ワークブック</param>
        ///// <param name="SheetName">シート名</param>
        ///// <param name="edf">フォーマット格納データ</param>
        //public static void GetExcelDataFormat(XLWorkbook wb, string SheetName, ref ExcelSheetFormat edf)
        //{
        //    //シート名取得
        //    edf.SheetName = SheetName;

        //    //クラス名取得
        //    edf.ClassName = GetExcelValueByHeading(wb, SheetName, ClassNameHeading);
        //    edf.ClassName += "Class";

        //    //コレクションタイプ取得
        //    edf.CollectionType = GetExcelValueByHeading(wb, SheetName, CollectionTypeHeading);

        //    //ID重複可否取得

        //    var temp = GetExcelValueByHeading(wb, SheetName, IdDuplicatableHeading);

        //    edf.IdDuplicatable = false;
        //    if("True" == temp
        //    || "true" == temp
        //    || "TRUE" == temp
        //        )
        //    {
        //        edf.IdDuplicatable = true;
        //    }

        //    //変数リスト取得
        //    edf.ValuableDefine = GetExcelValuableDefine(wb, SheetName);

        //}

        ///// <summary>
        ///// エクセルの内容からEnum変換するテーブル情報を取得
        ///// </summary>
        ///// <param name="ws">ワークシート</param>
        ///// <param name="enumDic">Enum変換するテーブルデータ</param>
        //public static void GetEnumDeclare(IXLWorksheet ws, ref Dictionary<string, Dictionary<string, string>> enumDic)
        //{
        //    //変数名はテーブルから取得する
        //    var tables = ws.Tables;

        //    //シートにあるテーブルを処理
        //    foreach (var temp in tables)
        //    {

        //        if (ExclusionTableName.Contains(temp.Name))
        //        {
        //            continue;
        //        }

        //        if (enumDic == null)
        //        {
        //            enumDic = new Dictionary<string, Dictionary<string, string>>();
        //        }

        //        enumDic.Add(temp.Name, new Dictionary<string, string>());


        //        var dic = enumDic[temp.Name];

        //        foreach (var row in temp.DataRange.Rows())
        //        {
        //            var key = row.Cell(1).GetString();
        //            var val = row.Cell(2).GetString();

        //            string duplicateKey = null;

        //            foreach(var kvp in dic)
        //            {
        //                if(kvp.Value == val)
        //                {
        //                    duplicateKey = kvp.Key;
        //                }
        //            }

        //            if (duplicateKey == null)
        //            {
        //                dic.Add(key, val);
        //            }
        //            else
        //            {
        //                dic.Add(key, duplicateKey);
        //            }

        //        }

        //    }

        //}

        ///// <summary>
        ///// 規定されたヘッダからデータを取得する
        ///// </summary>
        ///// <param name="wb">ワークブック</param>
        ///// <param name="SheetName">シート名</param>
        ///// <param name="Heading">ヘッダパラメータ名</param>
        ///// <returns>ヘッダ部に設定された値</returns>
        //public static string GetExcelValueByHeading(XLWorkbook wb, string SheetName, string Heading)
        //{
        //    var cells = wb.Worksheet(SheetName).Search(Heading);
        //    IXLCell cell = null;

        //    foreach (var temp in cells)
        //    {
        //        cell = temp;
        //        break;
        //    }

        //    IXLCell retCell = null;
        //    if (cell != null)
        //    {
        //        retCell = cell.CellRight();
        //    }

        //    if (retCell != null)
        //    {
        //        Debug.Log(Heading + ": " + retCell.Value.ToString());
        //        return retCell.Value.ToString();
        //    }

        //    return null;
        //}


        ///// <summary>
        ///// 列名と型のディクショナリを取得する
        ///// </summary>
        ///// <param name="wb">ワークブック</param>
        ///// <param name="SheetName">シート名</param>
        ///// <returns>列名と型のディクショナリ</returns>
        //public static Dictionary<string, string> GetExcelValuableDefine(XLWorkbook wb, string SheetName)
        //{

        //    //変数名はテーブルから取得する
        //    //シートにテーブルは１つしかない
        //    var table = wb.Worksheet(SheetName).Tables.FirstOrDefault(); ;

        //    Dictionary<string, string> retDic = new Dictionary<string, string>();

        //    if (table != null)
        //    {
        //        foreach (var cell in table.HeadersRow().Cells())
        //        {
        //            var ValuableName = cell.Value.ToString();

        //            var Type = wb.Worksheet(SheetName).Cell(cell.Address.RowNumber - 1, cell.Address.ColumnNumber).Value.ToString();

        //            retDic.Add(ValuableName, Type);
        //        }
        //    }

        //    if (retDic.Count > 0)
        //    {
        //        Debug.Log("Created ValuableDic:" + retDic.Count.ToString() + " Values");
        //        return retDic;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 列名と列番号のディクショナリを取得する
        ///// </summary>
        ///// <param name="wb">ワークブック</param>
        ///// <param name="SheetName">シート名</param>
        ///// <returns>列名と列番号のディクショナリ</returns>
        //public static Dictionary<string, int> GetExcelValuableColumn(XLWorkbook wb, string SheetName)
        //{

        //    //変数名はテーブルから取得する
        //    //シートにテーブルは１つしかない
        //    var table = wb.Worksheet(SheetName).Tables.FirstOrDefault(); ;

        //    Dictionary<string, int> retDic = new Dictionary<string, int>();

        //    if (table != null)
        //    {
        //        var tableLeft = table.RangeAddress.FirstAddress.ColumnNumber;

        //        foreach (var cell in table.HeadersRow().Cells())
        //        {
        //            var ValuableName = cell.Value.ToString();

        //            var Column = cell.Address.ColumnNumber - tableLeft + 1;

        //            retDic.Add(ValuableName, Column);
        //        }
        //    }

        //    if (retDic.Count > 0)
        //    {
        //        Debug.Log("Created ValuableColumnDic:" + retDic.Count.ToString() + " Values");
        //        return retDic;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 指定された値が格納されているテーブル内の行番号を取得
        ///// </summary>
        ///// <param name="table">テーブル</param>
        ///// <param name="columnNumber">列番号</param>
        ///// <param name="cellValue">探索する値</param>
        ///// <returns></returns>
        //public static int GetTableRowNumber(IXLTable table, int columnNumber, string cellValue)
        //{
        //    var tableTop = table.RangeAddress.FirstAddress.RowNumber;

        //    foreach (var row in table.Rows())
        //    {
        //        if (row.Cell(columnNumber).Value.ToString() == cellValue)
        //        {
        //            return row.RowNumber() - tableTop + 1;
        //        }
        //    }

        //    return -1; // 見つからなかった場合
        //}


        ///// <summary>
        ///// 指定されたテーブルを取得
        ///// </summary>
        ///// <param name="wb">ワークブック</param>
        ///// <param name="SheetName">シート名</param>
        ///// <returns></returns>
        //public static IXLTable GetExcelSheetTable(XLWorkbook wb, string SheetName)
        //{
        //    //シートにテーブルは１つしかない前提
        //    var table = wb.Worksheet(SheetName).Tables.FirstOrDefault(); ;

        //    return table;
        //}


        /// <summary>
        /// インポート指定のあるエクセルを取得
        /// </summary>
        /// <param name="assetsList"></param>
        /// <param name="folderList"></param>
        /// <returns></returns>
        public static List<string> GetImportCsvList(List<string> assetsList, List<string> folderList)
        {
            var ImportCsvList = new List<string>();

            //変更のあったフォルダが含まれているパスのみを取得してインポートするCSVを抽出
            foreach (var asset in assetsList)
            {
                foreach (var folder in folderList)
                {
                    if (asset.Contains(folder))
                    {
                        var file = Path.GetFileName(asset);
                        if ((file.Contains("Data_") && file.Contains(".csv")) && !file.Contains("~$"))
                        {
                            ImportCsvList.Add(asset);
                        }
                    }
                }
            }
            return ImportCsvList;
        }


        /// <summary>
        /// 設定ファイルに記述されたファイルを取得
        /// </summary>
        /// <returns>見つかったエクセルファイルのリスト</returns>
        public static List<string> GetAllExcelList()
        {
            var ImportExcelList = new List<string>();

            var folderList = GetExcelFolderList();
            var ExcelList = GetMasterDataExcelList();

            //フォルダリスト以下のエクセルを探索
            foreach (var folder in folderList)
            {
                var files = new List<string>(Directory.GetFiles(folder, "*.xls?", SearchOption.TopDirectoryOnly));

                foreach (var file in files)
                {
                    foreach (var excelName in ExcelList)
                    {
                        if ((file.Contains(excelName + ".xlsx") || file.Contains(excelName + ".xlsm")) && !file.Contains("~$") && !file.Contains(".meta"))
                        {
                            ImportExcelList.Add(file);

                        }
                    }
                }
            }

            return ImportExcelList;
        }

        /// <summary>
        /// エクセルのパス名を取得
        /// </summary>
        /// <param name="file">ファイル名</param>
        /// <returns>パス</returns>
        public static string GetExcelFilePath(string file)
        {
            var ImportExcelList = GetAllExcelList();
            var path = "";

            foreach (var excelName in ImportExcelList)
            {
                if ((excelName.Contains(file + ".xlsx") || excelName.Contains(file + ".xlsm")))
                {
                    path = excelName;
                }
            }

            return path;
        }

        /// <summary>
        /// Excelで設定されるテーブル名を調整する
        /// </summary>
        /// <param name="type">テーブル名（Enum型名）</param>
        /// <returns>Enum型名文字列</returns>
        public static string GetEnumTypeName(string type)
        {
            var typeStr = type;
            typeStr = typeStr.Replace(UsingCsvCommon.TableDeclareWorkSheetName + "[", "");
            typeStr = typeStr.Replace("]", "");

            var enumName = Regex.Replace(typeStr, @"([a-z])([A-Z])", "$1_$2");
            enumName = Regex.Replace(enumName, @"([A-Z]{2,})([A-Z][a-z])", "$1_$2");
            return enumName.ToUpper();

        }

        /// <summary>
        /// 文字列からIDを抽出する（基本カンマ区切り想定、10-15のような記述で１０から１５まで登録する）
        /// </summary>
        /// <param name="ids">文字列</param>
        /// <returns>IDリスト</returns>
        public static List<string> GetIdList(string ids)
        {
            //一旦カンマ区切りで整形
            var addIdList = ids.Split(',').ToList();
            for (int i = addIdList.Count - 1; i >= 0; i--)
            {
                addIdList[i] = addIdList[i].Trim();
                if (addIdList[i] == "") { addIdList.RemoveAt(i); }
            }

            var ret = new List<string>();
            foreach (string idStr in addIdList)
            {
                if (idStr.Contains("-"))
                {
                    string[] range = idStr.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            ret.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    ret.Add(idStr);
                }
            }

            return ret;
        }

    }

    public class ExcelSheetFormat
    {
        public string SheetName;
        public string ClassName;
        public string CollectionType;
        public bool IdDuplicatable;
        public Dictionary<string, string> ValuableDefine;
        public Dictionary<string, string> ValuableColumn;
    }


    public class ExcelDataDictionary {

        public Dictionary<string, Dictionary<string, string>> EnumDic;
        public Dictionary<string, ExcelSheetFormat> ExcelSheetDic;

        public ExcelDataDictionary()
        {
            EnumDic = new Dictionary<string, Dictionary<string, string>>();
            ExcelSheetDic = new Dictionary<string, ExcelSheetFormat>();
        }

    }


}


#endif