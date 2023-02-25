using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Noise Map")]
    public int MapWidth = 10;
    public int MapHeight = 10;
    public float MapScale = 1;
    public Vector2 MapOffset;

    [Header("Generation")]
    [Range(0f, 1f)]
    public float FillValue = 0.5f;

    public GameObject BlockTest;

    private float[,] _noiseMap;

    void Start()
    {
        _noiseMap = NoiseGenerator.GenerateNoiseMap(MapWidth, MapHeight, MapScale, MapOffset);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                //Debug.Log(_noiseMap[x, y]);
                if (_noiseMap[x, y] <= FillValue)
                {
                    Instantiate(BlockTest, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }

        _noiseMap = NoiseGenerator.GenerateNoiseMap(MapWidth, MapHeight, MapScale, new Vector2(10, 10));

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                //Debug.Log(_noiseMap[x, y]);
                if (_noiseMap[x, y] <= FillValue)
                {
                    Instantiate(BlockTest, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
