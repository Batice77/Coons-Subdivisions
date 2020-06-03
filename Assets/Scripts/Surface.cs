using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{
    public Curve c1;
    public Curve c2;
    public List<Vector3> lines;

    public Surface()
    {
        c1 = new Curve();
        c2 = new Curve();
    }

    public Surface(Curve c1, Curve c2)
    {
        this.c1 = c1;
        this.c2 = c2;
        lines = new List<Vector3>();
    }

    public void GenerateLines()
    {
        for (int i = 0; i < c1.points.Count; i++)
        {
            lines.Add(c1.points[i].position);
            lines.Add(c2.points[i].position);
        }
    }

    public void Draw(Color color)
    {
        for (int i = 0; i < lines.Count; i+=2)
        {
            Debug.DrawLine(lines[i], lines[i + 1], color);
        }
    }
}
