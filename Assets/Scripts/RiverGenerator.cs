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
    public int numOfRivers = 6;
    

    public void GenerateRiverData(Tile[,] tiles, GameObject debugPoint)
    {
        List<Tile> riverElements = new List<Tile>();
        riverData = new Dictionary<int, List<Tile>>();
        // Start from a random non sea tile
        

        for (int i = 0; i < numOfRivers; i++)
        {
            int triesPerRiver = 1000;
            riverElements = new List<Tile>();
            Tile tile = RandomNonSeaTile(tiles);
            riverElements.Add(tile);
            Instantiate(debugPoint, tile.transform.position, Quaternion.identity);
            Tile nextTile;
            
            bool isAlreadyInRiver = true;
            int previousDir = Int32.MinValue;
            do
            {
                // Go toward a random direction
                int direction;
                do
                {
                    direction = RandomDirection();
                    // Make sure its not going back on itself
                    try
                    {
                        isAlreadyInRiver = riverElements.Contains(tile.nearbyTiles[direction].GetComponent<Tile>());
                    }
                    catch
                    {
                        // ignored because it reached the side of the map.
                    }

                    triesPerRiver--;
                    if (triesPerRiver < 0) { break; }
                } while (IsOppositeDirection(direction, previousDir) || isAlreadyInRiver);
                if (triesPerRiver < 0) { break; }
                try
                {
                    nextTile = tile.nearbyTiles[direction].GetComponent<Tile>();
                }
                catch
                {
                    break;
                }
                
                if (!nextTile.IsWater())
                {
                    riverElements.Add(nextTile);
                    Instantiate(debugPoint, nextTile.transform.position, Quaternion.identity);
                }
                else
                {
                    // If a sea is hit than the method ends
                    break;
                }

                tile = nextTile;
                previousDir = direction;
                // If a sea is hit than the method ends
            } while (!nextTile.IsWater());
            
            riverData.Add(i,riverElements);
        }
        
        Debug.Log(riverData.Count);
        foreach (KeyValuePair<int, List<Tile>> kvp in riverData)
        {
            foreach (Tile t in kvp.Value)
            {
                Debug.Log ("Key = " +kvp.Key +" , Value = "+ t.position);
            }
        }
    }

    private bool IsOppositeDirection(int dir1, int dir2)
    {
        if (dir2 == Int32.MinValue)
        {
            Debug.Log("False");
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
    
    private static Tile RandomNonSeaTile(Tile[,] tiles)
    {
        Tile tile = null;
        do
        {
            int x = Random.Range(0, tiles.GetLength(0));
            int y = Random.Range(0, tiles.GetLength(1));

            if (!tiles[x, y].IsWater())
            {
                tile = tiles[x, y];
            }
        } while (tile == null);

        return tile;
    }
}
