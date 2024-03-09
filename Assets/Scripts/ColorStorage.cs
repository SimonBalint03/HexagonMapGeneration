using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorStorage : MonoBehaviour
{
    [Header("Water Layer")]
    public Color water;
    public Color deepWater;
    public Color land;
    
    [Header("Temperature Layer")]
    public Color veryCold;
    public Color cold;
    public Color mildTemp;
    public Color hot;
    public Color veryHot;
    
    [Header("Temperature Layer")]
    public Color noRain;
    public Color fewRain;
    public Color mildRain;
    public Color muchRain;
    public Color lotsRain;
    
    [Header("Biome Colors")]
    public Color tropicalRainForest;
    public Color savanna;
    public Color desert;
    
    public Color rainForest;
    public Color forest;
    public Color temperateForest;
    
    public Color taiga;
    public Color tundra;
    public Color ice;
    
}
