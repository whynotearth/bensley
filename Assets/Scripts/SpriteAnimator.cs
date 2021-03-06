using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    SpriteRenderer rend;
    public float animSpd = 1;
    float time;
    int idx;
    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
        rend.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * animSpd;
        time = Mathf.Clamp01(time);
        if(time >= 1)
        {
            idx++;
            if(idx == sprites.Length)
            {
                idx = 0;
            }
            rend.sprite = sprites[idx];
            time = 0;
        }
    }
}
