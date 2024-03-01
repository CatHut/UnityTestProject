using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    [MenuItem("Window/My Editor Window")]
    public static void ShowExample()
    {
        MyEditorWindow wnd = GetWindow<MyEditorWindow>();
        wnd.titleContent = new GUIContent("My Editor Window");
    }

    public void CreateGUI()
    {
        // UXMLテンプレートのロード
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Asset/UI Toolkit/Sample.uxml");
        visualTree.CloneTree(rootVisualElement);

        // USSスタイルの適用
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Asset/UI Toolkit/Sample.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        // UI操作の例（ボタンの追加など）
        var myButton = new Button(() => Debug.Log("Clicked")) { text = "Click Me" };
        rootVisualElement.Add(myButton);
    }
}
