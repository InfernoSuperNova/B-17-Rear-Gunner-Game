using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToObject : MonoBehaviour
{
    public GameObject moveTo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTo == null)
        {
            return;
        }
        transform.position = moveTo.transform.position;
    }
}
