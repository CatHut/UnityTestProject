#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
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
            var AddressableOperationConfigData = AddressableOperatorConfig.LoadSettings();
            var AssSetting = AddressableAssetSettingsDefaultObject.Settings;

            //MasterData
            ProcessAddressableSetting(AssSetting, AddressableOperationConfigData.MasterDataAddressableSetting);


            // その他
            foreach (var setting in AddressableOperationConfigData.AddressableSettingList)
            {
                ProcessAddressableSetting(AssSetting, setting);
            }
        }


        private static void ProcessAddressableSetting(AddressableAssetSettings assSettings, AddressableOperatorConfig.AddressableSetting setting)
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




        [MenuItem("Tools/CatHut/AddressableOperator/RemoveAsset", false, 3)]
        private static void RemoveAllAssets()
        {
            var AssSetting = AddressableAssetSettingsDefaultObject.Settings;

            List<AddressableAssetEntry> addressableList = new List<AddressableAssetEntry>();
            AssSetting.GetAllAssets(addressableList, false);

            var AddressableOperationConfigData = AddressableOperatorConfig.LoadSettings();

            foreach (var absl in AddressableOperationConfigData.AddressableSettingList)
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