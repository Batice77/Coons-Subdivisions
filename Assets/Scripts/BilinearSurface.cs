using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilinearSurface
{
    public List<Vector3> points;
    int u = 0;
    int v = 0;
    Vector3 P00;
    Vector3 P01;
    Vector3 P10;
    Vector3 P11;

    public BilinearSurface()
    {
        points = new List<Vector3>();
    }

    public void Create(Surface s1, Surface s2)
    {
        P00 = s1.curve1.points[0].position;
        P10 = s1.curve1.points[s1.curve1.points.Count - 1].position;
        P01 = s1.curve2.points[0].position;
        P11 = s1.curve2.points[s1.curve2.points.Count - 1].position;

        float dP00_P10 = Vector3.Distance(P00, P10); // u axis
        float dP00_P01 = Vector3.Distance(P00, P01); // v axis
        Vector3 u_director = P10 - P00;
        Vector3 v_director = P01 - P00;

        u = s1.curve1.points.Count;
        v = s2.curve1.points.Count;

        for(int j = 0; j < v; j++)
        {
            for (int i = 0; i < u; i++)
            {
                Vector3 new_point = (P00 + u_director * ((float)i / u)) + (P00 + v_director * ((float)j / v));

                Debug.Log("new_point : " + new_point);
                points.Add(new_point);
            }
        }
    }

    public void Draw(Color color)
    {
        for(int i = 0; i < u * v - 1; i++)
        {
            //Debug.Log("p[0] : " + points[0] + ", p[1] : " + points[1]);
            Debug.DrawLine(points[i], points[i + 1], color);
        }
    }
}
