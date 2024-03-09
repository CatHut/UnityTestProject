using CatHut;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MasterDataEditorOperationWindow : EditorWindow
{
    private List<string> csvMasterDataFolderList = new List<string>();
    private string scriptableObjectInstanceFolder = string.Empty;
    private string createdScriptableObjectClassFolder = string.Empty;

    [MenuItem("Tools/CatHut/MasterDataEditor/Operation")]
    public static void ShowWindow()
    {
        GetWindow<MasterDataEditorOperationWindow>("MasterDataEditor Operation");
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
        GUILayout.Label("MasterDataEditor Operator", EditorStyles.boldLabel);


        // マスター
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create MasterData Component Script", GUILayout.Width(200)))
        {
            var rmd = EditorSharedData.RawMasterData;  //RawMasterData
            MasterDataComponentGenerator.CreateMasterDataClass(rmd);

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();




    }
}
