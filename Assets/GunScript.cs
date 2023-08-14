using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float rateOfFire = 600; //rounds per minute
    public int tracerInterval = 3;
    public GameObject bulletPrefab;
    public bool shoot = false;
    public int chanceOfSkipTracer = 10;

    private int currentBullet = 0;
    private float previousBulletShootTime;
    private float bulletInterval;
    // Start is called before the first frame update
    void Start()
    {
        previousBulletShootTime = Time.time;
        bulletInterval = 60.0f / rateOfFire;
    }
    void FixedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;
        if (shoot && currentTime - previousBulletShootTime > bulletInterval)
        {
            Quaternion bulletAngleOffset = Quaternion.Euler(90, 0, 0);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation * bulletAngleOffset);
            //get the script from the bullet
            BulletBehavior bulletBehavior = newBullet.GetComponent<BulletBehavior>();
            bulletBehavior.bulletDir = transform.forward;
            if (currentBullet % tracerInterval == 0)
            {
                bulletBehavior.tracerEnabled = true;
            }
            int skipBullet = Random.Range(0, 100);
            if (skipBullet < chanceOfSkipTracer)
            {
            }
            else
            {
                currentBullet++;
            }    
            
            previousBulletShootTime = currentTime;
        }
    }
}
