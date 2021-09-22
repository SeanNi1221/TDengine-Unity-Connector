using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
namespace Sean21
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
        //Ignore timestamp primary key.
        for (int i= 1;i < result.column_meta.Count; i++) {
            ColumnMeta col = result.column_meta[i];
            FieldInfo field = type.GetField(col.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            Debug.Log("Got field: " + field.Name);
            field.SetValue( obj, deserializeValue(result.data[row].value[i], col.typeIndex) );
        }
        return obj;        
    }
    static Func<string, int, object> deserializeValue = (data, typeIndex) => {
        Type type = varType[typeIndex];        
        switch ( typeIndex)
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
        }
    };
}
}
