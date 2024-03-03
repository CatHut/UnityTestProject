using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MasterDataEditorSettingsWindow : EditorWindow
{
    private List<string> csvMasterDataFolderList = new List<string>();
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
        csvMasterDataFolderList = MasterDataEditorConfig.settings.CsvMasterDataPathList;
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
        for (int i = 0; i < csvMasterDataFolderList.Count; i++)
        {
            if (!AssetDatabase.IsValidFolder(csvMasterDataFolderList[i]))
            {
                GUIStyle errorStyle = new GUIStyle();
                errorStyle.normal.textColor = Color.red;
                GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
            }

            GUILayout.BeginHorizontal();
            //設定項目
            csvMasterDataFolderList[i] = EditorGUILayout.TextField("CsvMasterDataPath" + "[" + i.ToString() + "]", csvMasterDataFolderList[i]);
            if (GUILayout.Button("削除", GUILayout.Width(200)))
            {
                csvMasterDataFolderList.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("追加", GUILayout.Width(200)))
        {
            csvMasterDataFolderList.Add("");
        }


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
            MasterDataEditorConfig.settings.CsvMasterDataPathList = csvMasterDataFolderList;
            MasterDataEditorConfig.settings.ScriptableObjectInstancePath = scriptableObjectInstanceFolder;
            MasterDataEditorConfig.settings.CreatedScriptableObjectClassPath = createdScriptableObjectClassFolder;

            MasterDataEditorConfig.SaveSettings();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}
