using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainfallMap
{
    public float[,] GenerateRainfallMap(int width, int height, float scale)
    {
        float[,] precipitationMap = new float[width, height];

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
                float normalizedDistance = Mathf.Clamp01(distanceToTopBottom / (height * 0.5f));

                // Use Perlin noise for the first layer of precipitation
                float perlinValue1 = Mathf.PerlinNoise(x / scale + offset.x, y / scale + offset.y);

                // Use another layer of Perlin noise with a different scale for additional variation
                float perlinValue2 = Mathf.PerlinNoise(x / (scale * 2) + offset.x, y / (scale * 2) + offset.y);

                // Combine the two noise layers with subtraction and adjust amplitude to control overall precipitation
                float precipitation = Mathf.Clamp01(perlinValue1 - perlinValue2 );

                // Blend the precipitation based on the normalized distance to the top and bottom
                precipitationMap[x, y] = Mathf.Lerp(0f, 1f,
                    normalizedDistance * Random.Range(0.9f, 1f)) - precipitation * 2f;
            }
        }
        return precipitationMap;
    }
}
