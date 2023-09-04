using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using FMODUnity;
public class GunScript : MonoBehaviour
{
    public float rateOfFire = 600; //rounds per minute
    public int tracerInterval = 3;
    public GameObject bulletPrefab;
    public bool shoot = false;
    private bool playingAudio = false;
    public int chanceOfSkipTracer = 10;
    public Material tracer;
    public GameObject muzzleFlash;

    private int currentBullet = 0;
    private float previousBulletShootTime;
    private float bulletInterval;


    private StudioEventEmitter eventEmitter;



    // Start is called before the first frame update
    void Start()
    {
        eventEmitter = GetComponent<StudioEventEmitter>();
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
            bulletBehavior.tracer = tracer;
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

            Instantiate(muzzleFlash, transform.position, transform.rotation);
        }
        UpdateSound();
    }
    void UpdateSound()
    {
        if (shoot && !playingAudio)
        {
            playingAudio = true;
            eventEmitter.Play();
        }
        else if (!shoot && playingAudio)
        {
            playingAudio = false;
            eventEmitter.Stop(); 
        }
    }
}
