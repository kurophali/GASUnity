using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class TestBehaviour : MonoBehaviour
{
    protected string thisTypeName;
    private void GetThisClassName()
    {
        thisTypeName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    }
    private void Start()
    {
        GetThisClassName();
    }
}
