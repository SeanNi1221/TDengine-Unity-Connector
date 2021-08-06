using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sean.Bridge;

[CustomEditor(typeof(TDBridge))]
public class TDBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TDBridge td = (TDBridge)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Execute SQL", GUILayout.Height(32)))
        {
            TDBridge.PushSQL(td.sql);
        }
        if (GUILayout.Button("Initialize TDBridge"))
        {
            td.Initialize();
        }
    }
}