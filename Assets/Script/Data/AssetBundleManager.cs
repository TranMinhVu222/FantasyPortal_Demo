using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AssetBundleManager : MonoBehaviour
{
    public static string[] scene;
    public static string[] audioClip;
    public static string[] material;
    public static string[] prefab;

    private static string sceneGameToLoadAB;
    
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
                audioClip = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip")).GetAllAssetNames();
                foreach (string audioClipName in audioClip)
                {
                    sceneGameToLoadAB = Path.GetFileNameWithoutExtension(audioClipName).ToString();
                    Debug.Log("NameInPath(foreach): " + Path.GetFileNameWithoutExtension(audioClipName));
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
        scene = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenebundle")).GetAllScenePaths();
        audioClip = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/audioclip")).GetAllAssetNames();
        material = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/materialbundle")).GetAllAssetNames();
        prefab = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/energybundle")).GetAllAssetNames();
    }
    public static void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(scene[index]);
    }
}
