using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            for (int i = 0; i < portalAnimList.Count; i++)
            {
                if (portalAnimList[i].name == "EntryPortal(Clone)")
                {
                    entryPortal = portalAnimList[i];
                }

                if (portalAnimList[i].name == "EnterPortal(Clone)")
                {
                    enterPortal = portalAnimList[i];
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
}
