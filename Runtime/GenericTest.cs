using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GenericTest : MonoBehaviour
{
    public int intOutOfContainer;
    [SerializeField]
    public TypeContainer typeContainer;
    [Serializable]
    public class TypeContainer
    {
        [SerializeField]        
        public string stringInContainer;
        [SerializeField]
        public UnityEngine.Object typeInContainer;
    }
}
