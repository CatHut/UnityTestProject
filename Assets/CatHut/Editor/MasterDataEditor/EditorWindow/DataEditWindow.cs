using CatHut;
using Codice.Client.Commands;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class DataEditWindow : EditorWindow
{

    private List<TreeViewItemData<Item>> _rootItems;
    private TreeView TreeView;
    private Button ApplyButton;

    private readonly string UI_ITEM_MASTER_DATA_PATH = "MasterDataPath";


    [MenuItem("Tools/CatHut/MasterDataEditor/Data Edit")]
    public static void ShowWindow()
    {
        DataEditWindow wnd = GetWindow<DataEditWindow>();
        wnd.titleContent = new GUIContent("Data Edit");
    }

    public void OnEnable()
    {
        ApplyButton = new Button(() =>
        {
            // ここにボタンが押されたときの処理を記述
            ApplyValuesToGame();
        });

        EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
        {
            // ゲームが再生中かどうかでボタンの有効・無効を切り替え
            ApplyButton.SetEnabled(EditorApplication.isPlaying);

            // ゲーム再生状態に応じてボタンのテキストも更新
            ApplyButton.text = EditorApplication.isPlaying ? "Apply Values To Game" : "Apply Values To Game(Enter PlayMode)";
        };

        // ゲーム再生状態に応じてボタンのテキストも更新
        ApplyButton.text = EditorApplication.isPlaying ? "Apply Values To Game" : "Apply Values To Game(Enter PlayMode)";
        ApplyButton.SetEnabled(EditorApplication.isPlaying);


    }

    private void Reset()
    {

    }


    public void CreateGUI()
    {
        // ルート
        var root = rootVisualElement;

        // 垂直分割
        var varticalSplitView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Vertical);
        root.Add(varticalSplitView);
        var topPane = new VisualElement();
        var bottomPane = new VisualElement();

        varticalSplitView.Add(topPane);
        varticalSplitView.Add(bottomPane);

        // 編集対象のフォルダ選択ドロップダウン
        var folderList = MasterDataEditorConfig.settings.CsvMasterDataPathList;
        var masterDataPathDropdown = new PopupField<string>(UI_ITEM_MASTER_DATA_PATH, folderList, 0);
        masterDataPathDropdown.name = UI_ITEM_MASTER_DATA_PATH;
        masterDataPathDropdown.value = MasterDataEditorConfig.settings.SelectedMasterDataPath;

        // 値が変更されたときの処理を登録
        masterDataPathDropdown.RegisterValueChangedCallback(evt =>
        {
            //Treeビュー等の更新
            MasterDataEditorConfig.settings.SelectedMasterDataPath = evt.newValue;
            MasterDataEditorConfig.SaveSettings();

            //データリロード
            OnMasterDataSelectionChanged();

        });
        topPane.Add(masterDataPathDropdown);

        // 水平分割
        var horizontalSplitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
        bottomPane.Add(horizontalSplitView);

        var leftPane = new VisualElement();
        leftPane.name = "LeftPane";
        horizontalSplitView.Add(leftPane);

        // 編集領域（初期は空）
        var rightPane = new VisualElement();
        rightPane.name = "editArea";
        horizontalSplitView.Add(rightPane);

        //ツリービューを構成
        ConfigureTreeView(leftPane);

    }


    // CreateGUIメソッドのTreeView関連の構築部分を分離した関数
    private void ConfigureTreeView(VisualElement container)
    {
        var id = 0;
        var path = rootVisualElement.Q<PopupField<string>>(UI_ITEM_MASTER_DATA_PATH).value;
        EditorSharedData.UpdateData();
        var dgd = EditorSharedData.RawMasterData.EachPathDataGroupDic[path];

        _rootItems = new List<TreeViewItemData<Item>>();
        foreach (var dg in dgd)
        {
            var items = new List<TreeViewItemData<Item>>();
            foreach (var innerItem in dg.Value.FormatedCsvDic)
            {
                var item = new TreeViewItemData<Item>(id, new Item { Name = innerItem.Key, HierarchyLevel = 1, Id = id });
                id++;
                items.Add(item);
            }

            // ルートアイテムを作成し、子アイテムを追加
            if (items.Count > 0)
            {
                var rootItem = new TreeViewItemData<Item>(id, new Item { Name = dg.Key, HierarchyLevel = 0, Id = id }, items);
                id++;
                // ルートアイテムのみをデータソースに追加
                _rootItems.Add(rootItem);
            }
        }

        TreeView = new TreeView();
        TreeView.SetRootItems(_rootItems);
        TreeView.selectionType = SelectionType.Single;
        TreeView.style.flexGrow = 1;
        TreeView.selectionChanged += OnTreeViewSelectionChange;

        TreeView.makeItem = () => {
            var label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };

        TreeView.bindItem = (e, i) => e.Q<Label>().text = TreeView.GetItemDataForIndex<Item>(i).Name;

        TreeView.Rebuild();

        container.Add(TreeView);

        // 初期選択状態設定
        TreeView.SetSelectionById(0);
    }

    private void OnTreeViewSelectionChange(IEnumerable<object> selectedItems)
    {
        var tvi = (Item)selectedItems.First();

        var editArea = rootVisualElement.Q<VisualElement>("editArea");
        editArea.Clear();


        switch (tvi.HierarchyLevel)
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
        editArea.Clear();

        var name = GetSelectedAndParentItemNames(TreeView);

        var path = rootVisualElement.Q<PopupField<string>>(UI_ITEM_MASTER_DATA_PATH).value;

        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart;
        var gTable = EditorSharedData.RawMasterData.GrobalTableData;    //グローバルテーブル
        var dg = EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName];  //データグループ


        // VariableDicのListView
        var variableListView = new MultiColumnListView();
        variableListView.name = "DataListView";


        //ListView構築
        CreateDynamicColumns(editArea, variableListView);


        editArea.Add(new Button(() => {

            EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName].FormatedCsvDic[name.selectedName].DataPart.Save();
            var file = EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName].FormatedCsvDic[name.selectedName].DataPart.FilePath;

            AssetDatabase.ImportAsset(file);

        })
        { text = "Save" });


        //このボタン定義はOnEnableでやる
        editArea.Add(ApplyButton);


        editArea.Add(new Button(() => {

            UpdateDataEditUi(editArea);

        })
        { text = "Csv Reload" });

    }

    private void InitializeDataEditUi(VisualElement editArea)
    {
        CreateDataEditArea(editArea);
    }

    private void UpdateDataEditUi(VisualElement editArea)
    {
        var path = rootVisualElement.Q<PopupField<string>>(UI_ITEM_MASTER_DATA_PATH).value;

        //データリロード
        EditorSharedData.RawMasterData.DataReload(path);

        CreateDataEditArea(editArea);

    }


    private void CreateDataGroupEditArea(VisualElement editArea)
    {
        editArea.Clear();
        // 編集対象のフォルダ選択ドロップダウン
        var message = new Label("Select Child Element");

        editArea.Add(message);
    }

    private void OnMasterDataSelectionChanged()
    {

        var leftPane = rootVisualElement.Q<VisualElement>("LeftPane");
        leftPane.Clear();

        //ツリービューを構成
        ConfigureTreeView(leftPane);
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
        var name = GetSelectedAndParentItemNames(TreeView);

        //ヘッダ情報
        var header = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].FormatedCsvDic[name.selectedName].HeaderPart;

        //テーブルデータ
        var tables = EditorSharedData.RawMasterData.DataGroupDic[name.parentName].TableData.TableDic;

        var path = rootVisualElement.Q<PopupField<string>>(UI_ITEM_MASTER_DATA_PATH).value;

        //ソース設定 TODO
        variableListView.itemsSource = EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName].FormatedCsvDic[name.selectedName].DataPart.DataWithoutColumnTitle;


        // 列の定義を動的に行う
        List<string> columns = header.VariableDic.Keys.ToList();
        string indexVariable = header.IndexVariable;

        var tips = "半角のみ入力可(Only half-width characters)";


        foreach (string colName in columns)
        {
            var col = header.VariableDic[colName].ColumnIndex;
            var valInfo = header.VariableDic[colName];

            //データ
            var data = EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName].FormatedCsvDic[name.selectedName].DataPart.DataWithoutColumnTitle;

            Column column = new Column
            {
                title = colName,
                name = colName,
                width = valInfo.EditorViewType == "MLstring" ? MasterDataEditorCommon.MLstringAreaSize.x : 80,
                makeCell = () =>
                {

                    //インデックスは編集させない
                    if (colName == indexVariable)
                    {
                        return new Label();
                    }

                    switch (valInfo.EditorViewType)
                    {
                        case "byte":
                        case "ushort":
                        case "uint":
                            {
                                var temp = new UnsignedIntegerField() { tooltip = tips };
                                temp.RegisterCallback<KeyDownEvent>(evt => {
                                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                                    {
                                        evt.StopPropagation();
                                        if (evt.shiftKey)
                                        {
                                            var elem = evt.currentTarget as UnsignedIntegerField;
                                            var idx = elem.parent.IndexOf(elem);
                                            var nextElem = elem.parent[idx + 1];
                                            nextElem.Focus();

                                        }
                                        else
                                        {
                                            //ひとつ下の行の入力要素をフォーカスする
                                            var nextRowElement = variableListView.Q(name.selectedName + 1);
                                            nextRowElement.Focus();
                                        }
                                    }
                                });

                                return temp;
                            }
                        case "ulong":
                            {
                                return new UnsignedLongField() { tooltip = tips };
                            }

                        case "sbyte":
                        case "short":
                        case "int":
                            {
                                var temp = new IntegerField() { tooltip = tips };
                                temp.RegisterCallback<KeyDownEvent>(evt =>
                                {
                                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                                    {
                                        evt.StopPropagation();

                                        if (evt.shiftKey)
                                        {
                                        }
                                        else
                                        {
                                        }

                                    }
                                });

                                return temp;

                            }

                        case "long":
                            {
                                return new LongField() { tooltip = tips };
                            }

                        case "double":
                            {
                                return new DoubleField() { tooltip = tips };
                            }

                        case "float":
                            {
                                return new FloatField() { tooltip = tips };
                            }

                        case "bool":
                            {
                                var boolValue = new List<string>() { "true", "false" };
                                return new PopupField<string>(boolValue, 0);
                            }
                        case "string":
                            {
                                return new TextField();
                            }
                        case "MLstring":    //MultiLine string(複数行の編集領域を確保)
                            {
                                var field = new TextField();
                                field.multiline = true;
                                field.style.width = MasterDataEditorCommon.MLstringAreaSize.x;
                                field.style.height = MasterDataEditorCommon.MLstringAreaSize.y;
                                variableListView.fixedItemHeight = MasterDataEditorCommon.MLstringAreaSize.y;
                                return field;
                            }
                        // その他の型に対応するUIエレメントのバインドを追加
                        default:
                            {
                                if (valInfo.IsTableType)
                                {
                                    var typeName = MasterDataEditorCommon.ExtractTableName(valInfo.EditorViewType);
                                    var labels = tables[typeName].Labels;
                                    return new PopupField<string>(labels, 0);
                                }
                                else
                                {
                                    //不定だけど一応
                                    return new TextField();
                                }
                            }
                    }

                },
                bindCell = (e, i) =>
                {

                    //インデックスは編集させない
                    if (colName == indexVariable)
                    {
                        var label = e as Label;
                        label.text = data[i][col];
                        return;
                    }

                    switch (valInfo.EditorViewType)
                    {
                        case "byte":
                        case "ushort":
                        case "uint":
                            {
                                var intField = e as UnsignedIntegerField;
                                if (uint.TryParse(data[i][col], out uint uintResult))
                                {
                                    intField.SetValueWithoutNotify(uintResult);
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                intField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "ulong":
                            {
                                var ulongField = e as UnsignedLongField;
                                if (ulong.TryParse(data[i][col], out ulong ulongResult))
                                {
                                    ulongField.SetValueWithoutNotify(ulongResult);
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                ulongField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "sbyte":
                        case "short":
                        case "int":
                            {
                                var intField = e as IntegerField;
                                if (int.TryParse(data[i][col], out int intResult))
                                {
                                    intField.SetValueWithoutNotify(intResult);
                                }
                                else
                                {
                                    Debug.Log($"Parse failed for uint value: {data[i][col]} at row: {i}, col: {colName}");
                                }
                                intField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "long":
                            {
                                var longField = e as LongField;
                                if (long.TryParse(data[i][col], out long longResult))
                                {
                                    longField.SetValueWithoutNotify(longResult);
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
                                    doubleField.SetValueWithoutNotify(doubleResult);
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
                                    floatField.SetValueWithoutNotify(floatResult);
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
                                var popup = e as PopupField<string>;
                                popup.SetValueWithoutNotify(ConvertBoolean.ToBoolString(data[i][col]));
                                popup.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                            }
                            break;
                        case "string":
                            {
                                var textField = e as TextField;
                                textField.SetValueWithoutNotify(data[i][col]);
                                textField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue; });
                            }
                            break;
                        case "MLstring":
                            {
                                var textField = e as TextField;
                                textField.SetValueWithoutNotify(data[i][col]);
                                textField.RegisterValueChangedCallback(evt =>
                                {
                                    data[i][col] = evt.newValue;
                                });
                            }
                            break;
                        // その他の型に対応するUIエレメントのバインドを追加
                        default:
                            {
                                if (valInfo.IsTableType)
                                {
                                    var popup = e as PopupField<string>;
                                    popup.SetValueWithoutNotify(data[i][col]);
                                    popup.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue.ToString(); });
                                }
                                else
                                {
                                    //不定だけど一応
                                    var textField = e as TextField;
                                    textField.SetValueWithoutNotify(data[i][col]);
                                    textField.RegisterValueChangedCallback(evt => { data[i][col] = evt.newValue; });
                                }
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


    private void ApplyValuesToGame()
    {

        var name = GetSelectedAndParentItemNames(TreeView);
        var path = rootVisualElement.Q<PopupField<string>>(UI_ITEM_MASTER_DATA_PATH).value;

        var data = EditorSharedData.RawMasterData.EachPathDataGroupDic[path][name.parentName].FormatedCsvDic[name.selectedName];

        CsvReflector.ApplyValuesToGame(name.parentName, name.selectedName, data);

    }

    public static string ExtractTableName(string input)
    {
        // 正規表現パターン: 'Tables[' と ']' に囲まれた文字を抽出
        string pattern = @"Tables\[(.*?)\]";

        // 正規表現オブジェクトの生成
        Regex regex = new Regex(pattern);

        // 入力文字列に対してマッチングを試みる
        Match match = regex.Match(input);

        // マッチした場合、グループ1の値（括弧内の部分）を返す
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            // マッチしない場合は、適切な処理または空文字列を返す
            return string.Empty;
        }
    }


}
