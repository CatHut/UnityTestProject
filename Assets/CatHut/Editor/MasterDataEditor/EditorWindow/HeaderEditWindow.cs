using CatHut;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class HeaderEditWindow : EditorWindow
{

    [MenuItem("Tools/CatHut/MasterDataEditor/Header Edit")]
    public static void ShowWindow()
    {
        GetWindow<HeaderEditWindow>("Header Edit");
    }

    private void OnEnable()
    {
        // 初期化コード
        //設定ファイルをロード
        DataGroup dataGroup = new DataGroup();

    }

    private void OnGUI()
    {
        //表題

    }
}
