using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean.Bridge;

public class TH_Meter : MonoBehaviour
{
    public string id = "1000265";
    [DataTag]
    new public string name = "温湿度传感器";
    [DataTag]
    public string location = "Beijing";
    [DataField]
    public float temperature;
    [DataField]
    public float humidity;
    public string noData = "this field will not be in database because it does not have a DataTag/DataField attribute.";

    void Start()
    {
        TDBridge.CreateSTable<TH_Meter>();
        TDBridge.Insert(this);
    }
}
