using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public int tracerInterval = 5;
    public GameObject bulletPrefab;
    public bool shoot = false;

    private int currentBullet = 0;
    // Start is called before the first frame update
    void Start()
    {
     
    }
    void FixedUpdate()
    {
        //Fire 10 rounds/sec
        if (shoot)
        {
            
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            //get the script from the bullet
            BulletBehavior bulletBehavior = newBullet.GetComponent<BulletBehavior>();
            if (currentBullet % tracerInterval == 0)
            {
                bulletBehavior.tracerEnabled = true;
            }
            currentBullet++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
