using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static AddressableOperatorConfig;

public class AddressableOperatorWindow : EditorWindow
{
    private AddressableOperatorConfigData AddressableOperationConfigData;


    [MenuItem("Tools/CatHut/AddressableOperator/Settings")]
    public static void ShowWindow()
    {
        GetWindow<AddressableOperatorWindow>("Addressables Settings");
    }

    private void OnEnable()
    {
        // 初期化コード
        //設定ファイルをロード
        AddressableOperationConfigData = AddressableOperatorConfig.LoadSettings();

    }

    private void OnGUI()
    {
        //表題
        GUILayout.Space(20);

        // 保存ボタン
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Settings", GUILayout.Width(200)))
        {
            AddressableOperatorConfig.settings.AddressableSettingList = AddressableOperationConfigData.AddressableSettingList;
            AddressableOperatorConfig.SaveSettings();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Addressables Settings", EditorStyles.boldLabel);


        GUILayout.BeginHorizontal();
        GUILayout.Label("Group", GUILayout.Width(100));
        GUILayout.Label("Folder Path", GUILayout.Width(300));
        GUILayout.Label("Extension", GUILayout.Width(100));
        GUILayout.EndHorizontal();

        var list = AddressableOperationConfigData.AddressableSettingList;

        for (int i = 0; i < list.Count; i++)
        {
            var setting = list[i];
            GUILayout.BeginHorizontal();
            setting.Group = EditorGUILayout.TextField(setting.Group, GUILayout.Width(100));
            setting.FolderPath = EditorGUILayout.TextField(setting.FolderPath, GUILayout.Width(300));
            setting.Extention = EditorGUILayout.TextField(setting.Extention, GUILayout.Width(100));
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                list.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add", GUILayout.Width(100)))
        {
            list.Add(new AddressableSetting());
        }



    }
}
