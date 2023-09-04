using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterFrames : MonoBehaviour
{
    public float framesToLive = 10;
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        framesToLive -= 1;
        if (framesToLive <= 0)
        {
            Destroy(gameObject);
        }
    }
}
