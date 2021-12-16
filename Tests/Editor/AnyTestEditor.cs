using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sean21.TDengineConnector;
[CustomEditor(typeof(AnyTest))]
public class AnyTestEditor : Editor
{
    AnyTest t;
    void OnEnabel()
    {
        t = target as AnyTest;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnyTest t = (AnyTest)target;
        if (GUILayout.Button("Run Test 1")) {
            // t.RunTest1();
            Debug.Log("hierarchy capacity: " + t.transform.hierarchyCapacity);
        }
        if (GUILayout.Button("Run Test 2")) {
            t.RunTest2();
        }
    }
}
