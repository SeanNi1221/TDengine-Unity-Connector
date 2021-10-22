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
    SerializedProperty prop;
    public override void OnInspectorGUI()
    {
        TDBridge td = (TDBridge)target;
        
        // Insert button before "request" field
        prop = serializedObject.GetIterator();
        while(prop.NextVisible(true))
        {
            if (prop.name == "request") {
                if (GUILayout.Button("Initialize & Login TDBridge", GUILayout.Height(32))) td.Initialize();
                break;
            }
            EditorGUILayout.PropertyField(prop);
        }
        //Draw "request" field
        EditorGUILayout.PropertyField(prop);
        
        serializedObject.ApplyModifiedProperties();        
    }
}
}