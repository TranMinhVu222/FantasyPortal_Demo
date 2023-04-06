using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class AssetBundleManager : MonoBehaviour
{
    private static AssetBundleManager instance;
    public static AssetBundleManager Instance { get => instance; }
    
    public static string[] scene;
    public static string[] audioClipArray; 
    public static string[] nameMaterialArray;
    public static string[] namePrefabArray;
    public static string[] nameTextureArray;

    public static List<AudioClip> audioClipList = new List<AudioClip>();
    public static List<GameObject> prefabList = new List<GameObject>();

    private string sceneGameToLoadAB;
    private GameObject instancePrefabBundle, energyPrefabBundle;

    public static AssetBundle sceneBundle, audioClipBundle, materialBundle, prefabBundle, textureBundle;

    public GameObject materials, prefabs, loadingPanel, FTUE;

    public AudioClip audioClips;

    public Sprite[] spritesBundleArray;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Error !!!");
        }
        instance = this;
    }
    
    public void UseAssetBundle(string typeOfAssetBundle)
    {
        switch (typeOfAssetBundle)
        {
            case "scenebundle":
                sceneBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle"));
                scene = sceneBundle.GetAllScenePaths();
                foreach (string sceneName in scene)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(sceneName).ToString();
                }
                break;
            case "audioclip":
                audioClipBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip"));
                audioClipArray = audioClipBundle.GetAllAssetNames();
                for (int i = 0; i < audioClipArray.Length; i++)
                {
                    audioClips = audioClipBundle.LoadAsset<AudioClip>(audioClipBundle.GetAllAssetNames()[i]);
                    audioClipList.Add(audioClips);
                    if(i == audioClipArray.Length - 1)
                        AudioManager.AudioManger.LoadAudioAssetBundle();
                }
                break;
            case "materialbundle":
                materialBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle"));
                nameMaterialArray = materialBundle.GetAllAssetNames();
                foreach (string materialName in nameMaterialArray)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(materialName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(materialName));
                }
                break;
            case "prefabbundle":
                prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/prefabbundle"));
                namePrefabArray = prefabBundle.GetAllAssetNames();
                foreach (string prefabName in namePrefabArray)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(prefabName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(prefabName));
                }
                break;
            case "texturebundle":
                textureBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/texturebundle"));
                nameTextureArray = textureBundle.GetAllAssetNames();
                foreach (string textureName in nameTextureArray)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(textureName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(textureName));
                }
                break;
        }
    }

    public void AssetBundleAvailable()
    {
        if (sceneBundle != null || audioClipBundle != null)
        {
            SpriteControllers.Instance.GetSpriteBundle(spritesBundleArray);
            if (audioClipArray != null)
            {
                AudioManager.AudioManger.LoadAudioAssetBundle();    
            }
        }
        else
        {
            textureBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/texturebundle"));
            audioClipBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip"));
            materialBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle"));
           
            if (textureBundle != null )
            {
                sceneBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle"));
            }
            if (materialBundle != null)
            {
                prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/prefabbundle")); 
            }
            if (prefabBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
            }
            UseAssetBundles();
        }
    }
    
    public void UseAssetBundles()
    {
        
        audioClipArray = audioClipBundle.GetAllAssetNames();
        nameMaterialArray = materialBundle.GetAllAssetNames();
        namePrefabArray = prefabBundle.GetAllAssetNames();
        nameTextureArray = textureBundle.GetAllAssetNames();
        
        //---- Get sprite from asset bundle
        spritesBundleArray = textureBundle.LoadAllAssets<Sprite>();
        
        SpriteControllers.Instance.GetSpriteBundle(spritesBundleArray);
        
        scene = sceneBundle.GetAllScenePaths();
        for (int i = 0; i < scene.Length; i++)
        {
            Debug.Log(scene[i]);
        }

        //---- Get Audio Clip from asset bundle
        
        if (audioClipArray != null)
        {
            audioClipArray = audioClipBundle.GetAllAssetNames();    
        }
        
        for (int i = 0; i < audioClipArray.Length; i++)
        {
            audioClips = audioClipBundle.LoadAsset<AudioClip>(audioClipBundle.GetAllAssetNames()[i]);
            audioClipList.Add(audioClips);    
            if(i == audioClipArray.Length - 1) AudioManager.AudioManger.LoadAudioAssetBundle();
        }
        if (!PlayerPrefs.HasKey("Completed FTUE") || PlayerPrefs.GetInt("Completed FTUE") == 0)
        {
            Debug.Log("VAR");
            LoadScene(0);
            FTUE.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Complete Menu FTUE") == 1)
        {
            FTUE.SetActive(false);
        }
        else
        {
            loadingPanel.SetActive(false);    
        }
    }
    
    //Use scene from asset bundle
    public void LoadScene(int index)
    {
        // SceneManager.LoadSceneAsync(scene[index]);
        SceneManager.LoadScene(scene[index]);
    }

    //TODO: Get energy prefab. Now, it have pink material 
    public List<GameObject> InstancePrefabsBundle(GameObject energy)
    {
        foreach (string namePrefab in namePrefabArray)
        {
            Debug.Log(namePrefab);
            if (Path.GetFileNameWithoutExtension(namePrefab) == "entryportal" || Path.GetFileNameWithoutExtension(namePrefab) == "enterportal")
            {
                prefabs = prefabBundle.LoadAsset<GameObject>(namePrefab);
                instancePrefabBundle = Instantiate(prefabs,energy.transform);
                instancePrefabBundle.SetActive(false);
                prefabList.Add(instancePrefabBundle);
                Path.GetFileNameWithoutExtension(namePrefab);
            }
        }
        return prefabList;
    }
}
