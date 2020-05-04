using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 5f;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;
    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    HeightMap heightMap;
    bool heightMapRecived;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int collidorLODIndex;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDist;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;


    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int collidorLODIndex, Transform parent,Transform viewer, Material material)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.detailLevels = detailLevels;
        this.collidorLODIndex = collidorLODIndex;
        this.viewer = viewer;

        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("TerrainChunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == collidorLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

    }


    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(
    meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCenter), OnHeightMapRecieved);
    }

    void OnHeightMapRecieved(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapRecived = true;

        UpdateTerrainChunk();
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }



    public void UpdateTerrainChunk()
    {
        if (heightMapRecived)
        {
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = isVisible();
            bool visible = viewerDistFromNearestEdge <= maxViewDist;
            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }
                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap,meshSettings);
                    }
                }
            }
            if (wasVisible != visible)
            {
                SetVisible(visible);

                if (onVisibilityChanged!= null)
                {
                    onVisibilityChanged(this, visible);
                }
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDistanceFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDistanceFromViewerToEdge < detailLevels[collidorLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[collidorLODIndex].hasRequestedMesh)
                {
                    lodMeshes[collidorLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (sqrDistanceFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[collidorLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[collidorLODIndex].mesh;
                    hasSetCollider = true;
                }

            }
        }

    }
    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }
    public bool isVisible()
    {
        return meshObject.activeSelf;

    }

}

class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataRecived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).createMesh();

        hasMesh = true;
        updateCallback();
    }


    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecived);
    }
}
