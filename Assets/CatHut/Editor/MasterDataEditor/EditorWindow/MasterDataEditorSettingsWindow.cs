using UnityEditor;
using UnityEngine;

public class MasterDataEditorSettingsWindow : EditorWindow
{
    private string csvMasterDataFolder = string.Empty;

    [MenuItem("Tools/CatHut/MasterDataEditor/Settings")]
    public static void ShowWindow()
    {
        GetWindow<MasterDataEditorSettingsWindow>("MasterDataEditor Settings");
    }

    private void OnEnable()
    {
        // 初期化コード
        MasterDataEditorConfig.LoadSettings();
        csvMasterDataFolder = MasterDataEditorConfig.settings.CsvMasterDataPath;
    }    

    private void OnGUI()
    {
        GUILayout.Label("MasterDataEditor Settings", EditorStyles.boldLabel);

        if (!AssetDatabase.IsValidFolder(csvMasterDataFolder))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        csvMasterDataFolder = EditorGUILayout.TextField("CsvMasterDataPath", csvMasterDataFolder);

        if (GUILayout.Button("Save Settings"))
        {
            MasterDataEditorConfig.settings.CsvMasterDataPath = csvMasterDataFolder;
            MasterDataEditorConfig.SaveSettings();
        }
    }
}
