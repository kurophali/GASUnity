using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B : A
{
    public override string GetClassName()
    {
        return this.GetType().Name;
    }
}
