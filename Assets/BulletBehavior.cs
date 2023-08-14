using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float timeToLive = 5.0f;
    public bool tracerEnabled = false;
    public float velocity = 500.0f; //m/s
    public float accuracyStdDev = 0.01f;
    public Vector3 bulletDir = Vector3.zero;
    //store the time it was created
    private float creationTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        creationTime = Time.time;
        //toggle the tracerEnabled effect
        GetComponent<TrailRenderer>().enabled = tracerEnabled;

        //Create a random direction for the bullet, starting at the bulleDir
        Vector3 randomDeviation = new Vector3(
            Random.Range(-accuracyStdDev, accuracyStdDev),
            Random.Range(-accuracyStdDev, accuracyStdDev),
            Random.Range(-accuracyStdDev, accuracyStdDev)
        );
        Vector3 newDir = bulletDir + randomDeviation;
        //Apply the force to reach the target speed with the given mass
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(newDir * velocity * rigidbody.mass / Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        //Destroy the bullet after timeToLive seconds
        if (Time.time - creationTime > timeToLive)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
