using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rotator : MonoBehaviour
{

    public Vector3 rotDir;
   
    // Start is called before the first frame update
    void Start()
    {
;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(rotDir);
    }
}
