using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAllFX : MonoBehaviour
{
    public List<GameObject> fxObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisableObjects()
    {
        foreach (GameObject obj in fxObjects)
        {
            obj.SetActive(false);
        }
    }
}
