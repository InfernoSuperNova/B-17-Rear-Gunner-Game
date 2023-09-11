using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAtSpeedForce : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 moveSpeed;
    void Start()
    {
        //apply an impulse based on the mass of the object to get it to move at the desired speed
        GetComponent<Rigidbody>().AddForce(moveSpeed * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody>().AddForce(moveSpeed * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
    }
}
