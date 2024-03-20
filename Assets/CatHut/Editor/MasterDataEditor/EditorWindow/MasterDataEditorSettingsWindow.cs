using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MasterDataEditorSettingsWindow : EditorWindow
{
    private MasterDataEditorConfig.MasterDataEditorConfigData configData;


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
        configData = MasterDataEditorConfig.settings;
    }

    private void OnGUI()
    {
        //表題
        GUILayout.Space(20);
        GUILayout.Label("MasterDataEditor Settings", EditorStyles.boldLabel);

        //指定されたフォルダがない場合に警告を表示
        GUILayout.Space(5);
        for (int i = 0; i < configData.CsvMasterDataPathList.Count; i++)
        {
            if (!AssetDatabase.IsValidFolder(configData.CsvMasterDataPathList[i]))
            {
                GUIStyle errorStyle = new GUIStyle();
                errorStyle.normal.textColor = Color.red;
                GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
            }

            GUILayout.BeginHorizontal();
            //設定項目
            configData.CsvMasterDataPathList[i] = EditorGUILayout.TextField("CsvMasterDataPath" + "[" + i.ToString() + "]", configData.CsvMasterDataPathList[i]);
            if (GUILayout.Button("削除", GUILayout.Width(200)))
            {
                configData.CsvMasterDataPathList.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("追加", GUILayout.Width(200)))
        {
            configData.CsvMasterDataPathList.Add("");
        }


        //表題
        GUILayout.Space(20);
        GUILayout.Label("Output Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(configData.CreatedScriptableObjectClassPath))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        configData.CreatedScriptableObjectClassPath = EditorGUILayout.TextField("CreatedScriptableObjectClassPath", configData.CreatedScriptableObjectClassPath);

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(configData.CreatedMasterDataClassPath))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        configData.CreatedMasterDataClassPath = EditorGUILayout.TextField("CreatedMasterDataClassPath", configData.CreatedMasterDataClassPath);

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(configData.ScriptableObjectInstancePath))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        configData.ScriptableObjectInstancePath = EditorGUILayout.TextField("ScriptableObjectInstancePath", configData.ScriptableObjectInstancePath);

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(configData.CreatedImporterPath))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        configData.CreatedImporterPath = EditorGUILayout.TextField("CreatedImporterPath", configData.CreatedImporterPath);

        GUILayout.Space(10);
        if (!AssetDatabase.IsValidFolder(configData.CreatedReflectorPath))
        {
            GUIStyle errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label("指定されたフォルダが存在しません。", errorStyle);
        }
        configData.CreatedReflectorPath = EditorGUILayout.TextField("CreatedReflectorPath", configData.CreatedReflectorPath);



        // 保存ボタン
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Settings", GUILayout.Width(200)))
        {
            MasterDataEditorConfig.settings = configData;

            MasterDataEditorConfig.SaveSettings();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}
