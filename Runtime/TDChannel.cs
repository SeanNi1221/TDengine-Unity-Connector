using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean21
{
[ExecuteInEditMode]
public class TDChannel : MonoBehaviour
{
    public UnityEngine.Object target;
    public string databaseName;
    public string superTableName;
    public string tableName;
    public TDRequest request;
    void Awake()
    {
        GetTarget();
        SetDefaultValues();
    }
    private void GetTarget()
    {
        if (target != null) return;
        Component[] components = GetComponents<Component>();
        foreach (Component comp in components) {
            UnityEngine.Object obj = comp as UnityEngine.Object;
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
                if ( dt != null || df != null) {
                    target = obj;
                    components = null;
                    return;
                }
            }
        }

    }
    private void SetDefaultValues()
    {
        if (string.IsNullOrEmpty(databaseName)) {
            databaseName = TDBridge.i.defaultDatabaseName;
        }
        if (string.IsNullOrEmpty(superTableName)) {
            if (target != null) superTableName = target.GetType().Name;
        }
        if (string.IsNullOrEmpty(tableName)) {
            tableName = gameObject.name;
        }
    }
    public void CreateDatabase()
    {
        TDBridge.CreateDatabase(databaseName);
    }
    public void CreateTableForTarget()
    {
        TDBridge.CreateTableUsing(target, databaseName, tableName, superTableName );
    }
    public void PushTags()
    {
        StartCoroutine(TDBridge.SetTags(target, databaseName, tableName));
    }
    public void SendRequest()
    {
        StartCoroutine(request.Send());
    }
    public void PullValues()
    {
        StartCoroutine(PullValuesCo());
    }
    public IEnumerator PullValuesCo()
    {
        request.sql = TDBridge.SQL.GetFirstRowWithoutTS(target, databaseName, tableName);
        yield return request.Send();
        Type targetType = target.GetType();
        Debug.Log("Target Type: " + targetType.Name);
        TDBridge.FromTD(ref target, request.result);
    }

}
}
