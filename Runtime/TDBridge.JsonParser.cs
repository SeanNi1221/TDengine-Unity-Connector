using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sean21
{
public partial class TDBridge
{
    public static Result Parse(string json)
    {
        Result r = new Result();
        string remaining = json;
        
        //Parse status
        r.status = Process(ref remaining, ":", ",");
        
        //Parse column meta
        string metas = Process(ref remaining, "[[", "]]", 1);
        while (metas.Length > 0)
        {
            string meta = Process(ref metas, "[", "]");
            string[] info = meta.Split(',');
            r.column_meta.Add(new ColumnMeta
            (
                info[0].Replace("\"", ""), int.Parse(info[1]), int.Parse(info[2])
            ));
        }
        
        //Parse data. Each value is treated as string because no template class are provided at current stage.
        string datas = Process(ref remaining, "[[", "]]", 1);
        while (datas.Length > 0)
        {
            string dataRow = Process(ref datas, "[", "]");
            List<string> currentData = new List<string>( dataRow.Split(',') );
            for (int i=0; i < currentData.Count; i++) {
                currentData[i] = currentData[i].Replace("\"", "");
            }
            r.data.Add(new Row(currentData));
        }

        //Pares rows
        r.rows = r.data.Count;
        return r;
    }
    public LoginResult ParseLogin(string json)
    {
        LoginResult r = new LoginResult();
        string remaining = json;
        //Parse status
        r.status = Process(ref remaining, ":", ",");
        //Parse code
        r.code = int.Parse( Process(ref remaining, ":", ",") );
        //parse desc
        r.desc = Process(ref remaining, ":", "}");
        return r;
    }
    //return the selected clip and remove it from the source.
    private static string Process(ref string source, string start, string end, int expand = 0)
    {
        int i_start = source.IndexOf(start) + start.Length - expand;
        int i_end = source.IndexOf(end) + expand;
        string target = source.Substring(i_start, i_end - i_start).Replace("\"", "");
        source = source.Substring(i_end + end.Length);
        return target;
    }

}

}