using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sean21.TDengineConnector;
using System;
public class AnyTest : MonoBehaviour
{
    public Tuple<int, string> testTuple;
    public GameObject a;
    public GameObject b;
    public void RunTest()
    {
        Component compA = a.GetComponent<TDChannel>();
        // Debug.Log("compA:" + comp)
        Component compB = b.GetComponent<TDChannel>();
        // compB = compA;
        compB = a.GetComponent<TDChannel>();
        Debug.Log("Done!");
    }
}
