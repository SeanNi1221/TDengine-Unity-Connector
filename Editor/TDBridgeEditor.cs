using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21
{
[CustomEditor(typeof(TDBridge))]
public class TDBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TDBridge td = (TDBridge)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Push SQL", GUILayout.Height(32)))
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