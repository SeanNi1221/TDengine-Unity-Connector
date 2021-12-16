using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
namespace Sean21.TDengineConnector
{
[Serializable]
internal class TDTreeView : TreeView 
{
    public TDTreeView(TreeViewState treeViewState) : base(treeViewState) { Reload(); }
    protected override TreeViewItem BuildRoot ()
    {
        var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
        var allItems = new List<TreeViewItem>{
            new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
            new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
            new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
            new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
            new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
            new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
            new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
            new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
            new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},            
        };
        SetupParentsAndChildrenFromDepths (root, allItems);
        return root;
    }
}


internal class TreeViewItem<T> : TreeViewItem where T : TreeElement
{
    public T data { get; set; }
    public TreeViewItem (int id, int depth, string displayName, T data) : base (id, depth, displayName) {  this.data = data; }
}
[Serializable]
public class TreeElement
{
    [SerializeField] int m_ID;
    [SerializeField] string m_Name;
    [SerializeField] int m_Depth;
    [NonSerialized] TreeElement m_Parent;
    [NonSerialized] List<TreeElement> m_Children;
    public int depth { get => m_Depth; set => m_Depth = value; }
    public TreeElement parent { get => m_Parent; set => m_Parent = value; }
    public List<TreeElement> children { get => m_Children; set => m_Children = value; }
    public bool hasChildren { get => children != null && children.Count > 0; }
    public string name { get => m_Name; set => m_Name = value; }
    public int id { get => m_ID; set => m_ID = value; }
    public TreeElement (){}
    public TreeElement (string name, int depth, int id) { m_Name = name; m_ID = id; m_Depth = depth; }

}
}