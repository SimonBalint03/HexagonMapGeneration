using System;
using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public static class FloatExtensions
{
    public static bool Between(this float value, float lowerBound, float upperBound)
    {
        return value >= lowerBound && value <= upperBound;
    }
}
public class NoiseAssignScript : MonoBehaviour
{
    [Header("Colors")]
    public Color waterColor;
    public Color deepWaterColor;
    public Color landColor;
    public Color hotColor;
    public Color coldColor;

    public Color desertColor;
    public Color jungleColor;
    public Color savannaColor;
    
    public Color tundraColor;
    public Color taigaColor;
    public Color iceColor;


    [Header("Parameters")] 
    public float frequency = 1f;
    
    [Range(-0.1f,1.1f)]public float waterHeight;
    [Range(0.1f, 1)] public float deepWaterThreshold;
    [Range(-0.1f,1.1f)]public float climateVariance;
    

    private GridCreator _gridCreator;
    private int width,height;
    
    private GameObject[,] _tiles;
    private Vector2 offset; // Added offset for changing the seed
    
    private float[,] waterHeightMap;
    public float waterScale = 10f;
    
    private float[,] climateNoiseMap;
    public float climateScale = 1f;
    
    private float[,] biomeNoiseMap;
    public float biomeScale = 5f;

    [Header("DEBUG")] public NoiseLayers noiseLayers;


    void Start()
    {
        _gridCreator = GetComponent<GridCreator>();
        width = _gridCreator.gridSize.x;
        height = _gridCreator.gridSize.y;
        _tiles = _gridCreator.GetTiles();
        GenerateNew();
    }
    
    public void GenerateNew()
    {
        waterHeightMap = CreateWaterHeightMap(waterScale);
        climateNoiseMap = CreateHeightMap(climateScale);
        biomeNoiseMap = CreateHeightMap(biomeScale);
        AssignTypes();
        RefreshColors();
    }

    private void AssignTypes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = _tiles[x, y].GetComponent<Tile>();
                // Water & Land
                if (waterHeightMap[x,y] > waterHeight)
                {
                    tile.WaterType = WaterType.Land;
                } 
                else if (waterHeightMap[x, y] < waterHeight * deepWaterThreshold)
                {
                    tile.WaterType = WaterType.DeepWater;
                } else if (waterHeightMap[x, y] < waterHeight )
                {
                    tile.WaterType = WaterType.Water;
                }
                
                // Climate
                tile.ClimateType = climateNoiseMap[x, y] > climateVariance ? ClimateType.Hot : ClimateType.Cold;

                tile.BiomeType = tile.ClimateType switch
                {
                    // Biome
                    ClimateType.Hot when biomeNoiseMap[x, y].Between(0, 0.33f) => BiomeType.Desert,
                    ClimateType.Hot when biomeNoiseMap[x, y].Between(0.33f, 0.66f) => BiomeType.Savanna,
                    ClimateType.Hot => BiomeType.Jungle,
                    ClimateType.Cold when biomeNoiseMap[x, y].Between(0, 0.33f) => BiomeType.Taiga,
                    ClimateType.Cold when biomeNoiseMap[x, y].Between(0.33f, 0.66f) => BiomeType.Tundra,
                    ClimateType.Cold => BiomeType.Ice,
                    _ => tile.BiomeType
                };
            }
        }
    }
    private void RefreshColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = _tiles[x, y].GetComponent<Tile>();
                switch (noiseLayers)
                {
                    case NoiseLayers.WaterHeight:
                        switch (tile.WaterType)
                        {
                            case WaterType.Land:
                                tile.SetColor(landColor);
                                break;
                            case WaterType.Water:
                                tile.SetColor(waterColor);
                                break;
                            case WaterType.DeepWater:
                                tile.SetColor(deepWaterColor);
                                break;
                        }
                        break;
                    case NoiseLayers.Climate:
                        tile.SetColor(tile.ClimateType == ClimateType.Hot ? hotColor : coldColor);
                        break;
                    case NoiseLayers.Biome:
                        switch (tile.BiomeType)
                        {
                            case BiomeType.Desert:
                                tile.SetColor(desertColor);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(savannaColor);
                                break;
                            case BiomeType.Jungle:
                                tile.SetColor(jungleColor);
                                break;
                            case BiomeType.Taiga:
                                tile.SetColor(taigaColor);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(tundraColor);
                                break;
                            case BiomeType.Ice:
                                tile.SetColor(iceColor);
                                break;
                        }
                        break;
                    case NoiseLayers.Combined:
                        switch (tile.BiomeType)
                        {
                            case BiomeType.Desert:
                                tile.SetColor(desertColor);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(savannaColor);
                                break;
                            case BiomeType.Jungle:
                                tile.SetColor(jungleColor);
                                break;
                            case BiomeType.Taiga:
                                tile.SetColor(taigaColor);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(tundraColor);
                                break;
                            case BiomeType.Ice:
                                tile.SetColor(iceColor);
                                break;
                        }

                        switch (tile.WaterType)
                        {
                            case WaterType.Water:
                                tile.SetColor(waterColor);
                                break;
                            case WaterType.DeepWater:
                                tile.SetColor(deepWaterColor);
                                break;
                        }
                        break;
                }
            }
        }
    }
    private float[,] CreateHeightMap(float scale)
    {
        float[,] noiseMap = new float[width, height];

        //Random seed for the map
        Random.InitState((int)System.DateTime.Now.Ticks);

        offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / scale * frequency + offset.x;
                float yCoord = (float)y / scale * frequency + offset.y;
                
                float sample = CustomNoise(xCoord, yCoord);
                noiseMap[x, y] = sample;
            }
        }
        return noiseMap;
    }

    private float[,] CreateWaterHeightMap(float scale)
    {
        float[,] noiseMap = new float[width, height];

        //Random seed for the map
        Random.InitState((int)System.DateTime.Now.Ticks);

        offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

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
        return noiseMap;
    }
    
    float CustomNoise(float x, float y)
    {
        float value = Mathf.PerlinNoise(x, y);
        return value;
    }

    private void OnValidate()
    {
        AssignTypes();
        RefreshColors();
    }
}
