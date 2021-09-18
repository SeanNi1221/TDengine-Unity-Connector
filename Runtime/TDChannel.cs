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
    public bool usingSuperTable = true;
    public bool autoCreate = false;
    public bool insertSpecific = false;
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
        request.sql = TDBridge.SQL.CreateDatabase(databaseName);
        StartCoroutine(request.Send());
    }

    public void CreateSuperTableForTarget()
    {
        request.sql = TDBridge.SQL.CreateSTable(target, databaseName, superTableName);
        StartCoroutine(request.Send());
    }
    public void DropSuperTableForTarget()
    {
        request.sql = "DROP STABLE IF EXISTS " + databaseName + "." + superTableName;
        StartCoroutine(request.Send());
    }
    public void CreateTableForTarget()
    {
        if (usingSuperTable) {
            request.sql = TDBridge.SQL.CreateTableUsing(target, databaseName, tableName, superTableName );
            StartCoroutine(request.Send());
        }
        else {
            request.sql = TDBridge.SQL.CreateTable(target, databaseName, tableName);
            StartCoroutine(request.Send());
        }
    }
    public void DropTableForTarget()
    {
        request.sql = "DROP TABLE IF EXISTS " + databaseName + "." + tableName;
        StartCoroutine(request.Send());
    }
    public void SetTags()
    {
        StartCoroutine(SetTagsCo());
    }
    public IEnumerator SetTagsCo() {
        List<string> sqls = TDBridge.SQL.SetTags(target, databaseName, tableName);
        foreach (string _sql in sqls) {
            request.sql = _sql;
            yield return request.Send();
        }
    }
    public void SendRequest()
    {
        StartCoroutine(request.Send());
    }
    public void Pull()
    {
        StartCoroutine(PullCo());
    }
    public IEnumerator PullCo()
    {
        request.sql = TDBridge.SQL.GetFirstRowWithoutTS(target, databaseName, tableName);
        yield return request.Send();
        Type targetType = target.GetType();
        Debug.Log("Target Type: " + targetType.Name);
        TDBridge.FromTD(ref target, request.result);
    }
    public void Alter()
    {
        StartCoroutine(TDBridge.AlterSTableOf(target, databaseName, superTableName));
    }
    public void Insert()
    {
        if (autoCreate) {
            if (insertSpecific) {
                request.sql = TDBridge.SQL.InsertSpecificUsing(target, databaseName, superTableName, tableName );
            }
            else {
                request.sql = TDBridge.SQL.InsertUsing(target, databaseName, superTableName, tableName );
            }
        }
        else {
            if (insertSpecific) {
                request.sql = TDBridge.SQL.InsertSpecific(target, databaseName, tableName );
            }
            else {
                request.sql = TDBridge.SQL.Insert(target, databaseName, tableName );
            }
        }
        StartCoroutine(request.Send());
    }
}
}
