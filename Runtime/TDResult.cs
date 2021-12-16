using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sean21.TDengineConnector
{
[Serializable]
public class TDResult {
    public string status;
    public List<ColumnMeta> column_meta = new List<ColumnMeta>();
    public List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

    public int rows;
    public void Clear() {
        status = null; column_meta = new List<ColumnMeta>(); data = new List<Dictionary<string, string>>(); rows = 0;
    }
    public class LoginResult {
        public string status;
        public int code;
        public string desc;
    }
    [Serializable]
    public struct ColumnMeta {
        public string name;
        public int typeIndex;
        public int length;
        public bool isResizable{
            get;
            private set;
        }
        public ColumnMeta( string a, int t, int l) {
            this.name = a;
            this.typeIndex = t;
            this.length = l;
            this.isResizable = (t == TDBridge.dataType.IndexOf("nchar") || t == TDBridge.dataType.IndexOf("binary"));
        }
    }
    
}
}
