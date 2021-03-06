using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuoteLooper : MonoBehaviour
{
    public List<Image> quoteObjects;
    public AnimationCurve quoteOpacity;
    float time;
    public float quoteDuration;
    float t;
    int r;
    int or;


    public List<Image> allImgs;
    float transTime;
    float tt;
    public float halfQuoteDuration;
    // Start is called before the first frame update
    void Start()
    {
        r = Random.Range(0, quoteObjects.Count);
        quoteObjects[r].gameObject.SetActive(true);
        or = r;

        halfQuoteDuration = quoteDuration * .5f;

        allImgs.AddRange(GetComponentsInChildren<Image>(true));
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time = Mathf.Clamp(time, 0, quoteDuration);
        t = time / quoteDuration;

        if (time == quoteDuration)
        {
            quoteObjects[r].gameObject.SetActive(false);
            r = Random.Range(0, quoteObjects.Count);
            while(r == or)
            {
                r = Random.Range(0, quoteObjects.Count);
            }
            or = r;
            quoteObjects[r].gameObject.SetActive(true);
            time = 0;
        }
        quoteObjects[r].color = new Color(1, 1, 1, quoteOpacity.Evaluate(t));

        if (TexBuilderWill.doneLoad)
        {
            transTime += Time.deltaTime;
            transTime = Mathf.Clamp(transTime, 0, halfQuoteDuration);
            tt = transTime / halfQuoteDuration;
            foreach(Image img in allImgs)
            {
                img.color = new Color(1, 1, 1, Mathf.Lerp(1, 0 , tt));
            }
            if(transTime == halfQuoteDuration)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}