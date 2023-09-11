using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float bulletDamage = 5.0f;
    public float timeToLive = 5.0f;
    public bool tracerEnabled = false;
    public float velocity = 800.0f; //m/s
    public float accuracyStdDev = 0.01f;
    public GameObject gameManager;
    private GameManager gameManagerScript;
    public Vector3 bulletDir = Vector3.zero;
    //store the time it was created
    private float creationTime = 0.0f;

    public Material tracer;

    public GameObject impactEffect;

    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        //apply the colour to the bullet's trail component's material's shader colour
        GetComponent<TrailRenderer>().material = tracer;
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
        //multiply by speed
        Vector3 futurePos = transform.position + bulletDir * velocity * Time.fixedDeltaTime;
        float dist = Vector3.Distance(transform.position, futurePos);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, bulletDir, out hit, dist))
        {

            GameObject hitObject = hit.collider.gameObject;
            //Now, someone who isn't lazy would probably have a gamemanager that would store a list of all the objects that can be hit, and then check if the hit object is in that list
            //But I'm lazy, so I'm just going to check if the object has an AircraftComponentBehavior script
            AircraftComponentBehavior aircraftComponentBehavior = hitObject.GetComponent<AircraftComponentBehavior>();
            if (aircraftComponentBehavior != null)
            {
                Instantiate(impactEffect, hit.point, transform.rotation);
                Debug.Log("Hit " + hitObject.name);
                aircraftComponentBehavior.Hit(bulletDamage);
            }

            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    //private void OnTriggerEnter(Collider other)
    //{
        
    //    if (other.gameObject.CompareTag("aircraft"))
    //    {
    //        Destroy(gameObject);
    //        Instantiate(impactEffect, prevPos, transform.rotation);
    //        //Do a call up for hitmarker
    //        Debug.Log(other.gameObject.name);

    //    }
    //}
}
