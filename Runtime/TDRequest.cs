using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
namespace Sean21.TDengineConnector
{
[Serializable]
public class TDRequest
{
    [Header("Settings")]
    [Tooltip("Time encoding method of the returned data. Overriding the global setting in TDBridge.")]
    public TDBridge.TimeEncoding timeEncoding;
    [Header("Terminal")]
    [TextArea(0,100)]
    public string sql;
    [Header("Response")]
    [TextArea(0,100)]
    public string json;
    public TDResult result;
    [HideInInspector]
    public TDChannel channel;
    [HideInInspector]
    public bool succeeded;
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
    public void Clear()
    {
        result.Clear();
        json = null;
        sql = null;
        succeeded = false;
    }
    public void Push()
    {
        Push(sql);
    }
    public void Push(string SQL)
    {
        if (channel) channel.SendRequest(SQL);
        else TDBridge.SendRequest(SQL);
    }
    public IEnumerator Send()
    {
        yield return Send(sql);
    }

    public IEnumerator Send(string SQL)
    {
        if (string.IsNullOrEmpty(SQL)) {
            if (TDBridge.DetailedDebugLog) Debug.LogWarning("Cannot send empty string as SQL, aborted!");
            yield break;
        }
        sql = SQL;
        string uri = TDBridge.ChooseUri(timeEncoding);
        using ( web = UnityWebRequest.Put(uri, SQL) ){
            Debug.Log("Connecting: " + web.uri);
            web.SetRequestHeader("Authorization", TDBridge.i.header);
            yield return operation = web.SendWebRequest();
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
            Debug.Log("Successfully sent Request: \n" + SQL);
            json = web.downloadHandler.text;
            result = TDBridge.Parse(json);
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