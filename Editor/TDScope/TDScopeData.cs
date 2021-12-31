using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sean21.TDengineConnector
{
public class TDScopeData : ScriptableObject
{
    [SerializeField]internal List<string> db_names = new List<string>();
}
}