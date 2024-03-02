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
        //設定ファイルをロード
        MasterDataEditorConfig.LoadSettings();
        csvMasterDataFolder = MasterDataEditorConfig.settings.CsvMasterDataPath;
    }    

    private void OnGUI()
    {
        //表題
        GUILayout.Space(20);
        GUILayout.Label("MasterDataEditor Settings", EditorStyles.boldLabel);

        //指定されたフォルダがない場合に警告を表示
        GUILayout.Space(5);
        if (!AssetDatabase.IsValidFolder(csvMasterDataFolder))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }

        //設定項目
        csvMasterDataFolder = EditorGUILayout.TextField("CsvMasterDataPath", csvMasterDataFolder);

        // 保存ボタン
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Settings", GUILayout.Width(200)))
        {
            MasterDataEditorConfig.settings.CsvMasterDataPath = csvMasterDataFolder;
            MasterDataEditorConfig.SaveSettings();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}
