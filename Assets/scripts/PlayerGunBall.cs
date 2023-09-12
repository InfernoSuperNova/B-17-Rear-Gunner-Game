using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerGunBall : MonoBehaviour
{
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float maxSpeed; //deg/sec
    public bool aimbot;
    public Transform Target;
    private GameManager GameManager;
    private GameObject targetedEnemy;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!aimbot)
        {
            float maxSpeed = this.maxSpeed * Time.deltaTime;

            Vector2 target = new Vector2(
                (Target.localRotation.eulerAngles.x + 180) % 360 - 180,
                (Target.localRotation.eulerAngles.y + 180) % 360 - 180
                );

            target.x = Mathf.Clamp(target.x, minY, maxY);
            target.y = Mathf.Clamp(target.y, minX, maxX);


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

            transform.localRotation = Quaternion.Euler(moveX, moveY, 0f);
        }
        else
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
            targetedEnemy = enemyList[closestEnemyIndex].gameObject;

            //get the velocity of the enemy
            Vector3 enemyVelocity = targetedEnemy.GetComponent<Rigidbody>().velocity;
            Vector3 ownVelocity = new Vector3(0, 0, -20);
            float projectileVelocity = 1100f;
            Vector3 relativeVelocity = enemyVelocity - ownVelocity;
            //create a new position for the targeting
            Vector3 targetPosition = targetedEnemy.transform.position + relativeVelocity * (closestEnemyDistance / projectileVelocity);
            //add gravity to the position
            targetPosition -= Physics.gravity * (closestEnemyDistance / projectileVelocity) * (closestEnemyDistance / projectileVelocity) * Time.deltaTime;


            //get the required azimuth and elevation to point at the enemy
            Vector3 targetDir = targetPosition - transform.position;
            float azimuth = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            float elevation = -Mathf.Atan2(targetDir.y, Mathf.Sqrt(targetDir.x * targetDir.x + targetDir.z * targetDir.z)) * Mathf.Rad2Deg;


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


            transform.localRotation = Quaternion.Euler(moveX, moveY, 0f);
        }
    }
}
