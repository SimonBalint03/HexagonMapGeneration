using System;
using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public HeightType heightType;
    public TemperatureType temperatureType;
    public BiomeType biomeType;
    public RainfallType rainfallType;
    public List<GameObject> nearbyTiles;
    
    private Material groundMat;
    private TileSide[] _sides = new TileSide[6] { TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal};
    
    private void Awake()
    {
        groundMat = GetComponent<MeshRenderer>().materials[0];
    }
    
    public void SetColor(Color newColor)
    {
        groundMat.color = newColor;
    }

    public void SetSide(TileSide tileSide, int side)
    {
        if (side > 5)
        {
            throw new Exception("Value: side needs to be smaller than 5.");
        }

        _sides[side] = tileSide;
    }

    public bool IsWater()
    {
        if (heightType is HeightType.Water or HeightType.DeepWater)
        {
            return true;
        }

        return false;
    }
    

}
