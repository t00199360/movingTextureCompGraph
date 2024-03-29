﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{


    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    // Start is called before the first frame update
    void Start()
    {
        MakeCube();
        UpdateMesh();
    }

    void MakeCube()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            MakeFace(i);
        }
    }

    void MakeFace(int dir)
    {
        vertices.AddRange(testOutcode.faceVertices(dir));
        int vCount = vertices.Count;

        triangles.Add(vCount -4);
        triangles.Add(vCount - 4 + 1);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4 + 3);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
