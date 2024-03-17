using CatHut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using UnityEditor.VersionControl;


namespace CatHut
{
    public static class MasterDataEditorCommon
    {
        private static int SaveCounter = 0;

        /// <summary>
        /// グローバルテーブルを取得
        /// </summary>
        /// <returns></returns>
        public static TableData GetGlobalTable()
        {
            List<string> folderPathList = MasterDataEditorConfig.settings.CsvMasterDataPathList;

            var _GrobalTableData = new TableData();

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                _GrobalTableData.AddTableData(folderPath);
            }

            return _GrobalTableData;

        }

        /// <summary>
        /// データグループ取得
        /// </summary>
        /// <returns></returns>
        public static SerializableDictionary<string, DataGroup> GetDataGroupDic()
        {
            List<string> folderPathList = MasterDataEditorConfig.settings.CsvMasterDataPathList;

            var _DataGroupDic = new SerializableDictionary<string, DataGroup>();

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                MasterDataEditorCommon.ImportHeaderMultiply(folderPath, ref _DataGroupDic);
            }

            foreach (var folderPath in folderPathList)
            {
                if (!Directory.Exists(folderPath)) { continue; }

                MasterDataEditorCommon.ImportDataMultiply(folderPath, ref _DataGroupDic);
            }

            //整合性チェック
            var keys1 = _DataGroupDic.Keys.ToList();
            foreach (var key1 in keys1)
            {

                var dg = _DataGroupDic[key1];

                var keys2 = dg.FormatedCsvDic.Keys.ToList();
                foreach (var key2 in keys2)
                {
                    var fc = dg.FormatedCsvDic[key2];

                    if (fc.DataPart == null)
                    {
                        dg.FormatedCsvDic.Remove(key2);
                        continue;
                    }

                    if (fc.DataPart.Data == null)
                    {
                        dg.FormatedCsvDic.Remove(key2);
                        continue;
                    }

                    if (fc.DataPart.Data.Count <= 0)
                    {
                        dg.FormatedCsvDic.Remove(key2);
                    }
                }

                if (dg.FormatedCsvDic.Count <= 0)
                {
                    _DataGroupDic.Remove(key1);
                }

            }

