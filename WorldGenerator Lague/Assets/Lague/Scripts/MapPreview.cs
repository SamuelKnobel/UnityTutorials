using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapPreview : MonoBehaviour
{





    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;




    public enum DrawMode { NoiseMap, Mesh, FallOfMap };
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;
    public Material terrainMaterial;



    [Range(0, MeshSettings.numSupportedLOD - 1)]
    public int editiorPreviewLOD;

    public bool autoUpdate;



    public void DrawMapInEditor()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

        if (drawMode == DrawMode.NoiseMap)       {
            DrawTexture(TextureGenerator.TextureFromHeigthMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)       {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editiorPreviewLOD));
        }
        else if (drawMode == DrawMode.FallOfMap)       {
            DrawTexture(TextureGenerator.TextureFromHeigthMap(new HeightMap(FallOffGenerator.generateFallOffMap(meshSettings.numVertsPerLine),0,1)));
        }
    }
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height)/10f;
        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {

        meshFilter.sharedMesh = meshData.createMesh();
        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }


    private void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;

        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }
}
