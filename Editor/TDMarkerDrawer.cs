using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
namespace Sean21.TDengineConnector{
[CustomPropertyDrawer(typeof(TDField))]
public class FieldMarkerDrawer: DecoratorDrawer {
    TDField tDField { get{ return attribute as TDField; } }
    GUIContent label = new GUIContent((Texture)Resources.Load("field_mark"), "TDengine Field");
    public override void OnGUI(Rect position) {
        Rect lengthPos = new Rect(position.x + 26, position.y + 4, 40, 10);
        EditorGUI.LabelField(position, label); 
        if (tDField.length > 0) EditorGUI.LabelField(lengthPos, tDField.length.ToString(), Styles.MarkLengthStyle);
    }
}
[CustomPropertyDrawer(typeof(TDTag))]
public class TagMarkerDrawer: DecoratorDrawer {
    TDTag tDTag { get{ return attribute as TDTag; } }
    GUIContent label = new GUIContent((Texture)Resources.Load("tag_mark"), "TDengine Tag");
    public override void OnGUI(Rect position) {
        Rect lengthPos = new Rect(position.x + 26, position.y + 4, 40, 10);
        EditorGUI.LabelField(position, label); 
        if (tDTag.length > 0) EditorGUI.LabelField(lengthPos, tDTag.length.ToString(), Styles.MarkLengthStyle);
    }
}
}
