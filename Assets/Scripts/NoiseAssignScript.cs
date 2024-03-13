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
    [Header("Height")]
    [Range(-0.1f,1.1f)]public float waterHeight;
    [Range(0.1f, 1)] public float deepWaterThreshold;
    [Range(-0.2f, 0.2f)] public float hilliness;
    
    
    
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

    public float climateScale = 1f;

    private float[,] tempNoiseMap;
    private TemperatureMap _temperatureMap;
    
    private float[,] rainNoiseMap;
    private RainfallMap _rainfallMap;

    private LandModelAssigner LandModelAssigner;
    
    [Header("DEBUG")] public NoiseLayers noiseLayers;

    private void OnValidate()
    {
        AssignTypes();
        RefreshColors();
    }

    void Start()
    {
        _temperatureMap = gameObject.AddComponent<TemperatureMap>();
        _rainfallMap = gameObject.AddComponent<RainfallMap>();
        _gridCreator = GetComponent<GridCreator>();
        cS = GetComponent<ColorStorage>();
        LandModelAssigner = GetComponent<LandModelAssigner>();
        
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
        
        LandModelAssigner.RefreshLandModels();
    }

    private void AssignTypes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = _tiles[x, y].GetComponent<Tile>();
                
                // Water & Land
                AssignWaterLevel(x, y, tile);
                
                // Temperature
                AssignTemperature(x, y, tile);
                
                // Rainfall
                AssignRainfall(x, y, tile); 
                
                // Biomes : Ref: https://forum.unity.com/proxy.php?image=http%3A%2F%2Fwww.ultimatepronoun.com%2Fimages%2Fminecraft%2Fbiomes_array.png&hash=6f234cc126f111767bbe0e4bee5a3cba
                AssignBiome(x, y, tile);

            }
        }
    }

    private void AssignBiome(int x, int y, Tile tile)
    {
        if (tempNoiseMap[x, y] > 0.6f * Random.Range(0.9f,1f) * temperature)
        {
            if (rainNoiseMap[x, y] > 0.8f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.RainForest;
            }
            else if (rainNoiseMap[x, y] > 0.5f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.SeasonalForest;
            }
            else if (rainNoiseMap[x, y] > 0.2f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.Plains;
            }
            else
            {
                tile.BiomeType = BiomeType.Desert;
            }
        }
        else if (tempNoiseMap[x, y] > 0.4f * Random.Range(0.9f,1f) * temperature)
        {
            if (rainNoiseMap[x, y] > 0.8f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.Swamp;
            }
            else if (rainNoiseMap[x, y] > 0.2f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.Forest;
            }
            else if (rainNoiseMap[x, y] > 0.1f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.Shrubland;
            }
            else
            {
                tile.BiomeType = BiomeType.Savanna;
            }
        }
        else if (tempNoiseMap[x, y] > 0.25f * Random.Range(0.9f,1f) * temperature)
        {
            if (rainNoiseMap[x, y] > 0.6f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.SeasonalForest;
            }
            else if (rainNoiseMap[x, y] > 0.3f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.BiomeType = BiomeType.Forest;
            }
            else
            {
                tile.BiomeType = BiomeType.Plains;
            }
        }
        else if (rainNoiseMap[x, y] > 0.3f * Random.Range(0.9f,1f) * rainfall)
        {
            tile.BiomeType = BiomeType.Taiga;
        }
        else if (rainNoiseMap[x, y] > 0.15f * Random.Range(0.9f,1f) * rainfall)
        {
            tile.BiomeType = BiomeType.Tundra;
        }
        else
        {
            tile.BiomeType = BiomeType.IceDesert;
        }
    }

    private void AssignRainfall(int x, int y, Tile tile)
    {
        if (rainNoiseMap[x, y] > rainfall * 0.8f)
        {
            tile.RainfallType = RainfallType.LotsRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.65f, rainfall * 0.8f))
        {
            tile.RainfallType = RainfallType.MuchRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.4f, rainfall * 0.65f))
        {
            tile.RainfallType = RainfallType.MildRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.15f, rainfall * 0.4f))
        {
            tile.RainfallType = RainfallType.FewRain;
        }
        else if (rainNoiseMap[x, y] < rainfall * 0.15f)
        {
            tile.RainfallType = RainfallType.NoRain;
        }
    }

    private void AssignTemperature(int x, int y, Tile tile)
    {
        if (tempNoiseMap[x, y] > temperature * 0.8f)
        {
            tile.TemperatureType = TemperatureType.VeryHot;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.6f, temperature * 0.8f))
        {
            tile.TemperatureType = TemperatureType.Hot;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.3f, temperature * 0.6f))
        {
            tile.TemperatureType = TemperatureType.Mild;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.1f, temperature * 0.3f))
        {
            tile.TemperatureType = TemperatureType.Cold;
        }
        else if (tempNoiseMap[x, y] < temperature * 0.1f)
        {
            tile.TemperatureType = TemperatureType.VeryCold;
        }
    }

    private void AssignWaterLevel(int x, int y, Tile tile)
    {
        if (waterHeightMap[x, y] > waterHeight)
        {
            if (waterHeightMap[x, y] > waterHeight*1.5f - hilliness)
            {
                tile.HeightType = HeightType.Mountain;
            } else if (waterHeightMap[x, y] > waterHeight*1.2f - hilliness)
            {
                tile.HeightType = HeightType.Hill;
            }
            else
            {
                tile.HeightType = HeightType.Flat;
            }
        }
        else if (waterHeightMap[x, y] < waterHeight * deepWaterThreshold)
        {
            tile.HeightType = HeightType.DeepWater;
        }
        else if (waterHeightMap[x, y] < waterHeight)
        {
            tile.HeightType = HeightType.Water;
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
                    case NoiseLayers.Height:
                        switch (tile.HeightType)
                        {
                            case HeightType.Flat:
                                tile.SetColor(cS.flat);
                                break;
                            case HeightType.Hill:
                                tile.SetColor(cS.hill);
                                break;
                            case HeightType.Mountain:
                                tile.SetColor(cS.mountain);
                                break;
                            case HeightType.Water:
                                tile.SetColor(cS.water);
                                break;
                            case HeightType.DeepWater:
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
                            case BiomeType.RainForest:
                                tile.SetColor(cS.rainForestColor);
                                break;
                            case BiomeType.Swamp:
                                tile.SetColor(cS.swampColor);
                                break;
                            case BiomeType.SeasonalForest:
                                tile.SetColor(cS.seasonalForestColor);
                                break;
                            case BiomeType.Forest:
                                tile.SetColor(cS.forestColor);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(cS.savannaColor);
                                break;
                            case BiomeType.Shrubland:
                                tile.SetColor(cS.shrublandColor);
                                break;
                            case BiomeType.Taiga:
                                tile.SetColor(cS.taigaColor);
                                break;
                            case BiomeType.Desert:
                                tile.SetColor(cS.desertColor);
                                break;
                            case BiomeType.Plains:
                                tile.SetColor(cS.plainsColor);
                                break;
                            case BiomeType.IceDesert:
                                tile.SetColor(cS.iceDesertColor);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(cS.tundraColor);
                            break;
                        }
                        break;
                    case NoiseLayers.Combined:
                        switch (tile.BiomeType)
                        {
                            case BiomeType.RainForest:
                                tile.SetColor(cS.rainForestColor);
                                break;
                            case BiomeType.Swamp:
                                tile.SetColor(cS.swampColor);
                                break;
                            case BiomeType.SeasonalForest:
                                tile.SetColor(cS.seasonalForestColor);
                                break;
                            case BiomeType.Forest:
                                tile.SetColor(cS.forestColor);
                                break;
                            case BiomeType.Savanna:
                                tile.SetColor(cS.savannaColor);
                                break;
                            case BiomeType.Shrubland:
                                tile.SetColor(cS.shrublandColor);
                                break;
                            case BiomeType.Taiga:
                                tile.SetColor(cS.taigaColor);
                                break;
                            case BiomeType.Desert:
                                tile.SetColor(cS.desertColor);
                                break;
                            case BiomeType.Plains:
                                tile.SetColor(cS.plainsColor);
                                break;
                            case BiomeType.IceDesert:
                                tile.SetColor(cS.iceDesertColor);
                                break;
                            case BiomeType.Tundra:
                                tile.SetColor(cS.tundraColor);
                                break;
                        }

                        switch (tile.HeightType)
                        {
                            case HeightType.Water:
                                tile.SetColor(cS.water);
                                break;
                            case HeightType.DeepWater:
                                tile.SetColor(cS.deepWater);
                                break;
                        }
                        break;
                    case NoiseLayers.GrayScaleHeight:
                        tile.SetColor(Color.Lerp(Color.white, Color.black, waterHeightMap[x,y]));
                        break;
                        
                }
            }
        }
    }

    // Helpers
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

}
