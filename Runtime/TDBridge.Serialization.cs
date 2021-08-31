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
        Debug.Log("Altering fields of " + stb_name + " finished.");
//Take action for tags
        yield return AlterColumns(currentTagsMeta, newTagsMeta,db_name, stb_name, action, true);
        Debug.Log("Altering tags of " + stb_name + " finished.");
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
        Debug.Log(dropping.Count + " columns dropped, " + resizing.Count + " columns resized, " + adding.Count + " columns added.");
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
//INSERT INTO d1001 VALUES (NOW, 10.2, 219, 0.32)
        public static string Insert(UnityEngine.Object obj, string db_name = null, string tb_name = null, string time = "NOW") {
            string action = "INSERT INTO ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj,tb_name);
            return action + Quote(db_name) + Dot + Quote(tb_name) + FieldValues(obj, time);
        }
        //INSERT INTO d1001 VALUES ('NOW', 10.2, 219, 0.32) d1002 VALUES ('NOW', 10.2, 219, 0.32) ...
        public static string Insert(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string[] _time = null) {
            var (tb_name, time) = multiTableInit(objects, _tb_name, _time);
            string action = "INSERT INTO ";
            List<string> tables = new List<string>();
            db_name = SetDatabaseName(db_name);
            for (int i=0; i< objects.Length; i++) {
                tb_name[i] = SetTableNameWith(objects[i], tb_name[i]);
                tables.Add( db_name + Dot + tb_name[i] + FieldValues(objects[i], time[i]) );
            }
            return action + string.Join(" ", tables);
        }
        //INSERT INTO d1001 (ts, current, phase) VALUES ('2021-07-13 14:06:33.196', 10.27, 0.31)
        public static string InsertSpecific(UnityEngine.Object obj, string db_name = null, string tb_name = null, string timestamp_field_name = "ts", string time = "NOW") {
            string action = "INSERT INTO ";
            db_name = SetDatabaseName(db_name);
            tb_name = SetTableNameWith(obj,tb_name);
            return action + db_name + Dot + tb_name + Space + FieldNames(obj, timestamp_field_name) + FieldValues(obj, time);
        }
        //INSERT INTO d1001 (ts, current, phase) VALUES ('NOW', 10.2, 219, 0.32) d1002 (ts, current, phase) VALUES ('NOW', 10.2, 219, 0.32) ...
        public static string InsertSpecific(UnityEngine.Object[] objects, string db_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
            var (tb_name, time) = multiTableInit(objects, _tb_name, _time);
            string action = "INSERT INTO ";
            List<string> tables = new List<string>();
            db_name = SetDatabaseName(db_name);
            string fieldNames = FieldNames(objects[0], timestamp_field_name);
            for (int i=0; i< objects.Length; i++) {
                tb_name[i] = SetTableNameWith(objects[i], tb_name[i]);
                tables.Add( db_name + Dot + tb_name[i] + Space + fieldNames + FieldValues(objects[i], time[i]) );
            }
            return action + string.Join(" ", tables);                        
        }
//INSERT INTO d21001 USING meters TAGS ('Beijing.Chaoyang', 2) VALUES ('2021-07-13 14:06:32.272', 10.2, 219, 0.32)
        public static string InsertUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
            string action = "INSERT INTO ";
            string operation = " USING ";
            db_name = SetDatabaseName(db_name);
            stb_name = db_name + Dot + SetSTableNameWith(obj, stb_name);
            tb_name = db_name + Dot + SetTableNameWith(obj, tb_name);
            return action + tb_name + operation + stb_name + TagValues(obj) + Space + FieldValues(obj);
        }
//INSERT INTO d21001 USING meters TAGS ('Beijing.Chaoyang', 2) VALUES ('2021-07-13 14:06:34.630', 10.2, 219, 0.32) 
//d21002 USING meters TAGS ('Beijing.Chaoyang', 2) VALUES ('2021-07-13 14:06:34.630', 10.2, 219, 0.32) ...
        public static string InsertUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string[] _time = null) {
            var (tb_name, time) = multiTableInit(objects, _tb_name, _time);
            string action = "INSERT INTO ";
            string operation = " USING ";
            List<string> tables = new List<string>();
            db_name = SetDatabaseName(db_name);
            stb_name = db_name + Dot + SetSTableNameWith(objects[0]);
            for (int i=0; i< objects.Length; i++) {
                tb_name[i] = db_name + Dot + SetTableNameWith(objects[i], tb_name[i]);
                tables.Add( tb_name[i] + operation + stb_name + TagValues(objects[i]) + Space + FieldValues(objects[i], time[i]) );
            }
            return action + string.Join(" ", tables);
        }
