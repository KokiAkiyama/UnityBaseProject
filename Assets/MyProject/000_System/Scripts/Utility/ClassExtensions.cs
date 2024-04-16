using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentLazy<T>(this Component self,ref T value)
    {
        if(value == null)  value = self.GetComponent<T>();
        return value;
    }
}

public static class MyUnityObjectExtensions
{
    public static bool IsUnityNull(this object obj)
    {
        return obj == null || (obj is UnityEngine.Object &&
            (UnityEngine.Object)obj == null);
    }
}
