using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
namespace Sean21.TDengineConnector
{
public class TDScopeWindow : EditorWindow
{
    [SerializeField] TreeViewState m_state;
    [SerializeField] TDScopeData m_data;
    TDTreeView m_view;
    static string TDFolderName = "UnityTDengineConnectorData";
    static string ScopeDataPath = "Assets/" + TDFolderName + "/TDScopeData.asset";
    float pointerX;
    Styles.LoadingIndicator loadingIndicator;
    [MenuItem("Window/TD Scope")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow<TDScopeWindow>();
        window.titleContent = new GUIContent("TD Scope", (Texture)Resources.Load("scope"));
        window.Show();
    }
    void OnEnable() {
        ValidateMembers();
    }

    void OnGUI() {
        //Test
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh")) m_view.Refresh();
        if (GUILayout.Button("Validate")) ValidateMembers();
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("db_names:" + m_data.db_names.Count.ToString());
        EditorGUILayout.LabelField("loading:" + m_view.loadingData.ToString());
        EditorGUILayout.EndHorizontal();
        
        //Release
        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        m_view.OnGUI(rect);
        loadingIndicator.Draw(position.height-5, position.width);
    }
    void Update() {
        loadingIndicator.Update(m_view.loadingData);
        if(m_view.loadingData) Repaint();
    }
    void ValidateMembers() {
        if (TDBridge.i == null) {
            Debug.LogError("TDBridge not available!");
            return;
        }
        if(m_state == null) m_state = new TreeViewState();        
        if (!m_data) {
            m_data = (TDScopeData)AssetDatabase.LoadAssetAtPath(ScopeDataPath, typeof(TDScopeData));
        }
        if (!m_data) {
            m_data = ScriptableObject.CreateInstance<TDScopeData>();
            AssetDatabase.CreateFolder("Assets", TDFolderName);
            AssetDatabase.CreateAsset(m_data, ScopeDataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        m_view = new TDTreeView(m_state, m_data);
        m_view.Refresh();
    }
}
}
