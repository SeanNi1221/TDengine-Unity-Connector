using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
namespace Sean21.TDengineConnector
{
public partial class TDBridge
{   
    public static T FromTD<T>(TDResult result, int row = 0) {
        return (T)FromTD(result, typeof(T), row);
    }

    public static object FromTD(TDResult result, Type type, int row = 0) {
        UnityEngine.Object obj = (UnityEngine.Object)Activator.CreateInstance(type);
        return FromTD(ref obj, result, type, row);
    }
    public static T FromTD<T>(ref T obj, TDResult result, int row = 0) {
        UnityEngine.Object _obj = obj as UnityEngine.Object;
        return (T)FromTD(ref _obj, result, typeof(T), row);
    }
    public static object FromTD(ref UnityEngine.Object obj, TDResult result, int row = 0) {
        return FromTD(ref obj, result, obj.GetType(), row);
    }
    private static object FromTD(ref UnityEngine.Object obj, TDResult result, Type type, int row = 0) {
        Debug.Log("Deserializing type: " + type.Name);
        for (int i= 0;i < result.column_meta.Count; i++) {
            TDResult.ColumnMeta col = result.column_meta[i];
            if (col.typeIndex == 9) {
                if(TDBridge.i.detailedDebugLog) Debug.Log("TIMESTAMP:" + result.data[row].value[i]);
            }
            FieldInfo field = type.GetField(col.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (field == null) {
                if(TDBridge.i.detailedDebugLog) Debug.Log(SQL.Quote(col.name) + " doesn't exist in target object.");
                continue;
            }
            if(TDBridge.i.detailedDebugLog) Debug.Log("Got " + SQL.Quote(field.Name));
            object fieldValue = field.GetValue(obj);
            field.SetValue( obj, deserializeValue(result.data[row].value[i], fieldValue));
        }
        Debug.Log("Deserializeing done.");
        return obj;        
    }
    public static Func<string, object, object> deserializeValue = (data, fieldValue) => {
        Type fieldType = fieldValue.GetType();
        switch ( varType.IndexOf(fieldType))
        {            
            default: {
                Debug.LogError("Unsupported type: " + fieldType.Name + ". Deserialization failed!");
                return data;
            }
            case 1: return deserializeBool(data);
            case 2: return deserializeByte(data);
            case 3: return deserializeInt16(data);
            case 4: return deserializeInt32(data);
            case 5: return deserializeInt64(data);
            case 6: return deserializeSingle(data);
            case 7: return deserializeDouble(data);
            case 8: return deserializeBin(data);
            case 9: return deserializeDateTime(data);
            case 10: return data;
            case 11: return deserializeVector2(data);
            case 12: return deserializeVector3(data);
            case 13: return deserializeQuaternion(data);
            case 14: return deserializeTransform(data, fieldValue as Transform);
        }
    };
    public static Func<string, bool> deserializeBool = data => data == "1" || data =="true";
    public static Func<string, Byte> deserializeByte = data => Byte.Parse(data);
    public static Func<string, Int16> deserializeInt16 = data => Int16.Parse(data);
    public static Func<string, int> deserializeInt = data => int.Parse(data);
    public static Func<string, Int32> deserializeInt32 = data => Int32.Parse(data);
    public static Func<string, Int64> deserializeInt64 = data => Int64.Parse(data);
    public static Func<string, float> deserializeFloat = data => float.Parse(data);
    public static Func<string, Single> deserializeSingle = data => Single.Parse(data);
    public static Func<string, Double> deserializeDouble = data => Double.Parse(data);
    public static Func<string, bin> deserializeBin = data => new bin(data);
    public static Func<string, DateTime> deserializeDateTime = data => DateTime.Parse(data);
    public static Func<string, Vector2> deserializeVector2= data => string.IsNullOrEmpty(data)||data=="null"? Vector2.zero : ParseVector2(data);
    public static Func<string, Vector3> deserializeVector3= data => string.IsNullOrEmpty(data)||data=="null"? Vector3.zero : ParseVector3(data);
    public static Func<string, Quaternion> deserializeQuaternion= data => string.IsNullOrEmpty(data)||data=="null"? Quaternion.identity : ParseQuaternion(data);
    public static Func<string, Transform, Transform> deserializeTransform= (data, tr) => string.IsNullOrEmpty(data)||data=="null"? tr : ParseTransform(data, tr);
}
}
