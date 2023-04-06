using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//39/3 xoa sprite trong 2d casual UI, trong ./Art/UI/Panel xoa chat-png-9 , ./Art/UI/Store xoa 2 stars, 20 star, 50 star
public class SpriteControllers : MonoBehaviour
{
    public static SpriteControllers instance;
    public static SpriteControllers Instance { get => instance; }
    public GameObject adPanel,_2stars,_20stars,_50stars,starTotal, starSystem, starUpgrade;
    public GameObject[] UILevelArray;
    public GameObject[] boosterArray;
    private Image adImageForPanel, _2starsImage,_20starsImage,_50starsImage, starTotalImage, starSystemImage, starUpgradeImage;
    private Image[] imageStar,imageBooster;
    private Sprite[] spritesArray;
    private Sprite starSprite, starBackSprite, lockSprite, useBoosterSprite, idleBoosterSprite;
    private List<Image> levelImagesList = new List<Image>();
    private List<Image> boosterImagesList = new List<Image>();

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
        adImageForPanel = adPanel.GetComponent<Image>();
        _2starsImage = _2stars.GetComponent<Image>();
        _20starsImage = _20stars.GetComponent<Image>();
        _50starsImage = _50stars.GetComponent<Image>();
        starTotalImage = starTotal.GetComponent<Image>();
        starSystemImage = starSystem.GetComponent<Image>();
        starUpgradeImage = starUpgrade.GetComponent<Image>();
        for (int i = 0; i < UILevelArray.Length; i++)
        {
            imageStar = UILevelArray[i].GetComponentsInChildren<Image>();
            AddImageToList(imageStar,levelImagesList);
        }

        for (int i = 0; i < boosterArray.Length; i++)
        {
            imageBooster = boosterArray[i].GetComponentsInChildren<Image>();
            AddImageToList(imageBooster,boosterImagesList);
        }
    }

    public void GetSpriteBundle(Sprite[] spritesBundleArray)
    {
        spritesArray = spritesBundleArray;
        SetSpriteBundle(spritesArray);
    }

    public void SetSpriteBundle(Sprite[] spritesArray)
    {
        for (int i = 0; i < spritesArray.Length; i++)
        {
            AddSprite(spritesArray[i]);
        }
        AddSpriteForGameObject(levelImagesList,starSprite,starBackSprite,lockSprite);
        AddSpriteForBooster(boosterImagesList, useBoosterSprite, idleBoosterSprite);
    }
    public void AddSprite(Sprite sprite)
    {
        if (sprite.name == "chat-png-9")
        {
            adImageForPanel.sprite = sprite;
        }

        if (sprite.name == "2stars")
        {
            _2starsImage.sprite = sprite;
        }

        if (sprite.name == "20stars")
        {
            _20starsImage.sprite = sprite;
        }

        if (sprite.name == "50stars")
        {
            _50starsImage.sprite = sprite;
        }

        if (sprite.name == "GUI_24")
        {
            starTotalImage.sprite = sprite;
            starSprite = sprite;
            starSystemImage.sprite = sprite;
            starUpgradeImage.sprite = sprite;
        }
        
        if (sprite.name == "GUI_25")
        {
            starBackSprite = sprite;
        }

        if (sprite.name == "GUI_0")
        {
            lockSprite = sprite;
        }

        if (sprite.name == "meter_icon_holder_purple")
        {
            idleBoosterSprite = sprite;
        }
        
        if (sprite.name == "usebooster")
        {
            useBoosterSprite = sprite;
        }
    }

    public void AddImageToList(Image[] imagesArray, List<Image> imageList)
    {
        for (int i = 0; i < imagesArray.Length; i++)
        {
            imageList.Add(imagesArray[i]);
        }
    }

    public void AddSpriteForGameObject(List<Image> imageList, Sprite starSprite, Sprite starBackSprite, Sprite lockSprite)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            if (imageList[i].name.StartsWith("Star"))
            {
                imageList[i].sprite = starSprite;
            }

            if (imageList[i].name.StartsWith("Background"))
            {
                imageList[i].sprite = starBackSprite;
            }

            if (imageList[i].name.StartsWith("Lock"))
            {
                imageList[i].sprite = lockSprite;
            }
        }
    }

    public void AddSpriteForBooster(List<Image> imageList, Sprite useBooster, Sprite idleBooster)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            if (imageList[i].name == "NoUseBoostersButton")
            {
                imageList[i].sprite = idleBooster;
            }

            if (imageList[i].name == "UseBoostersButton")
            {
                imageList[i].sprite = useBooster;
            }
        }
    }
}
