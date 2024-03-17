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
            AddressableOperatorCommon.ProcessAddressableSetting(AssSetting, AddressableOperationConfigData.MasterDataAddressableSetting);


            // その他
            foreach (var setting in AddressableOperationConfigData.AddressableSettingList)
            {
                AddressableOperatorCommon.ProcessAddressableSetting(AssSetting, setting);
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




    }
}
#endif