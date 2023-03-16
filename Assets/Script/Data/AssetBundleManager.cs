using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class AssetBundleManager : MonoBehaviour
{
    private static AssetBundleManager instance;
    public static AssetBundleManager Instance { get => instance; }
    
    public static string[] scene;
    public static string[] audioClipArray; 
    public static string[] nameMaterialArray;
    public static string[] namePrefabArray;

    public static List<AudioClip> audioClipList = new List<AudioClip>();
    public static List<GameObject> prefabList = new List<GameObject>();

    private string sceneGameToLoadAB;
    private GameObject instancePrefabBundle, energyPrefabBundle;

    public static AssetBundle sceneBundle, audioClipBundle, materialBundle, prefabBundle;

    public dynamic audioClips, materials, prefabs;

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
                    audioClips = audioClipBundle.LoadAsset(audioClipBundle.GetAllAssetNames()[i]);
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
            case "energybundle":
                prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle"));
                namePrefabArray = prefabBundle.GetAllAssetNames();
                foreach (string prefabName in namePrefabArray)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(prefabName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(prefabName));
                }
                break;
        }
    }

    public void AssetBundleAvailable()
    {
        if (sceneBundle != null || audioClipBundle != null)
        {
            Debug.Log("da unload");
            if (audioClipArray != null)
            {
                AudioManager.AudioManger.LoadAudioAssetBundle();    
            }
        }
        else
        {
            Debug.Log("da load");
            sceneBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle"));
            audioClipBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip"));
            materialBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle"));
            if (materialBundle != null)
            {
                prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle")); 
            }
            if (prefabBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
            }
            UseAssetBundle();
        }
    }
    
    public void UseAssetBundle()
    {
        scene = sceneBundle.GetAllScenePaths();
        audioClipArray = audioClipBundle.GetAllAssetNames();
        nameMaterialArray = materialBundle.GetAllAssetNames();
        namePrefabArray = prefabBundle.GetAllAssetNames();
        
        //---- Get Audio Clip from asset bundle
        if (audioClipArray != null)
        {
            audioClipArray = audioClipBundle.GetAllAssetNames();    
        }
        
        for (int i = 0; i < audioClipArray.Length; i++)
        {
            audioClips = audioClipBundle.LoadAsset(audioClipBundle.GetAllAssetNames()[i]);
            audioClipList.Add(audioClips);    
            if(i == audioClipArray.Length - 1) AudioManager.AudioManger.LoadAudioAssetBundle();
        }
    }
    
    //Use scene from asset bundle
    public void LoadScene(int index)
    {
        Debug.Log(index + " va " + scene.Length);
        SceneManager.LoadSceneAsync(scene[index]);
    }

    //TODO: Get energy prefab. Now, it have pink material 
    // public void LoadPrefabAssetBundle()
    // {
    //     if (materialBundle != null)
    //     {
    //         prefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle")); 
    //     }
    //     if (prefabBundle == null)
    //     {
    //         Debug.Log("Failed to load AssetBundle!");
    //     }
    //     nameMaterialArray = materialBundle.GetAllAssetNames();
    //     namePrefabArray = prefabBundle.GetAllAssetNames();
    // }
    public List<GameObject> InstancePrefabsBundle(GameObject energy)
    {
        foreach (string namePrefab in namePrefabArray)
        {
            if (Path.GetFileNameWithoutExtension(namePrefab) == "entryportal" || Path.GetFileNameWithoutExtension(namePrefab) == "enterportal")
            {
                prefabs = prefabBundle.LoadAsset<GameObject>(namePrefab);
                instancePrefabBundle = Instantiate(prefabs,energy.transform);
                instancePrefabBundle.SetActive(false);
                prefabList.Add(instancePrefabBundle);
                Path.GetFileNameWithoutExtension(namePrefab);
            }
            // FixShadersForEditor(prefabs);
        }
        return prefabList;
    }
    
    //TODO: Show material object load from asset bundle
//     #if UNITY_EDITOR
//     
//         public void FixShadersForEditor(GameObject prefab)
//         {
//             var renderers = prefab.GetComponentsInChildren<Renderer>(true);
//             foreach (var renderer in renderers)
//             {
//                 ReplaceShaderForEditor(renderer.sharedMaterials);
//             }
//
//             var tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
//             foreach (var tmp in tmps)
//             {
//                 ReplaceShaderForEditor(tmp.material);
//                 ReplaceShaderForEditor(tmp.materialForRendering);
//             }
//             
//             var spritesRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
//             foreach (var spriteRenderer in spritesRenderers)
//             {
//                 ReplaceShaderForEditor(spriteRenderer.sharedMaterials);
//             }
//
//             var images = prefab.GetComponentsInChildren<Image>(true);
//             foreach (var image in images)
//             {
//                 ReplaceShaderForEditor(image.material);
//             }
//             
//             var particleSystemRenderers = prefab.GetComponentsInChildren<ParticleSystemRenderer>(true);
//             foreach (var particleSystemRenderer in particleSystemRenderers)
//             {
//                 ReplaceShaderForEditor(particleSystemRenderer.sharedMaterials);
//             }
//
//             var particles = prefab.GetComponentsInChildren<ParticleSystem>(true);
//             foreach (var particle in particles)
//             {
//                 var renderer = particle.GetComponent<Renderer>();
//                 if (renderer != null) ReplaceShaderForEditor(renderer.sharedMaterials);
//             }
//         }
//
//         public void ReplaceShaderForEditor(Material[] materials)
//         {
//             for (int i = 0; i < materials.Length; i++)
//             {
//                 ReplaceShaderForEditor(materials[i]);
//             }
//         }
//
//         public void ReplaceShaderForEditor(Material material)
//         {
//             if (material == null) return;
//
//             var shaderName = material.shader.name;
//             var shader = Shader.Find(shaderName);
//
//             if (shader != null) material.shader = shader;
//         }
//     
// #endif
}
