using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Sean21.TDengineConnector
{
public partial class TDBridge
{
    public static TDResult Parse(string json)
    {
        TDResult r = new TDResult();
        string remaining = json;
        
        //Extract status
        r.status = Process(ref remaining, ":\"", "\"");
        //Extract column meta
        string metas = Process(ref remaining, "[[", "]]", 1);
        //Extract data        
        string datas = Process(ref remaining, "[[", "]]", 1);
        //Extract rows
        r.rows = int.Parse( Process(ref remaining, ":", "}") );
        //Split column meta
        while (metas.Length > 0)
        {
            string meta = Process(ref metas, "[", "]");
            string[] info = meta.Split(',');
            r.column_meta.Add(new TDResult.ColumnMeta
            (
                info[0].Replace("\"", ""), int.Parse(info[1]), int.Parse(info[2])
            ));
        }
        //Split data
        while (r.data.Count < r.rows)
        {
            string rowString = Process(ref datas, "[", "]");
            // List<string> value = new List<string>();
            TDResult.Row currentRow = new TDResult.Row(new List<string>());
            for (int j=0; j < r.column_meta.Count; j++) {
                Func<string> currentValue = () => {
                    switch (r.column_meta[j].typeIndex)
                    {
                        case 8: case 9: case 10:
                            return Process(ref rowString, "\"", "\"");
                        default:
                            rowString = rowString.TrimStart(',');
                            return Process(ref rowString, "", ",");
                    }
                };
                currentRow.value.Add(currentValue());
            }
            r.data.Add(currentRow);
        }
        return r;
    }
    public static TDResult.LoginResult ParseLogin(string json)
    {
        TDResult.LoginResult r = new TDResult.LoginResult();
        string remaining = json;
        //Parse status
        r.status = Process(ref remaining, ":\"", "\",");
        //Parse code
        r.code = int.Parse( Process(ref remaining, ":\"", "\",") );
        //parse desc
        r.desc = Process(ref remaining, ":", "}");
        return r;
    }
    //string: (-3.402823E+38, -3.402823E+38, -3.402823E+38),(-3.402823E+38, -3.402823E+38, -3.402823E+38),(-3.402823E+38, -3.402823E+38, -3.402823E+38)
    public static Transform ParseTransform(string s, Transform tr)
    {
        string[] property = s.Split(')');
        tr.localPosition = ParseVector3(property[0]);
        tr.localEulerAngles = ParseVector3(property[1]);
        tr.localScale = ParseVector3(property[2]);
        return tr;
    }

    //string: (-3.402823E+38, -3.402823E+38, -3.402823E+38, -3.402823E+38)
    public static Quaternion ParseQuaternion(string s)
    {
        float[] element = vectorElement(s);
        return new Quaternion(element[0], element[1], element[2], element[3]);
    }
    //string: (-3.402823E+38, -3.402823E+38, -3.402823E+38)
    public static Vector3 ParseVector3(string s)
    {
        float[] element = vectorElement(s);
        return new Vector3(element[0], element[1], element[2]);
    }
    //string: (-3.402823E+38, -3.402823E+38)
    public static Vector2 ParseVector2(string s)
    {
        float[] element = vectorElement(s);
        return new Vector2(element[0], element[1]);
    }
    static Func<string, float[]> vectorElement = (vectorString) => {
        if (string.IsNullOrEmpty(vectorString) || vectorString == "null")
            return null;
        vectorString = vectorString.Trim( new char[]{ '(' , ')' , ',' } );
        return Array.ConvertAll(vectorString.Split(','), float.Parse);
    };
    //digest string from left to right.
    private static string Process(ref string source, string start, string end, int expand = 0)
    {
        int i_start = source.IndexOf(start) + start.Length - expand;
        //clamp i_start
        if (i_start < 0 ) i_start = 0;
        //trim processed
        source = source.Substring(i_start);
        int i_end = source.IndexOf(end) + expand;
        //clamp i_end
        if (i_end < 0 ) i_end = source.Length;
        //extract target
        string target = source.Substring(0, i_end);
        //trim processed
        int i_newStart = i_end + end.Length;
        source = i_newStart < source.Length ? source.Substring(i_newStart) : string.Empty;
        return target;
    }
}

}