using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    [Multiline]
    public string propellerSpeedDescription = "(In degrees per second)";
    public float propellerSpeed = 100.0f;
    
    //store the axis it should spin on
    public Vector3 spinAxis;
    public Vector3 offset;
    private float currentAngle;
    private float nextAngle;
    private double physicsDelta = 0.0;
    // Start is called before the first frame update
    void Start()
    {
        currentAngle = 0.0f;
        nextAngle = 0.0f;
    }
    // Update on physics frames
    private void FixedUpdate()
    {
        physicsDelta = 0;
        //rotate the propeller
        currentAngle = nextAngle;
        nextAngle = nextAngle + propellerSpeed * Time.fixedDeltaTime;
    }
    
    // Update on display frames
    void Update()
    {
        physicsDelta += Time.deltaTime;
        Quaternion currentAngleTransform = Quaternion.Euler(spinAxis * currentAngle + offset);
        Quaternion nextAngleTransform = Quaternion.Euler(spinAxis * nextAngle + offset);
        Quaternion change = Quaternion.Lerp(currentAngleTransform, nextAngleTransform, Time.fixedDeltaTime * (float)physicsDelta);
        //apply rotation change
        transform.localRotation = change;
    }

}
