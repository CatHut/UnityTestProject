using CatHut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class DataEditWindow : EditorWindow
{

    private readonly List<TreeViewItemData<Item>> _rootItems = new();
    private TreeView treeView;


    [MenuItem("Tools/CatHut/MasterDataEditor/Data Edit")]
    public static void ShowWindow()
    {
        DataEditWindow wnd = GetWindow<DataEditWindow>();
        wnd.titleContent = new GUIContent("Data Edit");
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
        treeView.selectionChanged += OnTreeViewSelectionChange;

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

    private void OnTreeViewSelectionChange(IEnumerable<object> selectedItems)
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
                InitializeDataEditUi(editArea);
                break;
        }

    }


    private void CreateDataEditArea(VisualElement editArea)
    {
        var name = GetSelectedAndParentItemNames(treeView);

        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart; //ヘッダ情報
        var gTable = EditorSharedData.RawMasterData.GrobalTableData;    //グローバルテーブル
        var dg = EditorSharedData.RawMasterData.DataGroupDic[name.parentName];  //データグループ


        // VariableDicのListView
        var variableListView = new MultiColumnListView();
        variableListView.name = "DataListView";


        //ListView構築
        CreateDynamicColumns(editArea, variableListView);


        editArea.Add(new Button(() => {

            variableListView.Rebuild();
        })
        { text = "Save" });


        editArea.Add(new Button(() => {


            UpdateHeaderEditUi(editArea);

        })
        { text = "Csv Reload" });

        editArea.Add(new Button(() => {

            variableListView.RefreshItems();

        })
        { text = "TestRefresh" });


        //これはTreeViewに実装
        editArea.Add(new Button(() => Debug.Log("Button 2")) { text = "Create DataGroup" });
    }

    private void InitializeDataEditUi(VisualElement editArea)
    {
        CreateDataEditArea(editArea);
    }

    private void UpdateHeaderEditUi(VisualElement editArea)
    {
        var name = GetSelectedAndParentItemNames(treeView);
        var variableListView = editArea.Q<MultiColumnListView>("DataListView");

        //データ取得
        var data = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].DataPart.DataWithoutColumnTitle;

        //ソース設定
        variableListView.itemsSource = data;
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


    private void CreateDynamicColumns(VisualElement editArea, MultiColumnListView variableListView)
    {

        // 選択状態取得
        var name = GetSelectedAndParentItemNames(treeView);

        //ヘッダ情報
        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart; 


        //ソース設定
        variableListView.itemsSource = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].DataPart.DataWithoutColumnTitle; ;


        // 列の定義を動的に行う
        List<string> columns = header.VariableDic.Keys.ToList();

        foreach (string colName in columns)
        {
            Column column = new Column
            {
                title = colName,
                name = colName,
                width = 70,
                makeCell = () =>
                {
                    var type = header.VariableDic[colName].Type;

                    switch (type)
                    {
                        case "byte":
                        case "ushort":
                        case "uint":
                            {
                                return new UnsignedIntegerField();
                            }
                        case "ulong":
                            {
                                return new UnsignedLongField();
                            }

                        case "sbyte":
                        case "short":
                        case "int":
                            {
                                return new IntegerField();
                            }

                        case "long":
                            {
                                return new LongField();
                            }

                        case "double":
                            {
                                return new DoubleField();
                            }

                        case "float":
                            {
                                return new FloatField();
                            }

                        case "bool":
                            {
                                var boolValue = new List<string>() { "true", "false" };
                                return new PopupField<string>(boolValue, 0);
                            }

                        // その他の型に対応するUIエレメントのバインドを追加
                        case "string":
                        default:
                            {
                                return new TextField();
                            }
                    }

                },
                bindCell = (e, i) =>
                {
                    var col = header.VariableDic[colName].ColumnIndex;
                    var type = header.VariableDic[colName].Type;
                    
                    //データ
                    var data = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].DataPart.DataWithoutColumnTitle;
                    
                    Debug.Log("i:" + i.ToString());

                    switch (type)
                    {
                        case "byte":
                        case "ushort":
                        case "uint":
                            {
                                var intField = e as UnsignedIntegerField;
                                if (uint.TryParse(data[i][col], out uint uintResult))
                                {
                                    intField.value = uintResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                intField.RegisterValueChangedCallback(evt => { 
                                    data[i][col] = evt.newValue.ToString();
                                });
                            }
                            break;
                        case "ulong":
                            {
                                var ulongField = e as UnsignedLongField;
                                if (ulong.TryParse(data[i][col], out ulong ulongResult))
                                {
                                    ulongField.value = ulongResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                ulongField.RegisterValueChangedCallback(evt => { 
                                    data[i][col] = evt.newValue.ToString();
                                });
                            }
                            break;
                        case "sbyte":
                        case "short":
                        case "int":
                            {
                                var intField = e as IntegerField;
                                if (int.TryParse(data[i][col], out int intResult))
                                {
                                    intField.value = intResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                intField.RegisterValueChangedCallback(evt => {
                                    data[i][col] = evt.newValue.ToString();
                                });
                            }
                            break;
                        case "long":
                            {
                                var longField = e as LongField;
                                if (long.TryParse(data[i][col], out long longResult))
                                {
                                    longField.value = longResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                longField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "double":
                            {
                                var doubleField = e as DoubleField;
                                if (double.TryParse(data[i][col], out double doubleResult))
                                {
                                    doubleField.value = doubleResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                doubleField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "float":
                            {
                                var floatField = e as FloatField;
                                if (float.TryParse(data[i][col], out float floatResult))
                                {
                                    floatField.value = floatResult;
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                floatField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "bool":
                            {
                                var boolValue = new List<string>() { "true", "false" };
                                var popup = e as PopupField<string>;
                                popup.choices = boolValue;
                                popup.value = ConvertBoolean.ToBoolString(data[i][col]);
                                popup.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        // その他の型に対応するUIエレメントのバインドを追加
                        case "string":
                        default:
                            {
                                var textField = e as TextField;
                                textField.value = data[i][col];
                                textField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue; });
                            }
                            break;
                    }
                }
            };
            variableListView.columns.Add(column);
        }

        variableListView.Rebuild();

        editArea.Add(variableListView);
    }


}
