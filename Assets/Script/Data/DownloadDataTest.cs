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

public class DownloadDataTest : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storageReference;
    // Start is called before the first frame update
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
        
    }

    void CheckFile(string nameFile)
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fantasy-portal-92666532.appspot.com");
        StorageReference fileAssetBundle = storageReference.Child(nameFile);
        fileAssetBundle.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                if (File.Exists(Path.Combine(Application.persistentDataPath, "Test/" + nameFile)))
                {
                    File.Delete(Application.persistentDataPath + "/Test/"+ nameFile);    
                }
                StartCoroutine(DownloadFile(Convert.ToString(task.Result),nameFile, "Test"));
            }
            else
            {
                if (!Directory.Exists(Application.persistentDataPath + "/Test"))
                {
                    Debug.Log("Khong co file can thiet");
                }
                else
                {
                    Debug.Log("Da co file -----> Run Game");
                }
            }
        });
    }
    IEnumerator DownloadFile(string url, string nameFile, string nameFolder)
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
                Debug.Log("Xay ra loi");
            }
            else
            {
                if (!Directory.Exists(Application.persistentDataPath + "/"+nameFolder))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/"+nameFolder);
                }
                if (!File.Exists(Path.Combine(Application.persistentDataPath, nameFolder + "/" + nameFile)))
                {
                    
                    Uri uri = new Uri(urlDownload);

                    WebClient client = new WebClient();
                    
                    client.DownloadProgressChanged += DownloadProgressChanged;
                    client.DownloadFileCompleted += OnDownloadFileCompleted;
                    client.DownloadFileAsync(uri, Application.persistentDataPath + "/" + nameFolder + "/" + nameFile);
                    while (client.IsBusy)
                    {
                        yield return null;
                    }
                }
            }
        }
    }
    void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
    {
        Debug.Log("Tai xong file");
    }
    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        Debug.Log(e.BytesReceived +"/"+e.TotalBytesToReceive);
    }
}
