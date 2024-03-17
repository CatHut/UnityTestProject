#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using static AddressableOperatorConfig;

namespace CatHut
{
    public class AddressableOperatorCommon
    {

        private static readonly int CSV_GROUP_IDX = 0;  //グループ
        private static readonly int CSV_FOLDER_IDX = 1; //フォルダ
        private static readonly int CSV_EXT_IDX = 2;    //拡張子



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


        public static void ProcessAddressableSetting(AddressableAssetSettings assSettings, AddressableOperatorConfig.AddressableSetting setting)
        {
            var parentGroup = assSettings.groups.FirstOrDefault(g => g.Name == setting.Group);

            if (parentGroup == null)
            {
                Debug.Log("指定されたグループ:" + setting.Group + "が見つかりませんでした。");
                Debug.Log("指定されたグループ:" + setting.Group + "を追加します。");
                parentGroup = CreatePackedAssetsGroup(setting.Group, assSettings);
            }

            if (!assSettings.GetLabels().Contains(setting.Group))
            {
                assSettings.AddLabel(setting.Group);
            }

            var dic = AddressableOperatorCommon.GetGuidFileDic(setting.FolderPath);

            foreach (var keypair in dic)
            {
                if (keypair.Value.Contains(setting.Extention))
                {
                    var entry = assSettings.CreateOrMoveEntry(keypair.Key, parentGroup);
                    entry.SetLabel(setting.Group, true);
                    entry.SetAddress(Path.GetFileNameWithoutExtension(entry.address), false);
                }
            }
        }


        private static AddressableAssetGroup CreatePackedAssetsGroup(string groupName, AddressableAssetSettings setting)
        {
            AddressableAssetGroup newGroup = setting.CreateGroup(groupName, false, false, false, null);

            var groupSchema = newGroup.AddSchema<BundledAssetGroupSchema>();
            groupSchema.BuildPath.SetVariableByName(setting, AddressableAssetSettings.kLocalBuildPath);
            groupSchema.LoadPath.SetVariableByName(setting, AddressableAssetSettings.kLocalLoadPath);
            groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;

            var updateSchema = newGroup.AddSchema<ContentUpdateGroupSchema>();

            return newGroup;

        }

    }



}
#endif