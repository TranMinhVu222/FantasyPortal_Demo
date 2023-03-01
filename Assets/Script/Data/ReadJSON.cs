using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using Firebase;
using Firebase.Extensions;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ReadJSON : MonoBehaviour
{
    public const string SaveDirectory = "/SaveData";
    public const string FileName = "PurchaseMetaData.sav";
    private string fileDataTemp, fullPath;
    private string SceneGameToLoadAB;
    public static string[] scene;
    private double progress, totalBytes, bytes;
    public Text downloadingText, progressPercent;
    
    private FirebaseStorage storage;
    private StorageReference storageReference;

    static AssetBundle assetBundle;
    
    public Image progressBar;
    public GameObject loadingPanel, warningPanel,FTUE;
    

    public static UpgradeList upgradeList = new UpgradeList();
    public static UpgradeList tempUpgradeList = new UpgradeList();
    
    public double temp, totalByteDownload, byteDownloaded, saveByteDownloaded;

    public List<double> sizeList = new List<double>();

    public string urlDownload;
    // Start is called before the first frame update
    [System.Serializable]
    public class Upgrade
    {
        public int time;
        public int star;
        public int mana;
        public String version;
    }
    [System.Serializable]
    public class UpgradeList
    {
        public Upgrade[] upgrade;
    }
    
    public enum TypeOfAssetBundle
    {
        SCENE,
        AUDIOCLIP,
        PREFAB,
        MATERIAL
    }

    private TypeOfAssetBundle stateAssetBundle = TypeOfAssetBundle.SCENE;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Complete Menu FTUE"))
        {
            PlayerPrefs.SetInt("Complete Menu FTUE",0);
        }
        loadingPanel.SetActive(true);
        Time.timeScale = 1;
    }

    void Start()
    {
        
        Firebase.AppOptions appOptions = new Firebase.AppOptions();
        appOptions.ApiKey = "AIzaSyAB4Jf6k0ILTNh-Q-1GZNQhBtDH_pVJpcU";
        appOptions.AppId = "1:747740940142:android:74b129372bb90e70c2a37f";
        appOptions.MessageSenderId = "";
        appOptions.ProjectId = "fantasy-portal-92666532";
        appOptions.StorageBucket = "fantasy-portal-92666532.appspot.com";
    
        var app = Firebase.FirebaseApp.Create( appOptions );
        
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        //get reference of .json
        StorageReference fileJSON = storageReference.Child("PurchaseDatas.json");
        // Get the download link of file
             fileJSON.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
             {
                 if (!task.IsFaulted && !task.IsCanceled)
                 {
                     StartCoroutine(LoadFileJSON(Convert.ToString(task.Result)));
                 }
                 else
                 {
                     fullPath = Application.persistentDataPath + SaveDirectory + FileName;
                     if (File.Exists(fullPath))
                     {
                         string json = File.ReadAllText(fullPath);
                         upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                         if (!File.Exists(Application.persistentDataPath + "/AB/scenes"))
                         {
                             warningPanel.SetActive(true);
                         }
                         else
                         {
                             StartCoroutine(FinishLoading());    
                         }
                     }
                     else if(!File.Exists(fullPath))
                     {
                         warningPanel.SetActive(true);
                     }
                     else if(!File.Exists(Application.persistentDataPath + "/AB/scenes"))
                     {
                         warningPanel.SetActive(true);
                     }
                 }
             }
         );
        DownloadMultipleFileAssetBundle();
    }
    private IEnumerator LoadFileJSON(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            progress = Math.Round(unityWebRequest.downloadProgress * 100f,2);
            Debug.Log(progress +"%");
            bytes = Math.Round(unityWebRequest.downloadedBytes/1024f,2);
            Debug.Log(bytes + "bytes");
            if (progress > 0)
            {
                totalBytes = Math.Round((bytes * 100f / progress), 2);
                downloadingText.text = "Downloading "+ bytes + "KB" + "/" + totalBytes + "KB";
            }
            progressBar.fillAmount = unityWebRequest.downloadProgress;
            progressPercent.text = progress + "%";
            yield return null;
        }

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + unityWebRequest.error);
                warningPanel.SetActive(true);
                Debug.Log("khong co mang");
            }
            else
            {
                fileDataTemp = unityWebRequest.downloadHandler.text;
                var dir = Application.persistentDataPath + SaveDirectory;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                tempUpgradeList = JsonUtility.FromJson<UpgradeList>(fileDataTemp);
                GUIUtility.systemCopyBuffer = dir;
                fullPath = Application.persistentDataPath + SaveDirectory + FileName;
                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(dir + FileName, fileDataTemp);
                    string json = File.ReadAllText(fullPath);
                    upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                    if (!PlayerPrefs.HasKey("Present Version"))
                    {
                        PlayerPrefs.SetString("Present Version", upgradeList.upgrade[0].version);
                    }
                }
                else
                {
                    if (!PlayerPrefs.HasKey("Present Version"))
                    {
                        File.WriteAllText(dir + FileName, fileDataTemp);
                        string json = File.ReadAllText(fullPath);
                        upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                        PlayerPrefs.SetString("Present Version", upgradeList.upgrade[0].version);
                    }
                    switch (CompareVersion(tempUpgradeList.upgrade[0].version,PlayerPrefs.GetString("Present Version")))
                    {
                        case 1:
                            File.WriteAllText(dir + FileName, fileDataTemp);
                            string json = File.ReadAllText(fullPath);
                            upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                            PlayerPrefs.SetString("Present Version", upgradeList.upgrade[0].version);
                            break;
                        case -1:
                            json = File.ReadAllText(fullPath);
                            upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                            break;
                        case 0:
                            json = File.ReadAllText(fullPath);
                            upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                            break;
                        default:
                            break;
                    }
                }
                StartCoroutine(DelayLoadAssetBundle());
            }
        }
    }
    
    private void DownLoadAsset()
    {
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        //get reference of assetbundle
        StorageReference fileAssetBundle = storageReference.Child("scenebundle");
        // Get the download link of file
        fileAssetBundle.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadFileAssetBundle(Convert.ToString(task.Result)));
            }
            else
            {
                if (!Directory.Exists(Application.persistentDataPath + "/AB"))
                {
                    warningPanel.SetActive(true);
                }
                else
                {
                    StartCoroutine(FinishLoading());
                }
            }
        });
    }
    
    //TODO: Get size before download multiple files ------------------------------------------------------

    private void DownloadMultipleFileAssetBundle()
    {
        string[] bundleArray = new string[] {"scenebundle","audioclip","energybundle","materialbundle"};
        for (int i = 0; i < bundleArray.Length; i++)
        {
            //initialize storage reference
            storage = FirebaseStorage.DefaultInstance;
            storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
            //get reference of assetbundle
            StorageReference multipleFileAssetBundle = storageReference.Child(bundleArray[i]);
            // Get the download link of file
            multipleFileAssetBundle.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    StartCoroutine(GetFileSize(Convert.ToString(task.Result), 
                        (size) =>
                            {
                                temp += size;
                                sizeList.Add(temp);
                                Debug.Log("total size trong GetFileSize " + temp); //Has got total size to be downloaded in all assetbundle
                            },
                        (byteAssetBundles) =>
                            {
                                Debug.Log("dung luong tai " + multipleFileAssetBundle + ": " +" "+ byteAssetBundles);
                            }
                        ));
                }
                else
                {
                    Debug.LogWarning("LOI");
                }
            });
        }
    }
    
    IEnumerator GetFileSize(string url, Action<double> results, Action<double>byteAssetBundles)
    {
        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();
        string size =  uwr.GetResponseHeader("Content-Length");
        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error While Getting Length: " + uwr.error);
            if (results != null)
                results(-1);

            yield return null;
        }
        else
        {
            if (results != null)
                results(Math.Round((double)Convert.ToInt64(size) / 1048576,2));
        }
        
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            switch (stateAssetBundle)
            {
                case TypeOfAssetBundle.SCENE:
                    urlDownload = "scenebundle";
                    DownLoadedAssetBundle(urlDownload);
                    break;
                case TypeOfAssetBundle.PREFAB:
                    urlDownload = "energybundle";
                    DownLoadedAssetBundle(urlDownload);
                    break;
                case TypeOfAssetBundle.MATERIAL:
                    urlDownload = "materialbundle";
                    DownLoadedAssetBundle(urlDownload);
                    break;
                case TypeOfAssetBundle.AUDIOCLIP:
                    urlDownload = "audioclip";
                    DownLoadedAssetBundle(urlDownload);
                    break;
                default:
                    break;
            }
            UnityWebRequest unityWebRequests = UnityWebRequest.Get(url);
            unityWebRequests.SendWebRequest();
            while (!unityWebRequests.isDone)
            {
                totalByteDownload = Math.Round(unityWebRequests.downloadedBytes / 1048576f, 2);
                byteAssetBundles(totalByteDownload);
                yield return null;
            }

            if (unityWebRequests.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("vua lay xong progress cua: "+unityWebRequests.url);
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error: " + unityWebRequests.error);
                    Debug.Log("khong co mang");
                }
            }
        }
    }

    private void DownLoadedAssetBundle(string urlDownload)
    {
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        //get reference of assetbundle
        StorageReference fileAssetBundle = storageReference.Child(urlDownload);
        // Get the download link of file
        fileAssetBundle.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {

            }
            else
            {

            }
        });
    }
    
    

    //TODO: Finish check size asset bundle before download ---------------------------------------------------------
    IEnumerator LoadFileAssetBundle(string url)
    {
        string urlDownload = url;
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(urlDownload);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            yield return null;
        }

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                warningPanel.SetActive(true);
            }
            else
            {
                if (!Directory.Exists(Application.persistentDataPath + "/AB"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/AB");
                }

                if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/scenes")))
                {
                    Uri uri = new Uri(urlDownload);

                    WebClient client = new WebClient();

                    client.DownloadProgressChanged += Client_DownloadProgressChanged;

                    client.DownloadFileAsync(uri, Application.persistentDataPath + "/AB/scenes");

                    while (client.IsBusy)
                        yield return null;
                    Debug.Log("downloading...");
                }

                try
                {
                    if (assetBundle != null)
                    {
                        Debug.Log("da co assetbundle");
                        StartCoroutine(FinishLoading());
                    }
                    else
                    {
                        assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenes"));
                        scene = assetBundle.GetAllScenePaths();
                        Debug.Log("Scene.Lenght: " + scene.Length);
                        foreach (string sceneName in scene)
                        {
                            SceneGameToLoadAB = Path.GetFileNameWithoutExtension(sceneName).ToString();
                            Debug.Log("SceneNameInPath(foreach):: " + Path.GetFileNameWithoutExtension(sceneName));
                        }
                        StartCoroutine(FinishLoading());
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    // File.Delete(Application.persistentDataPath + "/AB/scenes");
                    // StartCoroutine(DownLoadAsset());
                }
            }
        }
    }
    
    //Create your ProgressChanged "Listener"
    private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //Show download progress
        Debug.Log(" " + Math.Round(e.BytesReceived/1048576f,2) + "/"+ Math.Round(e.TotalBytesToReceive/1048576f,2));
        
        progress = Math.Round(e.BytesReceived * 100f / e.TotalBytesToReceive,2);
        progressBar.fillAmount = (float)Math.Round(progress/100f,0);

        progressPercent.text = progress + "%";
        downloadingText.text = "Downloading "+ Math.Round(e.BytesReceived/1048576f,2) + "MB" + "/" + Math.Round(e.TotalBytesToReceive/1048576f,2) + "MB";
    }
    public int CompareVersion(string version1, string version2)
    {
        string[] s1 = version1.Split('.'); 
        string[] s2 = version2.Split('.');

        int maxLenght = Math.Max(s1.Length, s2.Length);
        for (int i = 0; i < maxLenght; i++)
        {
            int v1 = i < s1.Length ? int.Parse(s1[i]) : 0;
            int v2 = i < s2.Length ? int.Parse(s2[i]) : 0;
            int compare = v1.CompareTo(v2);
            if (compare != 0)
            {
                return compare;
            }
        }
        return 0;
    }

    // [NotNull]
    IEnumerator DelayLoadAssetBundle()
    {
        progressBar.fillAmount = 1f;
        progressPercent.text = "100%";
        yield return new WaitForSeconds(1f);
        DownLoadAsset();
    }
    IEnumerator FinishLoading()
    {
        yield return new WaitForSeconds(0);
        if (!PlayerPrefs.HasKey("Completed FTUE") || PlayerPrefs.GetInt("Completed FTUE") == 0)
        {
            SceneManager.LoadScene(1);
            FTUE.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Complete Menu FTUE") == 1)
        {
            FTUE.SetActive(false);
        }
        loadingPanel.SetActive(false);
    }

    public static void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(assetBundle.GetAllScenePaths()[index]);
    }
}