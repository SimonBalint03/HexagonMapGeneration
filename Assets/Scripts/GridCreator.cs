using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private GameObject[,] _tiles;


    private void Awake()
    {
        InstantiateGrid();
    }

    private void Start()
    {
        AssignNearbyTiles();
    }

    private void InstantiateGrid()
    {
        Transform hexParent = new GameObject("Hexagons").transform;
        hexParent.parent = transform;
        
        
        _tiles = new GameObject[gridSize.x, gridSize.y];
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

                Vector3 hexPosition = new Vector3(x, 0f, z);
                _tiles[q, r] = Instantiate(defaultTile, hexPosition, Quaternion.identity, hexParent);
                _tiles[q, r].GetComponent<Tile>().position = new Vector2Int(q, r);
            }
        }

        // After, offset the container to center the map.
        Vector3 offset = _tiles[_tiles.GetLength(0) - 1, _tiles.GetLength(1) - 1].transform.position;
        transform.position = -offset / 2;
    }

    private void AssignNearbyTiles()
    {
        Tile[,] tiles = GetTilesAsTile();
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile tile = tiles[x, y];
                List<NearbyTile> nearbyTiles = new List<NearbyTile>();
                try
                {
                    // Top right, dir: 0
                    if (tile.position.y % 2 == 0)
                    {
                        nearbyTiles.Add(new NearbyTile(0, tiles[x, y + 1]));
                    }
                    else
                    {
                        nearbyTiles.Add(new NearbyTile(0,tiles[x+1, y+1]));
                    }
                    
                }catch
                {
                    // ignored
                }
                try
                {
                    // Right
                    nearbyTiles.Add(new NearbyTile(1,tiles[x + 1, y]));
                }
                catch
                {
                    // ignored
                }
                try
                {
                    // Bottom right
                    if (tile.position.y % 2 == 0)
                    {
                        nearbyTiles.Add(new NearbyTile(2,tiles[x, y-1]));
                    }
                    else
                    {
                        nearbyTiles.Add(new NearbyTile(2,tiles[x+1, y-1]));
                    }
                }
                catch
                {
                    // ignored
                }
                try
                {
                    // Bottom left
                    if (tile.position.y % 2 == 0)
                    {
                        nearbyTiles.Add(new NearbyTile(3,tiles[x-1, y-1]));
                    }
                    else
                    {
                        nearbyTiles.Add(new NearbyTile(3,tiles[x, y-1]));
                    }
                }
                catch
                {
                    // ignored
                }
                try
                {
                    // Left
                    nearbyTiles.Add(new NearbyTile(4,tiles[x-1,y]));
                }
                catch
                {
                    // ignored
                }
                try
                {
                    // Top left
                    if (tile.position.y % 2 == 0)
                    {
                        nearbyTiles.Add(new NearbyTile(5,tiles[x-1, y+1]));
                    }
                    else
                    {
                        nearbyTiles.Add(new NearbyTile(5,tiles[x, y+1]));
                    }
                }
                catch
                {
                    // ignored
                }
                tile.nearbyTiles = nearbyTiles;
            }
        }
    }

    public GameObject[,] GetTiles()
    {
        return _tiles;
    }
    
    public Tile[,] GetTilesAsTile()
    {
        Tile[,] result = new Tile[_tiles.GetLength(0),_tiles.GetLength(1)];
        for (int i = 0; i < _tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _tiles.GetLength(1); j++)
            {
                result[i, j] = _tiles[i, j].GetComponent<Tile>();
            }
        }

        return result;
    }
}
