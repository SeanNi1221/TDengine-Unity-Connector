using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
namespace Sean21.TDengineConnector
{
[Serializable]
public class TDLane
{
    public object target;
    public readonly Dictionary<string, FieldInfo> field;
    public readonly Dictionary<string, FieldInfo> tag;
    protected virtual void Build()
    {
        Clear();
        FieldInfo[] targetFields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        DataTag dataTag;
        DataField dataField;
        foreach (FieldInfo info in targetFields) {
            dataTag = Attribute.GetCustomAttribute(info, typeof(DataTag)) as DataTag;
            if (dataTag != null) { tag.Add(info.Name, info);   continue; }
            else dataField = Attribute.GetCustomAttribute(info, typeof(DataField)) as DataField;
            if (dataField != null) { field.Add(info.Name, info); continue; }
            continue;
        }   
    }
    // protected static readonly Type targetType = target.GetType();
    // protected static readonly Emit<Func<object, string>> GetFieldEmitter = 
    //     GetFieldEmitter<Func<object, string>>.NewDynamicMethod()

    public virtual void Clear()
    {
        field.Clear();
        tag.Clear();
    }
    
}
}