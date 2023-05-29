using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Sprite[] spritesArray;
    
    private Vector3Int pos;
    
    void Start()
    {
        spritesArray = AssetBundleManager.Instance.spritesBundleArray;
        Debug.Log("so luong: " + spritesArray.Length);
        GetTilesInTilemap();
    }

    public void GetTilesInTilemap()
    {
        Tilemap tileMap = GetComponent<Tilemap>();
        BoundsInt bounds = tileMap.cellBounds;
        tileMap.CompressBounds();
        // tileMap.tileAnchor = new Vector3(-47f, -47f, 0);
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var tileDle = tileMap.GetTile(new Vector3Int(x, y, 0)) as Tile;
                if (tileDle != null)
                {
                    pos = new Vector3Int(x, y, 0);
                    Debug.Log("Tile name: " + tileDle.name);
                    Debug.Log("Tile sprite: " + tileDle.sprite);
                    
                    // SetSprite(tileDle.name,spritesArray,tileMap,pos);
                }
            }
        }
        //
    }
    
    // public void SetSprite(string nameTile, Sprite[] spritesArray, Tilemap tileMap, Vector3Int position)
    // {
    //     Tile tiles = ScriptableObject.CreateInstance<Tile>();
    //     for (int i = 0; i < spritesArray.Length; i++)
    //     {
    //         if (spritesArray[i].name == nameTile)
    //         {
    //             tiles.sprite = spritesArray[i];
    //             tileMap.SetTile(position, tiles);
    //             Debug.Log("f:/ " + nameTile + " == " + spritesArray[i]);
    //             tileMap.RefreshTile(position);
    //         }
    //     }
    // }
}
