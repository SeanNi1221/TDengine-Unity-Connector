using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
namespace Sean21.TDengineConnector
{
[Serializable]
[ExecuteInEditMode]
public class TDRequest
{
    [Header("Settings")]
    [Tooltip("Time encoding method of the returned data. Overriding the global setting in TDBridge.")]
    public TDBridge.TimeEncoding timeEncoding;
    public bool enableDebugLog = true;
    public bool enableTimer;
    [Header("Terminal")]
    [TextArea(0,100)]
    public string sql;
    [Header("Response")]
    [TextArea(0,100)]
    public string json;
    public float responseTime;
    public TDResult result;
    public TDLane lane{ get; internal set;}
    public Action OnFinished;
    public bool succeeded{get; private set;}
    [HideInInspector]
    public UnityWebRequest web;
    public UnityWebRequestAsyncOperation operation;
    public TDRequest()
    {
        if (TDBridge.i) this.timeEncoding = TDBridge.DefaultTimeEncoding;
    }
    public TDRequest(string sql)
    {
        this.sql = sql;
        if (TDBridge.i) this.timeEncoding = TDBridge.DefaultTimeEncoding;
    }
    public TDRequest(TDBridge.TimeEncoding format)
    {
        this.timeEncoding = format;
    }
    public TDRequest(string sql, TDBridge.TimeEncoding format)
    {
        this.sql = sql;
        this.timeEncoding = format;
    }
    public TDRequest(bool enableDebugLog, bool enableTimer = false)
    {
        this.enableDebugLog = enableDebugLog;
        this.enableTimer = enableTimer;
    }
    public void Clear()
    {
        result.Clear();

        json = null;
        sql = null;
        succeeded = false;
        responseTime = 0f;
    }
    public void SendImmediate()
    {
        SendImmediate(sql);
    }
    public void SendImmediate(string SQL)
    {
        if (lane) lane.SendRequest(SQL);
        else TDBridge.SendRequest(SQL);
    }
    public IEnumerator Send()
    {
        yield return Send(sql);
    }

    public IEnumerator Send(string SQL)
    {        
        succeeded = false;
        if (string.IsNullOrEmpty(SQL)) {
            if (enableDebugLog) if (TDBridge.DetailedDebugLog) Debug.LogWarning("Cannot send empty string as SQL, aborted!");
            yield break;
        }
        sql = SQL;
        string uri = TDBridge.ChooseUri(timeEncoding);
        using ( web = UnityWebRequest.Put(uri, SQL) ){
            if (enableDebugLog) Debug.Log("Connecting: " + web.uri);
            web.SetRequestHeader("Authorization", TDBridge.i.header);
            web.timeout = TDBridge.RequestTimeLimit;
            operation = web.SendWebRequest();
            if (enableTimer) {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += TDBridge.ConstantLoopUpdate;
#endif
                responseTime = 0f;
                while(!web.isDone) {
                    responseTime += Time.deltaTime;
                    //Incase Enable Timer is turned off during timing process.
                    if (!enableTimer) {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= TDBridge.ConstantLoopUpdate;
#endif                        
                        break;
                    }
                    yield return null;
                }
            }
            yield return operation;
#if UNITY_EDITOR
        if (enableTimer)
            UnityEditor.EditorApplication.update -= TDBridge.ConstantLoopUpdate;
#endif
#if UNITY_2020_1_OR_NEWER
            if (web.result == UnityWebRequest.Result.ConnectionError || web.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (web.isNetworkError || web.isHttpError)
#endif
            {
                Debug.LogError("Failed sending Request: " + SQL + " with error: " + web.error + requestHint(web.responseCode));
                succeeded = false;
                yield break;
            }
            if (enableDebugLog) Debug.Log("Request succeeded" + (enableTimer? " in " + responseTime + " s" : "") + ":\n" + SQL);
            json = web.downloadHandler.text;
            result = TDBridge.Parse(json);
            
            if (OnFinished != null) OnFinished();

            succeeded = true;
            yield break;
        }
    }
    private static Func<long, string> requestHint = x => {
        switch (x) {
            default:
                return "";
            case 400:
                return ", Possible cause: Invalid SQL statement.";
        }
    };
}
}