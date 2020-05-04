using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        Local, Global
    };


    /// <summary>
    /// Generates a Nosie Map using Perlin Noise
    /// increasing Lacunarity : more small features
    /// Persistance: how much do those small features influence the overall shape
    /// Based on https://www.youtube.com/watch?v=wbpMiKiSKm8
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
    /// <param name="seed"></param>
    /// <param name="scale"></param>
    /// <param name="octaves"> represents the number of Layers</param>
    /// <param name="persistance">Controls decrease in amplitude of octaves/Layers (Amplitude = persistance ^ LayerNumber -1), Range[0-1]</param>
    /// <param name="lacunarity">Controls increase in frequency of octaves (Frequ = Lacunarity ^ LayerNumber-1), Range >1 </param>
    /// <param name="offset">To scroll trough the Land</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings,Vector2 sampleCenter)
    {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffests = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x+ sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y- sampleCenter.y;
            octaveOffests[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNosieHeight = float.MinValue;
        float minLocalNosieHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffests[i].x) / settings.scale * frequency; // more zoomed out -->hight values change more rapidly! Offest makes that the values are taken from different points for each octave
                    float sampleY = (y - halfHeight + octaveOffests[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Per Default Perlin Noise Range between 0 and 1 -->to make it more intresting and add negative values --> *2 -1
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }
                if (noiseHeight > maxLocalNosieHeight)
                    maxLocalNosieHeight = noiseHeight;
                if (noiseHeight < minLocalNosieHeight)
                    minLocalNosieHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 2f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHight, 0, int.MaxValue);
                }

            }
        }

        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNosieHeight, maxLocalNosieHeight, noiseMap[x, y]);
                }
            }
        }
            // Normalize Noise Map to Values beetween 0 - 1

        return noiseMap;
    }


}
[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;
    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = .6f;
    public float lacunarity = 2;
    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);

    }

}
