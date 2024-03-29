using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// マスターデータエディターの設定管理
/// </summary>
[System.Serializable]
public static class MasterDataEditorConfig
{
    public static MasterDataEditorConfigData settings;

    /// <summary>
    /// 設定データの保存場所
    /// </summary>
    private static string ConfigDataFolder = Path.Combine(Application.dataPath, "Resources/CatHut");
    private static string ConfigDataFile = "MasterDataEditorSettings.json";

    static MasterDataEditorConfig()
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
    public static MasterDataEditorConfigData LoadSettings()
    {
        string path = System.IO.Path.Combine(ConfigDataFolder, ConfigDataFile);
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<MasterDataEditorConfigData>(json);
        }
        return new MasterDataEditorConfigData();
    }

    /// <summary>
    /// 設定データクラス
    /// </summary>
    [System.Serializable]
    public class MasterDataEditorConfigData
    {
        //全般設定
        public List<string> CsvMasterDataPathList = new List<string>();
        public string CreatedScriptableObjectClassPath = "";
        public string ScriptableObjectInstancePath = "";
        public string CreatedMasterDataClassPath = "";
        public string CreatedImporterPath = "";
        public string CreatedReflectorPath = "";


        //DataEditor設定
        public string SelectedMasterDataPath = "";

    }
}

