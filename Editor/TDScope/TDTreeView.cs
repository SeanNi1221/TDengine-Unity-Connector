using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
namespace Sean21.TDengineConnector
{
[Serializable]
// internal class TDTreeViewData
// {
//     public string[] db_names;
//     IEnumerator GetDBNames() {
//         yield return TDBridge.Request.Send("SHOW DATABASES");
//         db_names = TDBridge.Result.PassValue("name");
//     }
//     public void Refresh() {
//         TDBridge.Start(GetDBNames());
//     }
// }

internal class TDTreeView : TreeView
{
    TDScopeData data;
    int counter;
    TDRequest request = new TDRequest(false);
    TDResult result => request.result;
    public bool loadingData => !request.operation.isDone;
    public float loadingProgress => request.operation.progress;

    public TDTreeView(TreeViewState treeViewstate): base(treeViewstate) {
        Reload();
    }
    public TDTreeView(TreeViewState treeViewstate, TDScopeData data): base(treeViewstate) {
        this.data = data;
        Reload();
    }
    protected override TreeViewItem BuildRoot() {

        var root = new TreeViewItem{id=GenerateID(), depth = -1, displayName = "TDengine"};

        if (data == null && data.db_names == null) {
            root.AddChild(new TreeViewItem{id=GenerateID(), displayName = "TDScopeData Asset Unavailable"});
            goto build;
        }
        if (data.db_names.Count < 1) {
            root.AddChild(new TreeViewItem{id=GenerateID(), displayName = "No Databases found"});
            goto build;
        }
        foreach (string db_name in data.db_names) {
            root.AddChild(new TreeViewItem{id=GenerateID(), displayName = db_name});
        }
        build:
        SetupDepthsFromParentsAndChildren(root);
        Debug.Log("TreeView built");
        return root;
    }
    public void Refresh() {
        // TDBridge.Start(Refresh());
        request.OnFinished += PassDataToScope;
        TDBridge.Start(request.Send("SHOW DATABASES"));
    }
    public void PassDataToScope() {
        data.db_names = result.PassValue("name");
        Reload();
    }
    int GenerateID(){
        return ++counter;
    }
}
internal class TDTreeViewItem<T> : TreeViewItem where T : TDTreeElement
{
    public T data {get; set;}
    public TDTreeViewItem (int id, int depth, T data) :base(id, depth) {
        this.data = data;
    }
}
[Serializable]
internal class TDTreeElement
{
    public Dictionary<string, string> fields;
    public Dictionary<string, string> tags;
    public TDTreeElement(Dictionary<string, string> fields ) {
        this.fields = fields;
    }
    public TDTreeElement(Dictionary<string, string> fields, Dictionary<string, string> tags ) 
    : this(fields)
    {
        this.tags = tags;
    }
}
}