using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean21.TDengineConnector
{
[AttributeUsage(AttributeTargets.Field)]
public class DataField : Attribute
{    
    public int length; 
    public DataField(int l) {
        this.length = l <=0  ? TDBridge.DefaultTextLength : l;
    }
    public DataField() {
        this.length = TDBridge.DefaultTextLength;
    }
}
[AttributeUsage(AttributeTargets.Field)]
public class DataTag : Attribute
{
    public int length; 
    public DataTag(int l) {
        this.length = l <=0  ? TDBridge.DefaultTextLength : l;
    }
    public DataTag() {
        this.length = TDBridge.DefaultTextLength;
    }    
}
public partial class TDBridge
{
    const string Dot = ".";
    const string Space = " ";
    public static void CreateDatabase(string db_name = null, bool if_not_exists = true) {
        SendRequest(SQL.CreateDatabase(db_name, if_not_exists));
    }
    public static void CreateDatabase(TDLane lane) {
        SendRequest(SQL.CreateDatabase(lane));
    }
    public static void CreateSTable<T>(string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateSTable<T>(db_name, stb_name, timestamp_field_name, if_not_exists);
        SendRequest(sql);
    }
    public static void CreateSTable(UnityEngine.Object obj, string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateSTable(obj, db_name, stb_name, timestamp_field_name, if_not_exists);
        SendRequest(sql);
    }
    public static void CreateSTable(TDLane lane) {
        string sql = SQL.CreateSTable(lane);
        SendRequest(sql);
    }
    public static void CreateTable<T>(string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateTable<T>(db_name, tb_name,timestamp_field_name,if_not_exists);
        SendRequest(sql);
    }
    public static void CreateTable(UnityEngine.Object obj, string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateTable(obj, db_name, tb_name, timestamp_field_name, if_not_exists);
        SendRequest(sql);
    }
    public static void CreateTable(TDLane lane) {
        string sql = SQL.CreateTable(lane);
        SendRequest(sql);
    }
    public static void CreateTableUsing(UnityEngine.Object obj, string db_name = null, string tb_name = null, string stb_name = null, bool if_not_exists = true) {
        string sql = SQL.CreateTableUsing(obj, db_name, tb_name, stb_name, if_not_exists);
        SendRequest(sql);
    }
    public static void CreateTableUsing(UnityEngine.Object[] objects, string db_name = null, string[] tb_names = null, string stb_name = null, bool if_not_exists = true) {
        string sql = SQL.CreateTableUsing(objects, db_name, tb_names, stb_name, if_not_exists);
        SendRequest(sql);
    }
    public static void CreateTableUsing(params TDLane[] lanes) {
        string sql = SQL.CreateTableUsing(lanes);
        SendRequest(sql);
    }
    public static void Insert(UnityEngine.Object obj, string db_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.Insert(obj, db_name, tb_name, time);
        SendRequest(sql);
    }
    public static void Insert(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string[] _time = null) {
        string sql = SQL.Insert(objects, db_name,_tb_name, _time);
        SendRequest(sql);
    }
    public static void Insert(params TDLane[] lanes) {
        string sql = SQL.Insert(lanes);
        SendRequest(sql);
    }
    public static void InsertSpecific(UnityEngine.Object obj, string db_name = null, string tb_name = null,string timestamp_field_name = "ts", string time = "NOW") {
        string sql = SQL.InsertSpecific(obj, db_name, tb_name, timestamp_field_name, time);
        SendRequest(sql);
    }
    public static void InsertSpecific(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
        string sql = SQL.InsertSpecific(objects, db_name, _tb_name, timestamp_field_name, _time);
        SendRequest(sql);
    }
    public static void InsertSpecific(params TDLane[] lanes) {
        string sql = SQL.InsertSpecific(lanes);
        SendRequest(sql);
    }
    public static void InsertUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.InsertUsing(obj, db_name, stb_name, tb_name, time);
        SendRequest(sql);
    }
    public static void InsertUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string[] _time = null) {
        string sql = SQL.InsertUsing(objects, db_name, stb_name, _tb_name, _time);
        SendRequest(sql);
    }
    public static void InsertUsing(params TDLane[] lanes) {
        string sql = SQL.InsertUsing(lanes);
        SendRequest(sql);
    }
    public static void InsertSpecificUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.InsertSpecificUsing(obj, db_name, stb_name, tb_name, time);
        SendRequest(sql);
    }
    public static void InsertSpecificUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
        string sql = SQL.InsertSpecificUsing(objects, db_name, stb_name, _tb_name, timestamp_field_name, _time);
        SendRequest(sql);
    }
    public static void InsertSpecificUsing(params TDLane[] lanes) {
        string sql = SQL.InsertSpecificUsing(lanes);
        SendRequest(sql);
    }
    public static void SetTag(UnityEngine.Object obj, string tag_name, string db_name = null, string tb_name = null) {
        string sql = SQL.SetTag(obj, tag_name, db_name, tb_name);
        SendRequest(sql);
    }
    public static void SetTag(TDLane lane, string tag_name) {
        string sql = SQL.SetTag(lane, tag_name);
        lane.SendRequest(sql);
    }
    public static IEnumerator SetTags(UnityEngine.Object obj, string db_name = null, string tb_name = null, params string[] tag_names) {
        List<string> sqls = SQL.SetTags(obj, db_name, tb_name, tag_names);
        foreach (string sql in sqls) {
            yield return Request.Send(sql);
        }
    }
    public static IEnumerator SetTags(TDLane lane, params string[] tag_names) {
        List<string> sqls = SQL.SetTags(lane, tag_names);
        foreach (string sql in sqls) {
            yield return lane.request.Send(sql);
        }
    }
    public static IEnumerator AlterSTableOf<T>(string db_name = null, string stb_name = null) {
        stb_name = SQL.SetSTableNameWith<T>(stb_name);
        yield return AlterSTableOf(typeof(T), db_name, stb_name);
    }
    public static IEnumerator AlterSTableOf(UnityEngine.Object obj, string db_name = null, string stb_name = null) {
        stb_name = SQL.SetSTableNameWith(obj, stb_name);
        yield return AlterSTableOf(obj.GetType(), db_name, stb_name);
    }
    public static IEnumerator AlterSTableOf(System.Type _type, string db_name, string stb_name) {
        db_name = SQL.SetDatabaseName(db_name);
        string action = "ALTER STABLE ";
//Aqcuire table structure
    //fields
        TDRequest request = new TDRequest("SELECT FIRST(*) FROM " + db_name + Dot + stb_name);
        yield return request.Send();
        List<TDResult.ColumnMeta> currentMeta = request.result.column_meta;
        List<TDResult.ColumnMeta> newMeta = FieldMetasOf(_type);
    //tags
        request.sql = "SELECT * FROM " + db_name + Dot + stb_name + " LIMIT 1";
        yield return request.Send();
        request.result.column_meta.RemoveRange(0, currentMeta.Count);
        List<TDResult.ColumnMeta> currentTagsMeta = request.result.column_meta;
        List<TDResult.ColumnMeta> newTagsMeta = TagMetasOf(_type);
//Take action for fields
        yield return AlterColumns(currentMeta, newMeta,db_name, stb_name, action);
        if(TDBridge.i.detailedDebugLog) Debug.Log("Altering ---FIELDS--- of " + stb_name + " finished.");
//Take action for tags
        yield return AlterColumns(currentTagsMeta, newTagsMeta,db_name, stb_name, action, true);
        if(TDBridge.i.detailedDebugLog) Debug.Log("Altering ---TAGS--- of " + stb_name + " finished.");
    }
    public static IEnumerator AlterSTableOf(TDLane lane) {
        string db_name = lane.databaseName;
        string stb_name = lane.superTableName;
        string action = "ALTER STABLE ";
//Aqcuire table structure
    //fields
        yield return lane.request.Send("SELECT FIRST(*) FROM " + db_name + Dot + stb_name);
        List<TDResult.ColumnMeta> currentMeta = lane.request.result.column_meta;
        List<TDResult.ColumnMeta> newMeta = FieldMetasOf(lane);
    //tags
        lane.request.sql = "SELECT * FROM " + db_name + Dot + stb_name + " LIMIT 1";
        yield return lane.request.Send();
        lane.request.result.column_meta.RemoveRange(0, currentMeta.Count);
        List<TDResult.ColumnMeta> currentTagsMeta = lane.request.result.column_meta;
        List<TDResult.ColumnMeta> newTagsMeta = TagMetasOf(lane);
//Take action for fields
        yield return AlterColumns(currentMeta, newMeta,db_name, stb_name, action);
        if(TDBridge.i.detailedDebugLog) Debug.Log("Altering ---FIELDS--- of " + stb_name + " finished.");
//Take action for tags
        yield return AlterColumns(currentTagsMeta, newTagsMeta,db_name, stb_name, action, true);
        if(TDBridge.i.detailedDebugLog) Debug.Log("Altering ---TAGS--- of " + stb_name + " finished.");
    }
    public static IEnumerator AlterTableOf<T>(string db_name = null, string tb_name = null) {
        tb_name = SQL.SetTableNameWith<T>(tb_name);
        yield return AlterTableOf(typeof(T), db_name, tb_name);
    }
    public static IEnumerator AlterTableOf(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
        tb_name = SQL.SetTableNameWith(obj, tb_name);
        yield return AlterTableOf(obj.GetType(), db_name, tb_name);
    }
    public static IEnumerator AlterTableOf(System.Type _type, string db_name, string tb_name) {
        db_name = SQL.SetDatabaseName(db_name);
        string action = "ALTER TABLE ";
//Aqcuire table structure
        TDRequest request = new TDRequest("SELECT FIRST(*) FROM " + db_name + Dot + tb_name);
        yield return request.Send();
        List<TDResult.ColumnMeta> currentMeta = request.result.column_meta;
        List<TDResult.ColumnMeta> newMeta = FieldMetasOf(_type);
//Take action
        yield return AlterColumns(currentMeta, newMeta,db_name, tb_name, action);
        Debug.Log("Altering table " + tb_name + " finished.");
    }
    public static IEnumerator AlterTableOf(TDLane lane) {
        string db_name = lane.databaseName;
        string tb_name = db_name + Dot + lane.tableName;
        string action = "ALTER TABLE ";
//Aqcuire table structure
        yield return lane.request.Send("SELECT FIRST(*) FROM " + tb_name);
        List<TDResult.ColumnMeta> currentMeta = lane.request.result.column_meta;
        List<TDResult.ColumnMeta> newMeta = FieldMetasOf(lane);
//Take action
        yield return AlterColumns(currentMeta, newMeta,db_name, tb_name, action);
        Debug.Log("Altering table " + tb_name + " finished.");
    }

    static IEnumerator AlterColumns(List<TDResult.ColumnMeta> currentMeta, List<TDResult.ColumnMeta> newMeta, string db_name, string tb_name, string action, bool forTags = false) {
//Prepare SQL snippets
        string add = " ADD COLUMN ";
        string drop = " DROP COLUMN ";
        string resize = " MODIFY COLUMN ";
        if (forTags) {
            add = " ADD TAG ";
            drop = " DROP TAG ";
            resize = " MODIFY TAG ";
        }
        List<Coroutine> resizing = new List<Coroutine>();
        List<Coroutine> dropping = new List<Coroutine>();
        List<Coroutine> adding = new List<Coroutine>();
        List<TDResult.ColumnMeta> resized = new List<TDResult.ColumnMeta>();
        List<TDResult.ColumnMeta> dropped = new List<TDResult.ColumnMeta>();
        List<TDResult.ColumnMeta> added = new List<TDResult.ColumnMeta>();    
//Dorp deprecated and resize those with length changed.
        foreach(TDResult.ColumnMeta col in currentMeta) {
            bool shouldDrop = true;
    //ignore the primary key (timestamp) column.
            if(currentMeta.IndexOf(col) > 0) {
                foreach(TDResult.ColumnMeta newCol in newMeta) {
                    if (col.name.Equals(newCol.name.ToLower()) && col.typeIndex == newCol.typeIndex) {
                        shouldDrop = false;
                        int type = col.typeIndex;
    //For binary and nchar
                        if ( SQL.isTextData(type) ) {
                            if (col.length < newCol.length) {
                                resizing.Add(i.StartCoroutine(
                                    i.request.Send(action + SQL.Quote(db_name) + Dot + SQL.Quote(tb_name) + resize + col.name +
                                        Space + dataType[type] + SQL.Bracket(newCol.length.ToString())
                                    )
                                ));
                                resized.Add(col);
                            }
                            else if (col.length > newCol.length) { shouldDrop = true; } 
                        }
                        break;
                    }
                }
            }
            else { shouldDrop = false; }
            if (shouldDrop) {
                dropping.Add(
                    i.StartCoroutine(
                    i.request.Send(action + db_name + Dot + tb_name + drop + col.name)
                ));
                dropped.Add(col);
                Debug.LogWarning("Culomn " + col.name + " has been dropped, with all the data inside lost!");
            }
        }
        foreach (TDResult.ColumnMeta col in dropped ) { yield return currentMeta.Remove(col);}
//Waite for resizing and dropping finished.
        foreach (Coroutine c in resizing) { yield return c; }
        foreach (Coroutine c in dropping) { yield return c; }
//Add new columns from the class.
        foreach(TDResult.ColumnMeta newCol in newMeta) {
            bool shouldAdd = true;
            foreach(TDResult.ColumnMeta col in currentMeta) {
                if (col.name.Equals(newCol.name.ToLower())) {
                    shouldAdd = false;
                    break;
                }
            }
            if(shouldAdd) {
                string lengthDeclaration = newCol.isResizable? SQL.Bracket(newCol.length):string.Empty;
                adding.Add(
                    i.StartCoroutine(
                        i.request.Send(action + db_name + Dot + tb_name + add + newCol.name +
                            Space + dataType[newCol.typeIndex] + lengthDeclaration
                            )
                ));
                added.Add(newCol);
            }
        }
        foreach (Coroutine c in adding) { yield return c; }
//Conclude
        Debug.Log(dropping.Count + " columns dropped, " + resizing.Count + " columns resized, " + adding.Count + " columns added.");
        foreach (TDResult.ColumnMeta col_dropped in dropped) {
            foreach (TDResult.ColumnMeta col_added in added) {
                if (col_added.name == col_dropped.name) {
                    Debug.LogWarning("Some column(s) were added after dropped. That changed the sequence of the columns. The Insert(Object) method no longer works now!" +
                    "\n Solution A: Change the class' fields order according to the table in the database." + 
                    "\n Solution B: Instead of Insert(Object), Use the InsertSpecific(Object) method in the future, at the cost of performance.");
                }
            }
        }
    }
    public static List<TDResult.ColumnMeta> FieldMetasOf<T>() {
        return FieldMetasOf( typeof(T) );
    }
    public static List<TDResult.ColumnMeta> FieldMetasOf(UnityEngine.Object obj) {
        return FieldMetasOf( obj.GetType() );
    }
    public static List<TDResult.ColumnMeta> FieldMetasOf(System.Type type) {
        List<TDResult.ColumnMeta> list = new List<TDResult.ColumnMeta>();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
            DataField data = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( data != null) {
                TDResult.ColumnMeta current = new TDResult.ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<TDResult.ColumnMeta> FieldMetasOf(TDLane lane) {
        List<TDResult.ColumnMeta> list = new List<TDResult.ColumnMeta>();
        foreach (KeyValuePair<string, FieldInfo> pair in lane.fields) {
            string key = pair.Key;
            TDResult.ColumnMeta current = new TDResult.ColumnMeta(key, lane.types[key], lane.lengths[key]);
            list.Add(current);
        }
        return list;        
    }
    public static List<TDResult.ColumnMeta> TagMetasOf<T>() {
        return TagMetasOf( typeof(T) );
    }
    public static List<TDResult.ColumnMeta> TagMetasOf(UnityEngine.Object obj) {
        return TagMetasOf( obj.GetType() );
    }
    public static List<TDResult.ColumnMeta> TagMetasOf( System.Type type) {
        List<TDResult.ColumnMeta> list = new List<TDResult.ColumnMeta>();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
            DataTag data = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
            if ( data != null) {
                TDResult.ColumnMeta current = new TDResult.ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<TDResult.ColumnMeta> TagMetasOf(TDLane lane) {
        List<TDResult.ColumnMeta> list = new List<TDResult.ColumnMeta>();
        foreach (KeyValuePair<string, FieldInfo> pair in lane.tags) {
            string key = pair.Key;
            TDResult.ColumnMeta current = new TDResult.ColumnMeta(key, lane.types[key], lane.lengths[key]);
            list.Add(current);
        }
        return list;        
    }
    public static bool SerializeObject(object obj, out string result) {
        Type t = obj.GetType();
        int i = varType.IndexOf(t);
        if (i<1) { result = string.Empty; return false;}
        switch (i)
        {
            default: result = obj.ToString(); return true;
            case 5: result = serializeInt64((System.Int64)obj); return true;
            case 6: result = serializeSingle((float)obj); return true;
            case 7: result = serializeDouble((System.Double)obj); return true;
            case 8: result = serializeBin((bin)obj, null); return true;
            case 9: result = serializeDateTime((System.DateTime)obj); return true;
            case 10: result = serializeString((string)obj, null); return true;
            case 11: result = serializeVector2((Vector2)obj); return true;
            case 12: result = serializeVector3((Vector3)obj); return true;
            case 13: result = serializeQuaternion((Quaternion)obj); return true;
            case 14: result = serializeTransform(obj as Transform); return true;
        }
    }
    public static Func<UnityEngine.Object, FieldInfo, int, int?, string> serializeValue = (obj, field, typeIndex, textLength) => {
        var fieldValue =  field.GetValue(obj);
        if (fieldValue == null) return "NULL";
        // int typeIndex = TDBridge.varType.IndexOf(field.FieldType);
        switch (typeIndex)
        {
            default: return fieldValue.ToString();
            case 5: return serializeInt64((System.Int64)fieldValue);
            case 6: return serializeSingle((float)fieldValue);
            case 7: return serializeDouble((System.Double)fieldValue);
            case 8: return serializeBin((bin)fieldValue, textLength);
            case 9: return serializeDateTime((System.DateTime)fieldValue);
            case 10: return serializeString((string)fieldValue, textLength);
            case 11: return serializeVector2((Vector2)fieldValue);
            case 12: return serializeVector3((Vector3)fieldValue);
            case 13: return serializeQuaternion((Quaternion)fieldValue);
            case 14: return serializeTransform(fieldValue as Transform);
        }
    };
    public static Func<bool, string> serializeBool = value => value.ToString();
    public static Func<Byte, string> serializeByte = value => value.ToString();
    public static Func<Int16, string> serializeInt16 = value => value.ToString();
    public static Func<int, string> serializeInt = value => value.ToString();
    public static Func<Int32, string> serializeInt32 = value => value.ToString();
    public static Func<Int64, string> serializeInt64 = value => value.ToString("R");
    public static Func<float, string> serializeFloat = value => value.ToString("G9");
    public static Func<Single, string> serializeSingle = value => value.ToString("G9");
    public static Func<Double, string> serializeDouble = value => value.ToString("G17");
    // public static Func<bin, string> serializeBin = value => SQL.Quote(value);
    public static Func<bin, int?, string> serializeBin = (value, textLength) => { 
        if (textLength != null)
            if (value.String.Length>textLength) Debug.LogWarning("Value overlength: " + value + ". operation can fail!"); 
        return SQL.Quote(value);
    };
    public static Func<DateTime, string> serializeDateTime = value => SQL.Quote( value.ToString("yyyy-MM-dd HH:mm:ss.fff") );
    // public static Func<string, string> serializeString = value => SQL.Quote(value);
    public static Func<string, int?, string> serializeString = (value, textLength) => { 
        if (textLength != null)
            if (value.Length>textLength) Debug.LogWarning("Value overlength: " + value + ". operation can fail!"); 
        return SQL.Quote(value);
    };
    public static Func<Vector2, string> serializeVector2 = value => value.ToString("G9");
    public static Func<Vector3, string> serializeVector3 = value => value.ToString("G9");
    public static Func<Quaternion, string> serializeQuaternion = value => value.ToString("G9");
    public static Func<Transform, string> serializeTransform = value => {
        if (!value) return string.Empty;
        return SQL.Quote(value.localPosition.ToString("G9") + "," + value.localEulerAngles.ToString("G9") + "," + value.localScale.ToString("G9"));
    };
}
}

