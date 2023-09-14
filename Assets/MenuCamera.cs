using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public GameManager gameManager;
    public new Camera camera;
    public float smoothingFactor = 0.1f;
    Vector3 oldPos = new Vector3(0, 0, 0);
    Vector3 newPos = new Vector3(0, 0, 0);
    float lastUpdateTime;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        //get interpolation factor based on time since last update
        float t = (Time.time - lastUpdateTime) / Time.fixedDeltaTime;

        Vector3 pos = Vector3.Lerp(oldPos, newPos, t);
        
        transform.LookAt(pos);

    }
    private void FixedUpdate()
    {
        //chech if newPos is NaN

        lastUpdateTime = Time.time;
        //get the average position of all the aircraft
        Vector3 averagePosition = Vector3.zero;
        foreach (Aircraft aircraft in gameManager.enemies)
        {
            averagePosition += aircraft.transform.position;
        }
        averagePosition /= gameManager.enemies.Count;
        if (float.IsNaN(newPos.x))
        {
            newPos = averagePosition;
        }
        oldPos = newPos;
        newPos = Vector3.Lerp(oldPos, averagePosition, smoothingFactor);
            //averagePosition;
    }
}
