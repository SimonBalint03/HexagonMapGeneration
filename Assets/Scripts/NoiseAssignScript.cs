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
    [Header("Parameters")] 
    private float frequency = 1f;
    
    [Range(-0.1f,1.1f)]public float waterHeight;
    [Range(0.1f, 1)] public float deepWaterThreshold;
    [Range(-0.1f,2.1f)]public float temperature;
    [Range(-0.1f,2.1f)]public float rainfall;
    
    // Color
    private ColorStorage cS;

    private GridCreator _gridCreator;
    private int width,height;
    
    private GameObject[,] _tiles;
    private Vector2 offset;
    
    private float[,] waterHeightMap;
    public float waterScale = 10f;
    
    //private float[,] climateNoiseMap;
    public float climateScale = 1f;
    
    // private float[,] biomeNoiseMap;
    // public float biomeScale = 5f;

    private float[,] tempNoiseMap;
    private TemperatureMap _temperatureMap;
    
    private float[,] rainNoiseMap;
    private RainfallMap _rainfallMap;

    
    [Header("DEBUG")] public NoiseLayers noiseLayers;


    void Start()
    {
        _temperatureMap = gameObject.AddComponent<TemperatureMap>();
        _rainfallMap = gameObject.AddComponent<RainfallMap>();
        _gridCreator = GetComponent<GridCreator>();
        cS = GetComponent<ColorStorage>();
        
        width = _gridCreator.gridSize.x;
        height = _gridCreator.gridSize.y;
        _tiles = _gridCreator.GetTiles();
        GenerateNew();
    }
    
    public void GenerateNew()
    {
        waterHeightMap = CreateWaterHeightMap(waterScale);
        //climateNoiseMap = CreateHeightMap(climateScale);
        //biomeNoiseMap = CreateHeightMap(biomeScale);
        tempNoiseMap = _temperatureMap.GenerateTemperatureMap(width, height, climateScale);
        rainNoiseMap = _rainfallMap.GenerateRainfallMap(width, height, climateScale);
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
                
                // Temperature
                if (tempNoiseMap[x,y] > temperature * 0.8f)
                {
                    tile.TemperatureType = TemperatureType.VeryCold;
                } else if (tempNoiseMap[x,y].Between(temperature *0.6f,temperature *0.8f))
                {
                    tile.TemperatureType = TemperatureType.Cold;
                } else if (tempNoiseMap[x,y].Between(temperature *0.4f,temperature *0.6f))
                {
                    tile.TemperatureType = TemperatureType.Mild;
                } else if (tempNoiseMap[x,y].Between(temperature * 0.2f,temperature * 0.4f))
                {
                    tile.TemperatureType = TemperatureType.Hot;
                } else if (tempNoiseMap[x,y] < temperature * 0.2f)
                {
                    tile.TemperatureType = TemperatureType.VeryHot;
                }
                
                // Rainfall
                if (rainNoiseMap[x,y] > rainfall * 0.9f)
                {
                    tile.RainfallType = RainfallType.NoRain;
                } else if (rainNoiseMap[x,y].Between(rainfall * 0.65f,rainfall * 0.9f))
                {
                    tile.RainfallType = RainfallType.FewRain;
                } else if (rainNoiseMap[x,y].Between(rainfall * 0.4f,rainfall * 0.65f))
                {
                    tile.RainfallType = RainfallType.MildRain;
                } else if (rainNoiseMap[x,y].Between(rainfall * 0.15f,rainfall * 0.4f))
                {
                    tile.RainfallType = RainfallType.MuchRain;
                } else if (rainNoiseMap[x,y] < rainfall * 0.15f)
                {
                    tile.RainfallType = RainfallType.LotsRain;
                }
                
                // Biomes
                // HOT
                if (tempNoiseMap[x,y] < temperature * 0.3f)
                {
                    if (rainNoiseMap[x,y] < rainfall * 0.3f)
                    {
                        tile.BiomeType = BiomeType.TropicalRainForest;
                    }
                    else if(rainNoiseMap[x,y].Between(rainfall * 0.3f,rainfall * 0.6f))
                    {
                        tile.BiomeType = BiomeType.Savanna;
                    }
                    else
                    {
                        tile.BiomeType = BiomeType.Desert;
                    }
                } 
                // Mid temp
                else if (tempNoiseMap[x, y].Between(temperature * 0.3f, temperature * 0.6f))
                {
                    if (rainNoiseMap[x,y] < rainfall * 0.4f)
                    {
                        tile.BiomeType = BiomeType.RainForest;
                    }
                    else if(rainNoiseMap[x,y].Between(rainfall * 0.4f,rainfall * 0.7f))
                    {
                        tile.BiomeType = BiomeType.Forest;
                    }
                    else
                    {
                        tile.BiomeType = BiomeType.TemperateForest;
                    }
                }
                // Low temp
                else 
                {
                    if (rainNoiseMap[x,y] < rainfall * 0.4f)
                    {
                        tile.BiomeType = BiomeType.Taiga;
                    }
                    else if(rainNoiseMap[x,y].Between(rainfall * 0.4f,rainfall * 0.7f))
                    {
                        tile.BiomeType = BiomeType.Tundra;
                    }
                    else
                    {
                        tile.BiomeType = BiomeType.Ice;
                    }
                }
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
                                tile.SetColor(cS.land);
                                break;
                            case WaterType.Water:
                                tile.SetColor(cS.water);
                                break;
                            case WaterType.DeepWater:
                                tile.SetColor(cS.deepWater);
                                break;
                        }
                        break;
                    case NoiseLayers.Temperature:
                        switch (tile.TemperatureType)
                        {
                            case TemperatureType.VeryCold:
                                tile.SetColor(cS.veryCold);
                                break;
                            case TemperatureType.Cold:
                                tile.SetColor(cS.cold);
                                break;
                            case TemperatureType.Mild:
                                tile.SetColor(cS.mildTemp);
                                break;
                            case TemperatureType.Hot:
                                tile.SetColor(cS.hot);
                                break;
                            case TemperatureType.VeryHot:
                                tile.SetColor(cS.veryHot);
                                break;
                        }
                        break;
                    case NoiseLayers.Rainfall:
                        switch (tile.RainfallType)
                        {
                            case RainfallType.NoRain:
                                tile.SetColor(cS.noRain);
                                break;
                            case RainfallType.FewRain:
                                tile.SetColor(cS.fewRain);
                                break;
                            case RainfallType.MildRain:
                                tile.SetColor(cS.mildRain);
                                break;
                            case RainfallType.MuchRain:
                                tile.SetColor(cS.muchRain);
                                break;
                            case RainfallType.LotsRain:
                                tile.SetColor(cS.lotsRain);
                                break;
                        }
                        break;
                    case NoiseLayers.Biome:
                        switch (tile.BiomeType)
                        {
                            case BiomeType.TropicalRainForest:
                                tile.SetColor(cS.tropicalRainForest);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(cS.savanna);
                                break;
                            case BiomeType.Desert:
                                tile.SetColor(cS.desert);
                                break;
                            
                            case BiomeType.RainForest:
                                tile.SetColor(cS.rainForest);
                                break;
                            case BiomeType.Forest:
                                tile.SetColor(cS.forest);
                                break;
                            case BiomeType.TemperateForest:
                                tile.SetColor(cS.temperateForest);
                                break;
                            
                            case BiomeType.Taiga:
                                tile.SetColor(cS.tundra);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(cS.taiga);
                                break;
                            case BiomeType.Ice:
                                tile.SetColor(cS.ice);
                                break;
                        }
                        break;
                    case NoiseLayers.Combined:
                        switch (tile.BiomeType)
                        {
                            case BiomeType.TropicalRainForest:
                                tile.SetColor(cS.tropicalRainForest);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(cS.savanna);
                                break;
                            case BiomeType.Desert:
                                tile.SetColor(cS.desert);
                                break;
                            
                            case BiomeType.RainForest:
                                tile.SetColor(cS.rainForest);
                                break;
                            case BiomeType.Forest:
                                tile.SetColor(cS.forest);
                                break;
                            case BiomeType.TemperateForest:
                                tile.SetColor(cS.temperateForest);
                                break;
                            
                            case BiomeType.Taiga:
                                tile.SetColor(cS.tundra);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(cS.taiga);
                                break;
                            case BiomeType.Ice:
                                tile.SetColor(cS.ice);
                                break;
                        }

                        switch (tile.WaterType)
                        {
                            case WaterType.Water:
                                tile.SetColor(cS.water);
                                break;
                            case WaterType.DeepWater:
                                tile.SetColor(cS.deepWater);
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
