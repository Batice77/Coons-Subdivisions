using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClark : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshUtility meshUtility;

    [Range(0, 3)]
    public int NbSubdivision = 1;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        for (int csubdivision = 0; csubdivision < NbSubdivision; ++csubdivision)
        {
            meshUtility = new MeshUtility(meshFilter.sharedMesh);
            DebugGraph.meshUtility = meshUtility;

            foreach (Triangle face in meshUtility.Triangles)
            {
                List<Vertex> faceVertices = new List<Vertex>();
                faceVertices.AddRange(face.GetVertices());

                face.newPosition = Vertex.Average(faceVertices);
                /*GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = face.newPosition;
                go.transform.localScale = Vector3.one *0.1f;*/
            }

            foreach (Edge edge in meshUtility.Edges)
            {
                List<Vertex> edgeVertices = new List<Vertex>();
                edgeVertices.AddRange(edge.Vertices);

                // Récupérer les facePoints des faces adjacentes
                foreach (Triangle triangle in edge.Triangles)
                {
                    edgeVertices.Add(triangle.newPosition);
                }

                edge.newPosition = Vertex.Average(edgeVertices);
                /*GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = edge.newPosition;
                go.transform.localScale = Vector3.one * 0.3f;*/
            }

            foreach (Vertex vertex in meshUtility.Vertices)
            {
                // Récupérer les faces points des triangles contenant le vertex
                HashSet<Triangle> triangles = new HashSet<Triangle>();
                // Récupérer les mid points des arètes contenant le vertex
                List<Vertex> midPoints = new List<Vertex>();
                foreach (Edge edge in vertex.Edges)
                {
                    foreach (Triangle triangle in edge.Triangles)
                    {
                        triangles.Add(triangle);
                    }

                    List<Vertex> edgeVertices = new List<Vertex>();
                    edgeVertices.AddRange(edge.Vertices);
                    midPoints.Add(Vertex.Average(edgeVertices));
                }

                // Calculer la moyenne des faces points
                List<Vertex> facePoints = new List<Vertex>();
                foreach (Triangle triangle in triangles)
                {
                    facePoints.Add(triangle.newPosition);
                }
                Vector3 Q = Vertex.Average(facePoints); // moyenne des faces points lié à v
                Vector3 R = Vertex.Average(midPoints); // moyenne de tout les centres des arrètes lié à v
                Vector3 v = vertex;
                float n = midPoints.Count; // Number of edges using v

                vertex.newPosition = (1 / n) * Q + (2 / n) * R + ((n - 3) / n) * v;
            }

            MeshUtility mU = new MeshUtility();

            foreach (Triangle face in meshUtility.Triangles)
            {
                //Triangle face = meshUtility.triangles[0];

                Vector3 faceVec = face.newPosition;

                Edge[] faceEdges = face.Edges;
                Edge edge1 = faceEdges[0];
                Edge edge2 = faceEdges[1];
                Edge edge3 = faceEdges[2];
                Vector3 edgeVec1 = edge1.newPosition;
                Vector3 edgeVec2 = edge2.newPosition;
                Vector3 edgeVec3 = edge3.newPosition;

                // Trouver le vertex en commun
                Vertex vertCommun = edge1 ^ edge2;
                Vector3 vecCommun = vertCommun.newPosition;
                mU.CreateQuad(faceVec, edgeVec1, vecCommun, edgeVec2);

                vertCommun = edge3 ^ edge2;
                vecCommun = vertCommun.newPosition;
                mU.CreateQuad(faceVec, edgeVec3, vecCommun, edgeVec2);

                vertCommun = edge3 ^ edge1;
                vecCommun = vertCommun.newPosition;
                mU.CreateQuad(faceVec, edgeVec3, vecCommun, edgeVec1);
            }
           
            meshUtility = mU;
            DebugGraph.meshUtility = meshUtility;
            meshFilter.sharedMesh = meshUtility.ToMesh();
        }
    }

    private void OnDrawGizmos()
    {
        if (meshUtility == null) return;
        meshUtility.DrawMeshGizmos(transform.position);
    }
}