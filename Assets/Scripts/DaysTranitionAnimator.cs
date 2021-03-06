using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaysTranitionAnimator : MonoBehaviour
{
    float time;
    Image[] imgs;
    public AnimationCurve anim;
    public float animTime;
    // Start is called before the first frame update
    void OnEnable()
    {
        time = 0;
        imgs = GetComponentsInChildren<Image>(true);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time = Mathf.Clamp(time, 0, animTime);
        foreach(Image img in imgs)
        {
            img.color = new Color(1, 1, 1, anim.Evaluate(time / animTime));
        }
        if(time == animTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