            return _DataGroupDic;
        }


        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ImportHeaderMultiply(string folderPath, ref SerializableDictionary<string, DataGroup> ret)
        {

            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);

            if (ret == null)
            {
                ret = new SerializableDictionary<string, DataGroup>();
            }

            //一旦ヘッダ情報を生成(重複時は上書きされる)
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                var dg = new DataGroup();
                dg.SetHeaderInfo(subFolder);

                if (ret.ContainsKey(subFolderName) == true)
                {
                    Debug.LogWarning(subFolderName + "header is Already Registered. " + subFolder + " is Skipped.");
                }
                else
                {
                    ret[subFolderName] = dg;
                }
            }

            //要素のないキーは削除する
            foreach (var key in ret.Keys.ToList())
            {
                //格納状況により調整。
                ret[key].AdjustDicitonaryByHeader();

                if (ret[key].FormatedCsvDic.Count == 0)
                {
                    ret.Remove(key);
                }
            }
        }

        public static HashSet<string> GetChangedDataGroupNameList(List<string> list)
        {

            var ret = new HashSet<string>();
            foreach (var filePath in list)
            {
                if (Regex.IsMatch(Path.GetFileName(filePath), @"^(Data_|Header_).*\.csv$"))
                {
                    var directoryInfo = new DirectoryInfo(filePath).Parent?.Parent;
                    if (directoryInfo != null)
                    {
                        ret.Add(directoryInfo.Name);
                    }
                }
            }
            return ret;

        }


        /// <summary>
        /// フォルダ内にあるヘッダ情報を登録する。
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ImportDataMultiply(string folderPath, ref SerializableDictionary<string, DataGroup> ret)
        {
            var folder = folderPath;

            string[] subFolders = Directory.GetDirectories(folder);


            //データ読み込み
            foreach (string subFolder in subFolders)
            {
                string subFolderName = new DirectoryInfo(subFolder).Name;

                if (ret.ContainsKey(subFolderName))
                {
                    ret[subFolderName].AddData(subFolder);
                }
            }

        }


        public static bool TryConvert<T>(string str, out T ret)
        {
            ret = default(T);
            bool success = true;

            switch (typeof(T))
            {
                case Type t when t == typeof(int):
                    success = int.TryParse(str, out var intResult);
                    ret = (T)(object)intResult;
                    break;
                case Type t when t == typeof(uint):
                    success = uint.TryParse(str, out var uintResult);
                    ret = (T)(object)uintResult;
                    break;
                case Type t when t == typeof(short):
                    success = short.TryParse(str, out var shortResult);
                    ret = (T)(object)shortResult;
                    break;
                case Type t when t == typeof(ushort):
                    success = ushort.TryParse(str, out var ushortResult);
                    ret = (T)(object)ushortResult;
                    break;
                case Type t when t == typeof(long):
                    success = long.TryParse(str, out var longResult);
                    ret = (T)(object)longResult;
                    break;
                case Type t when t == typeof(ulong):
                    success = ulong.TryParse(str, out var ulongResult);
                    ret = (T)(object)ulongResult;
                    break;
                case Type t when t == typeof(float):
                    success = float.TryParse(str, out var floatResult);
                    ret = (T)(object)floatResult;
                    break;
                case Type t when t == typeof(double):
                    success = double.TryParse(str, out var doubleResult);
                    ret = (T)(object)doubleResult;
                    break;
                case Type t when t == typeof(char):
                    success = char.TryParse(str, out var charResult);
                    ret = (T)(object)charResult;
                    break;
                case Type t when t == typeof(bool):
                    success = bool.TryParse(str, out var boolResult);
                    ret = (T)(object)boolResult;
                    break;
                case Type t when t == typeof(byte):
                    success = byte.TryParse(str, out var byteResult);
                    ret = (T)(object)byteResult;
                    break;
                case Type t when t == typeof(sbyte):
                    success = sbyte.TryParse(str, out var sbyteResult);
                    ret = (T)(object)sbyteResult;
                    break;
                case Type t when t == typeof(string):
                    ret = (T)(object)str;
                    success = true;
                    break;
                case Type t when t.IsEnum:      //テーブル値用
                    success = Enum.TryParse(t, str, out var enumResult);
                    ret = (T)enumResult;
                    break;
                default:
                    success = false;
                    Debug.LogWarning($"Unsupported type: {typeof(T)}");
                    break;
            }

            if (!success)
            {
                Debug.LogWarning($"Conversion failed: Cannot convert \"{str}\" to {typeof(T)}.");
            }

            return success;
        }


        public static void RenameAssetsInGroup(string groupName)
        {
            // AddressableAssetSettingsを取得
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // MasterDataグループを検索
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                Debug.LogError(groupName + " group not found.");
                return;
            }

            // グループ内の全てのエントリに対して処理
            foreach (var entry in group.entries)
            {
                string assetPath = entry.AssetPath;
                string oldFileName = Path.GetFileNameWithoutExtension(assetPath);
                string newFileName = oldFileName + "_temp" + SaveCounter.ToString();

                // AssetDatabaseを使用してファイル名を変更
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                string errorMessage = AssetDatabase.RenameAsset(assetPath, newFileName);
                if (string.Empty != errorMessage)
                {
                    Debug.LogError("Could not rename asset: " + assetPath);
                    Debug.LogError(errorMessage);
                    continue;
                }

                // Addressableのエントリも更新
                entry.SetAddress(newFileName);

                Debug.Log($"Asset renamed: {oldFileName}.asset to {newFileName}.asset");
            }

            // Addressablesの設定を保存
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, group, true, true);

            // アセットデータベースをリフレッシュ
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            SaveCounter++;
        }

        

        public static void RenameAssetsBackInGroup(string groupName)
        {
            Regex tempRegex = new Regex(@"_temp\d*$", RegexOptions.Compiled);

            // AddressableAssetSettingsを取得
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // 指定されたグループを検索
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                Debug.LogError(groupName + " group not found.");
                return;
            }

            // グループ内の全エントリに対して処理
            foreach (var entry in group.entries)
            {
                string assetPath = entry.AssetPath;
                string fileName = Path.GetFileName(assetPath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
                string extension = Path.GetExtension(assetPath);

                // ファイル名から"_temp"と数字を削除
                string newFileNameWithoutExtension = tempRegex.Replace(fileNameWithoutExtension, "");

                // AssetDatabaseを使用してファイル名を変更
                string errorMessage = AssetDatabase.RenameAsset(assetPath, newFileNameWithoutExtension);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Debug.LogError("Could not rename asset: " + assetPath);
                    Debug.LogError(errorMessage);
                    continue;
                }

                // Addressableのエントリも更新（コメントアウトされていた行を復活）
                entry.SetAddress(newFileNameWithoutExtension);

                Debug.Log($"Asset renamed back: {fileName} to {newFileNameWithoutExtension + extension}");
            }

            // Addressablesの設定を保存
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, group, true, true);


            // アセットデータベースをリフレッシュ
            AssetDatabase.Refresh();
        }

    }


}
