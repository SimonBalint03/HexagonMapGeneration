using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public Vector2Int gridSize; // X is width, Y is height
    public GameObject tilePrefab;


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

                // Add offset for odd rows
                if (r % 2 != 0)
                {
                    x += Mathf.Sqrt(3) / 2f;
                }

                Vector3 hexPosition = new Vector3(x, 0f, z);
                tiles[q,r] = Instantiate(tilePrefab, hexPosition,tilePrefab.transform.rotation, transform);
            }
        }

        // tiles = new GameObject[gridSize.x,gridSize.y];
        // for (int x = 0; x < gridSize.x; x++)
        // {
        //     for (int y = 0; y < gridSize.y; y++)
        //     {
        //         tiles[x,y] = Instantiate(tilePrefab, new Vector3(y, 0, x),tilePrefab.transform.rotation, transform);
        //     }
        // }
    }

    public GameObject[,] GetTiles()
    {
        return tiles;
    }
}
