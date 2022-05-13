using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestBehaviour : MonoBehaviour
{
    private void Start()
    {
        Type type = typeof(B);
        A a = (A)Activator.CreateInstance(type);
        Debug.Log(a.GetClassName());
    }
}
