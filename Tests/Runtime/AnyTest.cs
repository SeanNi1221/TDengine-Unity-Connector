using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;
using System;
using System.Reflection;
[ExecuteInEditMode]
public class AnyTest : MonoBehaviour
{
    public FieldInfo[] fields = new FieldInfo[10];
    public string[] fieldNames = new string[10];
    public int value = 1;
    public object obj;
    void OnEnable()
    {
        
    }
    public void CheckValue()
    {
        Debug.Log("fields[0]: " + ((int)(fields[0].GetValue(this))).ToString());
        Debug.Log("obj: " + ((int)obj).ToString());        
    }
    public void RunTest1()
    {
        Debug.Log("Running Test1...");
        CheckValue();
        Debug.Log("Test1 Finished");
    }
    public void RunTest2()
    {
        Debug.Log("Running Test2...");
        Clear();
        Debug.Log("Test2 Finished");
    }
    public
    void Clear()
    {
        Array.Clear(fields, 0, fields.Length);
    }
    void Update()
    {
        for (int i=0; i < fieldNames.Length; i++) {
            if(fields[i] != null) fieldNames[i] = fields[i].Name;
            else fieldNames[i] = "null";
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += TDBridge.ConstantLoopUpdate;
#endif

    }

}
