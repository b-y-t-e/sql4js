using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public static class ObjectExtensions
{
    public static bool Equals2(this object Obj1, object Obj2)
    {
        return (Obj1 == null && Obj2 == null) || (Obj1 != null && Obj2 != null && Obj1.Equals(Obj2));
    }
}
