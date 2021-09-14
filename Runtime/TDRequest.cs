using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
namespace Sean21
{
[Serializable]
public class TDRequest
{
    public TDBridge.TimeEncoding timeEncoding;
    public UnityWebRequest web;
    public UnityWebRequestAsyncOperation operation;
    public TDBridge.Result result;
    public string json;
    [TextArea(0,50)]
    public string sql;
    public TDRequest(string sql)
    {
        this.sql = sql;
        this.timeEncoding = TDBridge.i.defaultTimeEncoding;
    }
    public TDRequest(string sql, TDBridge.TimeEncoding format)
    {
        this.sql = sql;
        this.timeEncoding = format;
    }

    public IEnumerator Send()
    {
        string uri = TDBridge.ChooseUri(timeEncoding);
        using ( web = UnityWebRequest.Put(uri, sql) ){
            Debug.Log("Connecting: " + web.uri);
            web.SetRequestHeader("Authorization", TDBridge.i.header);
            yield return operation = web.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
            if (web.result == UnityWebRequest.Result.ConnectionError || web.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (web.isNetworkError || web.isHttpError)
#endif
            {
                Debug.LogError("Failed sending Request: " + sql + " with error: " + web.error + TDBridge.requestHint(web.responseCode));
                yield break;
            }
            Debug.Log("Successfully sent Request: \n" + sql);
            this.json = web.downloadHandler.text;
            this.result = TDBridge.Parse(json);
            yield break;
        }
    }
}
}