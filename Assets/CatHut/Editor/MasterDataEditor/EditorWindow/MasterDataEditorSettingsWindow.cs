using UnityEditor;
using UnityEngine;

public class MasterDataEditorSettingsWindow : EditorWindow
{
    private string csvMasterDataFolder = string.Empty;
    private string scriptableObjectInstanceFolder = string.Empty;
    private string createdScriptableObjectClassFolder = string.Empty;

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
        scriptableObjectInstanceFolder = MasterDataEditorConfig.settings.ScriptableObjectInstancePath;
        createdScriptableObjectClassFolder = MasterDataEditorConfig.settings.CreatedScriptableObjectClassPath;
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

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(createdScriptableObjectClassFolder))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        createdScriptableObjectClassFolder = EditorGUILayout.TextField("CreatedScriptableObjectClassPath", createdScriptableObjectClassFolder);

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(scriptableObjectInstanceFolder))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        scriptableObjectInstanceFolder = EditorGUILayout.TextField("ScriptableObjectInstancePath", scriptableObjectInstanceFolder);

        // 保存ボタン
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Settings", GUILayout.Width(200)))
        {
            MasterDataEditorConfig.settings.CsvMasterDataPath = csvMasterDataFolder;
            MasterDataEditorConfig.settings.ScriptableObjectInstancePath = scriptableObjectInstanceFolder;
            MasterDataEditorConfig.settings.CreatedScriptableObjectClassPath = createdScriptableObjectClassFolder;

            MasterDataEditorConfig.SaveSettings();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}
