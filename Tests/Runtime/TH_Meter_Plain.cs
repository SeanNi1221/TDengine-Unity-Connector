using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.BridgeToTDengine;

public class TH_Meter_Plain
{
    [DataTag]
    public string id = "1000265";
    [DataTag(20)]
    public string name = "温湿度传感器";
    [DataTag(40)]
    public bin location = "Beijing";
    [DataField]
    public float temperature;
    [DataField]
    public float humidity;
    [DataField(40)]
    public string string_data = "This Is A String Field.";
}
