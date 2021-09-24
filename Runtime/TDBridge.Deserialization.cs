using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
namespace Sean21.BridgeToTDengine
{
public partial class TDBridge
{   
    public static T FromTD<T>(Result result, int row = 0) {
        return (T)FromTD(result, typeof(T), row);
    }

    public static object FromTD(Result result, Type type, int row = 0) {
        UnityEngine.Object obj = (UnityEngine.Object)Activator.CreateInstance(type);
        return FromTD(ref obj, result, type, row);
    }
    public static T FromTD<T>(ref T obj, Result result, int row = 0) {
        UnityEngine.Object _obj = obj as UnityEngine.Object;
        return (T)FromTD(ref _obj, result, typeof(T), row);
    }
    public static object FromTD(ref UnityEngine.Object obj, Result result, int row = 0) {
        return FromTD(ref obj, result, obj.GetType(), row);
    }
    private static object FromTD(ref UnityEngine.Object obj, Result result, Type type, int row = 0) {
        Debug.Log("Deserializing type: " + type.Name);
        for (int i= 0;i < result.column_meta.Count; i++) {
            ColumnMeta col = result.column_meta[i];
            if (col.typeIndex == 9) {
                Debug.Log("TIMESTAMP:" + result.data[row].value[i]);
            }
            FieldInfo field = type.GetField(col.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (field == null) {
                Debug.Log(SQL.Quote(col.name) + " doesn't exists in target object.");
                continue;
            }
            Debug.Log("Got " + SQL.Quote(field.Name));
            object fieldValue = field.GetValue(obj);
            field.SetValue( obj, deserializeValue(result.data[row].value[i], fieldValue));
        }
        return obj;        
    }
    static Func<string, object, object> deserializeValue = (data, fieldValue) => {
        Type fieldType = fieldValue.GetType();
        switch ( varType.IndexOf(fieldType))
        {
            default: return data;
            case 1: return data == "1" || data =="true";
            case 2: return Byte.Parse(data);
            case 3: return Int16.Parse(data);
            case 4: return Int32.Parse(data);
            case 5: return Int64.Parse(data);
            case 6: return Single.Parse(data);
            case 7: return Double.Parse(data);
            case 8: return new bin(data);
            case 9: return DateTime.Parse(data);
            case 11: return string.IsNullOrEmpty(data)||data=="null"? Vector2.zero : ParseVector2(data);
            case 12: return string.IsNullOrEmpty(data)||data=="null"? Vector3.zero : ParseVector3(data);
            case 13: return string.IsNullOrEmpty(data)||data=="null"? Quaternion.identity : ParseQuaternion(data);
            case 14: return string.IsNullOrEmpty(data)||data=="null"? fieldValue as Transform : ParseTransform(data, fieldValue as Transform);
        }
    };
    // static Func<string, int, object> deserializeValue = (data, typeIndex) => {
    //     switch ( typeIndex)
    //     {
    //         default: return data;
    //         case 1: return data == "1" || data =="true";
    //         case 2: return Byte.Parse(data);
    //         case 3: return Int16.Parse(data);
    //         case 4: return Int32.Parse(data);
    //         case 5: return Int64.Parse(data);
    //         case 6: return Single.Parse(data);
    //         case 7: return Double.Parse(data);
    //         case 8: return new bin(data);
    //         case 9: return DateTime.Parse(data);

    //     }
    // };


}
}
