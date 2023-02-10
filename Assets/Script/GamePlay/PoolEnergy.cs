using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnergy : MonoBehaviour
{
    [SerializeField] private Energy enegryPortal;
    private List<Energy> listEnergy = new List<Energy>();
    private int amountToPool = 4;
    public Energy newEnergy;
    void Start()
    {
        for (int i = 0; i< amountToPool; i++)
        {
            CreateNewEnergy();
        }
    }

    private Energy CreateNewEnergy()
    {
        newEnergy = Instantiate(enegryPortal);
        listEnergy.Add(newEnergy);
        return newEnergy;
    }

    public Energy GetEnergy()
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
