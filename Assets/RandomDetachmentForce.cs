using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomDetachmentForce
{
    private static float forceRange = 1000;
    private static float torqueRange = 20;
    public static Vector3 RandomForce()
    {
        return new Vector3(Random.Range(-forceRange, forceRange), Random.Range(-forceRange, forceRange), Random.Range(-forceRange, forceRange));
    }
    public static Quaternion RandomTorque()
    {
        return new Quaternion(Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange));
    }
}
