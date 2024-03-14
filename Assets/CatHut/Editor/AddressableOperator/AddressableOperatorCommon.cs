#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using static AddressableOperatorConfig;

namespace CatHut
{
    public class AddressableOperatorCommon
    {
        public static readonly string ADDRESSABLE_ASSET_SETTING_PATH = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        private static readonly int CSV_GROUP_IDX = 0;  //グループ
        private static readonly int CSV_FOLDER_IDX = 1; //フォルダ
        private static readonly int CSV_EXT_IDX = 2;    //拡張子

        /// <summary>
        /// Addressableの対象にするフォルダリストを取得
        /// </summary>
        /// <returns>Groupをキーとしたフォルダリスト</returns>
        public static AddressableOperatorConfigData GetAddressableBuildSetting()
        {


            return null;

        }

        /// <summary>
        /// 指定フォルダ以下のファイルのGUIDを取得する
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>GUIDパスリスト</returns>
        public static List<string> GetGuidList(string folder)
        {
            var guids = AssetDatabase.FindAssets("", new string[] { folder }).ToList();
            return guids;
        }

        public static List<string> GetGuidList(List<string> folders)
        {
            var guids = AssetDatabase.FindAssets("", folders.ToArray()).ToList();
            return guids;
        }


        /// <summary>
        /// 指定フォルダ以下のファイルパスを取得する
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>ファイルパスリスト</returns>
        public static List<string> GetFileList(string folder)
        {
            var guids = GetGuidList(folder);
            var pathList = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();

            return pathList;
        }

        public static List<string> GetFileList(List<string> folders)
        {
            var guids = GetGuidList(folders);
            var pathList = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();
            return guids;
        }

        /// <summary>
        /// 指定フォルダ以下のGUIDキー、パスバリューのディクショナリを取得する
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetGuidFileDic(string folder)
        {
            var guids = GetGuidList(folder);
            var pathList = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();

            Dictionary<string, string> ret = Enumerable.Range(0, guids.Count).ToDictionary(i => guids[i], i => pathList[i]);

            return ret;
        }

        public static Dictionary<string, string> GetGuidFileDic(List<string> folders)
        {
            var guids = GetGuidList(folders);
            var pathList = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();

            Dictionary<string, string> ret = Enumerable.Range(0, guids.Count).ToDictionary(i => guids[i], i => pathList[i]);

            return ret;
        }

    }



}
#endif