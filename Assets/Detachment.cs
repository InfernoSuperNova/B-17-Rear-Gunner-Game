using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Detachment
{
    public static void DetachChildrenRecursive(Transform obj)
    {

        foreach (Transform child in obj)
        {
            if (child.childCount > 0)
            {
                DetachChildrenRecursive(child);
            }
            //get scale relative to world
            Vector3 scale = child.lossyScale;

            DetachSelf(child);
            child.localScale = scale;
        }



    }
    public static void DetachSelf(Transform obj)
    {
        obj.SetParent(null, false);
        obj.AddComponent<Rigidbody>();
    }
    public static void DetachParent(Transform obj)
    {
        DetachChildrenRecursive(obj.parent);
    }
}
