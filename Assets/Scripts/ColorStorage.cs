using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorStorage : MonoBehaviour
{
    [Header("Height Layer")]
    public Color deepWater;
    public Color water;
    public Color flat;
    public Color hill;
    public Color mountain;
    
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
    public Color rainForestColor;
    public Color swampColor;
    public Color seasonalForestColor;
    public Color forestColor;
    public Color savannaColor;
    public Color shrublandColor;
    public Color taigaColor;
    public Color desertColor;
    public Color plainsColor;
    public Color iceDesertColor;
    public Color tundraColor;    
}
