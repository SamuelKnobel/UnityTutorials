﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings,int levelOfDetail)
    {
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

    int numVertsPerLine = meshSettings.numVertsPerLine;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        MeshData meshData = new MeshData(numVertsPerLine,skipIncrement, meshSettings.useFlatShading);

        int[,] vertexIndecisMap = new int[numVertsPerLine, numVertsPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        for (int y = 0; y < numVertsPerLine; y ++)
        {
            for (int x = 0; x < numVertsPerLine; x ++)
            {
                bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                bool isSkipedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3&& ((x-2) % skipIncrement!=0 || (y - 2) % skipIncrement != 0);
    
                if (isOutOfMeshVertex)
                {
                    vertexIndecisMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if(!isSkipedVertex)
                {
                    vertexIndecisMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < numVertsPerLine; y ++)
        {
            for (int x = 0; x < numVertsPerLine; x ++)
            {
                bool isSkipedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);
                if (!isSkipedVertex)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndecisMap[x, y];
                    Vector2 percent = new Vector2(x - 1, y-1) /(numVertsPerLine-3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x,-percent.y) * meshSettings.meshWorldSize;
                    float height = heightMap[x, y];
                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int dstToMainVertexA = ((isVertical)?y - 2:x-2) % skipIncrement;
                        int dstToMainVertexB = skipIncrement-dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;


                        float heighMainVertexA = heightMap[(isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y];
                        float heighMainVertexB = heightMap[(isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y];

                        height = heighMainVertexA * (1 - dstPercentFromAToB) + heighMainVertexB * (dstPercentFromAToB);
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    bool createTrianlge = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));


                    if (createTrianlge)
                    {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;
                        int a = vertexIndecisMap[x, y];
                        int b = vertexIndecisMap[x + currentIncrement, y];
                        int c = vertexIndecisMap[x, y + currentIncrement];
                        int d = vertexIndecisMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
        }
        meshData.ProcessMesh();
        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertecies;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;
    bool useFlatshading;

    public MeshData(int numVertsPerLine,int skipIncrement, bool useFlatshading)
    {
        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) / (numVertsPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement * 4 + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        vertices = new Vector3[numMeshEdgeVertices+ numEdgeConnectionVertices+ numMainVertices];
        uvs = new Vector2[vertices.Length];

        int numMeshEdgeTriangles = 8*(numVertsPerLine - 3)  - 4 ; //((numVertsPerLine - 3) * 4 - 4) * 2;
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;

        triangles = new int[3*(numMeshEdgeTriangles+ numMainTriangles)];

        this.useFlatshading = useFlatshading;
        outOfMeshVertecies = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24*(numVertsPerLine - 2)];/* new int[((numVertsPerLine -1)*4-4)*2*3];*/

    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            outOfMeshVertecies[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

    }

    public Vector3[] CaclulateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangelIndex = i * 3;
            int vertexIndexA = triangles[normalTriangelIndex];
            int vertexIndexB = triangles[normalTriangelIndex + 1];
            int vertexIndexC = triangles[normalTriangelIndex + 2];
            Vector3 triangleNormal = SurfaceNormalFromIndecies(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }
        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangelIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangelIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangelIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangelIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndecies(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0)
                vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0)
                vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0)
                vertexNormals[vertexIndexC] += triangleNormal;
        }
        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }


    Vector3 SurfaceNormalFromIndecies(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = indexA < 0 ? outOfMeshVertecies[-indexA - 1] : vertices[indexA];
        Vector3 pointB = indexB < 0 ? outOfMeshVertecies[-indexB - 1] : vertices[indexB];
        Vector3 pointC = indexC < 0 ? outOfMeshVertecies[-indexC - 1] : vertices[indexC];


        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh()
    {
        if (useFlatshading)
        {
            FlatShading();
        }
        else
            bakeNormals();
    }
    void bakeNormals()
    {
        bakedNormals = CaclulateNormals();
    }

    void FlatShading()
    {
        Vector3[] flatShadedVerties = new Vector3[triangles.Length];
        Vector2[] flatShadedUVs = new Vector2[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVerties[i] = vertices[triangles[i]];
            flatShadedUVs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }
        vertices = flatShadedVerties;
        uvs = flatShadedUVs;
    }

    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        if (useFlatshading)
        {
            mesh.RecalculateNormals();
        }
        else
            mesh.normals = bakedNormals;
        return mesh;
    }
}


