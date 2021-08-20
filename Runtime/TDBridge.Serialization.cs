using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean.Bridge
{
public partial class TDBridge
{
    public static List<System.Type> varType = new List<System.Type>{ typeof(System.String), typeof(System.Boolean), typeof(System.Byte), typeof(System.Int16), typeof(System.Int32), typeof(System.Int64), typeof(System.Single), typeof(System.Double), typeof(Bin), typeof(System.DateTime),typeof(System.String) };
    public static List<string> dataType = new List<string>{ "nchar", "bool", "tinyint", "smallint", "int", "bigint", "float", "double", "binary", "timestamp", "nchar" };
    public static void CreateSTable<T>(string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string sql = SQL.CreateSTable<T>(db_name, stb_name, timestamp_field_name, if_not_exists);
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
    public static IEnumerator AlterTableOf<T>(string db_name = null, string tb_name = null) {
        //Prepare SQL snippets
        db_name = SQL.SetDatabaseName(db_name);
        tb_name = SQL.SetTableNameWith<T>(tb_name);
        string action = "ALTER TABLE ";
        string add = " ADD COLUMN ";
        string drop = " DROP COLUMN ";
        string resize = " MODIFY COLUMN ";
        
        //Prepare table structures
        Result inDB = null;
        yield return i.StartCoroutine(
            Request("SELECT * FROM " + Quote(db_name) + Dot + Quote(tb_name) + " LIMIT 1", inDB)
        );
        List<ColumnMeta> currentMeta = inDB.columnMeta;
        List<ColumnMeta> newMeta = FieldMetasOf<T>();
        List<Coroutine> resizing = new List<Coroutine>();
        List<Coroutine> dropping = new List<Coroutine>();
        List<Coroutine> adding = new List<Coroutine>();
        
        //Dorp deprecated columns and resize exsisting if needed. Ignore the timestamp column.
        foreach(ColumnMeta col in currentMeta) {
            bool shouldDrop = true;
            if(col.typeIndex != dataType.IndexOf("timestamp")) {
                foreach(ColumnMeta newCol in newMeta) {
                    if (col.attribute.Equals(newCol.attribute.ToLower()) && col.typeIndex == newCol.typeIndex) {
                        shouldDrop = false;
                        int type = col.typeIndex;
                        if (type == dataType.IndexOf("nchar") || type == dataType.IndexOf("binary") ) {
                            if (col.length != newCol.length) {
                                resizing.Add(i.StartCoroutine(
                                    i.Request(action + Quote(db_name) + Dot + Quote(tb_name) + resize + Quote(col.attribute) +
                                        Space + dataType[type] + Bracket(newCol.length.ToString())
                                    )
                                ));
                            }
                        }
                        break;
                    }
                }
            }
            else { shouldDrop = false; }
            if (shouldDrop) {
                dropping.Add(
                    i.StartCoroutine(
                    i.Request(action + Quote(db_name) + Dot + Quote(tb_name) + drop + Quote(col.attribute))
                ));
            }
        }
        yield return resizing;
        yield return dropping;
        
        //Add new columns from the class.
        foreach(ColumnMeta newCol in newMeta) {
            bool shoudAdd = true;
            foreach(ColumnMeta col in currentMeta) {
                if (col.attribute.Equals(newCol.attribute.ToLower())) {
                    shoudAdd = false;
                    break;
                }
            }
            if(shoudAdd) {
                string lengthDeclaration = newCol.isResizable? Bracket(newCol.length):string.Empty;
                adding.Add(
                    i.StartCoroutine(
                        i.Request(action + Quote(db_name + Dot + Quote(tb_name) + add + Quote(newCol.attribute) +
                            Space + dataType[newCol.typeIndex] + lengthDeclaration
                        )
                )));
            }
        }
        yield return adding;
    }
    public static partial class SQL
    {
        public static string CreateSTable<T>(string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
            string action = if_not_exists? "CREATE STABLE IF NOT EXISTS ":"CREATE STABLE ";
            db_name = SetDatabaseName(db_name);
            stb_name = SetSTableNameWith<T>(stb_name);
            string fieldTypes = FieldTypes<T>(timestamp_field_name);
            string tagTypes = TagTypes<T>();
            string sql = action + Quote(db_name) + Dot + Quote(stb_name) + fieldTypes + tagTypes;
            Debug.Log("SQL- CREATE STABLE from type " + typeof(T).Name + ":" + "\n" + sql);
            return sql;
        }
        public static string CreateTable<T>(string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
            string action = if_not_exists? "CREATE TABLE IF NOT EXISTS ":"CREATE TABLE ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith<T>(tb_name);
            string fieldTypes = FieldTypes<T>(timestamp_field_name);
            string sql = action + Quote(db_name) + Dot + Quote(tb_name) + fieldTypes;
            Debug.Log("SQL- CREATE TABLE from type " + typeof(T).Name + ":" + "\n" + sql);
            return sql;
        }
        public static string CreateTable(UnityEngine.Object obj, string db_name = null, string tb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
            string action = if_not_exists? "CREATE TABLE IF NOT EXISTS ":"CREATE TABLE ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj, tb_name);
            string fieldTypes = FieldTypes(obj, timestamp_field_name);
            string sql = action + Quote(db_name) + Dot + Quote(tb_name) + fieldTypes;
            Debug.Log("SQL- CREATE TABLE from object " + obj.name + ":" + "\n" + sql);
            return sql;
        }
        public static string CreateTableUsing(UnityEngine.Object obj, string db_name = null, string tb_name = null, string stb_name = null, bool if_not_exists = true) {
            string action = if_not_exists? "CREATE TABLE IF NOT EXISTS ":"CREATE TABLE ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj, tb_name);
            stb_name = SetSTableNameWith(obj, stb_name);
            string tagValues = TagValues(obj);
            string sql = action + Quote(db_name) + Dot + Quote(tb_name) + " USING " + Quote(db_name) + Dot + Quote(stb_name) + tagValues;
            Debug.Log("SQL- CREATE TABLE from object " + obj.name + " using " + stb_name + ":" + "\n" + sql);
            return sql;            
        }
        public static string CreateTableUsing(UnityEngine.Object[] objects, string db_name = null, string[] tb_names = null, string stb_name = null, bool if_not_exists = true) {
            string action = if_not_exists? " IF NOT EXISTS " : " ";            
            db_name = SetDatabaseName(db_name);
            string tables = null;
            if (tb_names == null) {
                tb_names = new string[objects.Length];
            }
            stb_name = SetSTableNameWith(objects[0]);
            foreach (UnityEngine.Object obj in objects) {
                int index = System.Array.IndexOf(objects, obj);
                tb_names[index] = SetTableNameWith(obj, tb_names[index]);
                string currentTagValues = TagValues(obj);
                string currentTable = action + Quote(db_name) + Dot + Quote(tb_names[index]) + " USING " + Quote(db_name) + Dot + Quote(stb_name) + currentTagValues + Space;
                tables += currentTable;
            }
            string sql = "CREATE TABLE " + tables;
            Debug.Log("SQL- CREATE TABLE from " + objects.Length + " objects using"  + stb_name + ":" + "\n" + sql );
            return sql;
        }
        public static string FieldTypes<T>(string timestamp_field_name = "ts") {
            List<string> fieldTypes = new List<string>{ Quote(timestamp_field_name) + " TIMESTAMP" };
            foreach (var field in typeof(T).GetFields()) {            
                DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
                if ( df != null) {
                    int typeIndex = varType.IndexOf(field.FieldType);
                    string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
                    if (typeIndex == dataType.IndexOf("nchar") || typeIndex == dataType.IndexOf("binary")) {
                        typeOfThis += Bracket( df.length.ToString() );
                    }
                    fieldTypes.Add(typeOfThis);
                }
            }
            return " (" + string.Join("," , fieldTypes) + ") ";
        }
        public static string FieldTypes(UnityEngine.Object obj, string timestamp_field_name = "ts") {
            List<string> fieldTypes = new List<string>{ "'" + timestamp_field_name + "'" + " TIMESTAMP" };
            foreach (var field in obj.GetType().GetFields()) {            
                DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
                if ( df != null) {
                    int typeIndex = varType.IndexOf(field.FieldType);
                    string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
                    if (typeIndex == dataType.IndexOf("nchar") || typeIndex == dataType.IndexOf("binary")) {
                        typeOfThis += Bracket( df.length.ToString() );
                    }
                    fieldTypes.Add(typeOfThis);
                }
            }
            return " (" + string.Join("," , fieldTypes) + ") ";
        }
        public static string TagTypes<T>() {
            List<string> tagTypes = new List<string>();
            foreach (var field in typeof(T).GetFields()) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if ( dt != null ) {
                    int typeIndex = varType.IndexOf(field.FieldType);
                    string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
                    if (typeIndex == dataType.IndexOf("nchar") || typeIndex == dataType.IndexOf("binary")) {
                        typeOfThis += Bracket( dt.length.ToString() );
                    }
                    tagTypes.Add(typeOfThis);
                }            
            }            
            return " TAGS" + Bracket( string.Join("," , tagTypes) );
        }
        public static string TagTypes(UnityEngine.Object obj) {
            List<string> tagTypes = new List<string>();
            foreach (var field in obj.GetType().GetFields()) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if ( dt != null ) {
                    int typeIndex = varType.IndexOf(field.FieldType);
                    string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
                    if (typeIndex == dataType.IndexOf("nchar") || typeIndex == dataType.IndexOf("binary")) {
                        typeOfThis += Bracket( dt.length.ToString() );
                    }
                    tagTypes.Add(typeOfThis);
                }            
            }            
            return " TAGS" + Bracket( string.Join("," , tagTypes) );
        }
        public static string TagValues(UnityEngine.Object obj) {
            List<string> tagValues = new List<string>();
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if (dt != null) {
                    string value = field.GetValue(obj).ToString();
                    int typeIndex = varType.IndexOf(field.FieldType);
                    if (typeIndex == dataType.IndexOf("nchar") || typeIndex == dataType.IndexOf("binary")) {
                        value = Quote(value);
                    }                                        
                    tagValues.Add(value);
                }
            }
            return " TAGS" + Bracket( string.Join(", " , tagValues) );
        }
        public static string SetTableNameWith<T>(string tb_name = null) {
            if (string.IsNullOrEmpty(tb_name)) {
                tb_name = typeof(T).Name + "_instance_" + TimeStamp14;
            }
            return tb_name;
        }
        public static string SetTableNameWith(UnityEngine.Object obj, string tb_name = null) {
            if (string.IsNullOrEmpty(tb_name)) {
                if (obj is Component) {
                    GameObject go = (obj as Component).gameObject;
                    if (go)
                    {
                        tb_name = String.Concat(Array.FindAll(go.name.ToCharArray(), Char.IsLetterOrDigit));
                    }
                    else { tb_name = obj.name + "_instance_" + TimeStamp14; }
                }
                else {
                    tb_name = obj.name + "_instance_" + TimeStamp14;
                }
            }
            return tb_name;
        }
        public static string SetSTableNameWith<T>(string stb_name = null) {
            if (string.IsNullOrEmpty(stb_name)) {
                stb_name = typeof(T).Name;
            }
            return stb_name;
        }
        public static string SetSTableNameWith(UnityEngine.Object obj, string stb_name = null) {
            if (string.IsNullOrEmpty(stb_name)) {
                stb_name = obj.GetType().Name;
            }
            return stb_name;
        }
        public static string SetDatabaseName(string db_name = null) {
            if (string.IsNullOrEmpty(db_name)) {
                db_name = TDBridge.i.defaultDatabaseName;
            }
            return db_name;
        }
    }
    public static List<ColumnMeta> FieldMetasOf<T>() {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in typeof(T).GetFields()) {            
            DataField data = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( data != null) {
                ColumnMeta current = new ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<ColumnMeta> FieldMetasOf(UnityEngine.Object obj) {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
            DataField data = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( data != null) {
                ColumnMeta current = new ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<ColumnMeta> TagMetasOf<T>() {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in typeof(T).GetFields()) {            
            DataTag data = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
            if ( data != null) {
                ColumnMeta current = new ColumnMeta(field.Name, varType.IndexOf(field.FieldType), data.length);
                list.Add(current);
            }
        }
        return list;
    }
    public static List<ColumnMeta> TagMetasOf(UnityEngine.Object obj) {
        List<ColumnMeta> list = new List<ColumnMeta>();
        foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {            
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

