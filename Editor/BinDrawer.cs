using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomPropertyDrawer(typeof(bin))]
public class BinDrawer : PropertyDrawer
{
    SerializedProperty stringProp;
    GUIContent binLabel;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent lable)
    {
       
        EditorGUI.BeginProperty(position, lable, property);
        stringProp = property.FindPropertyRelative("String");
        binLabel = new GUIContent(fieldInfo.Name + " (bin)", null, "Unsupported characters will be ignored.");
        EditorGUI.PropertyField(position, stringProp, binLabel);  
        EditorGUI.EndProperty();
    }
}
}