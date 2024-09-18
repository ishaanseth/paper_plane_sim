using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PaperMeshGenerator : MonoBehaviour
{
    Mesh mesh;
    int[] triangles;
    Vector3[] vertices;
    public float cellSize = 10f;
    public int height = 30;
    public int width = 20;
    public Vector3 gridOffset;

    void Awake()
    {
        // Create a new mesh instance and assign it to the MeshFilter
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Start()
    {
        MakeDiscreteProceduralGrid();
        UpdateMesh();
    }

    void MakeDiscreteProceduralGrid()
    {
        // Create vertices
        vertices = new Vector3[height * width * 4];
        triangles = new int[height * width * 6];

        // Set tracker integers
        int v = 0;
        int t = 0;

        // Calculate offsets to center the grid
        float vertexOffset = cellSize * 0.5f;
        
        // Generate vertices
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellOffset = new Vector3(x * cellSize, y * cellSize, 0);
                
                vertices[v+0] = new Vector3(-vertexOffset, -vertexOffset, 0) + cellOffset + gridOffset; // Flat grid in the XY plane
                vertices[v+1] = new Vector3(-vertexOffset,  vertexOffset, 0) + cellOffset + gridOffset;
                vertices[v+2] = new Vector3( vertexOffset, -vertexOffset, 0) + cellOffset + gridOffset;
                vertices[v+3] = new Vector3( vertexOffset,  vertexOffset, 0) + cellOffset + gridOffset;


                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;
                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + 3;

                v += 4;
                t += 6;
            }
        }
    }

    void UpdateMesh()
    {
        // Clear the mesh to avoid overlapping data
        mesh.Clear();

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals to make sure lighting works correctly
        mesh.RecalculateNormals();
    }
}
