using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Sprite[] spritesArray;
    private Vector3Int pos;
    void Start()
    {
        spritesArray = AssetBundleManager.Instance.spritesBundleArray;
        GetTilesInTilemap();
        
    }

    public void GetTilesInTilemap()
    {
        Tilemap tileMap = GetComponent<Tilemap>();

        BoundsInt bounds = tileMap.cellBounds;
        TileBase[] allTiles = tileMap.GetTilesBlock(bounds);
        tileMap.tileAnchor = new Vector3(-47f, -47f, 0);
        for (int x = 0; x < bounds.size.x; x++) 
        {
            for (int y = 0; y < bounds.size.y; y++) 
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    pos = new Vector3Int(x, y, 0);
                    SetSprite(tile.name,spritesArray,tileMap,pos);
                }
            }
        }
    }

    public void SetSprite(string nameTile, Sprite[] spritesArray, Tilemap tileMap, Vector3Int position)
    {
        Tile tiles = new Tile();
        for (int i = 0; i < spritesArray.Length; i++)
        {
            if (spritesArray[i].name == nameTile)
            {
                tiles.sprite = spritesArray[i];
                tileMap.SetTile(position, tiles);
            }
        }
    }
}
