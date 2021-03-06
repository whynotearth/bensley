using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class GetWebTex : MonoBehaviour
{
    public GameObject testObj;
    public Material mat;
    public string url;
    public bool getTex;

    public List<string> fileAddress;
    public List<Texture2D> texList;
    int index;

    public TextAsset imageAsset;
    // Start is called before the first frame update
    void Start()
    {
        mat = testObj.GetComponent<Renderer>().material;
        var info = new DirectoryInfo(@"C:\Users\Sander\Desktop\WorldSpinnerTest\Insta");
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            fileAddress.Add(file.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (index < fileAddress.Count)
        {
            mat.mainTexture = LoadPNG(fileAddress[index]);
        }
        index++;
    }
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            mat.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

}
