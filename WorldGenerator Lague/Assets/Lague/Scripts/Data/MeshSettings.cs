﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{

    public const int numSupportedLOD = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    public static readonly int[] supportedFlatshadedChunkSizes = { 48, 72, 96 };

    public float meshScale = 2.5f; // scales everything
    public bool useFlatShading;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    public int flatshadedChunkSizeIndex;

    // Number of Verts per line of mesh rendered at LOD 0. Include the 2 extra verts, that are excluded from final mesh, but used for calculation normals
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] +5;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 1 - 2) * meshScale;

        }
    }
}