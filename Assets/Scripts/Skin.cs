using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class PaperSkinnedMeshGenerator : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh mesh;
    int[] triangles;
    Vector3[] vertices;
    public float cellSize = 10f;
    public int height = 30;
    public int width = 20;
    public Vector3 gridOffset;

    // New fields for bones and weights
    Transform[] bones;
    Matrix4x4[] bindPoses;
    BoneWeight[] boneWeights;

    void Awake()
    {
        // Get or create SkinnedMeshRenderer and mesh
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        mesh = new Mesh();
        skinnedMeshRenderer.sharedMesh = mesh;

        // Create bones based on the grid size
        CreateBones();
    }

    void Start()
    {
        MakeDiscreteProceduralGrid();
        UpdateMesh();
    }

    void CreateBones()
    {
        // We will create a bone per vertex row (one for each row in the grid)
        bones = new Transform[height];
        bindPoses = new Matrix4x4[height];

        for (int i = 0; i < height; i++)
        {
            // Create a new GameObject for each bone
            GameObject bone = new GameObject("Bone" + i);
            bone.transform.parent = transform;
            bone.transform.localPosition = new Vector3(0, i * cellSize, 0); // Set bone position based on grid row
            bone.transform.localRotation = Quaternion.identity;

            bones[i] = bone.transform;
            bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }

        // Set the bones and bind poses in the SkinnedMeshRenderer
        skinnedMeshRenderer.bones = bones;
        skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
    }

    void MakeDiscreteProceduralGrid()
    {
        // Create vertices
        vertices = new Vector3[height * width * 4];
        triangles = new int[height * width * 6];
        boneWeights = new BoneWeight[vertices.Length];

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
                
                vertices[v + 0] = new Vector3(-vertexOffset, -vertexOffset, 0) + cellOffset + gridOffset; // Flat grid in the XY plane
                vertices[v + 1] = new Vector3(-vertexOffset,  vertexOffset, 0) + cellOffset + gridOffset;
                vertices[v + 2] = new Vector3( vertexOffset, -vertexOffset, 0) + cellOffset + gridOffset;
                vertices[v + 3] = new Vector3( vertexOffset,  vertexOffset, 0) + cellOffset + gridOffset;

                // Create triangles
                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;
                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + 3;

                // Assign bone weights (each row is affected by a bone)
                AssignBoneWeights(v, y);

                v += 4;
                t += 6;
            }
        }
    }

    void AssignBoneWeights(int vertexIndex, int boneIndex)
    {
        // Set bone weight for the vertices to be influenced by one bone (you can customize weights for smoother blending)
        boneWeights[vertexIndex + 0].boneIndex0 = boneIndex;
        boneWeights[vertexIndex + 0].weight0 = 1.0f;

        boneWeights[vertexIndex + 1].boneIndex0 = boneIndex;
        boneWeights[vertexIndex + 1].weight0 = 1.0f;

        boneWeights[vertexIndex + 2].boneIndex0 = boneIndex;
        boneWeights[vertexIndex + 2].weight0 = 1.0f;

        boneWeights[vertexIndex + 3].boneIndex0 = boneIndex;
        boneWeights[vertexIndex + 3].weight0 = 1.0f;
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

        // Assign bone weights
        mesh.boneWeights = boneWeights;

        // Update the skinned mesh renderer with the new mesh
        skinnedMeshRenderer.sharedMesh = mesh;

        skinnedMeshRenderer.updateWhenOffscreen = true;
    }
}
