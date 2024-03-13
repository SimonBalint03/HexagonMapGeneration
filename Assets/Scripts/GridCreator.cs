using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCreator : MonoBehaviour
{
    public Vector2Int gridSize; // X is width, Y is height
    public GameObject defaultTile;
    public GameObject hillTile;
    public GameObject waterTile;
    public GameObject deepWaterTile;
    public GameObject mountainTile;


    private GameObject[,] tiles;


    private void Awake()
    {
        tiles = new GameObject[gridSize.x,gridSize.y];
        for (int q = 0; q < gridSize.x; q++)
        {
            for (int r = 0; r < gridSize.y; r++)
            {
                float x = Mathf.Sqrt(3) * (q + 0.5f);
                float z = 1.5f * r;

                // Offset for odd rows
                if (r % 2 != 0)
                {
                    x += Mathf.Sqrt(3) / 2f;
                }

                Vector3 hexPosition = new Vector3(x, 0f, z );
                tiles[q,r] = Instantiate(defaultTile, hexPosition,Quaternion.identity, transform);
            }
        }
        
        // After, offset the container to center the map.
        Vector3 offset = tiles[tiles.GetLength(0)-1, tiles.GetLength(1)-1].transform.position;
        transform.position = -offset / 2;

    }

    public GameObject[,] GetTiles()
    {
        return tiles;
    }
    
    public Tile[,] GetTilesAsTile()
    {
        Tile[,] result = new Tile[tiles.GetLength(0),tiles.GetLength(1)];
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                result[i, j] = tiles[i, j].GetComponent<Tile>();
            }
        }

        return result;
    }
}
