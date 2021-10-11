using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
namespace Sean21.TDengineConnector
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
    public static string JsonText {get{return TDBridge.i.jsonText;}}
    [Tooltip("Partial parsed result from the Json Text. Use your custom class/method to parse the 'data' section of the json. Checkout the Documentation for more information." + "\n" +
        "Do not modify this field because it's pointless.")]
    public TDResult result;
    public static TDResult Result {get{return TDBridge.i.result;}}
    [Header("Terminal")]
    [TextArea(0,50)]
    [Tooltip("Insert your SQL statement to be executed.")]
    public string SQL_ = "show databases";
    string ip;
    string token;
    public string header {
        get; private set;
    }
    private string uriLogin;
    private string uriSQL;
    private string uriUnix;
    private string uriUTC;
    public enum TimeEncoding { Normal, Unix, UTC }  
    public static readonly List<System.Type> varType = new List<System.Type>{ typeof(System.Object), typeof(System.Boolean), typeof(System.Byte), typeof(System.Int16), typeof(System.Int32), typeof(System.Int64), typeof(System.Single), typeof(System.Double), typeof(bin), typeof(System.DateTime), typeof(System.String), typeof(Vector2), typeof(UnityEngine.Vector3), typeof(UnityEngine.Quaternion), typeof(UnityEngine.Transform)};
    public static readonly List<string> dataType = new List<string>{ "nchar(100)", "bool", "tinyint", "smallint", "int", "bigint", "float", "double", "binary", "timestamp", "nchar", "nchar(36)", "nchar(54)", "nchar(72)", "nchar(164)" };
    void Awake()
    {
        Initialize();
    }
    void OnValidate()
    {
        SetInstance(); FetchURI(); FetchHeader();
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
            token = ParseLogin(json).desc;
#if UNITY_EDITOR            
            Debug.Log ("Method:" + authorizationMethod + ", token:" + token + ", IP:" + ip);
#endif
            header = "Taosd " + token;
#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError(request.error);
                yield break;
            }
            yield break; 
        }
    }
    public static void PushSQL(string sql) {
        i.StartCoroutine(Push(sql));
    }
    public static IEnumerator Push(string sql)
    {            
        string uri = ChooseUri(i.defaultTimeEncoding);
        using ( UnityWebRequest request = UnityWebRequest.Put(uri, sql) ){
            request.SetRequestHeader("Authorization", i.header);
            yield return request.SendWebRequest();                
#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("Failed pushing SQL: \n" + sql + "\n with error: " + request.error + requestHint(request.responseCode));
                yield break;
            }
            Debug.Log("Successfully pushed SQL: \n" + sql);
#if UNITY_EDITOR                        
            i.jsonText = request.downloadHandler.text;
            i.result = Parse(i.jsonText);
#endif
            yield break;
        }
    }
    public static Func<long, string> requestHint = x => {
        switch (x) {
            default:
                return "";
            case 400:
                return ", Possible cause: Invalid SQL statement.";
        }
    };
    public void Initialize()
    {
        SetInstance();
        FetchURI();
        FetchHeader();
        jsonText = null;
        result = null;
        Debug.Log("TDBridge initialized." + 
        "\n Authorization Method: " + authorizationMethod + ", token:" + token + ", IP:" + ip);
    }
    private void SetInstance()
    {
        if (!i) {
            i = this;
        }
        else if (i != this){
            Debug.LogWarning ("Multiple is of TDBridge is running, this may cause unexpected behaviours!. The newer i is ignored!");
        }
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
                header = "Basic " + token;
                break;
            case AuthorizationMethod.Taosd:
                StartCoroutine(Login_co());
                break;
        }
    }
    public static string ChooseUri(TimeEncoding format)
    {
        // string uri;
        switch (format) {
            default:
                return i.uriSQL;
            case TimeEncoding.Normal:
                return i.uriSQL;
            case TimeEncoding.Unix:
                return i.uriUnix;
            case TimeEncoding.UTC:
                return i.uriUTC;
        }
        // return uri;
    }
    static string Base64Encode(string t){
        var tBytes =  System.Text.Encoding.UTF8.GetBytes(t);
        return System.Convert.ToBase64String(tBytes);
    }
    static string Base64Decode(string base64){
        var base64Bytes = System.Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }
    public static byte[] ASCIIEncode(string s) {
        return System.Text.Encoding.ASCII.GetBytes(s);
    }
    public static string ASCIIDecode(byte[] bArray) {
        return System.Text.Encoding.ASCII.GetString(bArray);
    }
}
//For BINARY type in the database.
[Serializable]
public struct bin {
    public string String;
    public byte[] Byte {
        get {
            return TDBridge.ASCIIEncode(String);
        }
        set {
            String = TDBridge.ASCIIDecode(value);
        }
    }
    public bin( string s = null) {
        this.String = s;
    }
    public bin( byte[] b ) {
        this.String = TDBridge.ASCIIDecode(b);
    }
    public override string ToString() {
        return this.String;
    }
    public static implicit operator string(bin b) => b.String;
    public static implicit operator byte[](bin b) => b.Byte;
    public static implicit operator bin(string s) => new bin(s);
    public static implicit operator bin(byte[] b) => new bin(b);
}
}