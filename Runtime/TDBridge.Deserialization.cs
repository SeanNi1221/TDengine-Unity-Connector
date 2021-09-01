using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sean21
{
public partial class TDBridge
{
    public static object FromTD<T>(Result result, int row = 0) {
        T obj;
        //Ignore timestamp primary key.
        for (int i= 0;;) {}
        foreach ( var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ) {
        }
    }
    public static object FromTD(Result result, Type type, int row = 0) {
        // typeof(type) obj;
        // var _obj = obj as typof(type); 
    }
}
}
