using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RotatorPosser : MonoBehaviour
{

    public Vector3 rotDir;
    public bool switchRot;
    public float animationMult;
    float time;

    public AnimationCurve rotateZ;
    public AnimationCurve transformX;
    public AnimationCurve transformY;
    Vector3 startpos;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(startpos.x + transformX.Evaluate(time), startpos.y + transformY.Evaluate(time), startpos.z);
        time += Time.deltaTime * animationMult;
        time = Mathf.Clamp01(time);
        if(time >= 1)
        {
            time = 0;
            if (switchRot)
                {
                rotDir *= -1;
                }
        }
        transform.Rotate(rotDir);
    }
}
