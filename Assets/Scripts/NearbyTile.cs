using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyTile
{
    private int _direction;
    private Tile _tileData;

    public NearbyTile(int direction, Tile tileData)
    {
        _direction = direction;
        _tileData = tileData;
    }

    public int Direction
    {
        get => _direction;
        set => _direction = value;
    }

    public Tile TileData
    {
        get => _tileData;
        set => _tileData = value;
    }
}
