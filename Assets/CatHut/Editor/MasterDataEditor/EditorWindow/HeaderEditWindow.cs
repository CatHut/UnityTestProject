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
        // �������R�[�h
        //�ݒ�t�@�C�������[�h
        DataGroup dataGroup = new DataGroup();

    }

    private void OnGUI()
    {
        //�\��

    }
}
