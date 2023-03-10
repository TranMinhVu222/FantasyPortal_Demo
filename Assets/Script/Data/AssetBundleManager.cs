using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;



public class AssetBundleManager : MonoBehaviour
{
   
    public static string[] scene;
    public static string[] audioClipArray; 
    public static string[] material;
    public static string[] prefab;

    public static List<AudioClip> audioClipList = new List<AudioClip>();
    
    private static string sceneGameToLoadAB;

    public static AssetBundle sceneBundle, audioClipBundle, materialBundle, prefabBundle;

    public static dynamic audioClips;

    public static void UseAssetBundle(string typeOfAssetBundle)
    {
        switch (typeOfAssetBundle)
        {
            case "scenebundle":
                scene = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle")).GetAllScenePaths();
                Debug.Log("Lenght: " + scene.Length);
                foreach (string sceneName in scene)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(sceneName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(sceneName));
                }
                break;
            case "audioclip":
                audioClipBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip"));
                audioClipArray = audioClipBundle.GetAllAssetNames();
                foreach (string audioClipName in audioClipArray)
                {
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(audioClipName));
                }
                for (int i = 0; i < audioClipArray.Length; i++)
                {
                    audioClips = audioClipBundle.LoadAsset(audioClipBundle.GetAllAssetNames()[i]);
                    audioClipList.Add(audioClips);
                    if(i == audioClipArray.Length - 1)
                        AudioManager.AudioManger.LoadAudioAssetBundle();
                }
                break;
            case "materialbundle":
                material = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle")).GetAllAssetNames();
                foreach (string materialName in material)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(materialName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(materialName));
                }
                break;
            case "energybundle":
                prefab = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle")).GetAllAssetNames();
                foreach (string prefabName in prefab)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(prefabName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(prefabName));
                }
                break;
        }
    }

    public static void AssetBundleAvailable()
    {
        if (scene != null || audioClipArray != null || material != null || prefab != null)
        {
            AssetBundle.UnloadAllAssetBundles(false);
            if (audioClipArray != null)
            {
                AudioManager.AudioManger.LoadAudioAssetBundle();    
            }
        }
        else
        {
            scene = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle")).GetAllScenePaths();
            audioClipBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip"));
            materialBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle"));
            prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle"));  
            UseAssetBundle();
        }
    }
    public static void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(scene[index]);
    }

    public static void UseAssetBundle()
    {
        audioClipArray = audioClipBundle.GetAllAssetNames();
        if (audioClipArray != null)
        {
            audioClipArray = audioClipBundle.GetAllAssetNames();    
        }
        foreach (string audioClipName in audioClipArray)
        {
            Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(audioClipName));
        }
        for (int i = 0; i < audioClipArray.Length; i++)
        {
            audioClips = audioClipBundle.LoadAsset(audioClipBundle.GetAllAssetNames()[i]);
            audioClipList.Add(audioClips);    
            if(i == audioClipArray.Length - 1)
                AudioManager.AudioManger.LoadAudioAssetBundle();
        }
    }
}
