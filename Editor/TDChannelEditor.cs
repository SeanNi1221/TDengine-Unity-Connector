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
    SerializedProperty requestProp;
    SerializedProperty usingSuperTableProp;
    SerializedProperty autoCreateProp;
    SerializedProperty insertSpecificProp;
    void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
        databaseNameProp = serializedObject.FindProperty("databaseName");
        superTableNameProp = serializedObject.FindProperty("superTableName");
        tableNameProp = serializedObject.FindProperty("tableName");
        requestProp = serializedObject.FindProperty("request");
        usingSuperTableProp = serializedObject.FindProperty("usingSuperTable");
        autoCreateProp = serializedObject.FindProperty("autoCreate");
        insertSpecificProp = serializedObject.FindProperty("insertSpecific");
    }
    public override void OnInspectorGUI()
    {        
        GUIContent createDB = new GUIContent(" Create Database", (Texture)Resources.Load("createDB"), "CREATE DATABASE IF NOT EXISTS [Database Name]");
        GUIContent createS = new GUIContent(" Create Super Table", (Texture)Resources.Load("createS"), "Create Super Table for the type of Target");
        GUIContent dropS = new GUIContent( (Texture)Resources.Load("removeS"), "Drop Super Table");
        GUIContent create = new GUIContent(" Create Table", (Texture)Resources.Load("create"), "Create Table for Target");
        GUIContent drop = new GUIContent( (Texture)Resources.Load("remove"), "Drop Table");
        GUIContent pull = new GUIContent(" Pull All", (Texture)Resources.Load("pull"), "Pull Tags and the last row of Fields from the table");
        GUIContent push = new GUIContent(" Set Tags", (Texture)Resources.Load("push"), "Set Tags by current values of the Target object");
        GUIContent alter = new GUIContent( (Texture)Resources.Load("alter"), "Alter Super Table\n \n"+
            "Performs the following operations: \n"+
            "1. Drop all columns that no longer exists in Target class.\n"+
            "2. Resize NCHAR and BINARY columns if needed, according to Target class.\n"+
            "3. Add new columns of Target class into the super table.\n"+
            "*Note: NCHAR/BINARY columns with shorter length in the Target class than in the table will be DROPPED and re-created.");
        GUIContent autoCreateLabel = new GUIContent("Auto Create", "On insert, auto create table if not exists using the super table.");
        GUIContent insertSpecificLabel = new GUIContent(" Insert Specific", "Turn this on if the sequence of fields differ from that of comlumns in the table.");

        GUIContent insert = new GUIContent(" Insert", (Texture)Resources.Load("insert"), "Insert Values");
        GUIContent send = new GUIContent(" Send Request", (Texture)Resources.Load("terminal"), "Send SQL Request");
        GUIContent clearRequest = new GUIContent( (Texture)Resources.Load("clear"), "Clear Request");
        
        TDChannel td = (TDChannel)target;
        // DrawDefaultInspector();
        EditorGUILayout.PropertyField(targetProp);
        
        //DB
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(databaseNameProp);
        if ( GUILayout.Button(createDB, GUILayout.Height(24)) ){
            td.CreateDatabase();
        }
        GUILayout.EndHorizontal();
        
        //STable
        EditorGUILayout.PropertyField(superTableNameProp);
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(createS, GUILayout.Height(24)) ){
            td.CreateSuperTableForTarget();
        }
        if ( GUILayout.Button(dropS, GUILayout.Width(32), GUILayout.Height(24) ) ){
            td.DropSuperTableForTarget();
        }
        GUILayout.EndHorizontal();

        //Table
        EditorGUILayout.PropertyField(tableNameProp);
        EditorGUILayout.PropertyField(usingSuperTableProp);
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(create, GUILayout.Height(24)) ){
            td.CreateTableForTarget();
        }
        if ( GUILayout.Button(drop, GUILayout.Width(32), GUILayout.Height(24) ) ){
            td.DropTableForTarget();
        }        
        GUILayout.EndHorizontal();
        GUILayout.Space(16);

        //Pull & Push
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(pull, GUILayout.Height(24)) ){
            td.Pull();
        }
        if ( GUILayout.Button(push, GUILayout.Height(24)) ){
            td.SetTags();
        }        
        if ( GUILayout.Button(alter, GUILayout.Width(32), GUILayout.Height(24)) ){
            td.Alter();
        } 
        GUILayout.EndHorizontal();
        GUILayout.Space(16);
        
        //Insert
        EditorGUILayout.PropertyField(autoCreateProp, autoCreateLabel);
        EditorGUILayout.PropertyField(insertSpecificProp, insertSpecificLabel);
        if ( GUILayout.Button(insert, GUILayout.Height(24)) ){
            td.Insert();
        }        

        //Request
        EditorGUILayout.PropertyField(requestProp);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(send, GUILayout.Height(32)))
        {
            td.SendRequest();           
        }
        if ( GUILayout.Button(clearRequest, GUILayout.Width(32), GUILayout.Height(32))){
            td.request.Clear();
        } 
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();        
    }
}
}
