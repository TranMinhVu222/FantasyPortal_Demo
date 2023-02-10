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
using Firebase;
using Firebase.Extensions;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ReadJSON : MonoBehaviour
{
    public const string SaveDirectory = "/SaveData";
    public const string FileName = "PurchaseMetaData.sav";
    
    private string fileDataTemp;
    private FirebaseStorage storage;
    private StorageReference storageReference;
    private double progress;
    private double totalBytes;
    private double bytes;
    public Text downloadingText, progressPercent;
    
    public Image progressBar;
    public GameObject loadingPanel, warningPanel,FTUE;
    

    public UpgradeList upgradeList = new UpgradeList();
    public UpgradeList tempUpgradeList = new UpgradeList();
    
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
        appOptions.MessageSenderId = "XXXXXXXXXXXXX";
        appOptions.ProjectId = "fantasy-portal-92666532";
        appOptions.StorageBucket = "fantasy-portal-92666532.appspot.com";
    
        var app = Firebase.FirebaseApp.Create( appOptions );
        
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        //get reference of .json
        StorageReference fileJSON = storageReference.Child("PurchaseDatas.json");
        //Get the download link of file
        fileJSON.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    StartCoroutine(LoadFileJSON(Convert.ToString(task.Result)));
                }
                else
                {
                    string fullPath = Application.persistentDataPath + SaveDirectory + FileName;
                    if (File.Exists(fullPath))
                    {
                        progressBar.fillAmount = 1f;
                        progressPercent.text = "100.00%";
                        string json = File.ReadAllText(fullPath);
                        upgradeList = JsonUtility.FromJson<UpgradeList>(json);
                        StartCoroutine(FinishLoading());
                    }
                    else if(!File.Exists(fullPath))
                    {
                        warningPanel.SetActive(true);
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
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.Log("Error: " + unityWebRequest.error);
            }
            else
            {
                progressBar.fillAmount = 1f;
                downloadingText.text = "Downloading " + totalBytes + "KB" + "/" + totalBytes +"KB";
                progressPercent.text = "100.00%";
                StartCoroutine(FinishLoading());
                fileDataTemp = unityWebRequest.downloadHandler.text;
                var dir = Application.persistentDataPath + SaveDirectory;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                tempUpgradeList = JsonUtility.FromJson<UpgradeList>(fileDataTemp);
                GUIUtility.systemCopyBuffer = dir;
                string fullPath = Application.persistentDataPath + SaveDirectory + FileName;
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
            }
        }
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
    IEnumerator FinishLoading()
    {
        yield return new WaitForSeconds(2f);
        if (PlayerPrefs.GetInt("Completed FTUE") == 0)
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
}