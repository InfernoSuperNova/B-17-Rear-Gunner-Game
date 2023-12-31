using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class Detachment
{
    public static void DetachChildrenRecursive(Transform obj)
    {
        DetachSelf(obj);

        //we want to make a new list of the children so that we aren't losing track of items
        var collected = new List<Transform>();
        foreach (Transform child in obj)
        {
            collected.Add(child);
        }

        foreach (Transform child in collected)
        {

            DetachChildrenRecursive(child);

        }



    }
    public static void DetachSelf(Transform obj)
    {
        Quaternion rotation = obj.rotation;
        Vector3 scale = obj.lossyScale;
        obj.SetParent(null, true);
        //check if it already has a rigidbody
        if (obj.GetComponent<Rigidbody>() == null)
        {
            obj.AddComponent<Rigidbody>();
        }
        
        obj.localScale = scale;
        obj.rotation = rotation;
        
        var rigidbody = obj.GetComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var force = RandomDetachmentForce.RandomForce();
        rigidbody.AddForce(force);
        var torque = RandomDetachmentForce.RandomTorque();
        rigidbody.AddTorque(torque.x, torque.y, torque.z);

        //add destroyaftertime script
        var deleteAfterFrames = obj.gameObject.AddComponent<DeleteAfterFrames>();
        deleteAfterFrames.framesToLive = 600;
    }
    public static void DetachParent(Transform obj)
    {
        DetachChildrenRecursive(obj.parent);
    }
}
