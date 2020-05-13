using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClark : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshUtility meshUtility;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshUtility = new MeshUtility(meshFilter.sharedMesh);
        DebugGraph.meshUtility = meshUtility;

        /*print(meshUtility.vertices.Count);
        foreach(Vertex v in meshUtility.vertices)
        {
            print(v.Edges.Count);
        }*/

        meshUtility.edges[2].color = Color.grey;

        meshFilter.sharedMesh = meshUtility.ToMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (meshUtility == null) return;
        meshUtility.DrawMeshGizmos(transform.position);
    }
}
