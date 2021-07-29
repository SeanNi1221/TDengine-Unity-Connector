using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
        [SerializeField]
        [Header("Authorization")]
        public AuthorizationMethod authorizationMethod = AuthorizationMethod.Basic;
        public string userName = "root";
        public string password = "taosdata";
        [Header("Operation")]
        [TextArea]
        public string sql = "show databases";
        string token;
        string header;
        void Awake()
        {
            Initialize();
        }
        void OnValidate()
        {
            ip = ipForEditor;
        }
        // public static string GetToken() {

        // }
        // IEnumerator GetToken_co() {}
        public static void SQL(string sql) {
            if (!instance) {
                Debug.LogError("Please Initialize TDBridge first!");
            }
            instance.StartCoroutine(instance.SQL_co(sql));
        }
        IEnumerator SQL_co(string sql)
        {            
            string uri = "http://" + ip + ":" + port + "/rest/" + "sql";
            string authorization;
            string postContent;
            string response;
            using ( UnityWebRequest request = UnityWebRequest.Put(uri, sql) ){
                Debug.Log("try connecting: " + request.uri);
                request.SetRequestHeader("Authorization", header);
                yield return request.SendWebRequest();                
                authorization = request.GetRequestHeader("Authorization").ToString();
                postContent = System.Text.Encoding.UTF8.GetString(request.uploadHandler.data);
                response = request.downloadHandler.text;
                Debug.Log("autorization: " + authorization);
                Debug.Log("post content: " + postContent);
                Debug.Log("response:" + response);
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);
                    yield break;
                }
                yield break;
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
        public void Initialize()
        {
            if (!instance) {
                instance = this;
            }
            else if (instance != this){
                Debug.LogWarning ("Multiple instances of TDBridge is running, this may cause unexpected behaviours!. The newer instance is ignored!");
            }
            if (Application.isEditor) {
                ip = ipForEditor;
            }
            else {
                ip = ipForBuild;
            }
            switch(authorizationMethod)
            {
                case AuthorizationMethod.Basic:
                    token = Base64Encode(userName + ":" + password);
                    Debug.Log ("userName: " + userName + ", password:" + password + ", token:" + token + ", IP:" + ip);
                    header = "Basic " + token;
                    break;
                case AuthorizationMethod.Taosd:
                    break;
            }
            Debug.Log("TDBridge initialized.");
        }
    }
}