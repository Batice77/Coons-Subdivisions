using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SQRT3 : MonoBehaviour
{
    [Range(1,11)]
    public int exec;
    MeshFilter filter;
    MeshUtility util;
    List<Vertex> vs;
    float[] coef = new float[10];

    // Start is called before the first frame update
    void Awake()
    {
        filter = GetComponent<MeshFilter>();
        util = new MeshUtility(filter.sharedMesh);
        DebugGraph.meshUtility = util;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Step();
            
        }
    }

    public void Step()
    {
        MeshUtility temp = new MeshUtility();
        setCoef(util.Vertices.Count);
        List<Vertex> vs = new List<Vertex>();

        //Centers
        foreach (Triangle t in util.Triangles)
        {
            Vertex vert = new Vertex(((t.GetVertices()[0].Position + t.GetVertices()[1].Position + t.GetVertices()[2].Position) / 3));
            temp.FindOrCreateVertex(vert);
            vs.Add(vert);
        }
        foreach (Vertex v in vs)
            foreach (Vertex v2 in vs)
                if (v != v2)
                    temp.FindOrCreateEdge(v, v2);
        List<Edge> edges = new List<Edge>();
        foreach( Edge e in temp.Edges)
            edges.Add(e);
        foreach (Edge e in edges)
            foreach (Vertex v in vs)
                if (v != e.Vertices[0] && v != e.Vertices[1] && e.Vertices[0] != e.Vertices[1])
                    temp.CreateTriangle(v, e.Vertices[0], e.Vertices[1]);


        foreach (Triangle t in util.Triangles)
        {
            Vector3 vt = getVertexPoint(t);
            temp.FindOrCreateVertex(vt);
        }

        //foreach (Vertex v in util.Vertices)
        //{
        //    for (int j = 0; j < v.GetTriangles().Length; j++)
        //    {
        //        Triangle t = v.GetTriangles()[j];
        //        Triangle tnext = null;
        //        int i = 0;

        //        do
        //        {
        //            if (i > v.GetTriangles().Length)
        //                tnext = null;
        //            else
        //                tnext = v.GetTriangles()[i];
        //            temp.CreateTriangle(tnext.GetVertices()[0], tnext.GetVertices()[1], tnext.GetVertices()[2]);

        //            t = tnext;

        //            i++;
        //        } while (t != v.GetTriangles()[j] && t != null);
        //    }
        //}
        Debug.Log(temp.Vertices.Count);
        Debug.Log(temp.Edges.Count);
        Debug.Log(temp.Triangles.Count);
        util = temp;
        DebugGraph.meshUtility = util;
        filter.sharedMesh = util.ToMesh();
    }

    public Vertex getVertexPoint(Triangle t)
    {
        Vertex p = new Vertex(Vector3.zero);
        p.newPosition = Vector3.zero;
        int vSize = t.GetVertices().Length;
        foreach (Vertex v in t.GetVertices())
        {
            p.newPosition += v.Position;
        }
        p.newPosition = p.newPosition * (coef[vSize] / (float)vSize);
        p.newPosition = p.newPosition * (1.0f - coef[vSize]);

        return new Vertex(p.newPosition);
    } 

    public void setCoef(int length)
    {
        for (int i = 1; i < length; i++)
            coef[i] = (4.0f - (2.0f * Mathf.Cos(Mathf.PI / (float)i))) / 9.0f;
    }

    //public void Step2()
    //{
    //    MeshUtility temp = new MeshUtility();
    //    int i = 0;
    //    //Parcours tout les triangles et les faces
    //    foreach (Triangle triangle in util.Triangles)
    //    {
    //        List<Vertex> vertices = new List<Vertex>();
    //        foreach (Edge edge in triangle.Edges)
    //        {
               
    //            vertices.Add(edge.Vertices[0]);
    //            Vertex v = Vertex.Average(new List<Vertex> { edge.Vertices[0], edge.Vertices[1] });
    //            vertices.Add(v);
    //            vertices.Add(edge.Vertices[1]);

    //            temp.FindOrCreateVertex(edge.Vertices[0]);
    //            temp.FindOrCreateVertex(v);
    //            temp.FindOrCreateVertex(edge.Vertices[1]);

    //            temp.FindOrCreateEdge(edge.Vertices[0], v);
    //            temp.FindOrCreateEdge(v, edge.Vertices[1]);
    //        }

    //        if (i < 6)
    //        {
    //            if (i % 2 == 0)
    //            {
    //                temp.CreateTriangle(vertices[0], vertices[1], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[2], vertices[4]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[4], vertices[5], vertices[7]);
    //            }
    //            else
    //            {
    //                temp.CreateTriangle(vertices[0], vertices[4], vertices[1]);
    //                temp.CreateTriangle(vertices[4], vertices[5], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[7], vertices[2]);
    //            }
    //        }
    //        else if (i >= 8)
    //        {
    //            if (i % 2 == 0)
    //            {
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[0]);
    //                temp.CreateTriangle(vertices[1], vertices[2], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[4], vertices[3], vertices[7]);
    //            }
    //            else
    //            {
    //                temp.CreateTriangle(vertices[0], vertices[4], vertices[1]);
    //                temp.CreateTriangle(vertices[4], vertices[3], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[7], vertices[2]);
    //            }
    //        }
    //        else
    //        {
    //            if (i % 2 == 0)
    //            {
    //                temp.CreateTriangle(vertices[0], vertices[1], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[2], vertices[4]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[4], vertices[3], vertices[7]);
    //            }
    //            else
    //            {
    //                temp.CreateTriangle(vertices[0], vertices[4], vertices[1]);
    //                temp.CreateTriangle(vertices[4], vertices[5], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[4], vertices[7]);
    //                temp.CreateTriangle(vertices[1], vertices[7], vertices[2]);
    //            }
    //        }

    //        vertices.Clear();
    //        i++;
    //        if (i > exec)
    //            break;
    //    }

    //    Debug.Log(temp.Vertices.Count);
    //    Debug.Log(temp.Edges.Count);
    //    Debug.Log(util.Edges.Count);
    //    util = temp ;
    //    DebugGraph.meshUtility = util;
    //    filter.sharedMesh = util.ToMesh();
    //}

    //public void Step1()
    //{
    //    //For each triangle, split it into three triangle at their middle
    //    MeshUtility temp = new MeshUtility(GetComponent<MeshFilter>().sharedMesh);
    //    temp.triangles.Clear();
    //    temp.edges.Clear();
    //    temp.vertices.Clear();

    //    for (int i = 0; i < 1; i++)
    //    {
    //        List<Vertex> vs = new List<Vertex>();
    //        vs.Add(util.triangles[i].GetVertices()[0]);
    //        vs.Add(util.triangles[i].GetVertices()[1]);
    //        vs.Add(util.triangles[i].GetVertices()[2]);
    //        Vertex v1 = new Vertex(new Vector3(vs[0].position.x - vs[1].position.x, vs[0].position.y - vs[1].position.y));
    //        vs.Add(v1);

    //        //vs.Add(util.triangles[i+1].GetVertices()[2]);
    //        temp.vertices.Add(vs[0]);
    //        temp.vertices.Add(vs[1]);
    //        temp.vertices.Add(vs[2]);
    //        temp.vertices.Add(vs[3]);
    //        //temp.vertices.Add(Vertex.Average(vs));

    //        Edge e1 = new Edge(vs[0], vs[1]);
    //        Edge e2 = new Edge(vs[1], vs[2]);
    //        Edge e3 = new Edge(vs[0], vs[2]);
    //        //Edge e4 = new Edge(vs[0], vs[3]);
    //        //Edge e5 = new Edge(vs[2], vs[3]);

    //        temp.edges.Add(e1);
    //        temp.edges.Add(e2);
    //        temp.edges.Add(e3);
    //        //temp.edges.Add(e4);
    //        //temp.edges.Add(e5);

    //        Triangle t1 = new Triangle(e2, e1, e3);
    //        //Triangle t2 = new Triangle(e4, e5, e3);
    //        temp.triangles.Add(t1);
    //        //temp.triangles.Add(t2);

    //        temp.edges[i].color = Color.green;
    //    }

    //    //for (int i = 0; i < util.triangles.Count; i++)
    //    //{
    //    //    List<Vertex> vs = new List<Vertex>();
    //    //    vs.Add(util.triangles[i].GetVertices()[0]);
    //    //    vs.Add(util.triangles[i].GetVertices()[1]);
    //    //    vs.Add(util.triangles[i].GetVertices()[2]);
    //    //    //vs.Add(util.triangles[i+1].GetVertices()[2]);
    //    //    temp.vertices.Add(vs[0]);
    //    //    temp.vertices.Add(vs[1]);
    //    //    temp.vertices.Add(vs[2]);
    //    //    //temp.vertices.Add(vs[3]);
    //    //    temp.vertices.Add(Vertex.Average(vs));

    //    //    Edge e1 = new Edge(vs[0], vs[1]);
    //    //    Edge e2 = new Edge(vs[1], vs[2]);
    //    //    Edge e3 = new Edge(vs[0], vs[2]);
    //    //    //Edge e4 = new Edge(vs[0], vs[3]);
    //    //    //Edge e5 = new Edge(vs[2], vs[3]);

    //    //    temp.edges.Add(e1);
    //    //    temp.edges.Add(e2);
    //    //    temp.edges.Add(e3);
    //    //    //temp.edges.Add(e4);
    //    //    //temp.edges.Add(e5);

    //    //    Triangle t1 = new Triangle(e2, e1, e3);
    //    //    //Triangle t2 = new Triangle(e4, e5, e3);
    //    //    temp.triangles.Add(t1);
    //    //    //temp.triangles.Add(t2);

    //    //    temp.edges[i * 2 + 1].color = Color.green;
    //    //}


    //    //foreach(Triangle t in util.triangles)
    //    //{
    //    //    //Centers
    //    //    temp.vertices.Add(Vertex.Average(t.GetVertices().OfType<Vertex>().ToList()));
    //    //    if (temp.vertices.Count >= 3)
    //    //        break;
    //    //}
    //    //for (int i = 0; i < util.triangles.Count; i+=3)
    //    //{

    //    //    Edge e1 = new Edge(temp.vertices[i], temp.vertices[i + 1]);
    //    //    Edge e2 = new Edge(temp.vertices[i+1], temp.vertices[i + 2]);
    //    //    Edge e3 = new Edge(temp.vertices[i+2], temp.vertices[i]);
    //    //    temp.edges.Add(e1);
    //    //    temp.edges.Add(e2);
    //    //    temp.edges.Add(e3);

    //    //    Triangle triangle = new Triangle(e1, e2, e3);
    //    //    temp.triangles.Add(triangle);
    //    //    if (temp.triangles.Count >= 1)
    //    //        break;
    //    //}


    //    Debug.Log(temp.vertices.Count);
    //    Debug.Log(temp.edges.Count);
    //    util = temp;
    //    GetComponent<MeshFilter>().sharedMesh = util.ToMesh();
    //    DebugGraph.meshUtility = util;
    //}

    private void OnDrawGizmos()
    {
        if (util == null) return;
        util.DrawMeshGizmos(transform.position);
    }
}
