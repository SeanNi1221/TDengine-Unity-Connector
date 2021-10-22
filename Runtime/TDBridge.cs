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
    [SerializeField]
    private static string IpForEditor {
        get { return i.ipForEditor; }
        set { i.ipForEditor = value; }
    }
    [Header("Server")]
    [Tooltip("The Server IP to be used if running in Unity Editor. This is for convenience during development." + "\n" +
    "e.g., In a typical B/S architecture, you have the same machine as both database server and web server, so you use 192.168.5.5 to connect to the database in the Unity Editor, and use 127.0.0.1 for the same purpose in the deployed webGL environment.")]
    [SerializeField]
    private string ipForEditor = "127.0.0.1";
    public static string IpForBuild {
        get { return i.ipForBuild; }
        set { i.ipForBuild = value; }
    }
    [Tooltip("The Server IP to be used if running in Build Application. It needs to direct to the same server as the above one.")]
    [SerializeField]
    private string ipForBuild = "127.0.0.1";
    public static string Port {
        get { return i.port; }
        set { i.port = value; }
    }
    [SerializeField]
    private string port = "6041";
    public enum AuthorizationMethod { Basic, Taosd }
    public static AuthorizationMethod Authorization {
        get { return i.authorizationMethod; }
        set { i.authorizationMethod = value; }
    }
    [Header("Authorization")]
    [SerializeField]
    private AuthorizationMethod authorizationMethod = AuthorizationMethod.Basic;
    public static string UserName {
        get { return i.userName; }
        set { i.userName = value; }
    }
    [SerializeField]
    private string userName = "root";
    public static string Password {
        get { return i.password; }
        set { i.password = value; }
    }
    [SerializeField]
    private string password = "taosdata";
    public static TimeEncoding DefaultTimeEncoding {
        get { return i.defaultTimeEncoding; }
        set { i.defaultTimeEncoding = value; }
    }
    [Header("Global Settings")]
    [Tooltip("Use this time encoding method if not declared")]    
    [SerializeField]
    private TimeEncoding defaultTimeEncoding = TimeEncoding.Normal;
    public static int DefaultTextLength {
        get { return i.defaultTextLength; }
        set { i.defaultTextLength = value; }
    }
    [Tooltip("Length for NCHAR and BINARY if not declared.")]
    [SerializeField]
    private int defaultTextLength = 100;
    public static string DefaultDatabaseName {
        get { return i.defaultDatabaseName; }
        set { i.defaultDatabaseName = value; }
    }
    [Tooltip("Database name if not declared")]
    [SerializeField]
    private string defaultDatabaseName = "test";
    public static bool DetailedDebugLog {
        get { return i.detailedDebugLog; }
        set { i.detailedDebugLog = value; }
    }
    [SerializeField]
    private bool detailedDebugLog = false;
    public static TDRequest Request{
        get{ return i.request; }
    }
    [SerializeField]
    private TDRequest request = new TDRequest("show databases");
    private string ip;
    private string token;
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
    void OnEnable()
    {
        Initialize();
    }
    public static void Login() {
        i.StartCoroutine(i.Login_co());
    }
    IEnumerator Login_co()
    {
        using ( UnityWebRequest request = UnityWebRequest.Get(uriLogin) ){
#if UNITY_EDITOR
            Debug.Log("Logging in... " + request.uri + "\n This message will not be sent in built");
#else
            Debug.Log("Logging in... ");
#endif
            yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError(request.error);
                yield break;
            }
            string json = request.downloadHandler.text;
#if UNITY_EDITOR
            i.request.json = json;
#endif
            token = ParseLogin(json).desc;
            header = "Taosd " + token;
            yield break; 
        }
    }
    public static void SendRequest() {
        i.StartCoroutine(Request.Send());
    }
    public static void SendRequest(string sql) {
        i.StartCoroutine(Request.Send(sql));
    }
    public static void ClearRequest() {
        i.request.Clear();
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
        SetInstance(); FetchURI(); StartCoroutine(FetchHeader());
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
    private IEnumerator FetchHeader() {
        switch(authorizationMethod)
        {
            case AuthorizationMethod.Basic:
                token = Base64Encode(userName + ":" + password);
                header = "Basic " + token; 
                break;
            case AuthorizationMethod.Taosd:
                yield return StartCoroutine(Login_co());
                break;
        }
#if UNITY_EDITOR        
        Debug.Log("Logged in. Authorization Method: " + authorizationMethod + ", token:" + token + ", IP:" + ip +
            "\n TD Bridge Initialized! This Message will not be sent in built.");
#else
        Debug.Log("Logged in!");
#endif 

    }
    public static string ChooseUri(TimeEncoding format)
    {
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