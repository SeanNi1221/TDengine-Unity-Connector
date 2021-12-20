using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;

public class TH_Meter_Plain
{
    [TDTag]
    public string id = "1000265";
    [TDTag(20)]
    public string name = "温湿度传感器";
    [TDTag(40)]
    public bin location = "Beijing";
    [TDField]
    public float temperature;
    [TDField]
    public float humidity;
    [TDField(40)]
    public string string_data = "This Is A String Field.";
}
