using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
public class DownloadDataGame : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storageReference;
    public const string SaveDirectory = "/SaveData";
    public const string FileName = "PurchaseMetaData.sav";
    private string fullPath;
    private string fileDataTemp;
    public static UpgradeList upgradeList = new UpgradeList();
    public static UpgradeList tempUpgradeList = new UpgradeList();
    private string sizeJSON;
    private bool chek, checkCompleted;
    private int countCheckVersion, cout, count, countDownloadFile;
    private double subtotalSize,totalSize, progress, totalBytes, bytes, bytesDownloading, temp1, temp2,temp3, progressDownloaded, progressDataText;
    private string dateOfFile, sizeOfFile;
    public Text downloadingText, progressPercent, downloadAssetBundleText;
    public Image progressBar;
    public GameObject loadingPanel, warningPanel, progressBarPar, percentText;
    private bool checkGetSize;
    private string nameFile = "audioclip";
    private string[] fileInFireBaseArray =
        {"audioclip", "prefabbundle", "materialbundle", "texturebundle", "scenebundle"};
    
    private List<string> urlList = new List<string>();
    public List<string> updateFileList = new List<string>();
    public List<string> sizeUpdateList = new List<string>();

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
    
    private void Start()
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
        checkGetSize = true;
        countDownloadFile = 0;
        chek = true;
        // CheckDateAndSizeForLoad(nameFile);
        // RequestDownloadFileJSON(storageReference);
        StorageReference fileJSON = storageReference.Child("PurchaseDatas.json");
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
                        string json = File.ReadAllText(Application.persistentDataPath + SaveDirectory + FileName);
                        upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                        for (int i = 0; i < fileInFireBaseArray.Length; i++)
                        {
                            if (!File.Exists(Application.persistentDataPath + "/File AB/" + fileInFireBaseArray[i]))
                            {
                                chek = false;
                                warningPanel.SetActive(true);
                            }

                            if (i == fileInFireBaseArray.Length - 1 && chek)
                            {
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
                        for (int i = 0; i < fileInFireBaseArray.Length; i++)
                        {
                            if (!File.Exists(Application.persistentDataPath + "/File AB/" + fileInFireBaseArray[i]))
                            {
                                warningPanel.SetActive(true);
                            }
                            if (i == fileInFireBaseArray.Length - 1 && chek)
                            {
                                StartCoroutine(FinishLoading());
                            }
                        }
                    }
                }
            }
        );
    }
    
    private IEnumerator LoadFileJSON(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Head(url);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            yield return null;
        }

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            var dir = Application.persistentDataPath + SaveDirectory;
            string dateJSON = unityWebRequest.GetResponseHeader("Last-Modified");
            sizeJSON = unityWebRequest.GetResponseHeader("Content-Length");
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                // Tai xong va khong co mang
                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Debug.Log("Error: " + unityWebRequest.error);
                    warningPanel.SetActive(true);
                }
                // tai xong va co ket noi mang
                else
                {
                    if (!PlayerPrefs.HasKey("Present Version"))
                    {
                        progressBarPar.SetActive(true);
                        percentText.SetActive(true);
                        PlayerPrefs.SetString("Present Version", dateJSON);
                        StartCoroutine(SaveLoadFileJSON(url));
                    }
                    else
                    {
                        if (CompareDate(PlayerPrefs.GetString("Present Version"),dateJSON))
                        {
                            progressBarPar.SetActive(true);
                            percentText.SetActive(true);
                            StartCoroutine(SaveLoadFileJSON(url));
                        }
                        else
                        {
                            UseFileJSON(unityWebRequest);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SaveLoadFileJSON(string url)
    {
        Debug.Log(url);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            progress = Math.Round(unityWebRequest.downloadProgress * 100f, 2);
            bytes = Math.Round(unityWebRequest.downloadedBytes / 1024f, 2);
            if (progress > 0)
            {
                totalBytes = Math.Round((bytes * 100f / progress), 2);
                downloadingText.text = "Downloading " + bytes + "KB" + "/" + totalBytes + "KB";
            }
            
            progressBar.fillAmount = unityWebRequest.downloadProgress;
            progressPercent.text = progress + "%";
            yield return null;
        }

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            // Tai xong va khong co mang
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Debug.Log("Error: " + unityWebRequest.error);
                warningPanel.SetActive(true);
            }
            // tai xong va co ket noi mang
            else
            {
                progressBar.fillAmount = 1;
                progressPercent.text = "100%";
                downloadingText.text = "Downloading " +  Math.Round((double)Convert.ToInt64(sizeJSON) / 1048576, 2) + "MB" + "/" +  Math.Round((double)Convert.ToInt64(sizeJSON) / 1048576, 2) + "MB";
                UseFileJSON(unityWebRequest);
            }
        }
    }

    private void UseFileJSON(UnityWebRequest unityWebRequest)
    {
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
            fileDataTemp = unityWebRequest.downloadHandler.text;
            File.WriteAllText(dir + FileName, fileDataTemp);
            string json = File.ReadAllText(fullPath);
            upgradeList = JsonUtility.FromJson<UpgradeList>(json);
        }
        else
        {
            // File.WriteAllText(dir + FileName, fileDataTemp);
            string json = File.ReadAllText(fullPath);
            json = File.ReadAllText(fullPath);
            upgradeList = JsonUtility.FromJson<UpgradeList>(json);
        }
        
        CheckDateAndSizeForLoad(nameFile);
    }
    private void CheckDateAndSizeForLoad(string nameFile)
    {
        Debug.Log(nameFile);
        checkGetSize = false;
        switch (nameFile)
        {
            case "audioclip":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile);
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile);
                        }
                    }, "prefabbundle");
               break;
            case "prefabbundle":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile);
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile);
                        }
                    }, "materialbundle");
                break;
            case "materialbundle":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile);
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile);
                        }
                    }, "texturebundle");
                break;
            case "texturebundle":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile);
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile);
                        }
                    }, "scenebundle");
                break;
            case "scenebundle":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile);
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile);
                        }
                    }, "null");
                break;
        }
    }
    
    private void GetDateAndSize(string nameFile, Action<string> dateFile, Action<string> sizeFile, string nextFile)
    {
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        StorageReference files = storageReference.Child(nameFile);
        files.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(DateAndSize(Convert.ToString(task.Result), nameFile,
                    date =>
                    {
                        dateFile(date);
                    }
                    , size =>
                    {
                        sizeFile(size);
                    }
                    ,nextFile));
            }
            else
            {
                if (!File.Exists(Application.persistentDataPath + "/File AB/" + nameFile))
                {
                    warningPanel.SetActive(true);
                }
            }
        });
    }

    IEnumerator DateAndSize(string url, string name, Action<string> date, Action<string> size, string nextFile)
    {
        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();
        if (uwr.isDone)
        {
            date(uwr.GetResponseHeader("Last-Modified"));
            size(uwr.GetResponseHeader("Content-Length"));
            if (nextFile != "null")
            {
                CheckDateAndSizeForLoad(nextFile);    
            }
            else if(nextFile == "null")
            {
                DownloadFiles(0);
            }
        }
    }

    private void SaveDateOrCompareDate(string dateFile, string date)
    {
        if (!PlayerPrefs.HasKey(dateFile))
        {
            PlayerPrefs.SetString(dateFile, date);
            updateFileList.Add(dateFile);
            checkGetSize = true;
        }
        else
        {
            if (CompareDate(PlayerPrefs.GetString(dateFile), date))
            {
                PlayerPrefs.SetString(dateFile, date);
                checkGetSize = true;
                updateFileList.Add(dateFile);
            }
        }
    }

    private void GetSizeFile(string size)
    {
        totalSize += Math.Round((double)Convert.ToInt64(size) / 1048576, 2);
        subtotalSize += Convert.ToInt64(size);
    }

    private void DownloadFiles(int countDownload)
    {
        if (updateFileList.Count == 0)
        {
            StartCoroutine(FinishLoading());
        }
        else
        {
            CheckFile("File AB", updateFileList[countDownload]);
        }
    }

    void CheckFile(string folder, string nameFile)
    {
        //initialize storage reference
        Debug.Log(nameFile);
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        //get reference of assetbundle
        StorageReference fileAssetBundle = storageReference.Child(nameFile);
        // Get the download link of file
        fileAssetBundle.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                if (File.Exists(Path.Combine(Application.persistentDataPath,  folder + "/" + nameFile)))
                {
                    File.Delete(Application.persistentDataPath + "/"+folder+"/"+ nameFile);    
                }
                progressBarPar.SetActive(true);
                percentText.SetActive(true);
                Debug.LogWarning(nameFile);
                StartCoroutine(DownloadFile(Convert.ToString(task.Result),nameFile, folder));
            }
            else
            {
                if (!Directory.Exists(Application.persistentDataPath + "/" + folder))
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
    
    IEnumerator DownloadFile(string url, string nameFile, string nameFolder)
    {
        Debug.LogWarning(url);
        string urlDownload = url;
        if (!Directory.Exists(Application.persistentDataPath + "/"+nameFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/"+nameFolder);
        }
        if (!File.Exists(Path.Combine(Application.persistentDataPath, nameFolder + "/" + nameFile)))
        {
                    
            Uri uri = new Uri(urlDownload);

            WebClient client = new WebClient();
                    
            client.DownloadProgressChanged += DownloadProgressChanged;
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadFileCompleted);
            client.DownloadFileAsync(uri, Application.persistentDataPath + "/" + nameFolder + "/" + nameFile);

            while (client.IsBusy)
            {
                checkCompleted = false;
                yield return null;
            }

            checkCompleted = true;
        }
        // UnityWebRequest unityWebRequest = UnityWebRequest.Get(urlDownload);
        // unityWebRequest.SendWebRequest();
        // while (!unityWebRequest.isDone)
        // {
        //     yield return null;
        // }
        //
        // if (unityWebRequest.result == UnityWebRequest.Result.Success)
        // {
        //     if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
        //         unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        //     {
        //         warningPanel.SetActive(true);
        //     }
        //     else
        //     {
        //         if (!Directory.Exists(Application.persistentDataPath + "/"+nameFolder))
        //         {
        //             Directory.CreateDirectory(Application.persistentDataPath + "/"+nameFolder);
        //         }
        //         if (!File.Exists(Path.Combine(Application.persistentDataPath, nameFolder + "/" + nameFile)))
        //         {
        //             
        //             Uri uri = new Uri(urlDownload);
        //
        //             WebClient client = new WebClient();
        //             
        //             client.DownloadProgressChanged += DownloadProgressChanged;
        //             client.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadFileCompleted);
        //             client.DownloadFileAsync(uri, Application.persistentDataPath + "/" + nameFolder + "/" + nameFile);
        //
        //             while (client.IsBusy)
        //             {
        //                 checkCompleted = false;
        //                 yield return null;
        //             }
        //
        //             checkCompleted = true;
        //         }
        //     }
        // }
    }
    
    void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
    {
            countDownloadFile += 1;
            if (countDownloadFile <= updateFileList.Count - 1)
            {
                temp1 = 0;
                temp2 = 0;
                temp3 = 0;
                DownloadFiles(countDownloadFile);
            }
    }
    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        temp1 = e.BytesReceived;
        if (temp1 > temp2 && e.BytesReceived <= e.TotalBytesToReceive && temp1 > temp3 )
        {
            temp2 = temp1 - temp3;
            temp3 += temp2;
            progress += temp2;
            // Debug.Log(temp1+" "+temp2+" " + temp3 +" "+e.TotalBytesToReceive+" "+ progress+"/"+subtotalSize);
            progressDataText = Math.Round(progress * 100f / subtotalSize,2);
            progressPercent.text = progressDataText + "%";
            downloadingText.text = "Downloading " + Math.Round(progress / 1048576, 2) + "MB" +  "/" +  Math.Round(subtotalSize/ 1048576, 2) + "MB";
            progressBar.fillAmount = (float)Math.Round(progress/subtotalSize,1);
            // Debug.Log(Math.Round(percent,2)+"%");
            if (progressBar.fillAmount== 1)
            {
                LoadAssetBundle();
                StartCoroutine(FinishLoading());
            }
        }
    }

    public void LoadAssetBundle()
    {
        try
        { 
            if (updateFileList.Count == countDownloadFile && checkCompleted)
            {
                for (int i = 0; i < fileInFireBaseArray.Length; i++)
                {
                    AssetBundleManager.Instance.UseAssetBundle(fileInFireBaseArray[i]);
                }
                StartCoroutine(FinishLoading());
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            // warningPanel.gameObject.SetActive(true);
        }
    }
    //ham so sanh date
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
    IEnumerator FinishLoading()
    {
        yield return new WaitForSeconds(0);
        AssetBundleManager.Instance.AssetBundleAvailable();
    }
}