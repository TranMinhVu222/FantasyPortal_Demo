using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using Firebase;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class ReadJSON : MonoBehaviour
{
    public const string SaveDirectory = "/SaveData";
    public const string FileName = "PurchaseMetaData.sav";
    private string fileDataTemp, fullPath;
    public static string sceneGameToLoadAB;
    private double progress, totalBytes, bytes;
    public int count;
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
    public List<string> fileToUpdatedList = new List<string>{"null"};
    public string[] bundleArray = {"scenebundle","audioclip","energybundle","materialbundle"};

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
                         for (int i = 0; i < bundleArray.Length; i++)
                         {
                             if (!File.Exists(Application.persistentDataPath + "/AB/" + bundleArray[i]))
                             {
                                 warningPanel.SetActive(true);
                             }
                             else
                             {
                                 AssetBundleManager.AssetBundleAvailable();
                                 StartCoroutine(FinishLoading());
                             }    
                         }
                     }
                     else if(!File.Exists(fullPath))
                     {
                         warningPanel.SetActive(true);
                     }
                     else
                     {
                         for (int i = 0; i < bundleArray.Length; i++)
                         {
                             if (!File.Exists(Application.persistentDataPath + "/AB/" + bundleArray[i]))
                             {
                                 warningPanel.SetActive(true);
                             }
                         }
                     }
                 }
             }
         );
    }
    private IEnumerator LoadFileJSON(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            progress = Math.Round(unityWebRequest.downloadProgress * 100f,2);
            // Debug.Log(progress +"%");
            bytes = Math.Round(unityWebRequest.downloadedBytes/1024f,2);
            // Debug.Log(bytes + "bytes");
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
         
            // Tai xong va khong co mang
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + unityWebRequest.error);
                warningPanel.SetActive(true);
            }
            // tai xong va co ket noi mang
            else
            {
                fileDataTemp = unityWebRequest.downloadHandler.text;
                var dir = Application.persistentDataPath + SaveDirectory;
                
                if (!Directory.Exists(dir)) // CHECK => lan dau tai file JSON nen can tao thu muc de luu
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
                else // CHECK => da tai File JSON truoc do roi
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
                // Da doc duoc file JSON -> Tai assetbundle
                StartCoroutine(DelayLoadAssetBundle());
            }
        }
    }
    
    //TODO: Get size before download multiple files ------------------------------------------------------

    private void DownloadMultipleFileAssetBundle()
    {
        for (int i = 0; i < bundleArray.Length; i++)
        {
            //Check => chua co file Asset bundle
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "AB/" + bundleArray[i])))
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
                                // Debug.Log("total size trong GetFileSize " + temp); //Has got total size to be downloaded in all assetbundle
                            },
                            bundleArray, count += 1
                        ));
                    }
                    else
                    {
                        warningPanel.SetActive(true);
                    }
                });    
            }
        }
    }
    
    IEnumerator GetFileSize(string url, Action<double> results, string[] bundleArray,int order)
    {
        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();
        string size =  uwr.GetResponseHeader("Content-Length");
        string date = uwr.GetResponseHeader("Last-Modified");
        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "AB/" + bundleArray[order - 1])))
            {
                warningPanel.SetActive(true);
            }
            else
            {
                AssetBundleManager.AssetBundleAvailable();
                StartCoroutine(FinishLoading());
            }
            Debug.Log("Error While Getting Length: " + uwr.error);
            if (results != null)
                results(-1);
            yield return null;
        }
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            if (results != null)
            {
                results(Math.Round((double)Convert.ToInt64(size) / 1048576, 2));
            }
            CheckVersion(bundleArray, date,order-1);
        }
        else
        {
            AssetBundleManager.AssetBundleAvailable();
            StartCoroutine(FinishLoading());
        }
    }
    
    private void SaveMultipleAssetBundle(string urlDownload)
    {
        Debug.Log(urlDownload);
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
                StartCoroutine(DownLoadedMultipleFileAssetBundle(Convert.ToString(task.Result), urlDownload));
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
    
    IEnumerator DownLoadedMultipleFileAssetBundle(string url, string nameFile)
    {
        Debug.Log(nameFile +" co tai ");
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

                if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/" + nameFile)))
                {
                    Debug.Log("check co tai file " + nameFile);
                    Uri uri = new Uri(urlDownload);

                    WebClient client = new WebClient();
                    
                    client.DownloadProgressChanged += DownloadProgressChanged;
                   
                    client.DownloadFileAsync(uri, Application.persistentDataPath + "/AB/"+ nameFile);

                    while (client.IsBusy)
                        yield return null;
                }
                
                try
                {
                    AssetBundleManager.UseAssetBundle(urlDownload);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    warningPanel.gameObject.SetActive(true);
                }
            }
        }
    }
    
    //Create your ProgressChanged "Listener"
    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //Show download progress
        Debug.Log(" " + Math.Round(e.BytesReceived/1048576f,2) + "/"+ Math.Round(e.TotalBytesToReceive/1048576f,2));
        
        progress = Math.Round(e.BytesReceived * 100f / e.TotalBytesToReceive,2);
        progressBar.fillAmount = (float)Math.Round(progress/100f,0);
        
        progressPercent.text = progress + "%";
        downloadingText.text = "Downloading "+ Math.Round(e.BytesReceived/1048576f,2) + "MB" + "/" + Math.Round(e.TotalBytesToReceive/1048576f,2) + "MB";
    }
    
    //TODO: Finish check size asset bundle before download ---------------------------------------------------------
    
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
        yield return new WaitForSeconds(0.5f);
        DownloadMultipleFileAssetBundle();
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

    public void CheckVersion(string[] nameAssetBundle, string date,int i)
    {
        string name = nameAssetBundle[i];
        switch (name)
        {
            case "scenebundle":
                if (!PlayerPrefs.HasKey("date scenebundle")) 
                {
                    PlayerPrefs.SetString("date scenebundle", date);
                    SaveMultipleAssetBundle(name);
                }
                else
                {
                    if (CompareDate(PlayerPrefs.GetString("date scenebundle"), date))
                    {
                        fileToUpdatedList.Add(name);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/" + name)))
                        {
                            File.Delete(Application.persistentDataPath + "/AB/"+name);    
                        }
                        PlayerPrefs.SetString("date scenebundle", date);
                        SaveMultipleAssetBundle(name);
                    }
                    else
                    {
                        Debug.Log("khong co file can update ten " + name );
                    }
                } 
                break;
            case "audioclip":
                if (!PlayerPrefs.HasKey("date audioclip"))
                {
                    PlayerPrefs.SetString("date audioclip", date);
                    SaveMultipleAssetBundle(name);
                }
                else
                {
                    if (CompareDate(PlayerPrefs.GetString("date audioclip"), date))
                    {
                        fileToUpdatedList.Add(name);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/" + name)))
                        {
                            File.Delete(Application.persistentDataPath + "/AB/"+name);    
                        }
                        PlayerPrefs.SetString("date audioclip", date);
                        SaveMultipleAssetBundle(name);
                    }
                    else
                    {
                        Debug.Log("khong co file can update ten " + name );
                    }
                }
                break;
            case "energybundle":
                if (!PlayerPrefs.HasKey("date energybundle"))
                {
                    PlayerPrefs.SetString("date energybundle", date);
                    SaveMultipleAssetBundle(name);
                }
                else
                {
                    if (CompareDate(PlayerPrefs.GetString("date energybundle"), date))
                    {
                        fileToUpdatedList.Add(name);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/" + name)))
                        {
                            File.Delete(Application.persistentDataPath + "/AB/"+name);    
                        }
                        PlayerPrefs.SetString("date energybundle", date);
                        SaveMultipleAssetBundle(name);
                    }
                    else
                    {
                        Debug.Log("khong co file can update ten " + name );
                    }
                }
                break;
            case "materialbundle":
                if (!PlayerPrefs.HasKey("date materialbundle"))
                {
                    PlayerPrefs.SetString("date materialbundle", date);
                    SaveMultipleAssetBundle(name);
                }
                else
                {
                    if (CompareDate(PlayerPrefs.GetString("date materialbundle"), date))
                    {
                        fileToUpdatedList.Add(name);
                        if (!File.Exists(Path.Combine(Application.persistentDataPath, "AB/" + name)))
                        {
                            File.Delete(Application.persistentDataPath + "/AB/"+name);    
                        }
                        PlayerPrefs.SetString("date materialbundle", date);
                        SaveMultipleAssetBundle(name);
                    }
                    else
                    {
                        Debug.Log("khong co file can update ten " + name );
                    }
                }
                break;
            default:
                return;
        }
    }

    public bool CompareDate(string savedDate, string lastModifiedDate)
    {
        DateTime convertSavedDate = DateTime.Parse(savedDate);
        DateTime convertLastModified = DateTime.Parse(lastModifiedDate);
        if (DateTime.Compare(convertSavedDate,convertLastModified) < 0)
        {
            return true;
        }
        return false;
    }
}