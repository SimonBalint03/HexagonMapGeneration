    using System;
using System.Collections;
using System.Collections.Generic;
    using TileTypes;
    using UnityEngine;

public class Tile : MonoBehaviour
{
    private Material groundMat;

    private void Awake()
    {
        groundMat = GetComponent<MeshRenderer>().materials[0];
    }
    
    public void SetColor(Color newColor)
    {
        groundMat.color = newColor;
    }
    
    public HeightType HeightType { get; set; }
    public TemperatureType TemperatureType { get; set; }
    public BiomeType BiomeType { get; set; }
    public RainfallType RainfallType { get; set; }
}