//INSERT INTO d21001 USING meters (groupId) TAGS (2) VALUES ('2021-07-13 14:06:33.196', 10.15, 217, 0.33)
        public static string InsertSpecificUsing(UnityEngine.Object obj, string db_name = null, string stb_name = null, string tb_name = null, string time = "NOW") {
            string action = "INSERT INTO ";
            string operation = " USING ";
            db_name = SetDatabaseName(db_name);
            stb_name = db_name + Dot + SetSTableNameWith(obj, stb_name);
            tb_name = db_name + Dot + SetTableNameWith(obj, tb_name);
            return action + tb_name + operation + stb_name + Space + TagNames(obj) + TagValues(obj) + Space + FieldValues(obj);            
        }
//INSERT INTO d21001 USING meters (groupId) TAGS (2) (ts, current, phase) VALUES ('2021-07-13 14:06:33.196', 10.15, 217, 0.33)
//d21002 USING meters (groupId) TAGS (2) (ts, current, phase) VALUES ('2021-07-13 14:06:33.196', 10.15, 217, 0.33) ...
        public static string InsertSpecificUsing(UnityEngine.Object[] objects, string db_name = null, string stb_name = null, string[] _tb_name = null, string timestamp_field_name = "ts", string[] _time = null) {
            var (tb_name, time) = multiTableInit(objects, _tb_name, _time);
            string action = "INSERT INTO ";
            string operation = " USING ";
            List<string> tables = new List<string>();
            db_name = SetDatabaseName(db_name);
            stb_name = db_name + Dot + SetSTableNameWith(objects[0]);
            string tagNames = TagNames(objects[0]);
            string fieldNames = FieldNames(objects[0], timestamp_field_name);
            for (int i=0; i< objects.Length; i++) {
                tb_name[i] = db_name + Dot + SetTableNameWith(objects[i], tb_name[i]);
                tables.Add( tb_name[i] + operation + stb_name + Space + tagNames + TagValues(objects[i]) + Space + fieldNames + FieldValues(objects[i], time[i]) );
            }
            return action + string.Join(" ", tables);
        }
        static Func< UnityEngine.Object[], string[], string[], (List<string>, List<string>) > multiTableInit = (objects, _tb_name, _time) => {
            List<string> tb_name = _tb_name == null? new List<string>{ SetTableNameWith(objects[0]) } : new List<string>(_tb_name);
            List<string> time = _time == null? new List<string>{ "NOW" } : new List<string>(_time);
            if (tb_name.Count < objects.Length) {
                for (int i=tb_name.Count; i<objects.Length; i++) {
                    tb_name.Add(String.Empty);
                }
            }
            if (time.Count < objects.Length) {
                for (int i=time.Count; i<objects.Length; i++) {
                    time.Add("NOW");
                }
            }
            return (tb_name, time);            
        };
//ALTER TABLE tb_name SET TAG tag1_name=new_tag1_value; ALTER TABLE tb_name SET TAG tag1_name=new_tag1_value;
        public static List<string> SetTags(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
            string action  = "ALTER TABLE ";
            string operation = " SET TAG ";
            List<string> sqls = new List<string>();
            db_name = SQL.SetDatabaseName(db_name);
            tb_name = SQL.SetTableNameWith(obj);
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if (dt != null) {
                    string new_tag_value = serializeValue(obj, field, dt.length);                                       
                    sqls.Add(action + db_name + Dot + tb_name + operation + field.Name + "=" + new_tag_value);
                }
            }
            return sqls;
        }
// //ALTER TABLE tb_name SET TAG tag_name=new_tag_value;
        public static string SetTag(UnityEngine.Object obj, string tag_name, string db_name = null, string tb_name = null) {
            string action  = "ALTER TABLE ";
            string operation = " SET TAG ";
            string new_tag_value = null;
            db_name = SQL.SetDatabaseName(db_name);
            tb_name = SQL.SetTableNameWith(obj);            
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if (dt != null && field.Name.Equals(tag_name, StringComparison.InvariantCultureIgnoreCase)) {
                    new_tag_value = serializeValue(obj, field, dt.length);                                       
                }
            }
            return action + db_name + Dot + tb_name + operation + tag_name + "=" + new_tag_value;
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
        public static string TagNames(UnityEngine.Object obj) {
            return TagNames(obj.GetType());
        }
        public static string TagNames<T>() {
            return TagNames(typeof(T));
        }
        //(location, groupId)
        public static string TagNames( System.Type type) {
            List<string> tagNames = new List<string>();
            foreach (var field in type.GetFields()) {
                DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
                if ( dt != null) {
                    tagNames.Add(field.Name);
                }
            }
            return Bracket( string.Join(", ", tagNames) );
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
            if (fieldValue == null) return "NULL";
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
        public static string SetTableNameWith(System.Type type, string tb_name =null) {
            if (string.IsNullOrEmpty(tb_name)) {
                tb_name = type.Name;
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

