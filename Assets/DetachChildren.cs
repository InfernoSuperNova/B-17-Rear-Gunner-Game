using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class DetachChildren
{
    public static void DetachChildrenRecursive(Transform obj)
    {
        foreach (Transform child in obj)
        {
            if (child.childCount > 0)
            {
                DetachChildrenRecursive(child);
            }
            child.DetachChildren();
            child.AddComponent<Rigidbody>();
        }
    }
}
