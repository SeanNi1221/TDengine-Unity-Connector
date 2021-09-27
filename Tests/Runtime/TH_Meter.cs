using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;

[ExecuteInEditMode]
public class TH_Meter : MonoBehaviour
{
    [DataTag]
    public string id = "1000265";
    [DataTag(20)]
    new public string name = "温湿度传感器";
    [DataTag(40)]
    public bin location = "Beijing";
    [DataField]
    public float temperature;
    [DataField]
    public float humidity;
    [DataField(200)]
    public string string_data = "This Is A String Field.";
    [DataField]
    public Transform placement;
    void Reset()
    {
        placement = transform;
    }
    void Start()
    {

        // TDBridge.CreateTableUsing(this);
        // TDBridge.CreateTable(this);
        // TDBridge.CreateTable<TH_Meter>();
        // TH_Meter meter1 = new
    }
}
