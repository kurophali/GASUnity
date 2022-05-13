using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class A
{
    public virtual string GetClassName()
    {
        return this.GetType().Name;
    }
}
