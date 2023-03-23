using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoolEnergy : MonoBehaviour
{
    [SerializeField] private GameObject enegryPortal;
    public Energy energy;
    private List<GameObject> listEnergy = new List<GameObject>();
    private List<GameObject> portalAnimList = new List<GameObject>();
    private int amountToPool = 4;
    public GameObject newEnergy;
    public GameObject enterPortal, entryPortal;
    private static PoolEnergy instance;
    public static PoolEnergy Instance { get => instance;}
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Error !!!");
        }
        instance = this;
    }
    void Start()
    {
        try
        {
            portalAnimList = new List<GameObject>();
            portalAnimList = AssetBundleManager.Instance.InstancePrefabsBundle(enegryPortal);
            Debug.Log(portalAnimList.Count + " ------");
            for (int i = 0; i < portalAnimList.Count; i++)
            {
                if (portalAnimList[i] != null)
                {
                    if (portalAnimList[i].name == "EntryPortal(Clone)")
                    {
                        // FixShadersForEditor(entryPortal);
                        entryPortal = portalAnimList[i];
                    }

                    if (portalAnimList[i].name == "EnterPortal(Clone)")
                    {
                        // FixShadersForEditor(enterPortal);
                        enterPortal = portalAnimList[i];
                    }
                }
            }
            energy.GetPortalAnim();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            enterPortal = Instantiate(enterPortal, enegryPortal.transform);
            entryPortal = Instantiate(entryPortal, enegryPortal.transform);
            energy.GetPortalAnim();
        }
        for (int i = 0; i< amountToPool; i++)
        {
            CreateNewEnergy();
        }
    }

    private GameObject CreateNewEnergy()
    {
        newEnergy = Instantiate(enegryPortal);
        listEnergy.Add(newEnergy);
        return newEnergy;
    }

    public GameObject GetEnergy()
    {
        for(int i = 0; i < listEnergy.Count; i++)
        {
            if(!listEnergy[i].gameObject.activeInHierarchy)
            {
                return listEnergy[i];
            }
        }
        
        newEnergy = CreateNewEnergy();
        return newEnergy;
    }
       // TODO: Show texture object load from asset bundle
// #if UNITY_EDITOR
//     public void FixShadersForEditor(GameObject prefab)
//          {
//              var renderers = prefab.GetComponentsInChildren<Renderer>(true);
//              foreach (var renderer in renderers)
//              {
//                  ReplaceShaderForEditor(renderer.sharedMaterials);
//              }
//
//              var tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
//              foreach (var tmp in tmps)
//              {
//                  ReplaceShaderForEditor(tmp.material);
//                  ReplaceShaderForEditor(tmp.materialForRendering);
//              }
//              
//              var spritesRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
//              foreach (var spriteRenderer in spritesRenderers)
//              {
//                  ReplaceShaderForEditor(spriteRenderer.sharedMaterials);
//              }
//
//              var images = prefab.GetComponentsInChildren<Image>(true);
//              foreach (var image in images)
//              {
//                  ReplaceShaderForEditor(image.material);
//              }
//              
//              var particleSystemRenderers = prefab.GetComponentsInChildren<ParticleSystemRenderer>(true);
//              foreach (var particleSystemRenderer in particleSystemRenderers)
//              {
//                  ReplaceShaderForEditor(particleSystemRenderer.sharedMaterials);
//              }
//
//              var particles = prefab.GetComponentsInChildren<ParticleSystem>(true);
//              foreach (var particle in particles)
//              {
//                  var renderer = particle.GetComponent<Renderer>();
//                  if (renderer != null) ReplaceShaderForEditor(renderer.sharedMaterials);
//              }
//          }
//
//          public void ReplaceShaderForEditor(Material[] materials)
//          {
//              for (int i = 0; i < materials.Length; i++)
//              {
//                  ReplaceShaderForEditor(materials[i]);
//              }
//          }
//
//          public void ReplaceShaderForEditor(Material material)
//          {
//              if (material == null) return;
//              var shaderName = material.shader.name;
//              var shader = Shader.Find(shaderName);
//              if (shader != null) material.shader = shader;
//          }
// #endif
}
