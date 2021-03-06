using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float dirY;
    public GameObject planetRoot;
    public AnimationCurve spinCurve;
    public float spinTime = 5;
    public float time;
    public float evalTime;
    Vector3 rot = new Vector3(0,0,0);
    float spinSpd;
    public static bool planetIsRotating;
    public float rotateThreshhold;
    public float rotY;
    public bool interactedWith;

    public float interactionTimeout = 10;
    float intrTime;
    float spinUpTime;

    // Start is called before the first frame update
    void Start()
    {
        dirY = .15f;
    }

    float yl;
    // Update is called once per frame
    void Update()
    {
        //intrTime += Time.deltaTime;
        //intrTime = Mathf.Clamp(intrTime, 0, interactionTimeout);
        //if(intrTime == interactionTimeout)
        //{
        //    interactedWith = false;
        //}



        //if (!interactedWith)
        //{
        //    spinUpTime += Time.deltaTime;
        //    spinUpTime = Mathf.Clamp01(spinUpTime);
        //    planetRoot.transform.Rotate(new Vector3(0, 0.15f * spinUpTime, 0));
        //}
        if (Input.GetMouseButton(0) && !TexBuilderWill.animationPlaying)
        {
            dirY = -Input.GetAxis("Mouse X");
            time = 0;
            intrTime = 0;
            spinUpTime = 0;
            interactedWith = true;
            if (dirY > rotateThreshhold || dirY < -rotateThreshhold)
            {
                planetIsRotating = true;

            }
        }
        time += Time.deltaTime;
        time = Mathf.Clamp(time,0, spinTime);
        evalTime = time / spinTime;
        if(dirY>0)yl = Mathf.Lerp(dirY, 1, evalTime);
        else yl = Mathf.Lerp(dirY, -1, evalTime);
        rot.y = yl * spinCurve.Evaluate(evalTime);
        planetRoot.transform.Rotate(rot);
        rotY = rot.y;

        
    }
    void LateUpdate()
    {
        //camX.transform.rotation = Quaternion.Euler(Mathf.Clamp(rotX, 60, 120), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

       // Debug.Log(rotX);
    }
}
