using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector{
internal static class Styles
{
    public static class Palette
    {
        public static Color32 Major1 = new Color32( 42, 131, 130, 255 );
        public static Color Minor1 = new Color(0.2f, 0.226f, 0.267f, 1f);
    }
    public static GUIStyle MarkLengthStyle{ 
        get{ 
            GUIStyle markLengthStyle = new GUIStyle(EditorStyles.boldLabel);
            markLengthStyle.normal.textColor = Palette.Major1;
            markLengthStyle.fontSize = 10;
            return markLengthStyle;
        }
    }
    public struct LoadingIndicator
    {
        // public float rollingBarRatio = 0.5f;
        // public float rollingSpeed = 1f;
        private float _width{get; set;}
        private bool _status{ get; set;}
        private float _pointerX{get;set;}
        public void Draw(float position_y, float width, float height = 5f, 
            float rollingBarRatio = 0.5f) 
        {
            if (!_status ) return;
            _width = width;
            Rect rect = new Rect(0, position_y, _width, height);
            EditorGUILayout.Space(height);
            EditorGUI.DrawRect(rect, Palette.Minor1);
            EditorGUI.DrawRect(new Rect(_pointerX, rect.y, _width * rollingBarRatio, height), Palette.Major1);
        }
        public void Update(bool status, float rollingSpeed = 1f) {
            _status = status;
            if (!_status) return;
            _pointerX = _pointerX > _width ?
                0 : _pointerX + rollingSpeed * _width/200;
            // Debug.Log($"pointer:{_pointerX} width:{_width}");
        }
    }
}
}