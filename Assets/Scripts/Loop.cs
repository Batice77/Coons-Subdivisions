using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshUtility meshUtility;
    MeshUtility newMeshUtility;
    List<Vector3> gizmoPointsToDraw;
    List<Vector3> gizmoPointsToDrawYellow;
    List<Vector3> gizmoPointsToDrawMagenta;

    IEnumerator Start()
    {
        int passes = 4;

        for (int i = 0; i < passes; ++i) {
            meshFilter = GetComponent<MeshFilter>();
            meshUtility = new MeshUtility(meshFilter.sharedMesh);
            newMeshUtility = new MeshUtility();
            DebugGraph.meshUtility = meshUtility;
            gizmoPointsToDraw = new List<Vector3>();
            gizmoPointsToDrawYellow = new List<Vector3>();
            gizmoPointsToDrawMagenta = new List<Vector3>();

            //meshUtility.edges[2].color = Color.grey;

            MarkOriginalVertices();
            ComputeOriginalVerticesNewPositions();
            ComputeEdgesMiddlePositions();
            yield return StartCoroutine("SplitEdges");
            yield return StartCoroutine("FlipEdgesConnectingOldAndNewVertices");
            Debug.Log("Done flipping edges!");
            //ApplyNewVerticesPositions();

            //meshFilter.sharedMesh = meshUtility.ToMesh();
            meshFilter.sharedMesh = newMeshUtility.ToMesh();
        }
    }

    void Update()
    {
        //meshFilter.sharedMesh = newMeshUtility.ToMesh();
    }

    private void OnDrawGizmos()
    {
        if (meshUtility == null) {
            return;
        }

        meshUtility.DrawMeshGizmos(transform.position + new Vector3(1.1f, 0, 0));

        if (newMeshUtility == null) {
            return;
        }

        newMeshUtility.DrawMeshGizmos(transform.position);

        foreach (Vector3 p in gizmoPointsToDraw) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(p + new Vector3(1.1f, 0, 0), 0.01f);
        }
        foreach (Vector3 p in gizmoPointsToDrawYellow) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(p + new Vector3(1.1f, 0, 0), 0.01f);
        }
        foreach (Vector3 p in gizmoPointsToDrawMagenta) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(p + new Vector3(1.1f, 0, 0), 0.01f);
        }
    }

    private void MarkOriginalVertices()
    {
        foreach (var vertex in meshUtility.Vertices) {
            vertex.isNew = false;
        }
    }

    private string ConcatVec3(string val, Vector3 vec)
    {
        return val + " (" + vec.x + ", " + vec.y + ", " + vec.z + ")";
    }

    private void ComputeOriginalVerticesNewPositions()
    {
        foreach (var vertex in meshUtility.Vertices) {
            float n = vertex.Edges.Count;
            float u2 = (n == 3) ? 3f/16f
                : 1f/n*(5f/8f-Mathf.Pow((3f/8f+1f/4f*Mathf.Cos(2*Mathf.PI/n)), 2));
            vertex.newPosition = (1f - n*u2) * vertex.Position
                + u2 * SumNeighboursPositions(vertex);
            //gizmoPointsToDrawYellow.Add((Vector3) vertex.newPosition);
        }
    }

    private Vector3 SumNeighboursPositions(Vertex vertex)
    {
        HashSet<Vertex> neighbours = new HashSet<Vertex>();

        foreach (Edge edge in vertex.Edges) {
            neighbours.UnionWith(edge.Vertices);
        }

        neighbours.Remove(vertex);

        Debug.Assert(neighbours.Count == vertex.Edges.Count);

        Vector3 sum = Vector3.zero;

        foreach (Vertex neighbour in neighbours) {
            sum += neighbour.Position;
        }

        return sum;
    }

    private void ComputeEdgesMiddlePositions()
    {
        foreach (Edge edge in meshUtility.Edges) {
            Vector3 A = edge.Vertices[0].Position;
            Vector3 B = edge.Vertices[1].Position;

            Triangle TC = edge.Triangles[0];
            var TCVertices = new List<Vertex>(TC.GetVertices());
            TCVertices.Remove(A);
            TCVertices.Remove(B);
            Debug.Assert(TCVertices.Count == 1);
            Vector3 C = TCVertices[0];

            Triangle TD = edge.Triangles[1];
            var TDVertices = new List<Vertex>(TD.GetVertices());
            TDVertices.Remove(A);
            TDVertices.Remove(B);
            Debug.Assert(TDVertices.Count == 1);
            Vector3 D = TDVertices[0];

            edge.newPosition = 3f/8f * (A + B) + 1f/8f * (C + D);
            //gizmoPointsToDraw.Add(edge.newPosition);
            Debug.Assert(edge.newPosition[0] != 0f || edge.newPosition[1] != 0f
                    || edge.newPosition[2] != 0f);
        }
    }

    private IEnumerator SplitEdges()
    {
        foreach (Edge edge in meshUtility.Edges) {
            if (edge.isNew) {
                continue;
            }

            //edge.color = Color.cyan;

            yield return meshUtility.SplitEdge(edge, newMeshUtility);

            //edge.color = Color.green;
        }
    }

    private IEnumerator FlipEdgesConnectingOldAndNewVertices()
    {
        //foreach (Edge e in newMeshUtility.edges) {
        //    e.color = Edge.defaultColor;
        //}
        foreach (Edge edge in newMeshUtility.Edges) {
            if (!edge.isNew) {
                continue;
            }

            //foreach (Edge e in newMeshUtility.edges) {
                //if (e.Triangles.Count != 2) {
                if (edge.Triangles.Count != 2) {
                    edge.color = Color.grey;
                    Debug.Log("[FlipEdgesConnectingOldAndNewVertices] edge tris != 2: " + edge.Triangles.Count);
                    Debug.Log("[FlipEdgesConnectingOldAndNewVertices] skipping edge");
                    //yield return new WaitForSeconds(0.2f);
                    continue;
                }
            //}
            Vertex A = edge.Vertices[0];
            Vertex B = edge.Vertices[1];

            if (A.isNew && B.isNew) {
                continue;
            }

            //foreach (Triangle t in edge.Triangles) {
            //    foreach (Edge e in t.edges) {
            //        e.color = Color.magenta;
            //    }
            //}
            //edge.color = Color.white;

            //yield return new WaitForSeconds(0.1f);

            newMeshUtility.FlipEdge(edge);

            //edge.color = Color.white;

            //yield return new WaitForSeconds(0.1f);

            //foreach (Triangle t in edge.Triangles) {
            //    foreach (Edge e in t.edges) {
            //        e.color = Edge.defaultColor;
            //    }
            //}

            yield return null;
        }
    }

    private void ApplyNewVerticesPositions()
    {
        //foreach (Vertex vertex in meshUtility.vertices) {
        //    if (!vertex.isNew) {
        //        Debug.Log("Updating old vertex position " + vertex.position + " to new position." + vertex.newPosition);
        //        vertex.position = vertex.newPosition;
        //    }
        //}
    }
}
