using System;
using System.Collections;
using System.Collections.Generic;
using TileTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public HeightType heightType;
    public TemperatureType temperatureType;
    public BiomeType biomeType;
    public RainfallType rainfallType;
    public List<NearbyTile> nearbyTiles;
    
    public bool isRiver;
    public bool isCity;
    public bool isForest;
    
    private Material groundMat;
    private TileSide[] _sides = new TileSide[6] { TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal ,TileSide.Normal};
    
    private void Awake()
    {
        groundMat = GetComponent<MeshRenderer>().materials[0];
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), position, (int)heightType, (int)temperatureType, (int)biomeType, (int)rainfallType, isRiver, isCity);
    }

    public override bool Equals(object other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != this.GetType()) return false;
        return Equals((Tile)other);
    }

    protected bool Equals(Tile other)
    {
        return base.Equals(other) && position.Equals(other.position) && heightType == other.heightType && temperatureType == other.temperatureType && biomeType == other.biomeType && rainfallType == other.rainfallType && isRiver == other.isRiver && isCity == other.isCity;
    }

    private List<Tile> FindNearbyTiles(List<Tile> list)
    {
        List<Tile> result = new List<Tile>();

        foreach (Tile tile in list)
        {
            foreach (NearbyTile nearbyTile in tile.nearbyTiles)
            {
                if (!list.Contains(nearbyTile.TileData))
                {
                    result.Add(nearbyTile.TileData);
                }
            }
        }
        result.AddRange(list);

        return result;
    }
    
    public List<Tile> GetNearbyTilesInRange(int range)
    {
        List<Tile> result = new List<Tile>();
        foreach (NearbyTile nearbyTile in nearbyTiles)
        {
            result.Add(nearbyTile.TileData);
        }

        for (int i = 0; i < range-1; i++)
        {
            result = FindNearbyTiles(result);
        }
        
        return result;
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

    public void SetRiver(bool state)
    {
        isRiver = state;
    }

    public bool IsWater()
    {
        if (heightType is HeightType.Water or HeightType.DeepWater)
        {
            return true;
        }

        return false;
    }

    public bool HasWaterNearby()
    {
        foreach (NearbyTile nTile in nearbyTiles)
        {
            if (nTile.TileData.IsWater()) { return true; }
        }

        return false;
    }
    
    public bool HasRiverNearby()
    {
        foreach (NearbyTile nTile in nearbyTiles)
        {
            if (nTile.TileData.isRiver) { return true; }
        }

        return false;
    }

    // Doesn't work for water
    public bool IsBelowHeightOrEqual(HeightType height)
    {
        switch (heightType)
        {
            case HeightType.Mountain:
                return true;
            case HeightType.Hill:
                return height is HeightType.Hill or HeightType.Flat;
            case HeightType.Flat:
                return height == HeightType.Flat;
            case HeightType.Water or HeightType.DeepWater:
                return true;
        }

        return false;
    }

}
