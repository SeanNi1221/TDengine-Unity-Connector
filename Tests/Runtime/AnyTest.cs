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
    public Transform obj;
    void OnEnable()
    {
        
    }
    public void RunTest1()
    {
        Debug.Log("Running Test1...");
        var now = System.DateTime.Now;
        Debug.Log(now.ToString());
        Debug.Log($"second:{now.Second.ToString()}");
        Debug.Log("Test1 Finished");
    }
    public void RunTest2()
    {
        Debug.Log("Running Test2...");
        Debug.Log("Test2 Finished");
    }
    IEnumerator ParentCoroutine() {
        Debug.Log("Parent Coroutine started!");
        yield return ChildCoroutine();
        Debug.Log("Parent Coroutine continued after child broke!");
    }
    IEnumerator ChildCoroutine() {
        for (int i=0; i<3; i++) {
            Debug.Log($"Child Coroutine iteration: {i}");
            yield return null;
        }
        yield break;
    }
    void Update() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += TDBridge.ConstantLoopUpdate;
#endif
    
    }
}
