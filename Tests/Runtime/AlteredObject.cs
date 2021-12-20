using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;

public class AlteredObject : MonoBehaviour
{
    // [DataField]
    // public bool field1 = false;
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
    [TDField]
    public double field7;
    [TDField(21)]
    public bin field8 = "binaryContent";
    [TDField]
    public System.DateTime ts9 = System.DateTime.Now;
    [TDField(10)]
    public string field10 = "ncharContent";
    void OnEnable()
    {

    }
}

