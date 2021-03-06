
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using System.IO;

public class TexBuilderWill : MonoBehaviour
{

    public List<Texture2D> sourceTextures;
    public int srcX;
    public int srcY;
    public int rt;
    public int imgIndex;
    public Camera cam;
    Material mat;
    public List<ApiImage> FileAddress = new List<ApiImage>();
    //public List<string> 
    public int ImagesToLoad { get; } = 800;
    [SerializeField] private Sprite tempFullSizedImage;
    public string imgFolderAddress;
    public static bool doneLoad;
    bool playAnimationFwd;
    public static bool animationPlaying;

    //public GameObject loadAnimationAcnhor;
    //public Vector3 loadAnimSpinDir;
    public Sprite loadingImg;
    // Start is called before the first frame update

    private static string ImageListUrl = "https://stagingapi.whynot.earth/api/v0/planetcollage/by-tag/monday";

    public class ApiImage
    {
        public string Id { get; set; }

        public string Url { get; set; }
    }

    async void Start()
    {
        cam = Camera.main;

        ApiImage[] apiImages = await GetImageList();
        FileAddress.AddRange(apiImages);
        if (FileAddress.Count < ImagesToLoad)
        {
            Debug.LogWarning($"Not enough images found at path {ImageListUrl}! Found: {FileAddress.Count} Expected: {ImagesToLoad}. Duplicating {ImagesToLoad - FileAddress.Count} addresses to compensate...");
            
            while (FileAddress.Count < ImagesToLoad)
            {
                FileAddress.AddRange(FileAddress);
                if (FileAddress.Count > ImagesToLoad)
                {
                    FileAddress.RemoveRange(ImagesToLoad, FileAddress.Count - ImagesToLoad);
                }
            }
        }

        FileAddress = FileAddress.Take(ImagesToLoad).ToList();
        var textures = await Task.WhenAll(FileAddress.Select(f => LoadPNG(f.Url)));
        sourceTextures.AddRange(textures);
    }
    private void PrintBigFilePaths() {
        Directory.GetFiles("Assets");
    }
    private async Task<byte[]> GetImage(string url)
    {
        using (var webRequest = new UnityWebRequest(url))
        {
            webRequest.method = UnityWebRequest.kHttpVerbGET;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            await webRequest.SendWebRequest();
            return webRequest.downloadHandler.data;
        }
    }

    private async Task<ApiImage[]> GetImageList()
    {
        using (var webRequest = new UnityWebRequest(ImageListUrl))
        {
            webRequest.method = UnityWebRequest.kHttpVerbGET;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            await webRequest.SendWebRequest();
            if (!webRequest.isDone)
            {
                Debug.LogError("NOT ACTUALLY DONE JFC");
            }
            return JsonConvert.DeserializeObject<ApiImage[]>(webRequest.downloadHandler.text);
        }
    }

    //private async Task<byte[]> GetFullSizedImage(string id)
    private async Task<byte[]> GetFullSizedImage(int id)
    {
        ApiImage[] imageInfo;
        var content = JsonConvert.SerializeObject(new {Id = id});
        using (var webRequest = new UnityWebRequest(ImageListUrl))
        //using (var webRequest = new UnityWebRequest($"{ImageListUrl}/full"))
        {
            Debug.Log($"Getting full res from: {ImageListUrl}/full"); //I don't know if this is right..?
            webRequest.method = UnityWebRequest.kHttpVerbGET;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content));
            await webRequest.SendWebRequest();
            imageInfo = JsonConvert.DeserializeObject<ApiImage[]>(webRequest.downloadHandler.text);
        }
        if (id > imageInfo.Length - 1) {
            id = Random.Range(0, imageInfo.Length);
        }
        return await GetImage(imageInfo[id].Url);
    }

    int loadedNum;
    public async Task<Texture2D> LoadPNG(string filePath)
    {
        //Debug.Log($"Loading {filePath}");
        var texture = new Texture2D(2, 2);
        texture.LoadImage(await GetImage(filePath));
        loadedNum++;
        Debug.Log($"Loaded {loadedNum}/{ImagesToLoad} at {filePath}");
        return texture;
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
                if (sourceTextures[idx].width != sourceTextures[0].width || sourceTextures[idx].height != sourceTextures[0].height)
                {
                    Debug.LogError($"Image size does not match! Index {idx} = {sourceTextures[idx].width} * {sourceTextures[idx].height} Path: {FileAddress[idx].Url}");
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
        if (!doneLoad)
        {
            //loadAnimationAcnhor.transform.Rotate(loadAnimSpinDir);
            //return;
        }

        if (sourceTextures.Count == 0 || sourceTextures.Count != ImagesToLoad)
        {
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
            //loadAnimationAcnhor.SetActive(false);
        }

        if (Input.GetMouseButtonUp(0) && !animationPlaying)
        {
            if (CamRotate.planetIsRotating)
            {
                CamRotate.planetIsRotating = false;
            }
            else
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
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

                    //Debug.Log(tex);
                    //Debug.Log(pixelUV);

                    imgIndex = GetIndex(pixelUV);
                    //Debug.Log(imgIndex);
                    DisplayTex(imgIndex);
                    playAnimationFwd = true;
                }
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
        //Debug.Log("iXf " + iXf + ", iX " + iX);
        //Debug.Log("iYf " + iYf + ", iY " + iY);
        int i = ((iX * rt) + iY);

        return i;
    }

    //public Renderer testRend;
    public Image imageDisplay;

  async  void DisplayTex(int imgIndex)
    {
        Texture2D bigTex = new Texture2D(1, 1);
        //bigTex.LoadImage(await GetFullSizedImage(FileAddress[imgIndex].Id));
        bigTex.LoadImage(await GetFullSizedImage(imgIndex));


        imageDisplay.sprite = tempFullSizedImage;
        //imageDisplay.sprite = Sprite.Create(bigTex, new Rect(0.0f, 0.0f, bigTex.width, bigTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        //testRend.material.mainTexture = sourceTextures[imgIndex];

    }

    public GameObject containerObj;
    public float animTime;
    public float animSpeed;
    public AnimationCurve animCurve;
    public Vector3 startPos;
    public Vector3 endPos;
    public Image bgOverlay;
    public float overlayTrans;

    public AudioSource panelDownSrc;

    void PlayAnimation()
    {
        if (playAnimationFwd)
        {
            if (animTime == 0) panelDownSrc.Play();
            animTime += Time.deltaTime * animSpeed;
        }
        else
        {
            animTime -= Time.deltaTime * animSpeed;
        }
        animTime = Mathf.Clamp01(animTime);
        containerObj.transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, animCurve.Evaluate(animTime));
        bgOverlay.color = new Color(bgOverlay.color.r, bgOverlay.color.g, bgOverlay.color.b, animTime * overlayTrans);

        if (animTime != 0)
        {
            animationPlaying = true;
        }
        else
        {
            animationPlaying = false;
            imageDisplay.sprite = loadingImg;
        }
    }

    public void ReverseAnimation()
    {
        playAnimationFwd = false;
    }
}