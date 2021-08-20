using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean.Bridge;

public class TH_Meter : MonoBehaviour
{
    [DataTag]
    public string id = "1000265";
    [DataTag(4)]
    new public string name = "温湿度传感器";
    [DataTag]
    public string location = "Beijing";
    [DataField]
    public float temperature;
    [DataField]
    public float humidity;
    [DataField]
    public string string_data = "This Is A String Field.";
    public TH_Meter[] meterArray;
    void Start()
    {

        // TDBridge.CreateTableUsing(this);
        // TDBridge.CreateTable(this);
        // TDBridge.CreateTable<TH_Meter>();
        // TH_Meter meter1 = new
    }
}
