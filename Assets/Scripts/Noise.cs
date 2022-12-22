using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public enum NormalizeMode {Local,Golbal};
    public static float[,] GenerateNoiseMap(int mapWitdth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) 
    {
        float[,] noiseMap = new float[mapWitdth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] oactaveOffsets = new Vector2[octaves];

        float maxPossibleheight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) 
        {
            float offSetX = prng.Next(-10000,10000) + offset.x;
            float offSetY = prng.Next(-10000,10000) - offset.y;
            oactaveOffsets[i] = new Vector2(offSetX,offSetY);

            maxPossibleheight +=amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) 
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWitdth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) 
        {
            for (int x = 0; x < mapWitdth; x++) 
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) 
                {
                    float sampleX = (x - halfWidth + oactaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + oactaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) 
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight) 
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++) 
        {
            for (int x = 0; x < mapWitdth; x++) 
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x,y] + 1) / (maxPossibleheight);
                    noiseMap[x,y] = Mathf.Clamp(normalizedHeight,0 , int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
