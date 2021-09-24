using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean21.BridgeToTDengine
{
public partial class TDBridge
{
    const string Dot = ".";
    const string Space = " ";
    public static void CreateDatabase(string db_name = null, bool if_not_exists = true) {
        PushSQL(SQL.CreateDatabase(db_name, if_not_exists));
    }
    public static void CreateSTable<T>(string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateSTable<T>(db_name, stb_name, timestamp_field_name, if_not_exists);
        PushSQL(sql);
    }
    public static void CreateSTable(UnityEngine.Object obj, string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateSTable(obj, db_name, stb_name, timestamp_field_name, if_not_exists);
        PushSQL(sql);
    }
    public static void CreateTable<T>(string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateTable<T>(db_name, tb_name,timestamp_field_name,if_not_exists);
        PushSQL(sql);
    }
    public static void CreateTable(UnityEngine.Object obj, string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateTable(obj, db_name, tb_name, timestamp_field_name, if_not_exists);
        PushSQL(sql);
    }
    public static void CreateTableUsing(UnityEngine.Object obj, string db_name = null, string tb_name = null, string stb_name = null, bool if_not_exists = true) {
        string sql = SQL.CreateTableUsing(obj, db_name, tb_name, stb_name, if_not_exists);
        PushSQL(sql);
    }
    public static void CreateTableUsing(UnityEngine.Object[] objects, string db_name = null, string[] tb_names = null, string stb_name = null, bool if_not_exists = true) {
        string sql = SQL.CreateTableUsing(objects, db_name, tb_names, stb_name, if_not_exists);
        PushSQL(sql);
    }
    public static void Insert(UnityEngine.Object obj, string db_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.Insert(obj, db_name, tb_name, time);
        PushSQL(sql);
    }
    public static void Insert(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string[] _time = null) {
        string sql = SQL.Insert(objects, db_name,_tb_name, _time);
        PushSQL(sql);
    }
    public static void InsertSpecific(UnityEngine.Object obj, string db_name = null, string tb_name = null,string timestamp_field_name = "ts", string time = "NOW") {
        string sql = SQL.InsertSpecific(obj, db_name, tb_name, timestamp_field_name, time);
        PushSQL(sql);
    }
    public static void InsertSpecific(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
        string sql = SQL.InsertSpecific(objects, db_name, _tb_name, timestamp_field_name, _time);
        PushSQL(sql);
    }
    public static void InsertUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.InsertUsing(obj, db_name, stb_name, tb_name, time);
        PushSQL(sql);
    }
    public static void InsertUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string[] _time = null) {
        string sql = SQL.InsertUsing(objects, db_name, stb_name, _tb_name, _time);
        PushSQL(sql);
    }
    public static void InsertSpecificUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.InsertSpecificUsing(obj, db_name, stb_name, tb_name, time);
        PushSQL(sql);
    }
    public static void InsertSpecificUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
        string sql = SQL.InsertSpecificUsing(objects, db_name, stb_name, _tb_name, timestamp_field_name, _time);
        PushSQL(sql);
    }
    public static void SetTag(UnityEngine.Object obj, string tag_name, string db_name = null, string tb_name = null) {
        string sql = SQL.SetTag(obj, tag_name, db_name, tb_name);
        PushSQL(sql);
    }
    public static IEnumerator SetTags(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
        List<string> sqls = SQL.SetTags(obj, db_name, tb_name);
        foreach (string sql in sqls) {
            PushSQL(sql);
            yield return new WaitForEndOfFrame();
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
        List<ColumnMeta> currentMeta = request.result.column_meta;
        List<ColumnMeta> newMeta = FieldMetasOf(_type);
    //tags
        request.sql = "SELECT * FROM " + db_name + Dot + stb_name + " LIMIT 1";
        yield return request.Send();
        request.result.column_meta.RemoveRange(0, currentMeta.Count);
        List<ColumnMeta> currentTagsMeta = request.result.column_meta;
        List<ColumnMeta> newTagsMeta = TagMetasOf(_type);
//Take action for fields
        yield return AlterColumns(currentMeta, newMeta,db_name, stb_name, action);
        Debug.Log("Altering ---FIELDS--- of " + stb_name + " finished.");
//Take action for tags
        yield return AlterColumns(currentTagsMeta, newTagsMeta,db_name, stb_name, action, true);
        Debug.Log("Altering ---TAGS--- of " + stb_name + " finished.");
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
        List<ColumnMeta> currentMeta = request.result.column_meta;
        List<ColumnMeta> newMeta = FieldMetasOf(_type);
//Take action
        yield return AlterColumns(currentMeta, newMeta,db_name, tb_name, action);
        Debug.Log("Altering table " + tb_name + " finished.");
    }
    static IEnumerator AlterColumns(List<ColumnMeta> currentMeta, List<ColumnMeta> newMeta, string db_name, string tb_name, string action, bool forTags = false) {
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
        List<ColumnMeta> resized = new List<ColumnMeta>();
        List<ColumnMeta> dropped = new List<ColumnMeta>();
        List<ColumnMeta> added = new List<ColumnMeta>();    
//Dorp deprecated and resize those with length changed.
        foreach(ColumnMeta col in currentMeta) {
            bool shouldDrop = true;
    //ignore the primary key (timestamp) column.
            if(currentMeta.IndexOf(col) > 0) {
                foreach(ColumnMeta newCol in newMeta) {
                    if (col.name.Equals(newCol.name.ToLower()) && col.typeIndex == newCol.typeIndex) {
                        shouldDrop = false;
                        int type = col.typeIndex;
    //For binary and nchar
                        if ( SQL.isTextData(type) ) {
                            if (col.length < newCol.length) {
                                resizing.Add(i.StartCoroutine(
                                    Push(action + SQL.Quote(db_name) + Dot + SQL.Quote(tb_name) + resize + col.name +
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
                    Push(action + db_name + Dot + tb_name + drop + col.name)
                ));
                dropped.Add(col);
                Debug.LogWarning("Culomn " + col.name + " has been dropped, with all the data inside lost!");
            }
        }
        foreach (ColumnMeta col in dropped ) { yield return currentMeta.Remove(col);}
//Waite for resizing and dropping finished.
        foreach (Coroutine c in resizing) { yield return c; }
        foreach (Coroutine c in dropping) { yield return c; }
//Add new columns from the class.
        foreach(ColumnMeta newCol in newMeta) {
            bool shouldAdd = true;
            foreach(ColumnMeta col in currentMeta) {
                if (col.name.Equals(newCol.name.ToLower())) {
                    shouldAdd = false;
                    break;
                }
            }
            if(shouldAdd) {
                string lengthDeclaration = newCol.isResizable? SQL.Bracket(newCol.length):string.Empty;
                adding.Add(
                    i.StartCoroutine(
                        Push(action + db_name + Dot + tb_name + add + newCol.name +
                            Space + dataType[newCol.typeIndex] + lengthDeclaration
                            )
                ));
                added.Add(newCol);
            }
        }
        foreach (Coroutine c in adding) { yield return c; }
//Conclude
        Debug.Log(dropping.Count + " columns dropped, " + resizing.Count + " columns resized, " + adding.Count + " columns added.");
        foreach (ColumnMeta col_dropped in dropped) {
            foreach (ColumnMeta col_added in added) {
                if (col_added.name == col_dropped.name) {
                    Debug.LogWarning("Some column(s) were added after dropped. That changed the sequence of the columns. The Insert(Object) method no longer works now!" +
                    "\n Solution A: Change the class' fields order according to the table in the database." + 
                    "\n Solution B: Instead of Insert(Object), Use the InsertSpecific(Object) method in the future, at the cost of performance.");
                }
            }
        }
    }
    public static List<ColumnMeta> FieldMetasOf<T>() {
        return FieldMetasOf( typeof(T) );
    }
    public static List<ColumnMeta> FieldMetasOf(UnityEngine.Object obj) {
        return FieldMetasOf( obj.GetType() );
    }
    public static List<ColumnMeta> FieldMetasOf(System.Type type) {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
            DataField data = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( data != null) {
                ColumnMeta current = new ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<ColumnMeta> TagMetasOf<T>() {
        return TagMetasOf( typeof(T) );
    }
    public static List<ColumnMeta> TagMetasOf(UnityEngine.Object obj) {
        return TagMetasOf( obj.GetType() );
    }
    public static List<ColumnMeta> TagMetasOf( System.Type type) {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
            DataTag data = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
            if ( data != null) {
                ColumnMeta current = new ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
}
[AttributeUsage(AttributeTargets.Field)]
public class DataField : Attribute
{    
    public int length; 
    public DataField(int l) {
        this.length = l <=0  ? TDBridge.i.defaultTextLength : l;
    }
    public DataField() {
        this.length = TDBridge.i.defaultTextLength;
    }
}
[AttributeUsage(AttributeTargets.Field)]
public class DataTag : Attribute
{
    public int length; 
    public DataTag(int l) {
        this.length = l <=0  ? TDBridge.i.defaultTextLength : l;
    }
    public DataTag() {
        this.length = TDBridge.i.defaultTextLength;
    }    
}
}

