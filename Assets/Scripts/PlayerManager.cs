using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject prefab;
    
    private GameObject[,] _tiles;
    private GameObject _startingTile;

    void Start()
    {
        _tiles = FindObjectOfType<GridCreator>().GetTiles();
        InitPlayer();
    }

    private void InitPlayer()
    {
        int cntr = 0;
        while (cntr < 10000)
        {
            int randX = Random.Range(0,_tiles.GetLength(0));
            int randY = Random.Range(0,_tiles.GetLength(1));
            if (_tiles[randX,randY].GetComponent<Tile>().WaterType == WaterType.Land)
            {
                _startingTile = _tiles[randX, randY];
                // Init player
                Instantiate(prefab, _startingTile.transform.position, prefab.transform.rotation);
                return;
            }

            cntr++;
        }
        _startingTile = _tiles[Random.Range(0, _tiles.GetLength(0)), Random.Range(0, _tiles.GetLength(1))];
        // Init player
        Instantiate(prefab, _startingTile.transform.position, prefab.transform.rotation);
    }
}
