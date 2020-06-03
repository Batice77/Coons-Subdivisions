using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex
{
    Vector3 position;
    List<Edge> edges;
    public Color color;
    public Vector3 newPosition;
    public bool isNew;

    public override string ToString()
    {
        return position.ToString();
    }

    public bool Equals(Vertex other)
    {
        return position == other.position;
    }

    public override bool Equals(object obj) => Equals(obj as Vertex);

    public Vector3 Position
    {
        get { return position; }
    }

    public IList<Edge> Edges
    {
        get { return edges.AsReadOnly(); }
    }

    public Vertex(Vector3 point)
    {
        position = point;
        edges = new List<Edge>();

        color = Color.blue;
    }

    public static implicit operator Vertex(Vector3 vec) => new Vertex(vec);
    public static implicit operator Vector3(Vertex v) => v.position;

    public void InternalAddEdge(Edge edge)
    {
        edges.Add(edge);
    }

    public void InternalRemoveEdge(Edge edge)
    {
        edges.Remove(edge);
    }

    public static Vertex Average(List<Vertex> vertices)
    {
        Vector3 res = Vector3.zero;
        foreach (Vector3 v in vertices)
            res += v;
        return res / vertices.Count;
    }

    public Color GetColor()
    {
        return color;
    }
}

public class Edge
{
    Vertex[] vertices;
    List<Triangle> triangles;
    public Color color;
    public static Color defaultColor = Color.red;
    public Vector3 newPosition;
    public bool isNew;

    public Vertex[] Vertices
    {
        get { return vertices; }
    }
    public IList<Triangle> Triangles
    {
        get { return triangles.AsReadOnly(); }
    }

    public Edge(Vertex vertex1, Vertex vertex2)
    {
        vertices = new Vertex[2];
        vertices[0] = vertex1;
        vertices[1] = vertex2;

        // Add Edge to the vertice
        vertex1.InternalAddEdge(this);
        vertex2.InternalAddEdge(this);

        triangles = new List<Triangle>();

        color = defaultColor;
    }

    public void InternalAddTriangle(Triangle triangle)
    {
        triangles.Add(triangle);
    }

    public bool InternalRemoveTriangle(Triangle triangle)
    {
        return triangles.Remove(triangle);
    }

    public Color GetColor()
    {
        return color;
    }

    // Retourne le vertex en commun entre 2 edge
    public static Vertex operator ^(Edge left, Edge right)
    {
        if (left.vertices[0] == right.vertices[0] || left.vertices[0] == right.vertices[1])
        {
            return left.vertices[0];
        }
        return left.vertices[1];
    }
}

public class Triangle
{
    Edge[] edges;
    public Color color;
    public Vector3 newPosition;

    public Edge[] Edges
    {
        get { return edges; }
    }

    public bool Equals(Triangle other)
    {
        foreach (Vertex v in this.GetVertices()) {
            if (!other.GetVertices().Contains(v)) {
                return false;
            }
        }

        return true;
    }

    public Triangle(Edge edge1, Edge edge2, Edge edge3)
    {
        edges = new Edge[3];
        edges[0] = edge1;
        edges[1] = edge2;
        edges[2] = edge3;

        edge1.InternalAddTriangle(this);
        edge2.InternalAddTriangle(this);
        edge3.InternalAddTriangle(this);

        color = Color.green;
    }

    public Vertex[] GetVertices()
    {
        Vertex v1 = edges[0].Vertices[0];
        Vertex v2 = edges[1].Vertices[0];
        Vertex v3 = edges[2].Vertices[0];
        if (v1.Equals(v2))
        {
            if (v3.Equals(edges[0].Vertices[1]))
                v2 = edges[1].Vertices[1];
            else
                v1 = edges[0].Vertices[1];
        }
        else if (v2.Equals(v3))
        {
            if (v1.Equals(edges[2].Vertices[1]))
                v2 = edges[1].Vertices[1];
            else
                v3 = edges[2].Vertices[1];
        }
        else if (v1.Equals(v3))
        {
            if (v2.Equals(edges[0].Vertices[1]))
                v3 = edges[2].Vertices[1];
            else
                v1 = edges[0].Vertices[1];
        }
        Vertex[] som = new Vertex[3];
        som[0] = v1;
        som[1] = v2;
        som[2] = v3;
        return som;
    }

