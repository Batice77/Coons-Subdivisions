using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icosaedre : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshUtility meshUtility;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        meshUtility = new MeshUtility();
        DebugGraph.meshUtility = meshUtility;

        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        Vector3 p1 = new Vector3(-1, t, 0).normalized;
        Vector3 p2 = new Vector3(1, t, 0).normalized;
        Vector3 p3 = new Vector3(-1, -t, 0).normalized;
        Vector3 p4 = new Vector3(1, -t, 0).normalized;
        Vector3 p5 = new Vector3(0, -1, t).normalized;
        Vector3 p6 = new Vector3(0, 1, t).normalized;
        Vector3 p7 = new Vector3(0, -1, -t).normalized;
        Vector3 p8 = new Vector3(0, 1, -t).normalized;
        Vector3 p9 = new Vector3(t, 0, -1).normalized;
        Vector3 p10 = new Vector3(t, 0, 1).normalized;
        Vector3 p11 = new Vector3(-t, 0, -1).normalized;
        Vector3 p12 = new Vector3(-t, 0, 1).normalized;

        meshUtility.CreateTriangle(p1, p12, p6);
        meshUtility.CreateTriangle(p1, p6, p2);
        meshUtility.CreateTriangle(p1, p2, p8);
        meshUtility.CreateTriangle(p1, p8, p11);
        meshUtility.CreateTriangle(p1, p11, p12);
        meshUtility.CreateTriangle(p2, p6, p10);
        meshUtility.CreateTriangle(p6, p12, p5);
        meshUtility.CreateTriangle(p12, p11, p3);
        meshUtility.CreateTriangle(p11, p8, p7);
        meshUtility.CreateTriangle(p8, p2, p9);
        meshUtility.CreateTriangle(p4, p10, p5);
        meshUtility.CreateTriangle(p4, p5, p3);
        meshUtility.CreateTriangle(p4, p3, p7);
        meshUtility.CreateTriangle(p4, p7, p9);
        meshUtility.CreateTriangle(p4, p9, p10);
        meshUtility.CreateTriangle(p5, p10, p6);
        meshUtility.CreateTriangle(p3, p5, p12);
        meshUtility.CreateTriangle(p7, p3, p11);
        meshUtility.CreateTriangle(p9, p7, p8);
        meshUtility.CreateTriangle(p10, p9, p2);

        meshFilter.sharedMesh = meshUtility.ToMesh();
    }
}
