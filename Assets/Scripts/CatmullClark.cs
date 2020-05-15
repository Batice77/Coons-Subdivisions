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

        foreach (Triangle face in meshUtility.triangles)
        {
            List<Vertex> faceVertices = new List<Vertex>();
            faceVertices.AddRange(face.GetVertices());

            // Store facePoint in face color
            Vector3 facePoint = Vertex.Average(faceVertices);
            face.color = new Color(facePoint.x, facePoint.y, facePoint.z);
        }

        foreach (Edge edge in meshUtility.edges)
        {
            List<Vertex> edgeVertices = new List<Vertex>();
            edgeVertices.AddRange(edge.vertices);

            // Récupérer les facePoints des faces adjacentes
            foreach(Triangle triangle in edge.Triangles)
            {
                Color col = triangle.GetColor();
                Vector3 facePoint = new Vector3(col.r, col.g, col.b);
                edgeVertices.Add(facePoint);
            }

            // Store edgePoint in edge color
            Vector3 edgePoint = Vertex.Average(edgeVertices);
            edge.color = new Color(edgePoint.x, edgePoint.y, edgePoint.z);
        }

        foreach (Vertex vertex in meshUtility.vertices)
        {
            // Récupérer les faces points des triangles contenant le vertex
            HashSet<Triangle> triangles = new HashSet<Triangle>();
            // Récupérer les mid points des arètes contenant le vertex
            List<Vertex> midPoints = new List<Vertex>();
            foreach (Edge edge in vertex.Edges)
            {
                List<Vertex> edgeVertices = new List<Vertex>();
                edgeVertices.AddRange(edge.vertices);
                midPoints.Add(Vertex.Average(edgeVertices));

                foreach (Triangle triangle in edge.Triangles)
                {
                    triangles.Add(triangle);
                }
            }

            // Calculer la moyenne des faces points
            List<Vertex> facePoints = new List<Vertex>();
            foreach (Triangle triangle in triangles)
            {
                Color col = triangle.GetColor();
                facePoints.Add(new Vector3(col.r, col.g, col.b));
            }
            Vector3 Q = Vertex.Average(facePoints);
            Vector3 R = Vertex.Average(midPoints);
            Vector3 v = vertex;
            float n = midPoints.Count; // Number of edges using v

            // Store vertex point in vertex color
            Vector3 newV = (1/n)*Q+(2/n)*R+((n-3)/n)*v;
            vertex.color = new Color(newV.x, newV.y, newV.z);
        }

        MeshUtility mU = new MeshUtility();
       
        foreach (Triangle face in meshUtility.triangles)
        {
            //Triangle face = meshUtility.triangles[0];

            Color color = face.GetColor();
            Vector3 faceVec = new Vector3(color.r, color.g, color.b);

            Edge edge1 = face.edges[0];
            color = edge1.GetColor();
            Vector3 edgeVec1 = new Vector3(color.r, color.g, color.b);

            Edge edge2 = face.edges[1];
            color = edge2.GetColor();
            Vector3 edgeVec2 = new Vector3(color.r, color.g, color.b);

            // Trouver le vertex en commun
            Vertex vertCommun = edge1 ^ edge2;
            color = vertCommun.GetColor();
            Vector3 vecCommun = new Vector3(color.r, color.g, color.b);
            mU.CreateQuad(faceVec, edgeVec1, vecCommun, edgeVec2);

            Edge edge3 = face.edges[2];
            color = edge3.GetColor();
            Vector3 edgeVec3 = new Vector3(color.r, color.g, color.b);

            vertCommun = edge3 ^ edge2;
            color = vertCommun.GetColor();
            vecCommun = new Vector3(color.r, color.g, color.b);
            mU.CreateQuad(faceVec, edgeVec3, vecCommun, edgeVec2);

            vertCommun = edge3 ^ edge1;
            color = vertCommun.GetColor();
            vecCommun = new Vector3(color.r, color.g, color.b);
            mU.CreateQuad(faceVec, edgeVec3, vecCommun, edgeVec1);
        }

        meshUtility = mU;
        DebugGraph.meshUtility = meshUtility;
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