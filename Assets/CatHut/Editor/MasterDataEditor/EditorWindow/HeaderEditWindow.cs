using CatHut;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class HeaderEditWindow : EditorWindow
{

    private readonly List<TreeViewItemData<Item>> _rootItems = new();


    [MenuItem("Tools/CatHut/MasterDataEditor/Header Edit")]
    public static void ShowWindow()
    {
        HeaderEditWindow wnd = GetWindow<HeaderEditWindow>();
        wnd.titleContent = new GUIContent("Header Edit");
    }

    public void OnEnable()
    {
    }

    private void Reset()
    {
        //ツリービューを構成
        var id = 0;
        EditorSharedData.UpdateData();
        var dgd = EditorSharedData.RawMasterData.DataGroupDic;
        foreach (var dg in dgd)
        {
            var items = new List<TreeViewItemData<Item>>();
            foreach (var innerItem in dg.Value.FormatedCsvDic)
            {
                var item = new TreeViewItemData<Item>(id++, new Item { name = innerItem.Key });
                items.Add(item);
            }

            // ルートアイテムを作成し、子アイテムを追加
            var rootItem = new TreeViewItemData<Item>(id++, new Item { name = dg.Key }, items);

            // ルートアイテムのみをデータソースに追加
            _rootItems.Add(rootItem);
        }
    }


    public void CreateGUI()
    {
        // ルート
        var root = rootVisualElement;

        // 水平分割
        var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(splitView);

        var leftPane = new VisualElement();
        splitView.Add(leftPane);

        var treeView = new TreeView();
        treeView.SetRootItems(_rootItems);
        treeView.selectionType = SelectionType.Single;
        treeView.style.flexGrow = 1;
        treeView.selectionChanged += OnListViewSelectionChange;
        treeView.Rebuild();
        leftPane.Add(treeView);

        // 編集領域（初期は空）
        var rightPane = new VisualElement();
        rightPane.name = "editArea";
        splitView.Add(rightPane);
    }

    private void OnListViewSelectionChange(IEnumerable<object> selectedItems)
    {
        var selectedItem = selectedItems.First().ToString();
        var editArea = rootVisualElement.Q<VisualElement>("editArea");
        editArea.Clear();

        if (selectedItem == "Item1")
        {
            // Item1選択時のボタン
            editArea.Add(new Button(() => Debug.Log("Button 1")) { text = "Button 1" });
            editArea.Add(new Button(() => Debug.Log("Button 2")) { text = "Button 2" });
            editArea.Add(new Button(() => Debug.Log("Button 3")) { text = "Button 3" });
        }
        else if (selectedItem == "Item2")
        {
            // Item2選択時のボタン
            editArea.Add(new Button(() => Debug.Log("Button 4")) { text = "Button 4" });
            editArea.Add(new Button(() => Debug.Log("Button 5")) { text = "Button 5" });
            editArea.Add(new Button(() => Debug.Log("Button 6")) { text = "Button 6" });
        }
    }

    [Serializable]
    public struct Item
    {
        public string name;
    }

}
