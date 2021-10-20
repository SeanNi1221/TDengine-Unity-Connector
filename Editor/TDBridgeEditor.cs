using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomEditor(typeof(TDBridge))]
public class TDBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TDBridge td = (TDBridge)target;
        DrawDefaultInspector();
        GUIContent push = new GUIContent(" Push SQL", (Texture)Resources.Load("terminal"), "Push SQL statement");
        if (GUILayout.Button(push, GUILayout.Height(32)))
        {
            TDBridge.PushSQL(td.SQL_);           
        }
        if (GUILayout.Button("Initialize & Login TDBridge"))
        {
            td.Initialize();
        }
    }
}
}