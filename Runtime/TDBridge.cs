using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
namespace Sean21
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
    [Tooltip("Partial parsed result from the Json Text. Use your custom class/method to parse the 'data' section of the json. Checkout the Documentation for more information." + "\n" +
        "Do not modify this field because it's pointless.")]
    public Result result;
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
    [Serializable]
    public class Result {
        public string status;
        public List<ColumnMeta> column_meta = new List<ColumnMeta>();
        public List<Row> data = new List<Row>();
        public int rows;
    }
    public class LoginResult {
        public string status;
        public int code;
        public string desc;
    }
    [Serializable]
    public struct ColumnMeta {
        public string name;
        public int typeIndex;
        public int length;
        public bool isResizable{
            get;
            private set;
        }
        public ColumnMeta( string a, int t, int l) {
            this.name = a;
            this.typeIndex = t;
            this.length = l;
            this.isResizable = (t == dataType.IndexOf("nchar") || t == dataType.IndexOf("binary"));
        }
    }
    [Serializable]
    public struct Row {
        public List<string> value; 
        public Row(List<string> _value = null) {
            this.value = _value;
        }
    }

    public enum TimeEncoding { Normal, Unix, UTC }
    static string TimeStamp16 {
        get {return System.DateTime.Now.ToString("yyMMddHHmmssffff");}
    }    
    const string Dot = ".";
    const string Space = " ";

    public static readonly List<System.Type> varType = new List<System.Type>{ typeof(System.Object), typeof(System.Boolean), typeof(System.Byte), typeof(System.Int16), typeof(System.Int32), typeof(System.Int64), typeof(System.Single), typeof(System.Double), typeof(bin), typeof(System.DateTime),typeof(System.String) };
    public static readonly List<string> dataType = new List<string>{ "unkown", "bool", "tinyint", "smallint", "int", "bigint", "float", "double", "binary", "timestamp", "nchar" };
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
            token = ParseLogin(json).desc;
#if UNITY_EDITOR            
            Debug.Log ("Method:" + authorizationMethod + ", token:" + token + ", IP:" + ip);
#endif
            header = "Taosd " + token;
            if (request.isNetworkError || request.isHttpError)
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
            if (request.isNetworkError || request.isHttpError)
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
        if (!i) {
            i = this;
        }
        else if (i != this){
            Debug.LogWarning ("Multiple is of TDBridge is running, this may cause unexpected behaviours!. The newer i is ignored!");
        }
        FetchURI();
        FetchHeader();
        jsonText = null;
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
    static bool isValidForName(char c) {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_');
    }
    static bool isTextData(int typeIndex) {
        return (typeIndex == 8 || typeIndex == 10)? true:false;
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