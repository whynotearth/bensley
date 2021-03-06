using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GetLocalTex : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var info = new DirectoryInfo(@"C:\Users\Sander\Pictures\Lightroom Saved Photos");
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo) print(file);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
