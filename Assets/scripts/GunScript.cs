using System.Collections;
using System.Collections.Generic;
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
    public GameObject velocityInheritObject;
    private Rigidbody velocityInheritRB;
    public bool useChildAudioEmitter = false;
    public bool playerOwned = false;
    private int currentBullet = 0;
    private float previousBulletShootTime;
    private float bulletInterval;


    private StudioEventEmitter eventEmitter;



    // Start is called before the first frame update
    void Start()
    {
        velocityInheritRB = velocityInheritObject.GetComponent<Rigidbody>();
        if (useChildAudioEmitter)
        {
            eventEmitter = GetComponent<StudioEventEmitter>();
        }
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
            Rigidbody newBulletRB = newBullet.GetComponent<Rigidbody>();
            //check if velocityInheritRB is null
            if (velocityInheritRB == null)
            {
                newBulletRB.velocity = new Vector3(0, 0, 0);
            }
            else
            {
                newBulletRB.velocity = velocityInheritRB.velocity;
            }
            
            //get the script from the bullet
            BulletBehavior bulletBehavior = newBullet.GetComponent<BulletBehavior>();
            bulletBehavior.bulletDir = transform.forward;
            bulletBehavior.tracer = tracer;
            bulletBehavior.playerOwned = playerOwned;
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

            //instantiate as child of gun
            GameObject flash = Instantiate(muzzleFlash, transform.position, transform.rotation);
            //add linearmove script to flash
            MoveToObject flashMoveToObject = flash.AddComponent<MoveToObject>();
            flashMoveToObject.moveTo = gameObject;
        }
        UpdateSound();
    }
    void UpdateSound()
    {
        if (shoot && !playingAudio)
        {
            playingAudio = true;
            if (useChildAudioEmitter)
            {
                eventEmitter.Play();
            }
            
        }
        else if (!shoot && playingAudio)
        {
            playingAudio = false;
            if (useChildAudioEmitter)
            {
                eventEmitter.Stop();
            }
        }
    }
}
