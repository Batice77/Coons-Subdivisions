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
        foreach (var vertex in meshUtility.vertices) {
            vertex.isNew = false;
        }
    }

    private string ConcatVec3(string val, Vector3 vec)
    {
        return val + " (" + vec.x + ", " + vec.y + ", " + vec.z + ")";
    }

    private void ComputeOriginalVerticesNewPositions()
    {
        foreach (var vertex in meshUtility.vertices) {
            float n = vertex.Edges.Count;
            float u2 = (n == 3) ? 3f/16f
                : 1f/n*(5f/8f-Mathf.Pow((3f/8f+1f/4f*Mathf.Cos(2*Mathf.PI/n)), 2));
            vertex.newPosition = (1f - n*u2) * vertex.position
                + u2 * SumNeighboursPositions(vertex);
            //gizmoPointsToDrawYellow.Add((Vector3) vertex.newPosition);
        }
    }

    private Vector3 SumNeighboursPositions(Vertex vertex)
    {
        HashSet<Vertex> neighbours = new HashSet<Vertex>();

        foreach (Edge edge in vertex.Edges) {
            neighbours.UnionWith(edge.vertices);
        }

        neighbours.Remove(vertex);

        Debug.Assert(neighbours.Count == vertex.Edges.Count);

        Vector3 sum = Vector3.zero;

        foreach (Vertex neighbour in neighbours) {
            sum += neighbour.position;
        }

        return sum;
    }

    private void ComputeEdgesMiddlePositions()
    {
        foreach (Edge edge in meshUtility.edges) {
            Vector3 A = edge.vertices[0].position;
            Vector3 B = edge.vertices[1].position;

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
        foreach (Edge edge in meshUtility.edges) {
            if (edge.isNew) {
                continue;
            }

            //edge.color = Color.cyan;

            // split the edge
            Vertex v1 = edge.vertices[0];
            Vertex v2 = edge.vertices[1];
            Vertex newMiddle = new Vertex(edge.newPosition);
            newMiddle.isNew = true;
            Edge new1 = new Edge(v1, newMiddle);
            Edge new2 = new Edge(v2, newMiddle);
            new1.isNew = true;
            new2.isNew = true;

            if (edge.Triangles[0] == edge.Triangles[1]) {
                Debug.Log("Both edge triangles are identical!");
                if (edge.Triangles.Count != 2) {
                    Debug.Log("edge.Triangles.Count != 2");
                }
            }

            // for each touching triangle
            for (int i = 0; i < 2; ++i) {
                Triangle t = edge.Triangles[i];

                // create a new edge between middle vertex and opposite vertex
                List<Vertex> tVertices = new List<Vertex>(t.GetVertices());
                if (!tVertices.Remove(v1)) {
                    Debug.Log("Could not remove v1 from tVertices!");
                }
                if (!tVertices.Remove(v2)) {
                    Debug.Log("Could not remove v2 from tVertices!");
                }
                Vertex opposite = tVertices[0];

                Edge newEdge = new Edge(newMiddle, opposite);
                newEdge.isNew = true;

                // update triangle of the other edges of the old triangle
                List<Edge> tEdges = new List<Edge>(t.edges);
                //List<Edge> tEdgesColorCopy = new List<Edge>(t.edges);
                //foreach (Edge e in tEdges) {
                //    e.color = Color.yellow;
                //}
                //edge.color = Color.cyan;
                if (!tEdges.Remove(edge)) {
                    Debug.Log("Could not remove edge from tEdges!");
                }

                Vertex[] vertices = tEdges[0].vertices;
                HashSet<Vertex> commonVertices = new HashSet<Vertex>(vertices);
                commonVertices.IntersectWith(new1.vertices);

                // Apply new positions now for passing to new mesh
                if (opposite.newPosition.HasValue) {
                    opposite.position = (Vector3) opposite.newPosition;
                }
                if (v1.newPosition.HasValue) {
                    v1.position = (Vector3) v1.newPosition;
                }
                if (v2.newPosition.HasValue) {
                    v2.position = (Vector3) v2.newPosition;
                }

                Triangle newT1;
                Triangle newT2;
                if (commonVertices.Count != 0) {
                    newT1 = new Triangle(newEdge, new1, tEdges[0]);
                    newMeshUtility.AddTriangle(newMiddle, opposite, v1,
                            /*Color.cyan*/ null, 0, t);
                    newT2 = new Triangle(newEdge, new2, tEdges[1]);
                    newMeshUtility.AddTriangle(newMiddle, opposite, v2,
                            /*Color.cyan*/ null, 0, t);
                } else {
                    newT1 = new Triangle(newEdge, new2, tEdges[0]);
                    newMeshUtility.AddTriangle(newMiddle, opposite, v2,
                            /*Color.cyan*/ null, 0, t);
                    newT2 = new Triangle(newEdge, new1, tEdges[1]);
                    newMeshUtility.AddTriangle(newMiddle, opposite, v1,
                            /*Color.cyan*/ null, 0, t);
                }

                // Update old edges' triangles
                // Note: the new triangles have already been added to old edges
                if (!tEdges[0].Triangles.Remove(t)) {
                    Debug.Log("Could not remove t from tEdges[0]!");
                }
                if (!tEdges[1].Triangles.Remove(t)) {
                    Debug.Log("Could not remove t from tEdges[1]!");
                }

                //yield return new WaitForSeconds(0.1f);
                yield return null;

                //foreach (Edge e in tEdgesColorCopy) {
                //    e.color = Edge.defaultColor;
                //}
            }

            //edge.color = Color.green;
        }
    }

    private IEnumerator FlipEdgesConnectingOldAndNewVertices()
    {
        //foreach (Edge e in newMeshUtility.edges) {
        //    e.color = Edge.defaultColor;
        //}
        foreach (Vertex v in newMeshUtility.vertices) {
            Debug.Assert(v.position[0] != 0f || v.position[1] != 0f
                    || v.position[2] != 0f);
        }
        foreach (Edge edge in newMeshUtility.edges) {
            if (!edge.isNew) {
                continue;
            }

            //foreach (Edge e in newMeshUtility.edges) {
                //if (e.Triangles.Count != 2) {
                if (edge.Triangles.Count != 2) {
                    edge.color = Color.grey;
                    Debug.Log("[FlipEdgesConnectingOldAndNewVertices] edge tris != 2");
                    Debug.Log("[FlipEdgesConnectingOldAndNewVertices] skipping edge");
                    //yield return new WaitForSeconds(0.2f);
                    continue;
                }
            //}
            Vertex A = edge.vertices[0];
            Vertex B = edge.vertices[1];

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
