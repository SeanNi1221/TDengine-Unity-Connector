using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean21.TDengineConnector
{
[ExecuteInEditMode]
public class TDLane : MonoBehaviour
{
    public UnityEngine.Object target;
    public string databaseName;
    public string superTableName;
    public string timeStampName = "ts";
    public string tableName;
    public TDRequest request = new TDRequest();
    public bool usingSuperTable = true;
    public bool autoCreate = false;
    public bool insertSpecific = false;
    public string dataTime = "NOW";
    public readonly Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
    public readonly Dictionary<string, FieldInfo> tags = new Dictionary<string, FieldInfo>();
    public readonly Dictionary<string, int> lengths = new Dictionary<string, int>();
    public readonly Dictionary<string, int> types = new Dictionary<string, int>();
    public bool autoInitialize = true;
    public bool autoSetTarget = true;
    public bool autoSetDatabase = true;
    public bool autoSetSuperTable = true;
    public bool autoSetTable = true;
    void OnEnable()
    {
        if (autoSetTarget && target == null) AutoSetTarget();
        if (autoInitialize) Initialize();
    }
    public void Initialize()
    {
        MakeCache();
        if (autoSetDatabase) AutoSetDatabase();
        if (autoSetSuperTable) AutoSetSuperTable();
        if (autoSetTable) AutoSetTable();
        if (!request.lane) {
            request.lane = this;
        }
    }
    //Search for the first component that has either [DataTag] or [DataField] attribute, and make it target
    void AutoSetTarget()
    {
        Component[] components = GetComponents<Component>();
        foreach (Component comp in components) {
            var obj = comp as UnityEngine.Object;
            foreach (FieldInfo info in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (info.GetCustomAttribute<DataTag>() != null) goto got;
                if (info.GetCustomAttribute<DataField>() != null) goto got;
                continue;
                
                got:
                target = obj;
                return;
            }
        }
    }
    void AutoSetDatabase() {
        if (!string.IsNullOrEmpty(TDBridge.DefaultDatabaseName)) databaseName = TDBridge.DefaultDatabaseName;
    }
    void AutoSetSuperTable() {
        if (target != null) superTableName = SQL.SetSTableNameWith(target);
    }
    void AutoSetTable() {
        tableName = SQL.SetTableNameWith(target);
    }
    //Cache every Tag and Field in dictionaries to boost performance.
    public void MakeCache()
    {
        ClearCache();
        if (target == null) { Debug.LogWarning(tableName + " - No target assigned."); return; }
        FieldInfo[] targetFields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (targetFields.Length < 1) { Debug.Log(tableName + " - No field found."); return; }
        foreach (FieldInfo info in targetFields) {
            var dt = info.GetCustomAttribute<DataTag>();
            if( dt != null) { 
                tags.Add(info.Name, info);
                int infoType = TDBridge.varType.IndexOf(info.FieldType);
                types.Add(info.Name, infoType);
                if ( SQL.isTextData(infoType) ) lengths.Add(info.Name, dt.length); 
                continue;
            }
            var df = info.GetCustomAttribute<DataField>();
            if( df != null) { 
                fields.Add(info.Name, info); 
                int infoType = TDBridge.varType.IndexOf(info.FieldType);
                types.Add(info.Name, infoType);
                if ( SQL.isTextData(infoType) ) lengths.Add(info.Name, df.length); 
                continue; 
            }
        }
    }
    public void ClearCache()
    {
        fields.Clear();
        tags.Clear();
        types.Clear();
        lengths.Clear();
    }
    public void CreateDatabase()
    {
        request.sql = SQL.CreateDatabase(databaseName);
        StartCoroutine(request.Send());
    }

    public void CreateSuperTableForTarget()
    {
        request.sql = SQL.CreateSTable(target, databaseName, superTableName);
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
            request.sql = SQL.CreateTableUsing(target, databaseName, tableName, superTableName );
            StartCoroutine(request.Send());
        }
        else {
            request.sql = SQL.CreateTable(target, databaseName, tableName);
            StartCoroutine(request.Send());
        }
    }
    public void DropTableForTarget()
    {
        request.sql = "DROP TABLE IF EXISTS " + databaseName + "." + tableName;
        StartCoroutine(request.Send());
    }
    public void SendRequest()
    {
        StartCoroutine(request.Send());
    }
    public void SendRequest(string sql) {
        StartCoroutine(request.Send(sql));
    }
    public void PullImmediately(bool withTags = true)
    {
        StartCoroutine(Pull(withTags));
    }
    public void PullTagsImmediately(params string[] tagNames) {
        StartCoroutine(PullTags(tagNames));
    }
    public void PullFieldsImmediately(params string[] fieldNames) {
        StartCoroutine(PullFields(fieldNames));
    }
    public IEnumerator Pull(bool withTags = true)
    {        
        yield return request.Send(SQL.GetLastRow(this, "*", null, withTags));
        if (!request.succeeded) {
            Debug.LogError("Failed Pulling fields! Please enable 'Detailed Debug Log' for detailes.");
            yield break;
        }
        TDBridge.FromTD(this, request.result);
    }
    public IEnumerator PullTags(params string[] tagNames) {
        string sql = null;
        if (tagNames.Length < 1) sql = SQL.GetTags(this);
        else sql = "SELECT " + string.Join(", ", tagNames) + " FROM " + this.databaseName + "." + this.tableName;
        yield return request.Send(sql);
        TDBridge.FromTD(this, request.result, 0, true);
    }
    public IEnumerator PullFields(params string[] fieldNames) {
        string _fieldNames = fieldNames.Length < 1?
            "*" : string.Join(", ", fieldNames);
        yield return request.Send( SQL.GetLastRow(this, _fieldNames, null, false) );
        TDBridge.FromTD(this, request.result);
    }
    public void PushImmediate(params string[] tag_names)
    {
        StartCoroutine(Push(tag_names));
    }
    public IEnumerator Push(params string[] tag_names) {
        List<string> sqls = SQL.SetTags(this, tag_names);
        foreach (string sql in sqls) {
            request.sql = sql;
            yield return request.Send();
        }
    }
    public void Alter()
    {
        StartCoroutine(TDBridge.AlterSTableOf(target, databaseName, superTableName));
    }
    public void Insert()
    {
        if (autoCreate) {
            if (insertSpecific) {
                request.sql = SQL.InsertSpecificUsing(target, databaseName, superTableName, tableName );
            }
            else {
                request.sql = SQL.InsertUsing(target, databaseName, superTableName, tableName );
            }
        }
        else {
            if (insertSpecific) {
                request.sql = SQL.InsertSpecific(target, databaseName, tableName );
            }
            else {
                request.sql = SQL.Insert(target, databaseName, tableName );
            }
        }
        StartCoroutine(request.Send());
    }
    public void ClearRequest()
    {
        request.Clear();
    }
}
}
