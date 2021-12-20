using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector{
public static class Styles
{
    public static class Palette
    {
        public static Color32 Major1 = new Color32( 42, 131, 130, 255 );
    }
    public static GUIStyle MarkLengthStyle{ 
        get{ 
            GUIStyle markLengthStyle = new GUIStyle(EditorStyles.boldLabel);
            markLengthStyle.normal.textColor = Palette.Major1;
            markLengthStyle.fontSize = 10;
            return markLengthStyle;
        }
    }
}
}