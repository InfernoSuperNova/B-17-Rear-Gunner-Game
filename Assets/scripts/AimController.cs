using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    

    public Transform orientation;

    float xRotation;
    float yRotation;

    public GameObject[] guns;
    private GunScript[] gunScripts;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.localRotation = Quaternion.Euler(0, 0, 0f);
        gunScripts = new GunScript[guns.Length];
        for (int i = 0; i < guns.Length; i++)
        {
            gunScripts[i] = guns[i].GetComponent<GunScript>();
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw ("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw ("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        //clamp the rotation
        xRotation = Mathf.Clamp (xRotation, minY, maxY);
        yRotation = Mathf.Clamp (yRotation, minX, maxX);

        transform.localRotation = Quaternion.Euler (xRotation, yRotation, 0f);
        //orientation.localRotation = Quaternion.Euler (0f, targetY, 0f);
        foreach(var gunScript in gunScripts)
        {
            gunScript.shoot = Input.GetMouseButton(0);
        }
    }
}
