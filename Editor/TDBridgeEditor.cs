using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomEditor(typeof(TDBridge))]
[CanEditMultipleObjects]
public class TDBridgeEditor : Editor
{
    GUIStyle noteStyle = new GUIStyle();
    SerializedProperty prop;
    TDBridge td;
    public override void OnInspectorGUI()
    {
        td = (TDBridge)target;
        
        //Note style
        noteStyle.fontStyle = FontStyle.Normal;
        noteStyle.alignment = TextAnchor.MiddleCenter;

        // Insert button before "request" field
        prop = serializedObject.GetIterator();
        while(prop.NextVisible(true))
        {
            if (prop.name == "request") {
                EditorGUILayout.LabelField("*Please initialize to apply changes*", noteStyle);
                if (GUILayout.Button("Initialize & Login TDBridge", GUILayout.Height(32))) td.Initialize();
                break;
            }
            EditorGUILayout.PropertyField(prop);
        }
        //Draw "request" field
        EditorGUILayout.PropertyField(prop);
        
        serializedObject.ApplyModifiedProperties();  

        RequiresConstantRepaint();      
    }
}
}