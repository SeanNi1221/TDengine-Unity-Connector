using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean.Bridge;
public class DataManager : MonoBehaviour
{
    public TH_Meter[] meterArray;
    public List<TH_Meter> meterList;
    void Start()
    {
        // TDBridge.CreateSTable<TH_Meter>();
        TDBridge.CreateTableUsing(meterList.ToArray());
        // TDBridge.CreateTableUsing(meterArray[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
