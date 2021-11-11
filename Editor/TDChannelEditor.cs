using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21.TDengineConnector
{
[CustomEditor(typeof(TDChannel))]
[CanEditMultipleObjects]
public class TDChannelEditor : Editor
{
    SerializedProperty targetProp;
    SerializedProperty databaseNameProp;
    SerializedProperty superTableNameProp;
    SerializedProperty tableNameProp;
    SerializedProperty timeStampNameProp;
    SerializedProperty requestProp;
    SerializedProperty usingSuperTableProp;
    SerializedProperty autoCreateProp;
    SerializedProperty insertSpecificProp;
    SerializedProperty dataTimeProp;
    SerializedProperty autoInitializeProp;
    SerializedProperty autoSetTargetProp;
    SerializedProperty autoSetDatabaseProp;
    SerializedProperty autoSetSuperTableProp;
    SerializedProperty autoSetTableProp;

    protected static bool showCache = false;
    protected static bool showInit = false;
    void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
        databaseNameProp = serializedObject.FindProperty("databaseName");
        superTableNameProp = serializedObject.FindProperty("superTableName");
        tableNameProp = serializedObject.FindProperty("tableName");
        timeStampNameProp = serializedObject.FindProperty("timeStampName");
        requestProp = serializedObject.FindProperty("request");
        usingSuperTableProp = serializedObject.FindProperty("usingSuperTable");
        autoCreateProp = serializedObject.FindProperty("autoCreate");
        insertSpecificProp = serializedObject.FindProperty("insertSpecific");
        dataTimeProp = serializedObject.FindProperty("dataTime");
        autoInitializeProp = serializedObject.FindProperty("autoInitialize");
        autoSetTargetProp = serializedObject.FindProperty("autoSetTarget");
        autoSetDatabaseProp = serializedObject.FindProperty("autoSetDatabase");
        autoSetSuperTableProp = serializedObject.FindProperty("autoSetSuperTable");
        autoSetTableProp = serializedObject.FindProperty("autoSetTable");
    }
    public override void OnInspectorGUI()
    {        
        GUIContent initialize = new GUIContent(  (Texture)Resources.Load("init"), "Initialize");
        GUIContent createDB = new GUIContent( (Texture)Resources.Load("createDB"), "Create Database");
        GUIContent createS = new GUIContent( (Texture)Resources.Load("createS"), "Create Super Table");
        GUIContent dropS = new GUIContent( (Texture)Resources.Load("removeS"), "Drop Super Table");
        GUIContent create = new GUIContent( (Texture)Resources.Load("create"), "Create Table");
        GUIContent drop = new GUIContent( (Texture)Resources.Load("remove"), "Drop Table");
        GUIContent pull = new GUIContent(" Pull", (Texture)Resources.Load("pull"), "Read the last row(fields and tags) from the table and update values of the Target object");
        GUIContent push = new GUIContent(" Push Tags", (Texture)Resources.Load("push"), "Overwrite all tags of the table with current values of the Target object");
        GUIContent alter = new GUIContent( (Texture)Resources.Load("alter"), "Alter Super Table\n \n"+
            "Apply changes on the shape of the super table.\n"+
            "Specifically performs the following operations in sequence: \n"+
            "1. Drop all columns that no longer exists in Target object.\n"+
            "2. Resize NCHAR and BINARY columns if needed, according to the Target object.\n"+
            "3. Add new fields/tags of the Target object into the super table.\n"+
            "*Note: NCHAR/BINARY columns with shorter length in the Target object than in the super table will be DROPPED and re-created.");
        GUIContent autoCreateLabel = new GUIContent("Auto Create", "On insert, auto create table if not exists using the super table.");
        GUIContent insertSpecificLabel = new GUIContent(" Insert Specific", "Turn this on if the sequence of fields differ from that of comlumns in the table.");
        GUIContent insert = new GUIContent(" Insert", (Texture)Resources.Load("insert"), "Insert Values");
        
        GUIContent autoInitializeLabel = new GUIContent("Auto Initialize", "Auto Initialize TDChannel on enable");
        GUIContent autoSetTargetLabel = new GUIContent("Auto Set Target", "Scan the components of current GameObject, search for the first one that has either [DataTag] or [DataField] attribute, then make this component target.");
        GUIContent autoSetDatabaseLabel = new GUIContent("Auto Set Database", "Use 'Default Database Name' in the TDBridge as database name.");
        GUIContent autoSetSuperTableLabel = new GUIContent("Auto Set Super Table", "Use target type name as super table name");
        GUIContent autoSetTableLabel = new GUIContent("Auto Set Table Name", "Use current GameObject's name as table name. Unsupported characters will be culled.");

        TDChannel td = (TDChannel)target;
        var fields = td.fields;
        var tags = td.tags;
        
        //Initialization
        GUILayout.BeginHorizontal();
        showInit = EditorGUILayout.Foldout(showInit, "Initialization Settings", true, EditorStyles.foldout);
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(autoInitializeProp, autoInitializeLabel);
        GUILayout.EndHorizontal();
        if (showInit) {
            // EditorGUILayout.PropertyField(autoInitializeProp, autoInitializeLabel);
            EditorGUILayout.LabelField("What to do on Initialize:");
            EditorGUILayout.PropertyField(autoSetTargetProp, autoSetTargetLabel);
            EditorGUILayout.PropertyField(autoSetDatabaseProp, autoSetDatabaseLabel);
            EditorGUILayout.PropertyField(autoSetSuperTableProp, autoSetSuperTableLabel);
            EditorGUILayout.PropertyField(autoSetTableProp, autoSetTableLabel);
        }

        //Target
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(targetProp);
        if (EditorGUI.EndChangeCheck()) {
            if(td.autoInitialize) td.Initialize();
        }
        if ( GUILayout.Button(initialize, GUILayout.Width(32), GUILayout.Height(24)) ){
            td.Initialize();
        }      
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel = 1;

        //Fields & Tags Title
        int minWidth = 50;
        int spaceHeight = 10;
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(Screen.width/3));
        showCache = EditorGUILayout.Foldout(showCache, "Cache", true, EditorStyles.foldout);
        GUILayout.EndVertical();
        //Fields
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Fields:" + fields.Count.ToString(),GUILayout.MinWidth(minWidth));
        if (showCache) {
            GUILayout.Space(spaceHeight);
            foreach(var fieldName in fields.Keys)
                EditorGUILayout.LabelField(fieldName,GUILayout.MinWidth(minWidth));            
        }
        GUILayout.EndVertical();
        //Tags
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Tags:" + tags.Count.ToString(),GUILayout.MinWidth(minWidth));
        if (showCache) {
            GUILayout.Space(spaceHeight);
            foreach(var tagName in tags.Keys)
                EditorGUILayout.LabelField(tagName,GUILayout.MinWidth(minWidth));            
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel = 0;
        if (showCache) {
            GUILayout.Space(spaceHeight);
        }

        //DB
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(databaseNameProp);
        if ( GUILayout.Button(createDB, GUILayout.Width(56), GUILayout.Height(24)) ){
            td.CreateDatabase();
        }
        GUILayout.Space(32);
        GUILayout.EndHorizontal();
        
        //STable
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(superTableNameProp);
        if ( GUILayout.Button(createS, GUILayout.Width(56), GUILayout.Height(24)) ){
            td.CreateSuperTableForTarget();
        }
        if ( GUILayout.Button(dropS, GUILayout.Width(28), GUILayout.Height(24) ) ){
            td.DropSuperTableForTarget();
        }
        GUILayout.EndHorizontal();

        //Table
        GUILayout.BeginHorizontal();        
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(usingSuperTableProp);
        GUILayout.Space(32);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(tableNameProp);
        if ( GUILayout.Button(create, GUILayout.Width(56), GUILayout.Height(24)) ){
            td.CreateTableForTarget();
        }
        if ( GUILayout.Button(drop, GUILayout.Width(28), GUILayout.Height(24) ) ){
            td.DropTableForTarget();
        }        
        GUILayout.EndHorizontal();
        GUILayout.Space(16);

        EditorGUILayout.PropertyField(timeStampNameProp);
        //Pull & Push
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(pull, GUILayout.Height(24)) ){
            td.PullImmediately();
        }
        if ( GUILayout.Button(push, GUILayout.Height(24)) ){
            td.PushTagsImmediately();
        }        
        if ( GUILayout.Button(alter, GUILayout.Width(28), GUILayout.Height(24)) ){
            td.Alter();
        } 
        GUILayout.EndHorizontal();
        GUILayout.Space(16);
        
        //Insert
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(autoCreateProp, autoCreateLabel);
        EditorGUILayout.PropertyField(insertSpecificProp, insertSpecificLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(dataTimeProp);
        if ( GUILayout.Button(insert, GUILayout.Height(24)) ){
            td.Insert();
        }        
        GUILayout.EndHorizontal();

        //Request
        EditorGUILayout.PropertyField(requestProp,true);
        serializedObject.ApplyModifiedProperties();        
    }
}
}
