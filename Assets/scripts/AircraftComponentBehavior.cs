using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AircraftComponentBehavior : MonoBehaviour
{
    public float hp = 5000;
    private bool destroyed = false;


    public static GameObject bullet;
    public Enemy optionalTrigger;
    // Start is called before the first frame update
    void Start()
    {

        //get Enemy from parent
        optionalTrigger = transform.parent.gameObject.GetComponent<Enemy>();
        bullet = Resources.Load<GameObject>("Bullet");

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    //check flags of other gameobject
    //    if (other.gameObject.CompareTag("Bullet"))
    //    {
    //        Hit(5);
    //    }

        
    //}
    public void Hit(float damage)
    {
        if (destroyed) return;
        hp = hp - damage;
        //Debug.Log("Damaged component " + gameObject.name + " for " + damage + " damage. " + hp + " hp remaining!");


        if (hp >= 0) return;
        destroyed = true;
        
        Debug.Log("Part " + gameObject.name + " destroyed!");
        if (optionalTrigger != null)
        {
            optionalTrigger.AircraftDestroy();
        }
        if (CompareTag("Trigger detach on parent"))
        {

            Detachment.DetachParent(transform);
        }
        else
        {
            Detachment.DetachChildrenRecursive(transform);

        }
    }
}
