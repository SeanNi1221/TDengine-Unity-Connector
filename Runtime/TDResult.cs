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
    public List<Row> data = new List<Row>();
    public int rows;
    public TDResult (){
        status = null; column_meta = new List<ColumnMeta>(); data = new List<Row>(); rows = 0;
    }
    public TDResult (string _status, List<ColumnMeta> _column_meta, List<Row> _data) {
        status = _status; column_meta = _column_meta; data = _data; rows = data.Count;
    }
    public void Clear() {
        status = null; column_meta = new List<ColumnMeta>(); data = new List<Row>(); rows = 0;
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
    [Serializable]
    public struct Row {
        public List<string> value; 
        public Row(List<string> _value = null) {
            this.value = _value;
        }
    }
    public string Value(int valueIndex)
    {
        return Value(0, valueIndex);
    }
    public string Value(int rowIndex, int valueIndex)
    {
        return data[rowIndex].value[valueIndex];
    }
    public string Value(string column_name)
    {
        return Value(0, column_name);
    }
    public string Value(int rowIndex, string column_name)
    {
        int valueIndex = column_meta.FindIndex(x => x.name.Equals(column_name, StringComparison.OrdinalIgnoreCase));
        return data[rowIndex].value[valueIndex];
    }
    
}
}
