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
    private static string IpForEditor {
        get { return i.ipForEditor; }
        set { i.ipForEditor = value; }
    }
    [Header("Server")]
    [Tooltip("The Server IP to be used if running in Unity Editor.")]
    [SerializeField]
    private string ipForEditor = "127.0.0.1";
    public static string IpForBuild {
        get { return i.ipForBuild; }
        set { i.ipForBuild = value; }
    }
    [Tooltip("The Server IP to be used if running in Built App.")]
    [SerializeField]
    private string ipForBuild = "127.0.0.1";
    public static string Port {
        get { return i.port; }
        set { i.port = value; }
    }
    [SerializeField]
    [Tooltip("The Restful Connector port of TDengine server.")]
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
    public static int RequestTimeLimit {
        get { return i.requestTimeLimit; }
        set { i.requestTimeLimit = value; }
    }
    [Header("Global Settings")]
    [SerializeField]
    private int requestTimeLimit = 15;
    [Tooltip("Default time encoding method of newly created TD Requests")]    
    [SerializeField]
    private TimeEncoding defaultTimeEncoding = TimeEncoding.Normal;
    // public static int DefaultTextLength {
    //     get { return i.defaultTextLength; }
    //     set { i.defaultTextLength = value; }
    // }
    // [Tooltip("Default length for NCHAR and BINARY if not specified on declaration.")]
    // [SerializeField]
    // private int defaultTextLength = 100;
    public static string DefaultDatabaseName {
        get { return i.defaultDatabaseName; }
        set { i.defaultDatabaseName = value; }
    }
    [Tooltip("Default database name if not specified on declaration.")]
    [SerializeField]
    private string defaultDatabaseName = "test";
    public static bool DetailedDebugLog {
        get { return i.detailedDebugLog; }
        set { i.detailedDebugLog = value; }
    }
    [SerializeField]
    private bool detailedDebugLog = false;
    public static TDResult Result{
        get => i.request.result;
    }
    public static TDRequest Request{
        get => i.request;
    }

    public static Action OnInstantiated;
    public static Action OnInitialized;
    [SerializeField]
    private TDRequest request = new TDRequest("SHOW DATABASES");
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
    void Awake()
    {
        Initialize();
    }
    void OnEnable()
    {
        SetInstance();
    }
    public static void Start(IEnumerator coroutine) {
        i.StartCoroutine(coroutine);
    }
    public static void Login() {
        i.StartCoroutine(Login_co());
    }
    public static IEnumerator Login_co()
    {
        using ( UnityWebRequest login = UnityWebRequest.Get(i.uriLogin) ){
#if UNITY_EDITOR
            Debug.Log("Logging in... " + login.uri + "\n This message will not be sent in built");
#else
            Debug.Log("Logging in... ");
#endif
            yield return login.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
            if (login.result == UnityWebRequest.Result.ConnectionError || login.result == UnityWebRequest.Result.ProtocolError)
#else 
            if (login.isNetworkError || login.isHttpError)
#endif
            {
                Debug.LogError("Log in failed! " + login.error);
                yield break;
            }
            string json = login.downloadHandler.text;
#if UNITY_EDITOR
            Request.json = json;
#endif
            i.token = ParseLogin(json).desc;
            i.header = "Taosd " + i.token;
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
    public static void Initialize()
    {
        i.SetInstance(); i.FetchURI(); i.StartCoroutine(i.FetchHeader());
    }
    private void SetInstance()
    {
        if (i == null) {
            i = this;
        }
        else if (i != this){
            Debug.LogWarning ("Multiple is of TDBridge is running, this may cause unexpected behaviours!. The newer i is ignored!");
        }
        if (OnInstantiated != null) TDBridge.OnInstantiated();
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
        Debug.Log("TD Bridge Initialized!. Authorization Method: " + authorizationMethod + ", token:" + token + ", IP:" + ip +
            "\n This Message will not be sent in built.");
#else
        Debug.Log("TD Bridge Initialized!");
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

    //Edit mode coroutine supporter
#if UNITY_EDITOR
    public static void ConstantLoopUpdate()
    {
        if (!Application.isPlaying) {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        }
    }
#endif
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