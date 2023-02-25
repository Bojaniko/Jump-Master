using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    
    public static float[,] GenerateNoiseMap(int width, int height, float scale, Vector2 offset)
    {
        float[,] map = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = x * scale + offset.x;
                float yCoord = y * scale + offset.y;

                map[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
                //Debug.Log(map[x, y]);
            }
        }

        return map;
    }
}