using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TileTypes;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [Range(0.1f, 1)] public float threshold = 0.5f;
    private Tile[,] _tiles;
    private float[,] forestMap;
    private GameObject forestPrefab;
    
    
    private List<GameObject> fPrefabs = new List<GameObject>();
    
    // TODO: prefab should be a list in the future.
    public void GenerateForests(Tile[,] tiles, GameObject prefab)
    {
        _tiles = tiles;
        forestPrefab = prefab;
        forestMap = GenerateForestMap(tiles.GetLength(0), tiles.GetLength(1), 5);
        for (int x = 0; x < forestMap.GetLength(0); x++)
        {
            for (int y = 0; y < forestMap.GetLength(1); y++)
            {
                Tile tile = tiles[x, y];
                if( tile.IsWater() || tile.heightType == HeightType.Mountain)
                {continue;}

                if (tile.biomeType != BiomeType.Forest && tile.biomeType != BiomeType.SeasonalForest)
                {
                    continue;
                }
                
                tile.isForest = forestMap[x,y] > threshold;
            }
        }
        RefreshForestModels();
    }

    public void RefreshForestModels()
    {
        for (int i = 0; i < fPrefabs.Count; i++)
        {
            Destroy(fPrefabs[i]);
        }
        fPrefabs = new List<GameObject>();
        for (int x = 0; x < _tiles.GetLength(0); x++)
        {
            for (int y = 0; y < _tiles.GetLength(1); y++)
            {
                if (_tiles[x,y].isForest)
                {
                    fPrefabs.Add(Instantiate(forestPrefab, _tiles[x, y].transform.position,
                        forestPrefab.transform.rotation, _tiles[x, y].transform));
                }
                
            }
        }

        
    }
    
    private float[,] GenerateForestMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        //Random seed for the map
        Random.InitState((int)System.DateTime.Now.Ticks);

        Vector2 offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / scale + offset.x;
                float yCoord = (float)y / scale + offset.y;
                
                float sample = CustomNoise(xCoord, yCoord);
                noiseMap[x, y] = sample;
            }
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / scale + offset.x;
                float yCoord = (float)y / scale + offset.y;

                float sample = 0.5f*CustomNoise(2*xCoord, 2*yCoord);
                noiseMap[x, y] += sample;
            }
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / scale + offset.x;
                float yCoord = (float)y / scale + offset.y;

                float sample = 0.25f*CustomNoise(4*xCoord, 4*yCoord);
                noiseMap[x, y] += sample;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] /= (1 + 0.5f + 0.25f);
            }
        }
        // Reverse it?
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = 1 - noiseMap[x, y];
            }
        }
        
        return noiseMap;
    }
    
    float CustomNoise(float x, float y)
    {
        float value = Mathf.PerlinNoise(x, y);
        return value;
    }
}
