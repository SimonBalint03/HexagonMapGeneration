using System;
using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;

public class LandModelAssigner : MonoBehaviour
{
    private GameObject flatPrefab;
    private GameObject hillPrefab;
    private GameObject waterPrefab;
    private GameObject deepWaterTile;
    private GameObject mountainTile;

    private GameObject[,] _tiles;
    private GridCreator GridCreator;

    private void Start()
    {
        GridCreator = GetComponent<GridCreator>();

        flatPrefab = GridCreator.defaultTile;
        hillPrefab = GridCreator.hillTile;
        waterPrefab = GridCreator.waterTile;
        deepWaterTile = GridCreator.deepWaterTile;
        mountainTile = GridCreator.mountainTile;
    }

    public void RefreshLandModels()
    {
        AssignFlats();
        AssignHills();
        AssignWaters();
    }

    private void AssignHills()
    {
        _tiles = GridCreator.GetTiles();
        foreach (GameObject tile in _tiles)
        {
            if (tile.GetComponent<Tile>().HeightType == HeightType.Hill)
            {
                tile.GetComponent<MeshFilter>().mesh = hillPrefab.GetComponent<MeshFilter>().sharedMesh;
            } else if (tile.GetComponent<Tile>().HeightType == HeightType.Mountain)
            {
                tile.GetComponent<MeshFilter>().mesh = mountainTile.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
    
    private void AssignFlats()
    {
        _tiles = GridCreator.GetTiles();
        foreach (GameObject tile in _tiles)
        {
            if (tile.GetComponent<Tile>().HeightType == HeightType.Flat)
            {
                tile.GetComponent<MeshFilter>().mesh = flatPrefab.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
    private void AssignWaters()
    {
        _tiles = GridCreator.GetTiles();
        foreach (GameObject tile in _tiles)
        {
            if (tile.GetComponent<Tile>().HeightType == HeightType.Water)
            {
                tile.GetComponent<MeshFilter>().mesh = waterPrefab.GetComponent<MeshFilter>().sharedMesh;
            } else if (tile.GetComponent<Tile>().HeightType == HeightType.DeepWater)
            {
                tile.GetComponent<MeshFilter>().mesh = deepWaterTile.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}
