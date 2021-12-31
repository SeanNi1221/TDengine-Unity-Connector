using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomPropertyDrawer(typeof(TDRequest))]
public class TDRequestDrawer : PropertyDrawer
{
    SerializedProperty timeEncodingProp;
    SerializedProperty enableDebugLogProp;
    SerializedProperty enableTimerProp;
    SerializedProperty sqlProp;
    SerializedProperty responseTimeProp;
    SerializedProperty jsonProp;
    SerializedProperty resultProp;
    GUIStyle labelStyle = new GUIStyle();
    Color labelBG = Styles.Palette.Minor1;        
    //Field styles
    GUIContent timeEncodingLabel = new GUIContent("Time Encoding","Use this method to encode TIMESTAMP values in the returned data.");
    GUIContent enableDebugLogLabel = new GUIContent("Enable Debug Log","Enable the current TDRequest to send debug logs.");
    GUIContent enableTimerLabel = new GUIContent("Enable Timer","Measure the response time(second) since request sent.");
    GUIContent sqlLabel = new GUIContent("SQL Statement:");
    GUIContent jsonLabel = new GUIContent("Returned JSON:");
    GUIContent resultLabel = new GUIContent("Parsed Result:");

    //Button styles
    GUIContent send = new GUIContent(" Send Request", (Texture)Resources.Load("terminal"), "Send SQL Request");
    GUIContent clearRequest = new GUIContent( (Texture)Resources.Load("clear"), "Clear Request");

    internal Styles.LoadingIndicator loadingIndicator;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);
        
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        //References
        TDRequest td = fieldInfo.GetValue(property.serializedObject.targetObject) as TDRequest;
        
        timeEncodingProp = property.FindPropertyRelative("timeEncoding"); 
        enableDebugLogProp = property.FindPropertyRelative("enableDebugLog");      
        enableTimerProp = property.FindPropertyRelative("enableTimer");      
        sqlProp = property.FindPropertyRelative("sql");
        responseTimeProp = property.FindPropertyRelative("responseTime");
        jsonProp = property.FindPropertyRelative("json");
        resultProp = property.FindPropertyRelative("result");

        //Label style
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 12;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.normal.textColor = Color.white;

        //Draw
        EditorGUI.DrawRect(new Rect(position.x-20, position.y+16, position.width+40, 28), labelBG);
        EditorGUILayout.LabelField("Request", labelStyle);
        EditorGUILayout.PropertyField(timeEncodingProp, timeEncodingLabel);
        EditorGUILayout.PropertyField(enableDebugLogProp, enableDebugLogLabel);
        EditorGUILayout.PropertyField(enableTimerProp, enableTimerLabel);
        EditorGUILayout.PropertyField(sqlProp, sqlLabel);
        
        GUILayout.BeginHorizontal();  
        if (GUILayout.Button(send, GUILayout.Height(32))) td.SendImmediate();           
        if ( GUILayout.Button(clearRequest, GUILayout.Width(32), GUILayout.Height(32))) td.Clear();
        GUILayout.EndHorizontal();
        
        float endDivisionBias = 40f;
        if (enableTimerProp.boolValue) {
            EditorGUILayout.PropertyField(responseTimeProp);
            endDivisionBias = 24f;
        }
        Rect rect = GUILayoutUtility.GetLastRect();
        EditorGUI.DrawRect(new Rect(rect.x-20, rect.y + endDivisionBias, position.width+40, 2), labelBG);
        //loading indicator
        loadingIndicator.Draw(rect.y + endDivisionBias, Screen.width);
        loadingIndicator.Update( td.operation!=null && !(td.operation.isDone) );
        
        EditorGUILayout.PropertyField(jsonProp, jsonLabel);
        EditorGUILayout.PropertyField(resultProp, resultLabel);
    
        EditorGUI.EndProperty();
    }
}
}