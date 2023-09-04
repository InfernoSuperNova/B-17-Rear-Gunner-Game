using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CopyOrientation : MonoBehaviour
{
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float maxSpeed; //deg/sec

    public Transform Target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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


}
