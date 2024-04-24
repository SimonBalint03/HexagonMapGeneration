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
    [Range(0.1f, 2)] public float hilliness;
    
    
    
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

    private float[,] hillNoiseMap;
    private HillMap _hillMap;

    private RiverGenerator _riverGenerator;

    private CityGenerator _cityGenerator;

    [Header("Forests")] public GameObject forestPrefab;
    private ForestGenerator _forestGenerator;

    private LandModelAssigner LandModelAssigner;
    
    [Header("DEBUG")] public NoiseLayers noiseLayers;
    public GameObject riverDebugPoint;
    public GameObject cityDebugPoint;

    private void OnValidate()
    {
        AssignTypes();
        RefreshColors();
    }

    private void Start()
    {
        _temperatureMap = new TemperatureMap();
        _rainfallMap = new RainfallMap();
        _hillMap = new HillMap();
        _riverGenerator = gameObject.AddComponent<RiverGenerator>();
        _cityGenerator = gameObject.AddComponent<CityGenerator>();
        _forestGenerator = gameObject.AddComponent<ForestGenerator>();
        
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
        hillNoiseMap = _hillMap.GenerateHillMap(width, height, 5);
        AssignTypes();
        RefreshColors();
        
        _riverGenerator.GenerateRiverData(_gridCreator.GetTilesAsTile(),riverDebugPoint);
        _cityGenerator.GenerateCityData(_gridCreator.GetTilesAsTile(),cityDebugPoint);
        _forestGenerator.GenerateForests(_gridCreator.GetTilesAsTile(),forestPrefab);
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
                tile.biomeType = BiomeType.RainForest;
            }
            else if (rainNoiseMap[x, y] > 0.5f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.SeasonalForest;
            }
            else if (rainNoiseMap[x, y] > 0.2f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.Plains;
            }
            else
            {
                tile.biomeType = BiomeType.Desert;
            }
        }
        else if (tempNoiseMap[x, y] > 0.4f * Random.Range(0.9f,1f) * temperature)
        {
            if (rainNoiseMap[x, y] > 0.8f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.Swamp;
            }
            else if (rainNoiseMap[x, y] > 0.2f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.SeasonalForest;
            }
            else if (rainNoiseMap[x, y] > 0.1f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.Shrubland;
            }
            else
            {
                tile.biomeType = BiomeType.Savanna;
            }
        }
        else if (tempNoiseMap[x, y] > 0.1f * Random.Range(0.9f,1f) * temperature)
        {
            if (rainNoiseMap[x, y] > 0.35f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.SeasonalForest;
            }
            else if (rainNoiseMap[x, y] > 0.15f * Random.Range(0.9f,1f) * rainfall)
            {
                tile.biomeType = BiomeType.Forest;
            }
            else
            {
                tile.biomeType = BiomeType.Plains;
            }
        }
        else if (rainNoiseMap[x, y] > 0.15f * Random.Range(0.9f,1f) * rainfall)
        {
            tile.biomeType = BiomeType.Taiga;
        }
        else if (rainNoiseMap[x, y] > 0.08f * Random.Range(0.9f,1f) * rainfall)
        {
            tile.biomeType = BiomeType.Tundra;
        }
        else
        {
            tile.biomeType = BiomeType.IceDesert;
        }
    }

    private void AssignRainfall(int x, int y, Tile tile)
    {
        if (rainNoiseMap[x, y] > rainfall * 0.8f)
        {
            tile.rainfallType = RainfallType.LotsRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.65f, rainfall * 0.8f))
        {
            tile.rainfallType = RainfallType.MuchRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.2f, rainfall * 0.65f))
        {
            tile.rainfallType = RainfallType.MildRain;
        }
        else if (rainNoiseMap[x, y].Between(rainfall * 0.05f, rainfall * 0.2f))
        {
            tile.rainfallType = RainfallType.FewRain;
        }
        else if (rainNoiseMap[x, y] < rainfall * 0.05f)
        {
            tile.rainfallType = RainfallType.NoRain;
        }
    }

    private void AssignTemperature(int x, int y, Tile tile)
    {
        if (tempNoiseMap[x, y] > temperature * 0.8f)
        {
            tile.temperatureType = TemperatureType.VeryHot;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.6f, temperature * 0.8f))
        {
            tile.temperatureType = TemperatureType.Hot;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.3f, temperature * 0.6f))
        {
            tile.temperatureType = TemperatureType.Mild;
        }
        else if (tempNoiseMap[x, y].Between(temperature * 0.1f, temperature * 0.3f))
        {
            tile.temperatureType = TemperatureType.Cold;
        }
        else if (tempNoiseMap[x, y] < temperature * 0.1f)
        {
            tile.temperatureType = TemperatureType.VeryCold;
        }
    }

    private void AssignWaterLevel(int x, int y, Tile tile)
    {
        if (waterHeightMap[x, y] > waterHeight)
        {
            if (hillNoiseMap[x,y] * hilliness > 0.9f)
            {
                tile.heightType = HeightType.Mountain;
            }else if (hillNoiseMap[x, y] * hilliness > 0.7f)
            {
                tile.heightType = HeightType.Hill;
            }
            else
            {
                tile.heightType = HeightType.Flat;
            }
        }
        else if (waterHeightMap[x, y] < waterHeight * deepWaterThreshold * Random.Range(0.95f,1f))
        {
            tile.heightType = HeightType.DeepWater;
        }
        else if (waterHeightMap[x, y] < waterHeight)
        {
            tile.heightType = HeightType.Water;
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
                        switch (tile.heightType)
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
                        switch (tile.temperatureType)
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
                        switch (tile.rainfallType)
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
                        switch (tile.biomeType)
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
                        switch (tile.biomeType)
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

                        switch (tile.heightType)
                        {
                            case HeightType.Water:
                                tile.SetColor(cS.water);
                                break;
                            case HeightType.DeepWater:
                                tile.SetColor(cS.deepWater);
                                break;
                            case HeightType.Mountain:
                                tile.SetColor(cS.mountain);
                                break;
                        }
                        break;
                    case NoiseLayers.GrayScaleHeight:
                        switch (tile.heightType)
                        {
                            case HeightType.Water or HeightType.DeepWater:
                                tile.SetColor(Color.Lerp(Color.white, Color.black, waterHeightMap[x, y]));
                                break;
                            default:
                                tile.SetColor(Color.Lerp(Color.white, Color.black,
                                    Mathf.Max(waterHeightMap[x, y], hillNoiseMap[x, y])));
                                break;
                        }
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
