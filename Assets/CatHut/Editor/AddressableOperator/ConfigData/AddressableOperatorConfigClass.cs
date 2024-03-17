using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// マスターデータエディターの設定管理
/// </summary>
[System.Serializable]
public static class AddressableOperatorConfig
{
    public static AddressableOperatorConfigData settings;

    /// <summary>
    /// 設定データの保存場所
    /// </summary>
    private static string ConfigDataFolder = Path.Combine(Application.dataPath, "Resources/CatHut");
    private static string ConfigDataFile = "AddressableSettings.json";

    static AddressableOperatorConfig()
    {
        settings = LoadSettings();
    }


    /// <summary>
    /// 設定を保存する
    /// </summary>
    public static void SaveSettings()
    {
        if (!System.IO.Directory.Exists(ConfigDataFolder))
        {
            System.IO.Directory.CreateDirectory(ConfigDataFolder);
        }
        string json = JsonUtility.ToJson(settings);

        var path = System.IO.Path.Combine(ConfigDataFolder, ConfigDataFile);
        System.IO.File.WriteAllText(path, json);

        AssetDatabase.ImportAsset(path);
    }

    /// <summary>
    /// 設定を読み込む
    /// </summary>
    /// <returns>プロジェクト設定</returns>
    public static AddressableOperatorConfigData LoadSettings()
    {
        string path = System.IO.Path.Combine(ConfigDataFolder, ConfigDataFile);
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<AddressableOperatorConfigData>(json);
        }
        return new AddressableOperatorConfigData();
    }

    /// <summary>
    /// 設定データクラス
    /// </summary>
    [System.Serializable]
    public class AddressableOperatorConfigData
    {
        //マスターデータパス設定
        public AddressableSetting MasterDataAddressableSetting;

        //アセットパス設定
        public List<AddressableSetting> AddressableSettingList = new List<AddressableSetting>();

    }

    [System.Serializable]
    public class AddressableSetting
    {

        public string Group;
        public string FolderPath;
        public string Extention;

        public AddressableSetting()
        {

        }

        public AddressableSetting(string group, string path, string ext)
        {
            Group = group;
            FolderPath = path;
            Extention = ext;
        }
    }
}
