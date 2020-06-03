using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetraedre : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshUtility meshUtility;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        meshUtility = new MeshUtility();
        DebugGraph.meshUtility = meshUtility;

        Vector3 base1 = new Vector3(0, -0.5f, 0.5f);
        Vector3 base2 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 base3 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 top = new Vector3(0,0.5f,0);
        meshUtility.CreateTriangle(base1, base2, base3);

        meshUtility.CreateTriangle(base1, base2, top);
        meshUtility.CreateTriangle(base2, base3, top);
        meshUtility.CreateTriangle(base3, base1, top);

        meshFilter.sharedMesh = meshUtility.ToMesh();
    }
}
