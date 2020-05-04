using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveTreshholdForChunkUpdate = 25f;
    const float sqrviewerMoveTreshholdForChunkUpdate = viewerMoveTreshholdForChunkUpdate * viewerMoveTreshholdForChunkUpdate;
    
    public int collidorLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;


    public Transform viewer;

    public Material mapMaterial;
    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunksVisibleInViewDist;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    private void Start()
    {
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        float maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        UpdateVisibleChunks();

    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveTreshholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();

        }
    }
    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currenChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currenChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewdChunkCord = new Vector2(currenChunkCoordX + xOffset, currenChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewdChunkCord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewdChunkCord))
                    {
                        terrainChunkDictionary[viewdChunkCord].UpdateTerrainChunk();
                    }
                    else
                    {

                        TerrainChunk newChunk = new TerrainChunk(viewdChunkCord, heightMapSettings, meshSettings, detailLevels, collidorLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewdChunkCord,newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkvisibilityChanged;
                        newChunk.Load();


                    }
                }
            }
        }
    }    
    void OnTerrainChunkvisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
            visibleTerrainChunks.Remove(chunk);
    }
}
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLOD - 1)]
    public int lod;
    public float visibleDstThreshold; // range within this LOD is valid, outside the next lower LOD is displayed
    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }

}