using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomPropertyDrawer(typeof(TDRequest))]
public class TDRequestDrawer : PropertyDrawer
{
    GUIStyle labelStyle = new GUIStyle();
    Color labelBG = new Color(0.2f, 0.226f, 0.267f, 1f);
    void OnEnable()
    {
        GUIContent send = new GUIContent(" Send Request", (Texture)Resources.Load("terminal"), "Send SQL Request");
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //Lable style
        // GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 12;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.normal.textColor = Color.white;
        // Color labelBG = new Color(0.2f, 0.226f, 0.267f, 1f);
        
        //References
        TDRequest td = fieldInfo.GetValue(property.serializedObject.targetObject) as TDRequest;
        
        SerializedProperty timeEncodingProp = property.FindPropertyRelative("timeEncoding");       
        SerializedProperty sqlProp = property.FindPropertyRelative("sql");
        SerializedProperty jsonProp = property.FindPropertyRelative("json");
        SerializedProperty resultProp = property.FindPropertyRelative("result");

        //Field styles
        GUIContent sqlLabel = new GUIContent("SQL Statement:");
        GUIContent jsonLabel = new GUIContent("Returned JSON:");
        GUIContent resultLabel = new GUIContent("Parsed Result:");

        //Button styles
        GUIContent send = new GUIContent(" Send Request", (Texture)Resources.Load("terminal"), "Send SQL Request");
        GUIContent clearRequest = new GUIContent( (Texture)Resources.Load("clear"), "Clear Request");

        //Draw
        EditorGUI.DrawRect(new Rect(position.x-20, position.y+16, position.width+40, 28), labelBG);
        EditorGUILayout.LabelField("TD Request", labelStyle);
        EditorGUILayout.PropertyField(timeEncodingProp);
        EditorGUILayout.PropertyField(sqlProp, sqlLabel);
        
        GUILayout.BeginHorizontal();  
        if (GUILayout.Button(send, GUILayout.Height(32))) td.Push();           
        if ( GUILayout.Button(clearRequest, GUILayout.Width(32), GUILayout.Height(32))) td.Clear();
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(jsonProp, jsonLabel);
        EditorGUILayout.PropertyField(resultProp, resultLabel);

        EditorGUI.EndProperty();
    }
}
}