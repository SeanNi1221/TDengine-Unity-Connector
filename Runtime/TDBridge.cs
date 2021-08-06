using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SimpleJSON;
namespace Sean.Bridge
{
[ExecuteInEditMode]
public class TDBridge : MonoBehaviour
{
    public static TDBridge instance{get; private set;}
    [Header("Server")]
    [Tooltip("The Server IP to be used if running in Unity Editor. This is for convenience during development." + "\n" +
    "e.g., In a typical B/S architecture, you have the same machine as both database server and web server, so you use 192.168.5.5 to connect to the database in the Unity Editor, and use 127.0.0.1 for the same purpose in the deployed webGL environment.")]
    public string ipForEditor = "127.0.0.1";
    [Tooltip("The Server IP to be used if running in Build Application. It needs to direct to the save server as the above one.")]
    public string ipForBuild = "127.0.0.1";
    string ip;
    public string port = "6041";
    public enum AuthorizationMethod { Basic, Taosd }

    [Header("Authorization")]
    public AuthorizationMethod authorizationMethod = AuthorizationMethod.Basic;
    public string userName = "root";
    public string password = "taosdata";
    [Header("Response")]
    [TextArea(1,50)]
    public string jsonText;
    public JSONNode jsonNode;
    public TDResult result;
    public enum TimeEncoding { Normal, Unix, UTC }
    [Header("Terminal")]
    public TimeEncoding responseTimeEncoding = TimeEncoding.Normal;
    [TextArea]
    public string SQL = "show databases";
    string token;
    string header;
    private string uriLogin;
    private string uriSQL;
    private string uriUnix;
    private string uriUTC;
    [Serializable]
    public struct LoginResponse {
        public string status;
        public int code;
        public string desc;
    }
    [Serializable]
    public class TDResult {
        public string status;
        public List<ColumnMeta> columnMeta = new List<ColumnMeta>();
    }
    [Serializable]
    public struct ColumnMeta {
        public string attribute;
        public int type;
        public int length;
        public ColumnMeta( string a, int t, int l) {
            this.attribute = a;
            this.type = t;
            this.length = l;
        }
    }
    void Awake()
    {
        Initialize();
    }
    public static void Login() {
        instance.StartCoroutine(instance.Login_co());
    }
    IEnumerator Login_co()
    {
        using ( UnityWebRequest request = UnityWebRequest.Get(uriLogin) ){
            Debug.Log("Logging in... " + request.uri);
            yield return request.SendWebRequest();
            string json = request.downloadHandler.text;
            token = JsonUtility.FromJson<LoginResponse>(json).desc;
            Debug.Log ("Method:" + authorizationMethod + ", token:" + token + ", IP:" + ip);
            header = "Taosd " + token;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                yield break;
            }
            yield break; 
        }
    }
    public static void PushSQL(string sql, TimeEncoding format = TimeEncoding.Normal) {
        instance.StartCoroutine(instance.RequestSQL(sql, format));
    }
    //Result first
    public static IEnumerator Request(string sql, TDResult resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
        yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, TDResult resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, TDResult resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    //JsonNode first
    public static IEnumerator Request(string sql, JSONNode jsonNodeHolder = null, string jsonHolder = null, TDResult resultHolder = null) {
        yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, JSONNode jsonNodeHolder = null, string jsonHolder = null, TDResult resultHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, JSONNode jsonNodeHolder = null, string jsonHolder = null, TDResult resultHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    //Json first
    public static IEnumerator Request(string sql, string jsonHolder = null, JSONNode jsonNodeHolder = null, TDResult resultHolder = null) {
        yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, string jsonHolder = null, JSONNode jsonNodeHolder = null, TDResult resultHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, string jsonHolder = null, JSONNode jsonNodeHolder = null, TDResult resultHolder = null) {
    yield return instance.StartCoroutine(instance.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    IEnumerator RequestSQL(string sql, TimeEncoding format = TimeEncoding.Normal, TDResult resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null )
    {            
        string uri;
        switch (format) {
            default:
                uri = instance.uriSQL;
                break;
            case TimeEncoding.Normal:
                uri = instance.uriSQL;
                break;
            case TimeEncoding.Unix:
                uri = instance.uriUnix;
                break;
            case TimeEncoding.UTC:
                uri = instance.uriUTC;
                break;                                
        }
        using ( UnityWebRequest request = UnityWebRequest.Put(uri, sql) ){
            Debug.Log("try connecting: " + request.uri);
            request.SetRequestHeader("Authorization", instance.header);
            yield return request.SendWebRequest();                
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                yield break;
            }
            string authorization = request.GetRequestHeader("Authorization").ToString();
            string postContent = System.Text.Encoding.UTF8.GetString(request.uploadHandler.data);
            Debug.Log("posted content: " + postContent);
            
            jsonText = request.downloadHandler.text;
            if (jsonHolder != null) {
                jsonHolder = jsonText;
            }    
            yield return jsonNode = JSON.Parse(jsonText);
            if (jsonNodeHolder != null) {
                jsonNodeHolder = jsonNode;
            }

            result.status = jsonNode["status"].Value;
            result.columnMeta.Clear();
            foreach (JSONNode n in jsonNode["column_meta"]) {
                ColumnMeta _meta = new ColumnMeta(n[0].Value, n[1].AsInt, n[2].AsInt);
                result.columnMeta.Add(_meta);
            }
            if ( resultHolder != null ) {
                resultHolder = result;
            }
            yield break;
        }
    }
    public void Initialize()
    {
        if (!instance) {
            instance = this;
        }
        else if (instance != this){
            Debug.LogWarning ("Multiple instances of TDBridge is running, this may cause unexpected behaviours!. The newer instance is ignored!");
        }
        FetchURI();
        FetchHeader();
        jsonText = null;
        jsonNode = null;
        result = null;
        Debug.Log("TDBridge initialized.");
    }
    private void FetchURI() {
        if (Application.isEditor) {
            ip = ipForEditor;
        }
        else {
            ip = ipForBuild;
        }
        uriSQL = "http://" + ip + ":" + port + "/rest/sql";
        uriLogin = "http://" + ip + ":" + port + "/rest/login/" + userName + "/" + password;
        uriUnix = "http://" + ip + ":" + port + "/rest/sqlt";
        uriUTC = "http://" + ip + ":" + port + "/rest/sqlutc";
    }
    private void FetchHeader() {
        switch(authorizationMethod)
        {
            case AuthorizationMethod.Basic:
                token = Base64Encode(userName + ":" + password);
                Debug.Log ("Method:" + authorizationMethod + ", token:" + token + ", IP:" + ip);
                header = "Basic " + token;
                break;
            case AuthorizationMethod.Taosd:
                StartCoroutine(Login_co());
                break;
        }
    }
    public static string Base64Encode(string t){
        var tBytes =  System.Text.Encoding.UTF8.GetBytes(t);
        return System.Convert.ToBase64String(tBytes);
    }
    public static string Base64Decode(string base64){
        var base64Bytes = System.Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }
}
}