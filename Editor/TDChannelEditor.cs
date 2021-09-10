using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21
{
[CustomEditor(typeof(TDChannel))]
public class TDChannelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIContent createDB = new GUIContent("Create Database", (Texture)Resources.Load("createDB"), "CREATE DATABASE IF NOT EXISTS [Database Name]");

        TDChannel td = (TDChannel)target;
        DrawDefaultInspector();
        if ( GUILayout.Button(createDB) ){
            td.CreateDatabase();
        }

        if (GUILayout.Button("Send SQL Request", GUILayout.Height(32)))
        {
            td.SendRequest();           
        }        
        if (GUILayout.Button("Create Table for Target"))
        {
            td.CreateTableForTarget();           
        }
        if (GUILayout.Button("Push Values to Table"))
        {
            td.PushTags();
        }
        if (GUILayout.Button("Pull Values from Table"))
        {
            td.PullValues();
        }
    }
}
}
