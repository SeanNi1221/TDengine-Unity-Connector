using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
namespace Sean21.TDengineConnector
{
public class TDScopeWindow : EditorWindow
{
    TDBridge bridge;
    TDRequest request;
    TDResult result;
    bool showDatabases;
    List<string> dbNames = new List<string>();
    [SerializeField] TreeViewState m_TreeViewState;
    TDTreeView m_TreeView;
    SearchField m_SearchField;
    [MenuItem("Window/TD Scope")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow<TDScopeWindow>();
        window.titleContent = new GUIContent("TD Scope", (Texture)Resources.Load("scope"));
    }
    void Awake() {
        bridge = TDBridge.i;
        request = TDBridge.Request;
        result = TDBridge.Request.result;
        Refresh();
    }
    void OnEnable() {
        if (m_TreeViewState == null) m_TreeViewState = new TreeViewState();
        m_TreeView = new TDTreeView(m_TreeViewState);
        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
    }

    void OnGUI() {
        if (GUILayout.Button("Refresh")) Refresh();
        DoToolbar();
        DoTreeView();
        // showDatabases = EditorGUILayout.Foldout(showDatabases, "Database", true);
        // if (showDatabases) 
        //     foreach(string dbName in dbNames) 
        //         EditorGUILayout.LabelField(dbName);
    }
    void DoToolbar() {
        GUILayout.BeginHorizontal (EditorStyles.toolbar);
        GUILayout.Space(100);
        GUILayout.FlexibleSpace();
        m_TreeView.searchString = m_SearchField.OnToolbarGUI (m_TreeView.searchString);
        GUILayout.EndHorizontal();
    }
    void DoTreeView() {
        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        m_TreeView.OnGUI(rect);
    }
    void Refresh() {
        bridge.StartCoroutine(GetDatabases());
    }
    IEnumerator GetDatabases() {
        yield return request.Send("show databases");
        dbNames = new List<string>();
        for(int i=0; i<result.data.Count; i++ ) 
            dbNames.Add( result.Value(i, "name") );
    }
}
// internal class TDElement:TreeElement
// {
//     public string db_name, stb_name, tb_name;
//     public TDElement(string name, int depth, int id):base(name, depth, id)
//     {
//     }
// }

}
