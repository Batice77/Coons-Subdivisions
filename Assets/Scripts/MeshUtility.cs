﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vector3 position;
    List<Edge> edges;
    public Color color;

    public List<Edge> Edges
    {
        get { return edges; }
    }

    public Vertex(Vector3 point)
    {
        position = point;
        edges = new List<Edge>();

        color = Color.blue;
    }

    public static implicit operator Vertex(Vector3 vec) => new Vertex(vec);
    public static implicit operator Vector3(Vertex v) => v.position;

    public void AddEdge(Edge edge)
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
    public Vertex[] vertices;
    List<Triangle> triangles;
    public Color color;

    public List<Triangle> Triangles
    {
        get { return triangles; }
    }

    public Edge(Vertex vertex1, Vertex vertex2)
    {
        vertices = new Vertex[2];
        vertices[0] = vertex1;
        vertices[1] = vertex2;

        triangles = new List<Triangle>();

        color = Color.red;
    }

    public void AddTriangle(Triangle triangle)
    {
        triangles.Add(triangle);
    }

    public Color GetColor()
    {
        return color;
    }
}

public class Triangle
{
    public Edge[] edges;
    public Color color;

    public Triangle(Edge edge1, Edge edge2, Edge edge3)
    {
        edges = new Edge[3];
        edges[0] = edge1;
        edges[1] = edge2;
        edges[2] = edge3;

        color = Color.green;
    }

    public Vertex[] GetVertices()
    {
        Vertex v1 = edges[0].vertices[0];
        Vertex v2 = edges[1].vertices[0];
        Vertex v3 = edges[2].vertices[0];
        if (v1.Equals(v2))
        {
            if (v3.Equals(edges[0].vertices[1]))
                v2 = edges[1].vertices[1];
            else
                v1 = edges[0].vertices[1];
        }
        else if (v2.Equals(v3))
        {
            if (v1.Equals(edges[2].vertices[1]))
                v2 = edges[1].vertices[1];
            else
                v3 = edges[2].vertices[1];
        }
        else if (v1.Equals(v3))
        {
            if (v2.Equals(edges[0].vertices[1]))
                v3 = edges[2].vertices[1];
            else
                v1 = edges[0].vertices[1];
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
}

public class MeshUtility
{
    public List<Vertex> vertices;
    public List<Edge> edges;
    public List<Triangle> triangles;

    public MeshUtility(Mesh mesh)
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
        triangles = new List<Triangle>();

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
            if (edge.vertices[0] == vert1 && edge.vertices[1] == vert2 || edge.vertices[0] == vert2 && edge.vertices[1] == vert1)
                return edge;
        }
        Edge nEdge = new Edge(vert1, vert2);
        edges.Add(nEdge);

        // Add Edge to the vertice
        vert1.AddEdge(nEdge);
        vert2.AddEdge(nEdge);

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
            foreach(Edge edge in triangle.edges)
            {
                Gizmos.color = edge.GetColor();
                Vertex vert1 = edge.vertices[0];
                Vertex vert2 = edge.vertices[1];
                Gizmos.DrawLine(vert1+pos, vert2+pos);

                // Draw Vertices
                Gizmos.color = vert1.GetColor();
                Gizmos.DrawWireSphere(vert1+pos, 0.5f);
                Gizmos.color = vert2.GetColor();
                Gizmos.DrawWireSphere(vert2+pos, 0.5f);

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
}