    public Color GetColor()
    {
        return color;
    }

    // Retourne le vertex qui n'est pas en commun avec ceux de l'arète
    public static Vertex operator -(Triangle triangle, Edge edge)
    {
        Vertex vert1 = edge.Vertices[0];
        Vertex vert2 = edge.Vertices[1];

        foreach(Vertex v in triangle.GetVertices())
        {
            if(v!=vert1 || v!=vert2)
            {
                return v;
            }
        }
        return null;
    }
}

public class MeshUtility
{
    List<Vertex> vertices;
    List<Edge> edges;
    List<Triangle> triangles;

    public IList<Vertex> Vertices
    {
        get { return vertices.AsReadOnly(); }
    }
    public IList<Edge> Edges
    {
        get { return edges.AsReadOnly(); }
    }
    public IList<Triangle> Triangles
    {
        get { return triangles.AsReadOnly(); }
    }

    public MeshUtility(Mesh mesh=null)
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
        triangles = new List<Triangle>();

        if (mesh == null) return;
        for(int it=0; it< mesh.triangles.Length; it+=3)
        {
            Vertex vert1 = FindOrCreateVertex(mesh.vertices[mesh.triangles[it]]);
            Vertex vert2 = FindOrCreateVertex(mesh.vertices[mesh.triangles[it+1]]);
            Vertex vert3 = FindOrCreateVertex(mesh.vertices[mesh.triangles[it+2]]);

            Edge edge1 = FindOrCreateEdge(vert1, vert2);
            Edge edge2 = FindOrCreateEdge(vert2, vert3);
            Edge edge3 = FindOrCreateEdge(vert3, vert1);

            Triangle triangle = new Triangle(edge1, edge2, edge3);
            triangles.Add(triangle);
        }
    }

    Vertex FindOrCreateVertex(Vector3 point)
    {
        foreach (Vertex vert in vertices)
        {
            if (vert == point)
                return vert;
        }
        Vertex v = point;
        vertices.Add(v);
        return v;
    }

    Edge FindOrCreateEdge(Vertex vert1, Vertex vert2)
    {
        foreach (Edge edge in edges)
        {
            Vertex[] edgeVertices = edge.Vertices;
            if (edgeVertices[0] == vert1 && edgeVertices[1] == vert2 || edgeVertices[0] == vert2 && edgeVertices[1] == vert1)
                return edge;
        }
        Edge nEdge = new Edge(vert1, vert2);
        edges.Add(nEdge);
        return nEdge;
    }

    public Vector3 GetBarycentre()
    {
        Vector3 res = Vector3.zero;
        foreach (Vector3 v in vertices)
            res += v;
        return res / vertices.Count;
    }

    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> nVertices = new List<Vector3>();
        List<int> nTriangles = new List<int>();

        Vector3 center = GetBarycentre();

        foreach(Triangle triangle in triangles)
        {
            Vertex[] som = triangle.GetVertices();
            Vector3 v1 = som[0];
            Vector3 v2 = som[1];
            Vector3 v3 = som[2];

            // Centre du triangle
            Vector3 c = (v1 + v2 + v3) / 3;

            // Calcul normal du triangle
            Vector3 n = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            nTriangles.Add(nVertices.Count);
            nVertices.Add(v1);

            if (Vector3.Dot((c - center).normalized, n) <= 0)
            {
                nTriangles.Add(nVertices.Count);
                nVertices.Add(v3);

                nTriangles.Add(nVertices.Count);
                nVertices.Add(v2);
            }
            else
            {
                nTriangles.Add(nVertices.Count);
                nVertices.Add(v2);

                nTriangles.Add(nVertices.Count);
                nVertices.Add(v3);
            }
        }

        mesh.SetVertices(nVertices);
        mesh.SetTriangles(nTriangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public void DrawMeshGizmos(Vector3 pos)
    {
        Vector3 center = GetBarycentre();

        foreach (Triangle triangle in triangles)
        {
            // Draw Edges
            foreach(Edge edge in triangle.Edges)
            {
                Gizmos.color = edge.GetColor();
                Vertex[] edgeVertices = edge.Vertices;
                Vertex vert1 = edgeVertices[0];
                Vertex vert2 = edgeVertices[1];
                Gizmos.DrawLine(vert1+pos, vert2+pos);

                // Draw Vertices
                Gizmos.color = vert1.GetColor();
                Gizmos.DrawWireSphere(vert1+pos, 0.01f);
                Gizmos.color = vert2.GetColor();
                Gizmos.DrawWireSphere(vert2+pos, 0.01f);
            }

            // Draw Normal
            /*Gizmos.color = Color.blue;
            Vector3 c = Vector3.zero;
            Vertex[] vertices = triangle.GetVertices();
            foreach (Vector3 v in vertices)
            {
                c += v;
            }
            c /= vertices.Length;
            Gizmos.DrawLine(c, c + (f.GetNormal(center) * 0.5f));*/
        }
    }

    public int GetVertexIndex(Vertex vertex)
    {
        return vertices.FindIndex(x => x == vertex);
    }

    public int GetEdgeIndex(Edge edge)
    {
        return edges.FindIndex(x => x == edge);
    }

    public int GetTriangleIndex(Triangle triangle)
    {
        return triangles.FindIndex(x => x == triangle);
    }

    public IEnumerator SplitEdge(Edge edge, MeshUtility newMeshUtility)
    {
        // split the edge
        Vertex v1 = edge.Vertices[0];
        Vertex v2 = edge.Vertices[1];
        Vertex newMiddle = new Vertex(edge.newPosition);
        newMiddle.isNew = true;
        Edge new1 = new Edge(v1, newMiddle);
        Edge new2 = new Edge(v2, newMiddle);
        new1.isNew = true;
        new2.isNew = true;

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
            List<Edge> tEdges = new List<Edge>(t.Edges);
            //List<Edge> tEdgesColorCopy = new List<Edge>(t.edges);
            //foreach (Edge e in tEdges) {
            //    e.color = Color.yellow;
            //}
            //edge.color = Color.cyan;
            if (!tEdges.Remove(edge)) {
                Debug.Log("Could not remove edge from tEdges!");
            }

            Vertex[] vertices = tEdges[0].Vertices;
            HashSet<Vertex> commonVertices = new HashSet<Vertex>(vertices);
            commonVertices.IntersectWith(new1.Vertices);

            Vertex newOpposite;
            if (!opposite.isNew) {
                newOpposite = new Vertex(opposite.newPosition);
                newOpposite.isNew = opposite.isNew;
            } else {
                newOpposite = opposite;
            }
            Vertex newV1;
            if (!v1.isNew) {
                newV1 = new Vertex(v1.newPosition);
                newV1.isNew = v1.isNew;
            } else {
                newV1 = v1;
            }
            Vertex newV2;
            if (!v2.isNew) {
                newV2 = new Vertex(v2.newPosition);
                newV2.isNew = v2.isNew;
            } else {
                newV2 = v2;
            }

            Triangle newT1; // Those triangles must be created for edges to be
            Triangle newT2; // correctly updated for the rest of the algorithm
            if (commonVertices.Count != 0) {
                newT1 = new Triangle(newEdge, new1, tEdges[0]);
                newT2 = new Triangle(newEdge, new2, tEdges[1]);
            } else {
                newT1 = new Triangle(newEdge, new2, tEdges[0]);
                newT2 = new Triangle(newEdge, new1, tEdges[1]);
            }

            Triangle tNew = new Triangle(new Edge(newOpposite, newV1),
                    new Edge(newV1, newV2), new Edge(newV2, newOpposite));

            newMeshUtility.CreateTriangle(newMiddle, newOpposite, newV1,
                    /*Color.cyan*/ null, 0, tNew);
            newMeshUtility.CreateTriangle(newMiddle, newOpposite, newV2,
                    /*Color.cyan*/ null, 0, tNew);

            // Update old edges' triangles
            // Note: the new triangles have already been added to old edges
            if (!tEdges[0].InternalRemoveTriangle(t)) {
                Debug.Log("Could not remove t from tEdges[0]!");
            }
            if (!tEdges[1].InternalRemoveTriangle(t)) {
                Debug.Log("Could not remove t from tEdges[1]!");
            }
        }

        //yield return new WaitForSeconds(0.1f);
        yield return null;

        //foreach (Edge e in tEdgesColorCopy) {
        //    e.color = Edge.defaultColor;
        //}
    }

    public void CreateQuad(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4)
    {
        Vertex vert1 = FindOrCreateVertex(vec1);
        Vertex vert2 = FindOrCreateVertex(vec2);
        Vertex vert3 = FindOrCreateVertex(vec3);
        Vertex vert4 = FindOrCreateVertex(vec4);

        /*
        1-2, 2-4, 4-1
        2-3, 3-4, 2-4

        1-3, 3-4, 4-1
        1-2, 2-3, 3-1
        */

        Edge edge1 = FindOrCreateEdge(vert1, vert2);
        Edge edge2 = FindOrCreateEdge(vert2, vert3);
        Edge edge3 = FindOrCreateEdge(vert3, vert4);
        Edge edge4 = FindOrCreateEdge(vert4, vert1);

        /*List<Vertex> vertices = new List<Vertex>();
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert3);
        vertices.Add(vert4);
        Vertex quadCenter = Vertex.Average(vertices);
        vertices.Clear();

        vertices.Add(vert2);
        vertices.Add(vert4);
        Vertex edge24Center = Vertex.Average(vertices);
        vertices.Clear();

        vertices.Add(vert1);
        vertices.Add(vert3);
        Vertex edge13Center = Vertex.Average(vertices);

        if (Vector3.Distance(edge24Center, quadCenter) < Vector3.Distance(edge13Center, quadCenter))*/
        if (Vector3.Distance(vec2, vec4) < Vector3.Distance(vec1, vec3))
        {
            Edge edge5 = FindOrCreateEdge(vert2, vert4);

            Triangle triangle = new Triangle(edge1, edge5, edge4);
            triangles.Add(triangle);

            Triangle triangle2 = new Triangle(edge2, edge3, edge5);
            triangles.Add(triangle2);
        }
        else
        {
            Edge edge5 = FindOrCreateEdge(vert1, vert3);

            Triangle triangle = new Triangle(edge5, edge3, edge4);
            triangles.Add(triangle);

            Triangle triangle2 = new Triangle(edge1, edge2, edge5);
            triangles.Add(triangle2);
        }
    }

    public void CreateTriangle(Vertex vec1, Vertex vec2, Vertex vec3,
            Color? color=null, int? newEdge=null,
            Triangle overlappingTriangle=null)
    {
        Vertex vert1 = FindOrCreateVertex(vec1);
        Vertex vert2 = FindOrCreateVertex(vec2);
        Vertex vert3 = FindOrCreateVertex(vec3);

        vert1.isNew = vec1.isNew;
        vert2.isNew = vec2.isNew;
        vert3.isNew = vec3.isNew;

        Edge edge1 = FindOrCreateEdge(vert1, vert2);
        Edge edge2 = FindOrCreateEdge(vert2, vert3);
        Edge edge3 = FindOrCreateEdge(vert3, vert1);
        if (newEdge.HasValue && newEdge == 0) {
            edge1.isNew = true;
        } else if (newEdge.HasValue && newEdge == 1) {
            edge2.isNew = true;
        } else if (newEdge.HasValue && newEdge == 2) {
            edge3.isNew = true;
        }

        if (color.HasValue && triangles.Count != 0) {
            foreach (Edge edge in triangles[triangles.Count-1].Edges) {
                edge.color = Color.Lerp(Edge.defaultColor, (Color) color, .7f);
            }
            if (triangles.Count > 1) {
                foreach (Edge edge in triangles[triangles.Count-2].Edges) {
                    edge.color = Edge.defaultColor;
                }
            }
        }

        if (overlappingTriangle != null) {
            foreach (Triangle t in triangles) {
                if (t.Equals(overlappingTriangle)) {
                    foreach (Edge e in t.Edges) {
                        e.InternalRemoveTriangle(t);
                    }
                    triangles.Remove(t);
                    break;
                }
            }
        }

        Triangle triangle = new Triangle(edge1, edge2, edge3);
        triangles.Add(triangle);

        if (color.HasValue) {
            edge1.color = (Color) color;
            edge2.color = (Color) color;
            edge3.color = (Color) color;
        }
    }

    public void FlipEdge(Edge edge)
    {
        Vertex A = edge.Vertices[0];
        Vertex B = edge.Vertices[1];
        A.InternalRemoveEdge(edge);
        B.InternalRemoveEdge(edge);

        Triangle TC = edge.Triangles[0];
        var TCVertices = new List<Vertex>(TC.GetVertices());
        TCVertices.Remove(A);
        TCVertices.Remove(B);
        Debug.Assert(TCVertices.Count == 1);
        Vertex C = TCVertices[0];

        Triangle TD = edge.Triangles[1];
        var TDVertices = new List<Vertex>(TD.GetVertices());
        TDVertices.Remove(A);
        TDVertices.Remove(B);
        Debug.Assert(TDVertices.Count == 1);
        Vertex D = TDVertices[0];

        var edgeTriangles = new List<Triangle>(edge.Triangles);
        foreach (Triangle t in edgeTriangles) {
            foreach (Edge e in t.Edges) {
                if (!e.InternalRemoveTriangle(t)) {
                    Debug.Log("Could not remove t from e. Already removed?");
                }
            }
        }

        // Reuse edge by updating its vertices, effectively flipping it
        edge.Vertices[0] = C;
        edge.Vertices[1] = D;
        C.InternalAddEdge(edge);
        D.InternalAddEdge(edge);

        var TCEdges = new List<Edge>(TC.Edges);
        var TDEdges = new List<Edge>(TD.Edges);
        TCEdges.Remove(edge);
        TDEdges.Remove(edge);
        Debug.Assert(TCEdges.Count == 2);
        Debug.Assert(TDEdges.Count == 2);
        if (TCEdges[0].Vertices.Contains(TDEdges[0].Vertices[0])
                || TCEdges[0].Vertices.Contains(TDEdges[0].Vertices[1])) {
            TC.Edges[0] = TCEdges[0];
            TC.Edges[1] = TDEdges[0];
            TC.Edges[2] = edge;
            TCEdges[0].InternalAddTriangle(TC);
            TDEdges[0].InternalAddTriangle(TC);
            edge.InternalAddTriangle(TC);

            TD.Edges[0] = TCEdges[1];
            TD.Edges[1] = TDEdges[1];
            TD.Edges[2] = edge;
            TCEdges[1].InternalAddTriangle(TD);
            TDEdges[1].InternalAddTriangle(TD);
            edge.InternalAddTriangle(TD);
        } else {
            TC.Edges[0] = TCEdges[0];
            TC.Edges[1] = TDEdges[1];
            TC.Edges[2] = edge;
            TCEdges[0].InternalAddTriangle(TC);
            TDEdges[1].InternalAddTriangle(TC);
            edge.InternalAddTriangle(TC);

            TD.Edges[0] = TCEdges[1];
            TD.Edges[1] = TDEdges[0];
            TD.Edges[2] = edge;
            TCEdges[1].InternalAddTriangle(TD);
            TDEdges[0].InternalAddTriangle(TD);
            edge.InternalAddTriangle(TD);
        }

        //TC.edges[0].color = Color.magenta;
        //TC.edges[1].color = Color.magenta;
        //TC.edges[2].color = Color.magenta;
        //TD.edges[0].color = Color.magenta;
        //TD.edges[1].color = Color.magenta;
        //TD.edges[2].color = Color.magenta;
    }
}
