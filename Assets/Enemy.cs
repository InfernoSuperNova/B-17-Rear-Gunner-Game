using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DetachChildren.DetachChildrenRecursive(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
