using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AircraftComponentBehavior : MonoBehaviour
{
    public float hp = 5000;
    public bool enemy;
    private bool destroyed = false;
    GameManager GameManager;
    
    public static GameObject bullet;
    public Aircraft optionalTrigger;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //get Aircraft from parent
        optionalTrigger = transform.parent.gameObject.GetComponent<Aircraft>();
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
    public void Hit(float damage, bool playerOwned)
    {
        if (enemy && playerOwned)
        {
            GameManager.EnemyHit();
        }
        
        if (destroyed) return;
        hp = hp - damage;
        //Debug.Log("Damaged component " + gameObject.name + " for " + damage + " damage. " + hp + " hp remaining!");


        if (hp >= 0) return;
        destroyed = true;
        if (enemy && playerOwned)
        {
            GameManager.EnemyDestroyComponent();
        }
        
        if (optionalTrigger != null)
        {
            optionalTrigger.AircraftDestroy();

            if (enemy && playerOwned)
            {
                GameManager.EnemyKillAircraft();
            }
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
