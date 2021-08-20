using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SimpleJSON;
namespace Sean.Bridge
{


[ExecuteInEditMode]
public partial class TDBridge : MonoBehaviour
{
    public static TDBridge i{get; private set;}
    [Header("Server")]
    [Tooltip("The Server IP to be used if running in Unity Editor. This is for convenience during development." + "\n" +
    "e.g., In a typical B/S architecture, you have the same machine as both database server and web server, so you use 192.168.5.5 to connect to the database in the Unity Editor, and use 127.0.0.1 for the same purpose in the deployed webGL environment.")]
    public string ipForEditor = "127.0.0.1";
    [Tooltip("The Server IP to be used if running in Build Application. It needs to direct to the same server as the above one.")]
    public string ipForBuild = "127.0.0.1";
    public string port = "6041";
    public enum AuthorizationMethod { Basic, Taosd }

    [Header("Authorization")]
    public AuthorizationMethod authorizationMethod = AuthorizationMethod.Basic;
    public string userName = "root";
    public string password = "taosdata";
    [Header("Global Settings")]
    [Tooltip("Responded Time Encoding Method if not declared")]    
    public TimeEncoding defaultTimeEncoding = TimeEncoding.Normal;
    [Tooltip("Length for NCHAR and BINARY if not declared.")]
    public int defaultTextLength = 10;
    [Tooltip("Database name if not declared")]
    public string defaultDatabaseName = "test";
    [Header("Response")]
    [TextArea(0,50)]
    [Tooltip("The responded json from your TDengine server." + "\n" +
        "Do not modify this field because it's pointless.")]
    public string jsonText;
    public JSONNode jsonNode;
    [Tooltip("Partial parsed result from the Json Text. Use your custom class/method to parse the 'data' section of the json. Checkout the Documentation for more information." + "\n" +
        "Do not modify this field because it's pointless.")]
    public Result result;
    [Header("Terminal")]
    [TextArea(0,50)]
    [Tooltip("Insert your SQL statement to be executed.")]
    public string SQL_ = "show databases";
    string ip;
    string token;
    string header;
    private string uriLogin;
    private string uriSQL;
    private string uriUnix;
    private string uriUTC;
    [Serializable]
    public struct LoginResult {
        public string status;
        public int code;
        public string desc;
    }
    [Serializable]
    public class Result {
        public string status;
        public List<ColumnMeta> columnMeta = new List<ColumnMeta>();
    }
    [Serializable]
    public struct ColumnMeta {
        public string attribute;
        public int typeIndex;
        public int length;
        public bool isResizable{
            get;
            private set;
        }
        public ColumnMeta( string a, int t, int l) {
            this.attribute = a;
            this.typeIndex = t;
            this.length = l;
            this.isResizable = (t == dataType.IndexOf("nchar") || t == dataType.IndexOf("binary"));
        }
    }
    //For BINARY type in the database.
    public struct Bin {
        public byte[] Byte;
        public Bin( byte[] _Byte ) {
            this.Byte = _Byte;
        }
        public string Decode() {
            return ASCIIDecode(Byte);
        }
        public void Endoce( string s ) {
            Byte = ASCIIEncode(s);
        }
    }
    public enum TimeEncoding { Normal, Unix, UTC }
    static string TimeStamp14 {
        get {return System.DateTime.Now.ToString("yyMMddHHmmssff");}
    }    
    const string Dot = ".";
    const string Space = " ";
    void Awake()
    {
        Initialize();
    }
    public static void Login() {
        i.StartCoroutine(i.Login_co());
    }
    IEnumerator Login_co()
    {
        using ( UnityWebRequest request = UnityWebRequest.Get(uriLogin) ){
            Debug.Log("Logging in... " + request.uri);
            yield return request.SendWebRequest();
            string json = request.downloadHandler.text;
            token = JsonUtility.FromJson<LoginResult>(json).desc;
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
        i.StartCoroutine(i.RequestSQL(sql, format));
    }
    //Ignore results
    public IEnumerator Request(string sql) {
        yield return i.StartCoroutine(i.RequestSQL(sql));
    }
    //Result first
    public static IEnumerator Request(string sql, Result resultHolder, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
        yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, Result resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, Result resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    //JsonNode first
    public static IEnumerator Request(string sql, JSONNode jsonNodeHolder, string jsonHolder = null, Result resultHolder = null) {
        yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, JSONNode jsonNodeHolder = null, string jsonHolder = null, Result resultHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, JSONNode jsonNodeHolder = null, string jsonHolder = null, Result resultHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    //Json first
    public static IEnumerator Request(string sql, string jsonHolder, JSONNode jsonNodeHolder = null, Result resultHolder = null) {
        yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Normal, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUnix(string sql, string jsonHolder = null, JSONNode jsonNodeHolder = null, Result resultHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.Unix, resultHolder, jsonNodeHolder, jsonHolder));
    }
    public static IEnumerator RequestUTC(string sql, string jsonHolder = null, JSONNode jsonNodeHolder = null, Result resultHolder = null) {
    yield return i.StartCoroutine(i.RequestSQL(sql, TimeEncoding.UTC, resultHolder, jsonNodeHolder, jsonHolder));
    }
    IEnumerator RequestSQL(string sql, TimeEncoding format = TimeEncoding.Normal, Result resultHolder = null, JSONNode jsonNodeHolder = null, string jsonHolder = null )
    {            
        string uri;
        switch (format) {
            default:
                uri = i.uriSQL;
                break;
            case TimeEncoding.Normal:
                uri = i.uriSQL;
                break;
            case TimeEncoding.Unix:
                uri = i.uriUnix;
                break;
            case TimeEncoding.UTC:
                uri = i.uriUTC;
                break;                                
        }
        using ( UnityWebRequest request = UnityWebRequest.Put(uri, sql) ){
            Debug.Log("try connecting: " + request.uri);
            request.SetRequestHeader("Authorization", i.header);
            yield return request.SendWebRequest();                
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                yield break;
            }
            string authorization = request.GetRequestHeader("Authorization").ToString();
            string postContent = System.Text.Encoding.UTF8.GetString(request.uploadHandler.data);
            Debug.Log("pushed SQL: " + postContent);
            
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
        if (!i) {
            i = this;
        }
        else if (i != this){
            Debug.LogWarning ("Multiple is of TDBridge is running, this may cause unexpected behaviours!. The newer i is ignored!");
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
    static string Base64Encode(string t){
        var tBytes =  System.Text.Encoding.UTF8.GetBytes(t);
        return System.Convert.ToBase64String(tBytes);
    }
    static string Base64Decode(string base64){
        var base64Bytes = System.Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }
    static byte[] ASCIIEncode(string s) {
        return System.Text.Encoding.ASCII.GetBytes(s);
    }
    static string ASCIIDecode(byte[] bArray) {
        return System.Text.Encoding.ASCII.GetString(bArray);
    }
    static string Quote(string s) {
        if ( s.StartsWith("'") && s.EndsWith("'") ) {
            return s;
        }
        else return "'" + s + "'";
    }
    static string Quote(int l) {
        return Quote(l.ToString());
    }
    static string Quote(float f) {
        return Quote(f.ToString());
    }
    static string Bracket(string s, bool force = false) {
        if (force ) {
            return "(" + s + ")";
        }
        else {
            if ( s.StartsWith("(") && s.EndsWith(")") ) {
                return s;
            }
            else return "(" + s + ")";
        }
    }
    static string Bracket(int i) {
        return Bracket(i.ToString(), true);
    }
    static string Bracket(float f) {
        return Bracket(f.ToString(), true);
    }
}
}