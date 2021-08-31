using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21;

public class AlterSTable : MonoBehaviour
{
    [DataField]
    public bool Power = false;
    [DataField]
    public byte field2 = 9;
    [DataField]
    public short field3_renamed = 24;
    [DataField]
    public int field4 = 9898;
    [DataField]
    public double field5 = 986479868478;
    [DataField]
    public float field6 = 56.647f;
    [DataTag]
    public double profit = 5645.9949847;
    [DataTag(21)]
    public bin location = "Tokyo";
    [DataTag]
    public System.DateTime productionTime = System.DateTime.Now;
    [DataTag(100)]
    public string indroduction = "This is a device";
    void OnEnable()
    {

    }
}

