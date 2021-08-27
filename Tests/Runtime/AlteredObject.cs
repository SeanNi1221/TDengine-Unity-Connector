using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21;

public class AlteredObject : MonoBehaviour
{
    // [DataField]
    // public bool field1 = false;
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
    [DataField]
    public double field7;
    [DataField(21)]
    public bin field8 = "binaryContent";
    [DataField]
    public System.DateTime ts9 = System.DateTime.Now;
    [DataField(10)]
    public string field10 = "ncharContent";
    void OnEnable()
    {

    }
}

