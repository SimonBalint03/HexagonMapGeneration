using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class RiverGenerator : MonoBehaviour
{
    // List of tiles with river attribute
    public Dictionary<int, List<Tile>> riverData;
    public int numOfRivers = 20;
    

    public void GenerateRiverData(Tile[,] tiles, GameObject debugPoint)
    {
        List<Tile> riverElements;
        riverData = new Dictionary<int, List<Tile>>();
        // Start from a random non sea tile
        

        for (int i = 0; i < numOfRivers; i++)
        {
            int triesToGoForward = 2000; // Per river
            riverElements = new List<Tile>();
            Tile tile = RandomMountainTile(tiles); // Select a Mountain to start the river from.
            if(tile == null){ break; } // Tile is null when there are no mountains.
            
            
            // Add the first tile
            riverElements.Add(tile);
            tile.SetRiver(true);
            Instantiate(debugPoint, tile.transform.position, Quaternion.identity);
            
            Tile nextTile = null;
            int previousDir = Int32.MinValue;
            do
            {
                // If tile has water around it, end the loop. 
                if (tile.nearbyTiles.Find(nt => nt.TileData.IsWater()) != null) { break; }
                
                
                int direction;
                do
                {
                    // Go toward a random direction
                    direction = RandomDirection();
                    // Select the tile in that direction.
                    try
                    {
                        nextTile = tile.nearbyTiles.Find(nearbyTile => nearbyTile.Direction == direction).TileData;
                        triesToGoForward--;
                    }
                    catch { /* ignored */ }
                    
                } while (nextTile == null && triesToGoForward >= 0);

                if (triesToGoForward <= 0) { break; }
                // If the direction is not valid(goes back straight,left or right), restart the loop.
                if (IsOppositeDirection(direction, previousDir)) { continue; }
                // If nextTile is above tile, restart the loop.
                if (!tile.IsBelowHeightOrEqual(nextTile.heightType)) { continue; }
                // If nextTile is already a river tile, end the loop.
                if (nextTile.isRiver) { break; }
                // If nextTile is a water tile, end the loop.
                if(nextTile.IsWater()) { break; }
                
                
                
                
                
                riverElements.Add(nextTile);
                nextTile.SetRiver(true);
                Instantiate(debugPoint, nextTile.transform.position, Quaternion.identity);
                
                // Set tile to the "nextTile" since we want to continue.
                tile = nextTile;
                previousDir = direction;
                // If a sea is hit than the method ends
            } while (!nextTile.IsWater());

            // if (!riverElements[riverElements.Count-1].IsWater())
            // {
            //     riverElements[riverElements.Count - 1].heightType = HeightType.Water;
            // }
            
            riverData.Add(i,riverElements);
        }
        
        Debug.Log(riverData.Count);
    }

    List<GameObject> ShuffleList(List<GameObject> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
        return list;
    }

    private bool IsOppositeDirection(int dir1, int dir2)
    {
        if (dir2 == Int32.MinValue)
        {
            //Debug.Log("False");
            return false;
        }

        return dir1 switch
        {
            0 => dir2 is 3 or 2 or 4,
            1 => dir2 is 4 or 3 or 5,
            2 => dir2 is 5 or 0 or 4,
            3 => dir2 is 0 or 1 or 5,
            4 => dir2 is 1 or 2 or 0,
            5 => dir2 is 2 or 3 or 1,
            _ => false
        };
    }


    private int RandomDirection()
    {
        return Random.Range(0, 6);
    }
    
    // Returns a tile that is not water and has no water around it.
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

    private static Tile RandomMountainTile(Tile[,] tiles)
    {
        int tries = 1000;
        Tile tile = null;
        do
        {
            int x = Random.Range(0, tiles.GetLength(0));
            int y = Random.Range(0, tiles.GetLength(1));

            if (
                tiles[x,y].heightType == HeightType.Mountain &&  // Has to be mountain.
                !tiles[x,y].isRiver && // Can't be a river.
                !tiles[x,y].HasRiverNearby()) // Can't have river near it.
            {
                bool hasWaterOrRiver = false;
                foreach (NearbyTile nTile in tiles[x,y].nearbyTiles)
                {
                    if (nTile.TileData.IsWater() || nTile.TileData.isRiver)
                    {
                        hasWaterOrRiver = true;
                        break;
                    }
                }
                if(hasWaterOrRiver){continue;}
                tile = tiles[x, y];
            }

            tries--;
        } while (tile == null && tries > 0);

        return tile == null ? null : tile;
    }
}
