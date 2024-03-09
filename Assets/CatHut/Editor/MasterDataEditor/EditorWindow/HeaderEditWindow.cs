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
                var item = new TreeViewItemData<Item>(id, new Item { Name = innerItem.Key, HierarchyLevel = 1, Id = id }) ;
                id++;
                items.Add(item);
            }

            // ルートアイテムを作成し、子アイテムを追加
            var rootItem = new TreeViewItemData<Item>(id, new Item { Name = dg.Key, HierarchyLevel = 0, Id = id }, items);
            id++;
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
        treeView.bindItem = (e, i) => e.Q<Label>().text = treeView.GetItemDataForIndex<Item>(i).Name;

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
        var tvi = (Item)selectedItems.First();

        var editArea = rootVisualElement.Q<VisualElement>("editArea");
        editArea.Clear();


        switch(tvi.HierarchyLevel)
        {
            case 0:
                //DataGroup選択時
                //テーブル編集、FormatedCsvData追加ボタンとか表示
                CreateDataGroupEditArea(editArea);
                break;
            case 1:
                //FormatedCsvData選択時
                InitializeHeaderEditUi(editArea);
                break;
        }

    }


    private void CreateHeaderEditArea(VisualElement editArea)
    {
        var name = GetSelectedAndParentItemNames(treeView);

        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart; //ヘッダ情報
        var gTable = EditorSharedData.RawMasterData.GrobalTableData;    //グローバルテーブル
        var dg = EditorSharedData.RawMasterData.DataGroupDic[name.parentName];  //データグループ

        // ClassNameの入力フィールド
        var classNameField = new TextField("ClassName");
        classNameField.name = "ClassName";
        classNameField.value = header.ClassName;
        classNameField.isReadOnly = true;
        classNameField.focusable = false;
        editArea.Add(classNameField);

        // ParentNameの入力フィールド
        var parentNameField = new TextField("ParentName");
        parentNameField.name = "ParentName";
        parentNameField.value = header.ParentName;
        parentNameField.isReadOnly = true;
        parentNameField.focusable = false;
        editArea.Add(parentNameField);

        // IndexVariableの入力フィールド
        var options = header.VariableDic.Keys.ToList();
        var indexVariableDropdown = new PopupField<string>("IndexVariable", options, 0);
        indexVariableDropdown.name = "IndexVariable";
        indexVariableDropdown.value = header.IndexVariable;

        // 値が変更されたときの処理を登録
        indexVariableDropdown.RegisterValueChangedCallback(evt =>
        {
            // ここで変更された値を処理
            // 例: ヘッダのIndexVariableプロパティを更新
            header.IndexVariable = evt.newValue;
        });

        editArea.Add(indexVariableDropdown);

        // IndexDuplicatableのチェックボックス
        var boolValue = new List<string>() { "True", "False" };
        var indexDuplicatableDropdown = new PopupField<string>("indexDuplicatable", boolValue, 0);
        indexDuplicatableDropdown.name = "indexDuplicatable";
        indexDuplicatableDropdown.value = header.IndexDuplicatableString;

        // 値が変更されたときの処理を登録
        indexDuplicatableDropdown.RegisterValueChangedCallback(evt =>
        {
            // ここで変更された値を処理
            // 例: ヘッダのIndexVariableプロパティを更新
            header.IndexDuplicatableString = evt.newValue;
        });


        editArea.Add(indexDuplicatableDropdown);

        // VariableDicのListView
        var variableListView = new MultiColumnListView();
        variableListView.name = "VariableListView";

        var headerItem = header.VariableDic.Values.ToList();
        variableListView.itemsSource = headerItem;

        //テーブルリスト取得
        var typeList = TypeNames.ValueTypes.ToList();

        //Tables[name]という形式でTable型を追加
        typeList.AddRange(gTable.TableDic.Keys.ToList().Select(item => $"Tables[{item}]").ToList());
        typeList.AddRange(dg.TableData.TableDic.Keys.ToList().Select(item => $"Tables[{item}]").ToList());


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
                textField.value = variableInfo.Name;

                // 値が変更されたときの処理を登録
                textField.RegisterValueChangedCallback(evt =>
                {
                    // 編集された新しい値をデータソースに反映
                    variableInfo.Name = evt.newValue;
                });
            }
        };

        var typeColumn = new Column()
        {
            title = "Type",
            name = "Type", // The title of your column
            width = 100,
            makeCell = () => new PopupField<string>(typeList, 0), // PopupFieldを作成
            bindCell = (e, i) =>
            {
                var variableInfo = (VariableInfo)variableListView.itemsSource[i];
                var popupField = e as PopupField<string>;
                popupField.value = variableInfo.Type;

                // 値が変更されたときの処理を登録
                popupField.RegisterValueChangedCallback(evt =>
                {
                    // 編集された新しい値をデータソースに反映
                    variableInfo.Type = evt.newValue;
                });
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
                textField.value = variableInfo.Description;

                // 値が変更されたときの処理を登録
                textField.RegisterValueChangedCallback(evt =>
                {
                    // 編集された新しい値をデータソースに反映
                    variableInfo.Description = evt.newValue;
                });
            }
        };

        variableListView.columns.Add(nameColumn);
        variableListView.columns.Add(typeColumn);
        variableListView.columns.Add(descriptionColumn);

        variableListView.Rebuild();
        editArea.Add(variableListView);


        editArea.Add(new Button(() => {

            var updateDic = new Dictionary<string, VariableInfo>();

            //更新後のVariableInfoを取得
            foreach (var item in variableListView.itemsSource)
            {
                var vi = (VariableInfo)item;
                updateDic[vi.Name] = vi;
            }

            header.VariableDic = updateDic;

            header.Save();
            UpdateHeaderEditUi(editArea);


        })
        { text = "Save" });


        editArea.Add(new Button(() => {

            EditorSharedData.UpdateData();
            var selectedItem = (Item)(treeView.selectedItem);

            UpdateHeaderEditUi(editArea);

        })
        { text = "Header Csv Reload" });



        editArea.Add(new Button(() => Debug.Log("Button 2")) { text = "Create ScriptableObject And Inporter" });


        //これはTreeViewに実装
        editArea.Add(new Button(() => Debug.Log("Button 2")) { text = "Create DataGroup" });
    }

    private void InitializeHeaderEditUi(VisualElement editArea)
    {
        CreateHeaderEditArea(editArea);
    }

    private void UpdateHeaderEditUi(VisualElement editArea)
    {
        var name = GetSelectedAndParentItemNames(treeView);

        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart; //ヘッダ情報
        var gTable = EditorSharedData.RawMasterData.GrobalTableData;    //グローバルテーブル
        var dg = EditorSharedData.RawMasterData.DataGroupDic[name.parentName];  //データグループ


        var classNameField = editArea.Q<TextField>("ClassName");
        classNameField.value = header.ClassName;

        var parentNameField = editArea.Q<TextField>("ParentName");
        parentNameField.value = header.ParentName;

        var indexVariableDropdown = editArea.Q<PopupField<string>>("IndexVariable");
        indexVariableDropdown.value = header.IndexVariable;

        var indexDuplicatableDropdown = editArea.Q<PopupField<string>>("indexDuplicatable");
        indexDuplicatableDropdown.value = header.IndexDuplicatableString;

        var variableListView = editArea.Q<MultiColumnListView>("VariableListView");
        var headerItem = header.VariableDic.Values.ToList();
        variableListView.itemsSource = headerItem;
        variableListView.Rebuild();

    }


    private void CreateDataGroupEditArea(VisualElement editArea)
    {
        editArea.Add(new Button(() => {
            //ボタン押したときの処理書く
        })
        { text = "Add Table" });

        editArea.Add(new Button(() => {
            //ボタン押したときの処理書く
        })
        { text = "Delete Table" });

        editArea.Add(new Button(() => {
            //ボタン押したときの処理書く
        })
        { text = "Add Header" });

        editArea.Add(new Button(() => {
            //ボタン押したときの処理書く
        })
        { text = "Delete Header" });
    }




    // TreeViewから選択されたアイテムとその親アイテムの名前を取得する関数
    public (string parentName, string selectedName) GetSelectedAndParentItemNames(TreeView treeView)
    {
        var selected = (Item)treeView.selectedItem;
        var parentId = treeView.GetParentIdForIndex(treeView.selectedIndex);
        var parent = treeView.GetItemDataForId<Item>(parentId);
        return (parent.Name, selected.Name);


    }



    [Serializable]
    public struct Item
    {
        public string Name;
        public int HierarchyLevel;
        public int Id;
    }


    public void CreateScriptableObject(DataGroup dg)
    {

    }

    

}
