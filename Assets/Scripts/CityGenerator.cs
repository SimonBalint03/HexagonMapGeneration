using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = System.Numerics.Quaternion;

public class CityGenerator : MonoBehaviour
{

    public int triesToPlaceCity = 50;
    public void GenerateCityData(Tile[,] tiles, GameObject debugPoint)
    {

        List<Tile> invalidCityTiles = new List<Tile>();
        
        for (int i = 0; i < triesToPlaceCity; i++)
        {
            Tile sTile = RandomValidCityTile(tiles);
            bool flag = false;
            
            foreach (Tile tile in invalidCityTiles)
            {
                if (tile.Equals(sTile))
                {
                    flag = true;
                    break;
                }
                
            }

            if (!flag)
            {
                Instantiate(debugPoint, sTile.transform.position, UnityEngine.Quaternion.identity);
                invalidCityTiles.AddRange(sTile.GetNearbyTilesInRange(5));
                sTile.isCity = true;
            }
            
            // if (!invalidCityTiles.Contains(sTile))
            // {
            //     Instantiate(debugPoint, sTile.transform.position, UnityEngine.Quaternion.identity);
            //     invalidCityTiles = sTile.GetNearbyTilesInRange(5);
            // }
        }
        
        
    }

    private static Tile RandomValidCityTile(Tile[,] tiles)
    {
        int tries = 10000;
        Tile tile = null;
        
        do
        {
            tries--;
            if(tries < 0){ Debug.Log("No more tries for city placement."); break; }
            int x = Random.Range(0, tiles.GetLength(0));
            int y = Random.Range(0, tiles.GetLength(1));
            
            if (!tiles[x,y].IsWater() && tiles[x,y].heightType != HeightType.Mountain && !tiles[x,y].isCity)
            {
                bool keepTile = false;
                if (!tiles[x,y].isRiver)
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        keepTile = true;
                    }
                }
                else { keepTile = true; }

                if (Random.Range(0f, 1f) > 0.25f && !keepTile)
                {
                    if (tiles[x, y].HasWaterNearby())
                    {
                        keepTile = true;
                    }
                }

                if (keepTile)
                {
                    tile = tiles[x,y];
                }
            }
            
        } while (tile == null);

        return tile;
    }
    
    private static Tile RandomNonSeaTile(Tile[,] tiles)
    {
        Tile tile = null;
        do
        {
            int x = Random.Range(0, tiles.GetLength(0));
            int y = Random.Range(0, tiles.GetLength(1));

            if (!tiles[x, y].IsWater() && !tiles[x,y].HasWaterNearby())
            {
                tile = tiles[x, y];
            }
        } while (tile == null);

        return tile;
    }
}
