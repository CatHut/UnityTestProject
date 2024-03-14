#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace CatHut
{
    public class AddressableOperator
    {
        [MenuItem("Tools/CatHut/AddressableOperator/BuildAssets", false, 1)]
        private static void BuildAssets()
        {
            RemoveAllAssets();
            AddAssets();
            AddressableAssetSettings.BuildPlayerContent();
        }

        [MenuItem("Tools/CatHut/AddressableOperator/AddAssets", false, 2)]
        private static void AddAssets()
        {
            var AddressableBuildSetting = AddressableOperatorCommon.GetAddressableBuildSetting();
            var AssSetting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(AddressableOperatorCommon.ADDRESSABLE_ASSET_SETTING_PATH);

            var AssGroups = AssSetting.groups;


            foreach (var data in AddressableBuildSetting.AddressableSettingList)
            {
                var folder = data.FolderPath;

                var parentGroup = AssGroups.FirstOrDefault(x => x.Name == data.Group);

                if(parentGroup == null)
                {
                    Debug.Log("指定されたグループ:" + data.Group + "が見つかりませんでした。");
                    Debug.Log("指定されたグループ:" + data.Group + "を追加します。");
                    parentGroup = CreatePackedAssetsGroup(data.Group, AssSetting);
                }

                var dic = AddressableOperatorCommon.GetGuidFileDic(folder);

                foreach(var keypair in dic)
                {
                    //指定された拡張子のみ登録
                    if (keypair.Value.Contains(data.Extention))
                    {
                        var entry = AssSetting.CreateOrMoveEntry(keypair.Key, parentGroup);
                        entry.SetLabel(data.Group, true);
                        entry.SetAddress(Path.GetFileNameWithoutExtension(entry.address), false);
                    }
                }
            }
        }


        [MenuItem("Tools/CatHut/AddressableOperator/RemoveAsset", false, 3)]
        private static void RemoveAllAssets()
        {
            var AssSetting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(AddressableOperatorCommon.ADDRESSABLE_ASSET_SETTING_PATH);

            List<AddressableAssetEntry> addressableList = new List<AddressableAssetEntry>();
            AssSetting.GetAllAssets(addressableList, false);

            var AddressableBuildSettingList = AddressableOperatorCommon.GetAddressableBuildSetting();

            foreach (var absl in AddressableBuildSettingList.AddressableSettingList)
            {
                foreach (var data in addressableList)
                {
                    if (data.parentGroup == null) { continue; }

                    if (absl.Group == data.parentGroup.Name)
                    {
                        AssSetting.RemoveAssetEntry(data.guid);
                    }
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