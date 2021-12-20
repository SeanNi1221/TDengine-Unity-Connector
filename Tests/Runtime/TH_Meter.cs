using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;

[ExecuteInEditMode]
public class TH_Meter : MonoBehaviour
{
    public float test;
    [TDTag]
    public string id = "1000265";
    [TDTag(20)]
    new public string name = "温湿度传感器";
    [TDTag(40)]
    public bin location = "Beijing";
    [TDField]
    public float temperature;
    [TDField]
    public float humidity;
    [TDField(200)]
    public string string_data = "This Is A String Field.";
    [TDField]
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
