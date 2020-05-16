using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    Vector3 position;
    List<Edge> edges;
    public Color color;
    public Vector3 newPosition;
    public bool isNew;

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

        color = Color.red;
    }

    public void InternalAddTriangle(Triangle triangle)
    {
        triangles.Add(triangle);
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

    public void CreateTriangle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        Vertex vert1 = FindOrCreateVertex(vec1);
        Vertex vert2 = FindOrCreateVertex(vec2);
        Vertex vert3 = FindOrCreateVertex(vec3);

        Edge edge1 = FindOrCreateEdge(vert1, vert2);
        Edge edge2 = FindOrCreateEdge(vert2, vert3);
        Edge edge3 = FindOrCreateEdge(vert3, vert1);

        Triangle triangle = new Triangle(edge1, edge2, edge3);
        triangles.Add(triangle);
    }
}