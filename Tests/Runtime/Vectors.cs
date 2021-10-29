using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;
[ExecuteInEditMode]
public class Vectors : MonoBehaviour
{
    public string parseTest = "(4567.98345)";
    public string stringlengthTest;
    public float floatData = 12.123456789f;
    public Vector2 v2Data = new Vector2(float.MinValue, float.MinValue);
    // public Vector3 v3Data = new Vector3( Mathf.PI, -Mathf.PI*2, Mathf.PI*3);
    public Vector3 v3Data = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    public Quaternion qData = new Quaternion(float.MinValue, float.MinValue, float.MinValue, float.MinValue);
    public Transform trData;
    public Transform modifier;
    public string formatter = "";
    
    void Reset()
    {
        if (!trData) {
            trData = GameObject.Find("TH_Meter0001").transform;
        }
        if (!modifier) {
            modifier = GameObject.Find("Vectors").transform;
        }
    }
    public void RunTest()
    {
        float parsedFloat = float.Parse(parseTest);
        string fs = floatData.ToString();
        string parsedFs = float.Parse(fs).ToString();

        string v2s = v2Data.ToString(formatter);
        string v3s = v3Data.ToString(formatter);
        string qs = qData.ToString(formatter);
        string trs = 
            v3s + "," +
            v3s + "," +
            v3s;
        string mods = TDBridge.serializeTransform(modifier);

        print("parsed float:" + parsedFloat);
        print("length:" + stringlengthTest.Length);
        print("float string: " + fs + " Length: " + fs.Length + " TypeIndex: " + TDBridge.varType.IndexOf(floatData.GetType()) );
        print("Parsed float: " + parsedFs);

        print("v2  string: " + v2s + " Length: " + v2s.Length + " TypeIndex: " + TDBridge.varType.IndexOf(v2Data.GetType()) );
        print("v3  string: " + v3s + " Length: " + v3s.Length + " TypeIndex: " + TDBridge.varType.IndexOf(v3Data.GetType()) );
        print("q  string: " + qs + " Length: " + qs.Length + " TypeIndex: " + TDBridge.varType.IndexOf(qData.GetType()) );
        print("tr string: " + trs + " Length: " + trs.Length + " TypeIndex: " + TDBridge.varType.IndexOf(trData.GetType()) );
        
        print("modifer: " + mods);
        print("transform: " + TDBridge.serializeTransform(trData));
        print("result transform:" + TDBridge.serializeTransform(TDBridge.ParseTransform(mods, trData)));
        // print("result ref transform:" + TDBridge.serializeTransform(TDBridge.ParseTransform(mods, ref trData)));

    }
}
