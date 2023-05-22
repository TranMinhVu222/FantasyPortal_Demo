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
    public GameObject loadingPanel, warningPanel, progressBarPar, percentText, progressBar;
    private bool checkGetSize, checkDownloadJSON;
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
        //Set public access to public
        Firebase.AppOptions appOptions = new Firebase.AppOptions();
        appOptions.ApiKey = "AIzaSyAB4Jf6k0ILTNh-Q-1GZNQhBtDH_pVJpcU";
        appOptions.AppId = "1:747740940142:android:74b129372bb90e70c2a37f";
        appOptions.MessageSenderId = "";
        appOptions.ProjectId = "fantasy-portal-92666532";
        appOptions.StorageBucket = "fantasy-portal-92666532.appspot.com";
        var app = Firebase.FirebaseApp.Create( appOptions );
        // Get a reference to the storage service, using the default Firebase App
        storage = FirebaseStorage.DefaultInstance; 
        // Create a storage reference from our storage service
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        checkGetSize = true;
        countDownloadFile = 0;
        chek = true;
        checkDownloadJSON = false;
        // Tao tham quyen 
        StorageReference fileJSON = storageReference.Child("PurchaseDatas.json");
        // Bat dau download file
        fileJSON.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled) //kiem tra tinh trang mang
                {
                    //co mang
                    StartCoroutine(LoadFileJSON(Convert.ToString(task.Result)));
                }
                else
                {
                    //khong co mang => check file de su dung
                    fullPath = Application.persistentDataPath + SaveDirectory + FileName;
                    if (File.Exists(fullPath))
                    {
                        //đã có file JSON
                        string json = File.ReadAllText(Application.persistentDataPath + SaveDirectory + FileName);
                        upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                        for (int i = 0; i < fileInFireBaseArray.Length; i++)
                        {
                            //kiểm tra đã tồn tại đủ file assetbundle
                            if (!File.Exists(Application.persistentDataPath + "/File AB/" + fileInFireBaseArray[i]))
                            {
                                chek = false;
                                // hiện pop-up cảnh báo không đủ file để vào game
                                warningPanel.SetActive(true);
                            }

                            if (i == fileInFireBaseArray.Length - 1 && chek)
                            {
                                // vào Game
                                StartCoroutine(FinishLoading());
                            }
                        }
                    }
                    else if(!File.Exists(fullPath))
                    {
                        //chưa có file JSON
                        warningPanel.SetActive(true);
                    }
                    else
                    {
                        // Kiểm tra file asetbundle
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
    
    //gửi yêu cầu điều kiện để tải file JSON
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
                    //Lưu thời gian file được tạo trong lần đầu tiên tải
                    if (!PlayerPrefs.HasKey("Present Version")) 
                    {
                        progressBarPar.SetActive(true);
                        percentText.SetActive(true);
                        PlayerPrefs.SetString("Present Version", dateJSON); //Lưu thời gian file
                        StartCoroutine(SaveLoadFileJSON(url)); //Thực hiện tải xuống
                    }
                    else
                    {
                        //Kiểm tra phiên bản mới dựa vào so sánh thời gian file hiện tại trong thiết bị và thời gian file trên sever
                        if (CompareDate(PlayerPrefs.GetString("Present Version"),dateJSON))
                        {
                            progressBarPar.SetActive(true);
                            percentText.SetActive(true);
                            checkDownloadJSON = true;
                            PlayerPrefs.SetString("Present Version", dateJSON); //Lưu thời gian file update
                            StartCoroutine(SaveLoadFileJSON(url)); //Thực hiện tải xuống
                        }
                        //Không có phiên bản mới
                        else
                        {
                            UseFileJSON(unityWebRequest);
                        }
                    }
                }
            }
        }
    }

    //thực hiện việc tải xuống
    private IEnumerator SaveLoadFileJSON(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            //Lấy ra kích thước file đã tải xuống
            progress = Math.Round(unityWebRequest.downloadProgress * 100f, 2);
            bytes = Math.Round(unityWebRequest.downloadedBytes / 1024f, 2);
            if (progress > 0)
            {
                totalBytes = Math.Round((bytes * 100f / progress), 2);
                downloadingText.text = "Downloading " + bytes + "KB" + "/" + totalBytes + "KB";
            }
            
            progressBar.transform.localScale = new Vector3((unityWebRequest.downloadProgress),1f,0);
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
                progressBar.transform.localScale = new Vector3(1,1,0);
                progressPercent.text = "100%";
                downloadingText.text = "Downloading " +  Math.Round((double)Convert.ToInt64(sizeJSON) / 1048576, 2) + "MB" + "/" +  Math.Round((double)Convert.ToInt64(sizeJSON) / 1048576, 2) + "MB";
                //Thực hiện truy xuất data trong file JSON đã tải xuống
                UseFileJSON(unityWebRequest); 
            }
        }
    }

    //đọc và truy xuất data trong file JSON
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
        
        if (!File.Exists(fullPath) || checkDownloadJSON)
        {
            fileDataTemp = unityWebRequest.downloadHandler.text;
            File.WriteAllText(dir + FileName, fileDataTemp);
            string json = File.ReadAllText(fullPath);
            upgradeList = JsonUtility.FromJson<UpgradeList>(json); //Truy xuất data
        }
        else
        {
            // File.WriteAllText(dir + FileName, fileDataTemp);
            string json = File.ReadAllText(fullPath);
            json = File.ReadAllText(fullPath);
            upgradeList = JsonUtility.FromJson<UpgradeList>(json); //truy xuất data
        }
        
        CheckDateAndSizeForLoad(nameFile); //Thực hiện tải xuống file AssetBundle
    }
    
    //kiểm tra lần lượt từng file
    private void CheckDateAndSizeForLoad(string nameFile)
    {
        checkGetSize = false;
        switch (nameFile)
        {
            case "audioclip":
                GetDateAndSize(nameFile,
                    dateFile =>
                    {
                        dateOfFile = dateFile;
                        SaveDateOrCompareDate(nameFile,dateOfFile); //Thực hiện kiểm tra cập nhật file cần tải
                    }, sizeFile =>
                    {
                        sizeOfFile = sizeFile;
                        if (checkGetSize)
                        {
                            GetSizeFile(sizeOfFile); //Thực hiện lấy ra kích thước file cần tải
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
    
    //Gửi yêu cầu
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

    //lấy ra thời gian file được tạo và kích thước file
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

    //kiểm tra thời gian file trên sever để tải hoặc cập nhật
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

    //Phương thức lưu
    private void GetSizeFile(string size)
    {
        totalSize += Math.Round((double)Convert.ToInt64(size) / 1048576, 2);
        subtotalSize += Convert.ToInt64(size);
    }

    //Thực hiện download
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

    //Gửi yêu cầu tải và kiểm tra file đã tồn tại
    void CheckFile(string folder, string nameFile)
    {
        //initialize storage reference
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
                    //File đã tồn tại => xóa file cũ
                    File.Delete(Application.persistentDataPath + "/"+folder+"/"+ nameFile);    
                }
                progressBarPar.SetActive(true);
                percentText.SetActive(true);
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
    
    //Bắt đầu tải và tạo đường dẫn lưu file
    IEnumerator DownloadFile(string url, string nameFile, string nameFolder)
    {
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
    }
    
    //Hiện tiến trình tải xuống
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
            progressBar.transform.localScale = new Vector3((float)Math.Round(progress/subtotalSize,2),1,0);
            // Debug.Log(Math.Round(percent,2)+"%");
            if (progressBar.transform.localScale.x == 1f)
            {
                LoadAssetBundle();
                StartCoroutine(FinishLoading());
            }
        }
    }

    //Sử dụng file assetbundle
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