using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Sean21.TDengineConnector
{
public static class SQL
{
    const string Dot = ".";
    const string Space = " ";
    // select * from test.th_meter limit 1
    public static string GetFirstRowWithoutTS(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
        string action = "SELECT ";
        string option = " LIMIT 1";
        db_name = SetDatabaseName(db_name);
        tb_name = SetTableNameWith(obj, tb_name);
        string columnNames = ColumnNamesWithoutTS(obj);
        return action + columnNames + " FROM " + db_name + Dot + tb_name + option; 
    }
    // SELECT LAST_ROW(columnNames) FROM test.TH_Meter0001
    public static string GetLastRow(UnityEngine.Object obj, string columnNames = "*", bool withTags = false, string db_name = null, string tb_name = null) {
        string action = "SELECT LAST_ROW";
        db_name = SetDatabaseName(db_name);
        tb_name = SetTableNameWith(obj, tb_name);
        return withTags?
            action + Bracket(columnNames) + "," + TagNames(obj, false) + " FROM " + db_name + Dot + tb_name :            
            action + Bracket(columnNames) + " FROM " + db_name + Dot + tb_name; 
    }
    // SELECT tag1_name, tag2_name FROM test.TH_Meter0001
    public static string GetTags(UnityEngine.Object obj, string db_name = null, string tb_name = null) {
        string action = "SELECT ";
        db_name = SetDatabaseName(db_name);
        tb_name = SetTableNameWith(obj, tb_name);
        return action + TagNames(obj, false) + " FROM " + db_name + Dot + tb_name;           
    }  
    public static string CreateDatabase(string db_name = null, bool if_not_exists = true) {
        string action = if_not_exists? "CREATE DATABASE IF NOT EXISTS ":"CREATE DATABASE ";
        db_name = SetDatabaseName(db_name);
        return action + db_name;
    }
    public static string CreateSTable<T>(string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        return CreateSTable(typeof(T), db_name, stb_name, timestamp_field_name, if_not_exists);
    }
    public static string CreateSTable(UnityEngine.Object obj, string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        return CreateSTable(obj.GetType(), db_name, stb_name, timestamp_field_name, if_not_exists);
    }
    public static string CreateSTable(Type type, string db_name = null, string stb_name = null, string timestamp_field_name = "ts", bool if_not_exists = true) {
        string action = if_not_exists? "CREATE STABLE IF NOT EXISTS ":"CREATE STABLE ";
        db_name = SetDatabaseName(db_name);
        stb_name = SetSTableNameWith(type, stb_name);
        string fieldTypes = FieldTypes(type, timestamp_field_name);
        string tagTypes = TagTypes(type);
        string sql = action + Quote(db_name) + Dot + Quote(stb_name) + fieldTypes + tagTypes;
        Debug.Log("SQL- CREATE STABLE from type " + type.Name + ":" + "\n" + sql);
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
    public static string FieldNames(UnityEngine.Object obj, string timestamp_field_name = "ts", bool withBracket = true) {
        return FieldNames( obj.GetType(), timestamp_field_name, withBracket);
    }
    public static string FieldNames<T>(string timestamp_field_name = "ts", bool withBracket = true) {
        return FieldNames( typeof(T), timestamp_field_name, withBracket);
    }
    //(ts, current, phase)
    public static string FieldNames( System.Type type, string timestamp_field_name = "ts", bool withBracket = true) {
        List<string> fieldNames = new List<string>{ Quote(timestamp_field_name) };
        foreach (var field in type.GetFields()) {
            DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( df != null) {
                fieldNames.Add(field.Name);
            }
        }
        string names = string.Join(", ", fieldNames);
        return withBracket? Bracket(names) : names;
    }
    public static string ColumnNamesWithoutTS(UnityEngine.Object obj,  bool withBracket = false) {
        return ColumnNamesWithoutTS( obj.GetType(), withBracket);
    }
    public static string ColumnNamesWithoutTS<T>(bool withBracket = false) {
        return ColumnNamesWithoutTS( typeof(T), withBracket);
    }
//(current, phase, location)
    public static string ColumnNamesWithoutTS( System.Type type, bool withBracket = false) {
        List<string> columnNames = new List<string>();
        foreach (var field in type.GetFields()) {
            DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
            DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( dt!= null || df != null ) {
                columnNames.Add(field.Name);
            }
        }
        string names = string.Join(", ", columnNames);
        return withBracket? Bracket(names) : names;
    }
    public static string TagNames(UnityEngine.Object obj, bool withBracket = true) {
        return TagNames(obj.GetType(), withBracket);
    }
    public static string TagNames<T>(bool withBracket = true) {
        return TagNames(typeof(T), withBracket);
    }
    //(location, groupId)
    public static string TagNames( System.Type type, bool withBracket = true) {
        List<string> tagNames = new List<string>();
        foreach (var field in type.GetFields()) {
            DataTag dt = Attribute.GetCustomAttribute(field, typeof(DataTag)) as DataTag;
            if ( dt != null) {
                tagNames.Add(field.Name);
            }
        }
        string names = string.Join(", ", tagNames);
        return withBracket? Bracket( names ) : names;
    }
    public static string FieldTypes<T>(string timestamp_field_name = "ts") {
        return FieldTypes(typeof(T), timestamp_field_name);
    }
    public static string FieldTypes(UnityEngine.Object obj, string timestamp_field_name = "ts") {
        return FieldTypes(obj.GetType(), timestamp_field_name);
    }
    public static string FieldTypes(Type type, string timestamp_field_name = "ts") {
        List<string> fieldTypes = new List<string>{ "'" + timestamp_field_name + "'" + " TIMESTAMP" };
        foreach (var field in type.GetFields()) {            
            DataField df = Attribute.GetCustomAttribute(field, typeof(DataField)) as DataField;
            if ( df != null) {
                Type fieldType = field.FieldType;
                int typeIndex = TDBridge.varType.IndexOf(fieldType);
                switch (typeIndex)
                {
                    default: break;
                    case 0: 
                        Debug.LogWarning("Field Type '" + fieldType.Name + "' is not supported, value will be force converted to nchar(100)!");
                        break;
                    case -1:
                        Debug.LogError("Field Type '" + fieldType.Name + "' is not supported!");
                        break;
                }
                string typeOfThis = " '" + field.Name + "' " + TDBridge.dataType[typeIndex];
                if (isTextData(typeIndex)) {
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
                Type fieldType = field.FieldType;
                int typeIndex = TDBridge.varType.IndexOf(fieldType);
                switch (typeIndex)
                {
                    default: break;
                    case 0: 
                        Debug.LogWarning("Field Type '" + fieldType.Name + "' is not supported, value will be force converted to nchar(100)!");
                        break;
                    case -1:
                        Debug.LogError("Field Type '" + fieldType.Name + "' is not supported!");
                        break;
                }
                string typeOfThis = " '" + field.Name + "' " + TDBridge.dataType[typeIndex];
                if (isTextData(typeIndex)) {
                    typeOfThis += Bracket( dt.length.ToString() );
                }
                tagTypes.Add(typeOfThis);
            }            
        }            
        return " TAGS" + Bracket( string.Join("," , tagTypes) );
    }
    static Func<FieldInfo, int, string> serializeType = (field, length) => {
        int typeIndex = TDBridge.varType.IndexOf(field.FieldType);
        string typeOfThis = " '" + field.Name + "' " + TDBridge.dataType[typeIndex];
        if(isTextData(typeIndex)) return typeOfThis + Bracket( length.ToString() );
        else return typeOfThis;
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
        int typeIndex = TDBridge.varType.IndexOf(field.FieldType);
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
            case 11:
                return ((Vector2)fieldValue).ToString("");
            case 12:
                return ((Vector3)fieldValue).ToString("");
            case 13:
                return ((Quaternion)fieldValue).ToString("");
            case 14:
                return Quote(SerializeTransform(fieldValue as Transform));
        }
    };
    public static string SerializeTransform(Transform tr)
    {
        if (!tr) return string.Empty;
        return tr.localPosition.ToString("") + "," + tr.localEulerAngles.ToString("") + "," + tr.localScale.ToString("");
    }
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
    public static string Quote(string s) {
        if ( s.StartsWith("'") && s.EndsWith("'") ) {
            return s;
        }
        else return "'" + s + "'";
    }
    public static string Quote(int l) {
        return Quote(l.ToString());
    }
    public static string Quote(float f) {
        return Quote(f.ToString());
    }
    public static string Bracket(string s, bool force = false) {
        if (force ) {
            return "(" + s + ")";
        }
        else {
            if ( s.StartsWith("(") && s.EndsWith(")") ) {
                return s;
            }
            else return "(" + s + ")";
        }
    }
    public static string Bracket(int i) {
        return Bracket(i.ToString(), true);
    }
    public static string Bracket(float f) {
        return Bracket(f.ToString(), true);
    }
    public static bool isTextData(int typeIndex) {
        return (typeIndex == 8 || typeIndex == 10)? true:false;
    }
    public static string TimeStamp16 {
        get {return System.DateTime.Now.ToString("yyMMddHHmmssffff");}
    }
    public static bool isValidForName(char c) {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_');
    }  
}
}
