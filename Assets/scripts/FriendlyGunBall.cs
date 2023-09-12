using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FriendlyGunBall : MonoBehaviour
{
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float maxSpeed; //deg/sec
    public float engageDistance;
    public float engageSigma;
    private GameManager GameManager;
    private GameObject targetedEnemy;
    public GameObject velocityReferenceObject;
    private Rigidbody velocityReferenceRB;
    public List<GameObject> guns;
    private List<GunScript> gunScripts;

    // Start is called before the first frame update
    void Start()
    {
        velocityReferenceRB = velocityReferenceObject.GetComponent<Rigidbody>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gunScripts = new List<GunScript>();
        foreach (GameObject gun in guns)
        {
            gunScripts.Add(gun.GetComponent<GunScript>());
        }
    }

    // Update is called once per frame
    void Update()
    {



        //Choose the closest enemy
        List<Aircraft> enemyList = GameManager.enemies;

        int closestEnemyIndex = -1;
        float closestEnemyDistance = float.MaxValue;
        for (int i = 0; i < enemyList.Count; i++)
        {
            var enemy = enemyList[i];
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemyIndex = i;
            }
        }
        if (enemyList.Count == 0)
        {
            return;
        }
        targetedEnemy = enemyList[closestEnemyIndex].gameObject;

        //get the velocity of the enemy
        Vector3 enemyVelocity = targetedEnemy.GetComponent<Rigidbody>().velocity;
        Vector3 ownVelocity = velocityReferenceRB.velocity;
        float projectileVelocity = 1100f;
        Vector3 relativeVelocity = enemyVelocity - ownVelocity;
        //create a new position for the targeting
        Vector3 targetPosition = targetedEnemy.transform.position + relativeVelocity * (closestEnemyDistance / projectileVelocity);
        //add gravity to the position
        //targetPosition -= Physics.gravity * (closestEnemyDistance / projectileVelocity) * (closestEnemyDistance / projectileVelocity) * Time.deltaTime;
        Debug.DrawLine(targetedEnemy.transform.position, targetPosition, Color.green);
        Debug.DrawLine(transform.position, targetPosition, Color.blue);
        //get the required azimuth and elevation to point at the enemy
        Vector3 targetDir = targetPosition - transform.position;






        Vector3 relativeTargetPosition = Quaternion.Inverse(velocityReferenceObject.transform.rotation) * targetDir;
        float offsetDegree = 180f;
        Quaternion offsetRotation = Quaternion.Euler(0f, offsetDegree, 0f);
        relativeTargetPosition = offsetRotation * relativeTargetPosition;
        float azimuth = Mathf.Atan2(relativeTargetPosition.x, relativeTargetPosition.z) * Mathf.Rad2Deg;
        float elevation = -Mathf.Atan2(relativeTargetPosition.y, Mathf.Sqrt(relativeTargetPosition.x * relativeTargetPosition.x + relativeTargetPosition.z * relativeTargetPosition.z)) * Mathf.Rad2Deg;


        float maxSpeed = this.maxSpeed * Time.deltaTime;

        Vector2 target = new Vector2(elevation, azimuth);


        Vector2 current = new Vector2(
            (transform.localRotation.eulerAngles.x + 180) % 360 - 180,
            (transform.localRotation.eulerAngles.y + 180) % 360 - 180
            );


        //normalize the velocities in the two directions
        Vector2 diff = target - current;
        float moveSpeed = Mathf.Min(diff.magnitude, maxSpeed);
        Vector2 moveDir = diff.normalized;

        diff.x = moveDir.x * moveSpeed;
        diff.y = moveDir.y * moveSpeed;


        float moveX = current.x + Mathf.Clamp(diff.x, -maxSpeed, maxSpeed);
        float moveY = current.y + Mathf.Clamp(diff.y, -maxSpeed, maxSpeed);
        moveX = Mathf.Clamp(moveX, minX, maxX);
        moveY = Mathf.Clamp(moveY, minY, maxY);

        transform.localRotation = Quaternion.Euler(moveX, moveY, 0f);
        Vector3 turretForwardVector = transform.forward;
        if (closestEnemyDistance < engageDistance && Vector3.Dot(turretForwardVector, targetDir.normalized) > engageSigma)
        {
            foreach (GunScript gunScript in gunScripts)
            {
                gunScript.shoot = true;
            }
        }
        else
        {
            foreach (GunScript gunScript in gunScripts)
            {
                gunScript.shoot = false;
            }
        }
    }
}
