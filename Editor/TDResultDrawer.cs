using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace  Sean21.TDengineConnector
{
[CustomPropertyDrawer(typeof(TDResult))]
public class TDResultDrawer : PropertyDrawer
{
    SerializedProperty statusProp;
    SerializedProperty columnMetaProp;
    SerializedProperty dataProp;
    SerializedProperty rowsProp;
    GUIStyle titleStyle = new GUIStyle();    
    GUIStyle headerStyle = new GUIStyle();
    protected static bool showColumnMeta = true;
    protected static bool showData = true;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        titleStyle.fontSize = 10;
        titleStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontStyle = FontStyle.Bold;
        EditorGUI.BeginProperty(position, label, property);
        
        //References
        statusProp = property.FindPropertyRelative("status");
        columnMetaProp = property.FindPropertyRelative("column_meta");
        dataProp = property.FindPropertyRelative("data");
        rowsProp = property.FindPropertyRelative("rows");
        //Draw
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //status
        EditorGUILayout.PropertyField(statusProp);
        //rows
        EditorGUILayout.PropertyField(rowsProp);
        EditorGUILayout.Space(10);
        //comlum_meta
        showColumnMeta = EditorGUILayout.Foldout(showColumnMeta, "Column Meta", true,EditorStyles.foldoutHeader);
        if (showColumnMeta) {
            EditorGUI.indentLevel = 0;
            //Title
            Rect rect = GUILayoutUtility.GetLastRect();
            Rect columnRect = new Rect(rect.x + 10, rect.y + 20, 40, 16);
            Rect nameRect = new Rect(rect.x + 60, rect.y + 20, 64, 16);
            Rect typeRect = new Rect(rect.x + 64 + (position.width-64) * 0.62f, rect.y + 20, 64, 16);
            Rect lengthRect = new Rect(rect.x + 64 + (position.width-64) -36, rect.y + 20, 48, 16);
            EditorGUI.LabelField(columnRect, "Column", titleStyle);
            EditorGUI.LabelField(nameRect, "name", titleStyle);
            EditorGUI.LabelField(typeRect, "typeIndex", titleStyle);
            EditorGUI.LabelField(lengthRect, "length", titleStyle);
            GUILayout.Space(20);            
            //Content
            for (int i=0; i<columnMetaProp.arraySize; i++)
            {
                var column = columnMetaProp.GetArrayElementAtIndex(i);
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(40));
                EditorGUILayout.PropertyField(column);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel = 0;
        }
        EditorGUILayout.Space(10);
        //data
        showData = EditorGUILayout.Foldout(showData, "Data", true, EditorStyles.foldoutHeader);
        if (showData) {
            EditorGUI.indentLevel = 0;
            for (int i=0; i<dataProp.arraySize; i++)
            {
                var rowProp = dataProp.GetArrayElementAtIndex(i);
                var valuesProp = rowProp.FindPropertyRelative("value");
                EditorGUI.indentLevel = 1;
                valuesProp.isExpanded = EditorGUILayout.Foldout(valuesProp.isExpanded, "data " + i, true);
                if (valuesProp.isExpanded) {
                    //Title
                    Rect rect = GUILayoutUtility.GetLastRect();
                    Rect columnRect = new Rect(rect.x, rect.y + 20, 64, 16);
                    Rect keyRect = new Rect(rect.x + 50, rect.y + 20, 64, 16);
                    Rect valueRect = new Rect(rect.x + 170, rect.y + 20, 64, 16);
                    EditorGUI.LabelField(columnRect, "Column", titleStyle);
                    EditorGUI.LabelField(keyRect, "Key", titleStyle);
                    EditorGUI.LabelField(valueRect, "value", titleStyle);
                    GUILayout.Space(20);            
                    //Content
                    for (int j=0; j<valuesProp.arraySize; j++)
                    {
                        string name = columnMetaProp.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue;
                        var valueProp = valuesProp.GetArrayElementAtIndex(j);
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(j.ToString(), GUILayout.Width(40));
                        EditorGUILayout.LabelField(name,GUILayout.Width(120));
                        EditorGUILayout.PropertyField(valueProp,GUIContent.none, GUILayout.ExpandWidth(true));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUI.indentLevel = 0;
        }
        EditorGUI.EndProperty();

    }
}
[CustomPropertyDrawer(typeof(TDResult.ColumnMeta))]
public class ColumnMetaDrawer : PropertyDrawer
{
    public SerializedProperty nameProp;
    SerializedProperty typeIndexProp;
    SerializedProperty lengthProp;
    GUIStyle minorStyle = new GUIStyle();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        minorStyle.fontSize = 10;
        minorStyle.alignment = TextAnchor.MiddleLeft;
        EditorGUI.BeginProperty(position, label, property);

        nameProp = property.FindPropertyRelative("name");
        typeIndexProp = property.FindPropertyRelative("typeIndex");
        string typeString = TDBridge.dataType[typeIndexProp.intValue].ToUpper();
        lengthProp = property.FindPropertyRelative("length");
 

        var nameRect = new Rect(position.x , position.y, position.width * 0.6f, position.height);
        var typeIndexRect = new Rect(position.x + position.width * 0.61f, position.y, 40, position.height);
        var typeStringRect = new Rect(position.x + position.width * 0.7f, position.y, 64, position.height);
        var lengthRect = new Rect(position.x + position.width-50, position.y, 50, position.height);
        
        EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
        EditorGUI.PropertyField(typeIndexRect, typeIndexProp, GUIContent.none);
        EditorGUI.LabelField(typeStringRect, typeString, minorStyle);
        EditorGUI.PropertyField(lengthRect, lengthProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}

}