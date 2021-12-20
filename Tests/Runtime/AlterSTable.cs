using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;

public class AlterSTable : MonoBehaviour
{
    [TDField]
    public bool Power = false;
    [TDField]
    public byte field2 = 9;
    [TDField]
    public short field3_renamed = 24;
    [TDField]
    public int field4 = 9898;
    [TDField]
    public double field5 = 986479868478;
    [TDField]
    public float field6 = 56.647f;
    [TDTag]
    public double profit = 5645.9949847;
    [TDTag(21)]
    public bin location = "Tokyo";
    [TDTag]
    public System.DateTime productionTime = System.DateTime.Now;
    [TDTag(100)]
    public string indroduction = "This is a device";
    void OnEnable()
    {

    }
}

