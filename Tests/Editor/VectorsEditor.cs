using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sean21;
[CustomEditor(typeof(Vectors))]
public class VectorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Vectors v = (Vectors)target;
        if (GUILayout.Button("Run Test")) {
            v.RunTest();
        }
    }
}
