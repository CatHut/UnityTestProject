using CatHut;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class HeaderEditWindow : EditorWindow
{

    private readonly List<TreeViewItemData<Item>> _rootItems = new();
    private TreeView treeView;


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

        treeView = new TreeView(); 
        treeView.SetRootItems(_rootItems);
        treeView.selectionType = SelectionType.Single;
        treeView.style.flexGrow = 1;
        treeView.selectionChanged += OnListViewSelectionChange;

        treeView.makeItem = () => {
            var label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };

        // アイテムの内容を設定する処理
        treeView.bindItem = (e, i) => e.Q<Label>().text = treeView.GetItemDataForIndex<Item>(i).name;

        treeView.Rebuild();

        leftPane.Add(treeView);

        // 編集領域（初期は空）
        var rightPane = new VisualElement();
        rightPane.name = "editArea";
        splitView.Add(rightPane);


        //初期選択状態設定
        treeView.SetSelectionById(0);

    }

    private void OnListViewSelectionChange(IEnumerable<object> selectedItems)
    {
        var selectedItem = selectedItems.First().ToString();
        var editArea = rootVisualElement.Q<VisualElement>("editArea");
        editArea.Clear();

        var name = GetSelectedAndParentItemNames(treeView);

        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart;

        // ClassNameの入力フィールド
        var classNameField = new TextField("ClassName");
        classNameField.value = header.ClassName;
        classNameField.isReadOnly = true;
        editArea.Add(classNameField);

        // ParentNameの入力フィールド
        var parentNameField = new TextField("ParentName");
        parentNameField.value = header.ParentName;
        parentNameField.isReadOnly = true;
        editArea.Add(parentNameField);

        // IndexVariableの入力フィールド
        var options = header.VariableDic.Keys.ToList();
        var indexVariableDropdown = new PopupField<string>("IndexVariable", options, 0);
        indexVariableDropdown.value = header.IndexVariable;
        editArea.Add(indexVariableDropdown);

        // IndexDuplicatableのチェックボックス
        var boolValue = new List<string>(){ "True", "False"};
        var indexDuplicatableDropdown = new PopupField<string>("indexDuplicatable", boolValue, 0);
        indexDuplicatableDropdown.value = header.IndexDuplicatableString;
        editArea.Add(indexDuplicatableDropdown);

        // VariableDicのListView
        var variableListView = new MultiColumnListView();

        var headerItem = header.VariableDic.Values.ToList();
        variableListView.itemsSource = headerItem;

        //TODO テーブルリスト取得準備中
        // DataGroupからの取得でいいのでは・・・？考え中・・・
        var typeList = TypeNames.ValueTypes.ToList();


        // Create a new column
        var nameColumn = new Column()
        {
            title = "Name",
            name = "Name", // The title of your column
            width = 100,
            makeCell = () => new TextField(), // TextFieldを作成
            bindCell = (e, i) =>
            {
                var variableInfo = (VariableInfo)variableListView.itemsSource[i];
                var textField = e as TextField;
                textField.value = variableInfo.Name; // 仮のデータバインディング
            }
        };

        var typeColumn = new Column()
        {
            title="Type",
            name = "Type", // The title of your column
            width = 100,
            makeCell = () => new PopupField<string>(typeList, 0), // PopupFieldを作成
            bindCell = (e, i) =>
            {
                var variableInfo = (VariableInfo)variableListView.itemsSource[i];
                var popupField = e as PopupField<string>;
                popupField.value = variableInfo.Type; // 仮のデータバインディング
            }
        };
        var descriptionColumn = new Column()
        {
            title = "Description",
            name = "Description", // The title of your column
            width = 200,
            makeCell = () => new TextField(), // TextFieldを作成
            bindCell = (e, i) =>
            {
                var variableInfo = (VariableInfo)variableListView.itemsSource[i];
                var textField = e as TextField;
                textField.value = variableInfo.Description; // 仮のデータバインディング
            }
        };

        variableListView.columns.Add(nameColumn);
        variableListView.columns.Add(typeColumn);
        variableListView.columns.Add(descriptionColumn);


        //variableListView.makeItem = () => new VisualElement();
        //variableListView.bindItem = (element, i) =>
        //{
        //    var variableInfo = (VariableInfo)variableListView.itemsSource[i];
        //    var nameField = new TextField("Name") { value = variableInfo.Name };
        //    var typeDropdown = new PopupField<string>("Type", typeList, 0) { value = variableInfo.Type};
        //    var descriptionField = new TextField("Description") { value = variableInfo.Description };

        //    element.Add(nameField);
        //    element.Add(typeDropdown);
        //    element.Add(descriptionField);
        //};

        variableListView.Rebuild();
        editArea.Add(variableListView);



        editArea.Add(new Button(() => {

        
        }) { text = "Save" });


        editArea.Add(new Button(() => Debug.Log("Button 1")) { text = "Reload" });
        editArea.Add(new Button(() => Debug.Log("Button 2")) { text = "Create DataGroup" });


        if (selectedItem == "Item1")
        {
            // Item1選択時のボタン
            editArea.Add(new Button(() => Debug.Log("Button 3")) { text = "Button 3" });
        }
        else if (selectedItem == "Item2")
        {
            // Item2選択時のボタン
            editArea.Add(new Button(() => Debug.Log("Button 4")) { text = "Button 4" });
        }

    }


    // TreeViewから選択されたアイテムとその親アイテムの名前を取得する関数
    public (string parentName, string selectedName) GetSelectedAndParentItemNames(TreeView treeView)
    {
        var selected = (Item)treeView.selectedItem;
        var parentId = treeView.GetParentIdForIndex(treeView.selectedIndex);
        var parent = treeView.GetItemDataForId<Item>(parentId);
        return (parent.name, selected.name);


    }



    [Serializable]
    public struct Item
    {
        public string name;
    }

    

}
