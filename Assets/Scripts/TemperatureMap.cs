using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureMap : MonoBehaviour
{
    public float[,] GenerateTemperatureMap(int width, int height, float scale)
    {
        float[,] temperatureMap = new float[width, height];

        // Calculate the center of the map
        float centerY = height / 2f;
        //Random seed for the map
        Random.InitState((int)System.DateTime.Now.Ticks);

        Vector2 offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the distance from the top and bottom rows
                float distanceToTopBottom = Mathf.Min(y, height - 1 - y);

                // Normalize the distance to get a value between 0 and 1
                float normalizedDistance = Mathf.Clamp01(distanceToTopBottom / (height / 2f));

                // Use Perlin noise for additional variation
                float perlinValue = Mathf.PerlinNoise(x / scale + offset.x, y / scale + offset.y);

                // Assign temperature based on the normalized distance and noise
                temperatureMap[x, y] = Mathf.Lerp(1f - normalizedDistance, perlinValue, Random.Range(0.05f,0.2f));
            }
        }
        return temperatureMap;
    }
}
