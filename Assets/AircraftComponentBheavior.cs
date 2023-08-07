using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftComponentBheavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        // Add your collision handling logic here
    }
}
