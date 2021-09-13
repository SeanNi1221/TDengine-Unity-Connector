using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Sean21
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
    public bool usingSuperTable = true;
    void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
        databaseNameProp = serializedObject.FindProperty("databaseName");
        superTableNameProp = serializedObject.FindProperty("superTableName");
        tableNameProp = serializedObject.FindProperty("tableName");
        requestProp = serializedObject.FindProperty("request");
    }
    public override void OnInspectorGUI()
    {        
        GUIContent createDB = new GUIContent(" Create Database", (Texture)Resources.Load("createDB"), "CREATE DATABASE IF NOT EXISTS [Database Name]");
        GUIContent createS = new GUIContent(" Create Super Table", (Texture)Resources.Load("createS"), "Create Super Table for the type of Target");
        GUIContent dropS = new GUIContent( (Texture)Resources.Load("removeS"), "Drop Super Table");
        GUIContent create = new GUIContent(" Create Table", (Texture)Resources.Load("create"), "Create Table for Target");
        GUIContent drop = new GUIContent( (Texture)Resources.Load("remove"), "Drop Table");
        GUIContent pull = new GUIContent(" Pull", (Texture)Resources.Load("pull"), "Pull all values from the table");
        GUIContent push = new GUIContent(" Push", (Texture)Resources.Load("push"), "Push values into the table (for tags only)");
        GUIContent alter = new GUIContent( (Texture)Resources.Load("alter"), "Alter Super Table\n \n"+
            "Performs the following operations: \n"+
            "1. Drop all columns that no longer exists in Target class.\n"+
            "2. Resize NCHAR and BINARY columns if needed, according to Target class.\n"+
            "3. Add new columns of Target class into the super table.\n"+
            "*Note: NCHAR/BINARY columns with shorter length in the Target class than in the table will be DROPPED and re-created.");
        GUIContent send = new GUIContent(" Send Request", (Texture)Resources.Load("terminal"), "Send SQL Request");
        
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
        usingSuperTable = EditorGUILayout.Toggle("Using Super Table", usingSuperTable);
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(create, GUILayout.Height(24)) ){
            td.CreateTableForTarget(usingSuperTable);
        }
        if ( GUILayout.Button(drop, GUILayout.Width(32), GUILayout.Height(24) ) ){
            td.DropTableForTarget();
        }        
        GUILayout.EndHorizontal();
        GUILayout.Space(16);

        //Pull & Push
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(pull, GUILayout.Height(24)) ){
            td.PullValues();
        }
        if ( GUILayout.Button(push, GUILayout.Height(24)) ){
            td.SetTags();
        }        
        if ( GUILayout.Button(alter, GUILayout.Width(32), GUILayout.Height(24)) ){
            td.Alter();
        } 
        GUILayout.EndHorizontal();
        GUILayout.Space(16);

        //Request
        EditorGUILayout.PropertyField(requestProp);
        if (GUILayout.Button(send, GUILayout.Height(32)))
        {
            td.SendRequest();           
        }        
    }
}
}
