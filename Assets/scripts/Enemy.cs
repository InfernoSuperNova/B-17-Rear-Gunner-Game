using FMOD;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    PIDController pitch;
    PIDController roll;
    PIDController yaw;
    private GameObject target;
    private Rigidbody targetRb;
    public bool dogfightMode;
    public float power;
    public float pitchAuthority;
    public float yawAuthority;
    public float rollAuthority;
    public float yawSwitchThreshold;
    public float dragUp;
    public float dragForward;
    public float dragRight;
    public float angularDrag;
    public float liftCoefficient;
    public float speedControlCoefficient;
    public float pitchP;
    public float pitchI;
    public float pitchD;
    public float rollP;
    public float rollI;
    public float rollD;
    public float yawP;
    public float yawI;
    public float yawD;
    public float gunEngageAngle;
    public float gunEngageDistance;
    public GameObject[] guns;
    public float breakawayDistance;
    public float breakawayPointRadius;
    public float breakawayPointDist;
    private GunScript[] gunScripts;
    Rigidbody rb;
    private bool gunsEnabled = true;
    private GameManager GameManager;

    public GameObject destructionEffect;

    bool currentlyShooting = false;
    StudioEventEmitter engineSoundEmitter;
    StudioEventEmitter gunSoundEmitter;
    public EventReference shootSound;
    public EventReference engineSound;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        //find the target in the scene
        if (!dogfightMode)
        {
            target = GameObject.Find("Player");
            targetRb = target.GetComponent<Rigidbody>();
        }
        
        pitch = new PIDController(pitchP, pitchI, pitchD);
        roll = new PIDController(rollP, rollI, rollD);
        yaw = new PIDController(yawP, yawI, yawD);

        gunScripts = new GunScript[guns.Length];
        for (int i = 0; i < guns.Length; i++)
        {
            gunScripts[i] = guns[i].GetComponent<GunScript>();
        }
        InitializeSounds();
    }
    private void InitializeSounds()
    {
        engineSoundEmitter = gameObject.AddComponent<StudioEventEmitter>();
        engineSoundEmitter.EventReference = engineSound;
        engineSoundEmitter.Play();
            
        gunSoundEmitter = gameObject.AddComponent<StudioEventEmitter>();
        gunSoundEmitter.EventReference = shootSound;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 targetDir;
        float speedMultiplier = Vector3.Dot(transform.forward, rb.velocity) * speedControlCoefficient * Time.fixedDeltaTime;
        if (dogfightMode)
        {
            if (target == null || target.tag == "Player")
            {
                List<Enemy> enemyList = GameManager.enemies;
                
                int closestEnemyIndex = -1;
                float closestEnemyDistance = float.MaxValue;
                for (int i = 0; i < enemyList.Count; i++)
                {
                    var enemy = enemyList[i];
                    float distance = Vector3.Distance(enemy.transform.position, transform.position);
                    if (enemy == this) continue;
                    if (distance < closestEnemyDistance)
                    {
                        closestEnemyDistance = distance;
                        closestEnemyIndex = i;
                    }
                }
                if (closestEnemyIndex != -1)
                {
                    target = enemyList[closestEnemyIndex].gameObject;
                }
                else
                {
                    target = GameObject.Find("Player");
                }
                targetRb = target.GetComponent<Rigidbody>();
            }
        }
        //target doesn't have a rigidbody, so we need to calculate its velocity manually



        targetDir = (target.transform.position - transform.position);
        float projectileVelocity = 1100f;
        Vector3 relativeVelocity = targetRb.velocity - rb.velocity;
        //create a new position for the targeting
        Vector3 targetPosition = target.transform.position + relativeVelocity * (targetDir.magnitude / projectileVelocity);

        // Calculate the vector from the enemy to the target
        targetDir = (targetPosition - transform.position).normalized;


        // Calculate the desired roll and pitch angle changes in radians

        float desiredRollAngle = ConvertTo180Range(GetDesiredRollAngle(targetDir));
        
        float desiredPitchAngle = GetDesiredPitchAngle(targetDir);
        float yawAngleChangeDegrees;
        float desiredYawAngle = ConvertTo180Range(GetDesiredYawAngle(targetDir, out yawAngleChangeDegrees));
        roll.SetCurrentValue(ConvertTo180Range(transform.rotation.eulerAngles.z));
        pitch.SetCurrentValue(transform.rotation.eulerAngles.x);
        yaw.SetCurrentValue(ConvertTo180Range(transform.rotation.eulerAngles.y));
        if (Mathf.Abs(yawAngleChangeDegrees) < yawSwitchThreshold && Vector3.Dot(rb.velocity.normalized, targetRb.velocity.normalized) < yawSwitchThreshold)
        {
            desiredRollAngle = 0;
        }
        else
        {
            desiredYawAngle = ConvertTo180Range(transform.rotation.eulerAngles.y);
        }
        
        pitch.SetTargetValue(desiredPitchAngle);
        roll.SetTargetValue(desiredRollAngle);
        yaw.SetTargetValue(desiredYawAngle);

        roll.Update(Time.fixedDeltaTime);
        pitch.Update(Time.fixedDeltaTime);
        yaw.Update(Time.fixedDeltaTime);


        float pitchOut = pitch.GetControlOutput();
        float rollOut = roll.GetControlOutput();
        float yawOut = yaw.GetControlOutput();

        float clampedRoll = Mathf.Clamp(rollOut, -rollAuthority * speedMultiplier, rollAuthority * speedMultiplier);
        float clampedPitch = Mathf.Clamp(pitchOut, -pitchAuthority * speedMultiplier, pitchAuthority * speedMultiplier);
        float clampedYaw = Mathf.Clamp(yawOut, -yawAuthority * speedMultiplier, yawAuthority * speedMultiplier);

        //apply torque
        try { 
        rb.AddRelativeTorque(new Vector3(clampedPitch, clampedYaw, clampedRoll), ForceMode.Impulse);
        }
        catch
        {
            UnityEngine.Debug.LogError("One of these was not a number!");
            UnityEngine.Debug.Log(clampedPitch);
            UnityEngine.Debug.Log(clampedYaw);
            UnityEngine.Debug.Log(clampedRoll);

        }
        //engineSound force
        rb.AddForce(transform.forward * power * Time.fixedDeltaTime, ForceMode.Impulse);

        //calculate drag on all axes
        float dragUpForce = Vector3.Dot(transform.up, rb.velocity) * dragUp;
        float dragForwardForce = Vector3.Dot(transform.forward, rb.velocity) * dragForward;
        float dragRightForce = Vector3.Dot(transform.right, rb.velocity) * dragRight;
        rb.AddForce(dragUpForce * -transform.up * Time.fixedDeltaTime, ForceMode.Impulse);
        rb.AddForce(dragForwardForce * -transform.forward * Time.fixedDeltaTime, ForceMode.Impulse);
        rb.AddForce(dragRightForce * -transform.right * Time.fixedDeltaTime, ForceMode.Impulse);

        //add rotational drag
        rb.AddTorque(-rb.angularVelocity * Time.fixedDeltaTime * angularDrag * Vector3.Dot(transform.forward, rb.velocity), ForceMode.Impulse);

        //Calculate lift

        float liftForce = Vector3.Dot(transform.forward, rb.velocity) * liftCoefficient;
        rb.AddForce(liftForce * transform.up * Time.fixedDeltaTime, ForceMode.Impulse);

        //apply a constant force trying to tilt towards gravity

        ShootGuns(targetDir);
        if (!dogfightMode)
        {
            SwitchBehavior();
        }
        
    }

    private float GetDesiredRollAngle(Vector3 targetDir)
    {
        float rollAngleChange = Mathf.Asin(Vector3.Dot(-transform.right, targetDir));
        float rollAngleChangeDegrees = rollAngleChange * Mathf.Rad2Deg;
        float currentRollAngle = transform.rotation.eulerAngles.z;
        float desiredRollAngle = currentRollAngle + rollAngleChangeDegrees;
        return desiredRollAngle;
    }
    private float GetDesiredPitchAngle(Vector3 targetDir)
    {
        float pitchAngleChange = Mathf.Asin(Vector3.Dot(-transform.up, targetDir));
        float pitchAngleChangeDegrees = pitchAngleChange * Mathf.Rad2Deg;
        float currentPitchAngle = transform.rotation.eulerAngles.x;
        float desiredPitchAngle = currentPitchAngle + pitchAngleChangeDegrees;
        return desiredPitchAngle;
    }

    private float GetDesiredYawAngle(Vector3 targetDir, out float yawAngleChangeDegrees)
    {
        Vector3 cross = Vector3.Cross(transform.forward, targetDir);
        float dot = Vector3.Dot(Vector3.up, cross); // Assuming up is the vertical axis
        float yawAngleChange = Mathf.Asin(dot);
        yawAngleChangeDegrees = yawAngleChange * Mathf.Rad2Deg;
        float currentYawAngle = transform.rotation.eulerAngles.y;
        float desiredYawAngle = currentYawAngle + yawAngleChangeDegrees;
        return desiredYawAngle;
    }

    private void ShootGuns(Vector3 targetDir)
    {
        
        float error = Vector3.Dot(transform.forward, targetDir);
        
        float targetDist = Vector3.Distance(transform.position, target.transform.position);
        if (error > gunEngageAngle && targetDist <= gunEngageDistance)
        {
            if (!gunsEnabled) { return; }
            if (!currentlyShooting)
            {
                gunSoundEmitter.Play();
                currentlyShooting = true;
            }
            
            
            foreach (var gunScript in gunScripts)
            {
                if (gunScript != null)
                {
                    gunScript.shoot = true;
                    
                }
                
            }
        }
        else
        {
            if (currentlyShooting)
            {
                gunSoundEmitter.Stop();
                currentlyShooting = false;
            }
            foreach (var gunScript in gunScripts)
            {
                if (gunScript != null)
                {
                    gunScript.shoot = false;
                    
                }
            }
        }
    }
    private void SwitchBehavior()
    {
        UnityEngine.Debug.DrawLine(transform.position, target.transform.position, Color.red);
        //we want to go to a random breakaway point if we are close to the target
        if (Vector3.Distance(transform.position, target.transform.position) < breakawayDistance)
        {
            //if the target is already a breakaway point
            if (target.tag == "BreakawayPoint")
            {
                Destroy(target);
                gunsEnabled = true;
                UnityEngine.Debug.Log("engaging target!");
                target = GameObject.Find("Player");
                return;
            }
            gunsEnabled = false;
            UnityEngine.Debug.Log("Breaking away!");
            
            //we want to pick a random breakaway point offset behind the target
            Vector3 targetDir = (target.transform.position - transform.position).normalized;
            Vector3 breakawayPoint = target.transform.position + targetDir * -breakawayDistance;
            //and then we want to offset it in a sphere with a random radius
            float randomRadius = Random.Range(0, breakawayPointRadius);
            Vector3 randomOffset = Random.insideUnitSphere * randomRadius;
            breakawayPoint += randomOffset;
            //set the target to the breakaway point
            target = new GameObject("BreakawayPoint");
            target.transform.position = breakawayPoint;
            target.tag = "BreakawayPoint";
        }
    }
    float ConvertTo180Range(float angleIn360)
    {
        return (angleIn360 + 180.0f) % 360.0f - 180.0f;
    }

    public void AircraftDestroy()
    {
        UnityEngine.Debug.Log("Destroyed!");
        gunSoundEmitter.Stop();
        engineSoundEmitter.Stop();
        foreach (var gunScript in gunScripts)
        {
            if (gunScript != null)
            {
                gunScript.shoot = false;
            }
        }
        //instansiate explosion
        GameObject explosion = Instantiate(destructionEffect, transform.position, transform.rotation);
        
    }
}
