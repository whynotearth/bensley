using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TexBuilder : MonoBehaviour
{

    public List<Texture2D> sourceTextures;
    public int srcX;
    public int srcY;
    public int rt;
    public int imgIndex;
    public Camera cam;
    Material mat;
    public List<string> fileAddress;
    //public List<string> 
    public int imagesToLoad = 800;

    public string imgFolderAddress;
    int index;
    bool doneLoad;
    bool playAnimationFwd;
    public static bool animationPlaying;
    // Start is called before the first frame update
    void Start()
    {
        var info = new DirectoryInfo(@Application.dataPath + "/images/monday/thumbnails");
        Debug.Log(info);
        if (Application.isEditor)
        {
            info = new DirectoryInfo(imgFolderAddress);
        }
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            fileAddress.Add(file.ToString());
        }
        if(fileAddress.Count < imagesToLoad)
        {
            Debug.LogError("Not enough images found at path " + info + " Found: " + fileAddress.Count + " Expected: " + imagesToLoad);
        }
        fileAddress.RemoveRange(imagesToLoad, fileAddress.Count - imagesToLoad);
        cam = Camera.main;
        


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

    void BuildTex()
    {
        Renderer rend = GetComponent<Renderer>();
        Texture2D tgtTexture = new Texture2D(rt * srcX * 2, rt * srcY);
        rend.material.mainTexture = tgtTexture;

        int idx = 0;
        for (int iX = 0; iX < rt * 2; iX++)
        {
            for (int iY = 0; iY < rt; iY++)
            {
                if(sourceTextures[idx].width != sourceTextures[0].width || sourceTextures[idx].height != sourceTextures[0].height)
                {
                Debug.LogError("Image size does not match! Index " + idx + " = " + sourceTextures[idx].width + " * " + sourceTextures[idx].height + " Path: " + fileAddress[idx]);
                }
                Color[] colors = sourceTextures[idx].GetPixels(0);
                tgtTexture.SetPixels(iX * srcX, iY * srcY, srcX, srcY, colors);
                idx++;
            }
        }
        tgtTexture.Apply(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (index < fileAddress.Count)
        {
            sourceTextures.Add(LoadPNG(fileAddress[index]));
            index++;
            return;
        }
        if (!doneLoad)
        {

            srcX = sourceTextures[0].width;
            srcY = sourceTextures[0].height;

            rt = Mathf.RoundToInt(Mathf.Sqrt(sourceTextures.Count * 0.5f));
            BuildTex();
            // Debug.Log(rt);
            doneLoad = true;
        }
 
        if (Input.GetMouseButtonUp(0) && !animationPlaying && !CamRotate.planetIsRotating)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                {
                    Debug.Log(rend);
                    Debug.Log(meshCollider);
                    return;
                }

                Texture2D tex = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;

                Debug.Log(tex);
                Debug.Log(pixelUV);

                imgIndex = GetIndex(pixelUV);
                Debug.Log(imgIndex);
                DisplayTexTest(imgIndex);
                playAnimationFwd = true;
            }
        }
        PlayAnimation();
    }

    int GetIndex(Vector2 uvHit)
    {
        float iXf = uvHit.x / srcX;
        float iYf = uvHit.y / srcY;
        int iX = Mathf.FloorToInt(iXf);
        int iY = Mathf.FloorToInt(iYf);
        Debug.Log("iXf " + iXf + ", iX " + iX);
        Debug.Log("iYf " + iYf + ", iY " + iY);
        int i = ((iX * rt) + iY);

        return i;
    }

    //public Renderer testRend;
    public Image imageDisplay;

    void DisplayTexTest(int imgIndex)
    {
        imageDisplay.sprite = Sprite.Create(sourceTextures[imgIndex], new Rect(0.0f, 0.0f, sourceTextures[imgIndex].width, sourceTextures[imgIndex].height), new Vector2(0.5f, 0.5f), 100.0f);
        //testRend.material.mainTexture = sourceTextures[imgIndex];
 
    }

    public GameObject containerObj;
    public float animTime;
    public float animSpeed;
    public AnimationCurve animCurve;
    public Vector3 startPos;
    public Vector3 endPos;
    

    void PlayAnimation()
    {
        if (playAnimationFwd)
        {
            animTime += Time.deltaTime * animSpeed;
        }
        else
        {
            animTime -= Time.deltaTime * animSpeed;
        }
        animTime = Mathf.Clamp01(animTime);
        containerObj.transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, animCurve.Evaluate(animTime));
        if (animTime != 0)
            animationPlaying = true;
        else animationPlaying = false;
    }

    public void ReverseAnimation()
    {
        playAnimationFwd = false;
    }
}
