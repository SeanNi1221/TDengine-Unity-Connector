using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sean21.TDengineConnector;
[CustomEditor(typeof(AnyTest))]
public class AnyTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnyTest t = (AnyTest)target;
        if (GUILayout.Button("Run Test 1")) {
            t.RunTest1();
        }
        if (GUILayout.Button("Run Test 2")) {
            t.RunTest2();
        }
    }
}
