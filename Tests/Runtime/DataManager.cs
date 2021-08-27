using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21;
public class DataManager : MonoBehaviour
{
    public TH_Meter[] meterArray;
    public List<TH_Meter> meterList;
    public List<AlteredObject> alterTest;
    void OnEnable()
    {
        // TDBridge.CreateSTable<TH_Meter>();
        // TDBridge.CreateTableUsing(meterList.ToArray());
        // TDBridge.CreateTableUsing(meterArray[0]);
        // TDBridge.Insert(meterList[0]);
        // TDBridge.CreateTable(alterTest[0]);
        // TDBridge.Insert(alterTest[0]);
        // TDBridge.InsertSpecific(alterTest[0]);
        StartCoroutine(TDBridge.AlterTableOf(alterTest[0]));        
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
