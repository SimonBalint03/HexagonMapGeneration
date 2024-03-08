    using System;
using System.Collections;
using System.Collections.Generic;
    using TileTypes;
    using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetColor(Color newColor)
    {
        _spriteRenderer.color = newColor;
    }
    
    public WaterType WaterType { get; set; }

    public ClimateType ClimateType { get; set; }

    public BiomeType BiomeType { get; set; }
}
