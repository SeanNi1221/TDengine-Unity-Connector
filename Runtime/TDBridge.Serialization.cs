using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Sean21
{
public partial class TDBridge
{
    public static List<System.Type> varType = new List<System.Type>{ typeof(System.Object), typeof(System.Boolean), typeof(System.Byte), typeof(System.Int16), typeof(System.Int32), typeof(System.Int64), typeof(System.Single), typeof(System.Double), typeof(bin), typeof(System.DateTime),typeof(System.String) };
    public static List<string> dataType = new List<string>{ "unkown", "bool", "tinyint", "smallint", "int", "bigint", "float", "double", "binary", "timestamp", "nchar" };
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
    public static IEnumerator AlterTableOf(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
//Prepare SQL snippets
        db_name = SQL.SetDatabaseName(db_name);
        tb_name = SQL.SetTableNameWith(obj, tb_name);
        string action = "ALTER TABLE ";
        string add = " ADD COLUMN ";
        string drop = " DROP COLUMN ";
        string resize = " MODIFY COLUMN ";
//Aqcuire table structure
        TDRequest request = new TDRequest("SELECT * FROM " + db_name + Dot + tb_name + " LIMIT 1");
        yield return request.Send();
        List<ColumnMeta> currentMeta = request.result.column_meta;
        List<ColumnMeta> newMeta = FieldMetasOf(obj);
        List<Coroutine> resizing = new List<Coroutine>();
        List<Coroutine> dropping = new List<Coroutine>();
        List<Coroutine> adding = new List<Coroutine>();
        List<ColumnMeta> dropped = new List<ColumnMeta>();
        List<ColumnMeta> added = new List<ColumnMeta>();    
//Dorp deprecated and resize those with length changed.
        foreach(ColumnMeta col in currentMeta) {
            bool shouldDrop = true;
    //ignore the primary key (timestamp) column.
            if(currentMeta.IndexOf(col) > 0) {
                foreach(ColumnMeta newCol in newMeta) {
                    if (col.attribute.Equals(newCol.attribute.ToLower()) && col.typeIndex == newCol.typeIndex) {
                        shouldDrop = false;
                        int type = col.typeIndex;
    //For binary and nchar
                        if (type == 8 || type == 10 ) {
                            if (col.length < newCol.length) {
                                resizing.Add(i.StartCoroutine(
                                    Push(action + Quote(db_name) + Dot + Quote(tb_name) + resize + col.attribute +
                                        Space + dataType[type] + Bracket(newCol.length.ToString())
                                    )
                                ));
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
                    Push(action + db_name + Dot + tb_name + drop + col.attribute)
                ));
                dropped.Add(col);
                Debug.LogWarning("Culomn " + col.attribute + " has been dropped, with all the data inside lost!");
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
                if (col.attribute.Equals(newCol.attribute.ToLower())) {
                    shouldAdd = false;
                    break;
                }
            }
            if(shouldAdd) {
                string lengthDeclaration = newCol.isResizable? Bracket(newCol.length):string.Empty;
                adding.Add(
                    i.StartCoroutine(
                        Push(action + db_name + Dot + tb_name + add + newCol.attribute +
                            Space + dataType[newCol.typeIndex] + lengthDeclaration
                            )
                ));
                added.Add(newCol);
            }
        }
        foreach (Coroutine c in adding) { yield return c; }
//Conclude
        Debug.Log("Altering table Finished with " + dropping.Count + " columns dropped, " + resizing.Count + " columns resized, " + adding.Count + " columns added.");
        foreach (ColumnMeta col_dropped in dropped) {
            foreach (ColumnMeta col_added in added) {
                if (col_added.attribute == col_dropped.attribute) {
                    Debug.LogWarning("Some column(s) were added after dropped. That changed the sequence of the columns. The Insert(Object) method no longer works now!" +
                    "\n Solution A: Change the class' fields order according to the table in the database." + 
                    "\n Solution B: Instead of Insert(Object), Use the InsertSpecific(Object) method in the future, at the cost of performance.");
                }
            }
        }
    }
    public static void Insert(UnityEngine.Object obj, string db_name = null, string tb_name = null, string time = "NOW") {
        string sql = SQL.Insert(obj, db_name, tb_name, time);
        PushSQL(sql);
    }
    public static void InsertSpecific(UnityEngine.Object obj, string db_name = null, string tb_name = null,string timestamp_field_name = "ts", string time = "NOW") {
        string sql = SQL.InsertSpecific(obj, db_name, tb_name, timestamp_field_name, time);
        PushSQL(sql);
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
        public static string Insert(UnityEngine.Object obj, string db_name = null, string tb_name = null, string time = "NOW") {
            string action = "INSERT INTO ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj,tb_name);
            return action + Quote(db_name) + Dot + Quote(tb_name) + FieldValues(obj, time);
        }
        //INSERT INTO d1001 (ts, current, phase) VALUES ('2021-07-13 14:06:33.196', 10.27, 0.31)
        public static string InsertSpecific(UnityEngine.Object obj, string db_name = null, string tb_name = null, string timestamp_field_name = "ts", string time = "NOW") {
            string action = "INSERT INTO ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj,tb_name);
            return action + db_name + Dot + tb_name + Space + FieldNames(obj, timestamp_field_name) + Space + FieldValues(obj, time);
        }
        
        public static string FieldNames(UnityEngine.Object obj, string timestamp_field_name = "ts") {
            return FieldNames( obj.GetType(), timestamp_field_name);
        }
        public static string FieldNames<T>(string timestamp_field_name = "ts") {
            return FieldNames( typeof(T), timestamp_field_name);
        }
        //(ts, current, phase)
        public static string FieldNames( System.Type type, string timestamp_field_name = "ts") {
            List<string> fieldNames = new List<string>{ Quote(timestamp_field_name) };
            foreach (var field in type.GetFields()) {
                DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
                if ( df != null) {
                    fieldNames.Add(field.Name);
                }
            }
            return Bracket( string.Join(", ", fieldNames) );
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
            return Bracket( string.Join(", ", fieldTypes) );
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
            return TagTypes(typeof(T));
        }
        public static string TagTypes(UnityEngine.Object obj) {
            return TagTypes(obj.GetType());
        }
//TAGS (tag1_name tag_type1, tag2_name tag_type2 [, tag3_name tag_type3])
        public static string TagTypes(System.Type type) {
            List<string> tagTypes = new List<string>();
            foreach (var field in type.GetFields()) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if ( dt != null ) {
                    int typeIndex = varType.IndexOf(field.FieldType);
                    string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
                    if (typeIndex == 8 || typeIndex == 10) {
                        typeOfThis += Bracket( dt.length.ToString() );
                    }
                    tagTypes.Add(typeOfThis);
                }            
            }            
            return " TAGS" + Bracket( string.Join("," , tagTypes) );
        }
        static Func<FieldInfo, int, string> serializeType = (field, length) => {
            int typeIndex = varType.IndexOf(field.FieldType);
            string typeOfThis = " '" + field.Name + "' " + dataType[typeIndex];
            switch (typeIndex)
            {
                case 8: case 10: return typeOfThis + Bracket( length.ToString() );
                default: return typeOfThis;
            }
        };
//VALUES (NOW, 10.2, 219, 0.32)
        public static string FieldValues(UnityEngine.Object obj, string timestamp = "NOW") {
            List<string> fieldValues = new List<string>{timestamp};
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
                if (df != null) {
                    string value = serializeValue(obj, field, df.length);                  
                    fieldValues.Add(value);
                }
            }
            return " VALUES" + Bracket( string.Join(", " , fieldValues) );            
        }
//TAGS (tag_value1, ...)
        public static string TagValues(UnityEngine.Object obj) {
            List<string> tagValues = new List<string>();
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if (dt != null) {
                    string value = serializeValue(obj, field, dt.length);                                       
                    tagValues.Add(value);
                }
            }
            return " TAGS" + Bracket( string.Join(", " , tagValues) );
        }
        static Func<UnityEngine.Object, FieldInfo, int, string> serializeValue = (obj, field, textLength) => {
            var fieldValue =  field.GetValue(obj);
            int typeIndex = varType.IndexOf(field.FieldType);
            switch (typeIndex)
            {
                default: return fieldValue.ToString();
                case 9: 
                    var dateTime = (System.DateTime)fieldValue;
                    return Quote( dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") );
                case 8: case 10:
                    string textValue = fieldValue.ToString();
                    if( textValue.Length > textLength) { Debug.LogWarning("Value overlength: " + textValue + ". operation can fail!"); }
                    return Quote(textValue);
            }
        };
//Too expensive to excecute. Deprecated.
        static Func< List<int>, List<string>, List<string> > resortValues = (index, source) => {
            if (index == null) return source;
            List<string> target = new List<string>();
            foreach (int i in index) {
                target.Add(source[i]);
            }
            return target;
        }; 
        public static string SetTableNameWith<T>(string tb_name = null) {
            if (string.IsNullOrEmpty(tb_name)) {
                tb_name = typeof(T).Name + "_instance_" + TimeStamp16;
            }
            return tb_name;
        }
        public static string SetTableNameWith(UnityEngine.Object obj, string tb_name = null) {
            if (string.IsNullOrEmpty(tb_name)) {
                if (obj is Component) {
                    GameObject go = (obj as Component).gameObject;
                    if (go)
                    {
                        tb_name = String.Concat(Array.FindAll(go.name.ToCharArray(), isValidForName));
                    }
                    else { tb_name = obj.name + "_instance_" + TimeStamp16; }
                }
                else {
                    tb_name = obj.name + "_instance_" + TimeStamp16;
                }
            }
            return tb_name;
        }
        public static string SetSTableNameWith<T>(string stb_name = null) {
            return SetSTableNameWith(typeof(T), stb_name);
        }
        public static string SetSTableNameWith(UnityEngine.Object obj, string stb_name = null) {
            return SetSTableNameWith(obj.GetType(), stb_name);
        }
        public static string SetSTableNameWith(System.Type type, string stb_name = null) {
            if (string.IsNullOrEmpty(stb_name)) {
                stb_name = type.Name;
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

