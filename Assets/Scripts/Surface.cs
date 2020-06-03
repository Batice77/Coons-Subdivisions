using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{
    public Curve curve1;
    public Curve curve2;
    public List<Vector3> lines;

    public Surface()
    {
        curve1 = new Curve();
        curve2 = new Curve();
    }

    public Surface(Curve c1, Curve c2)
    {
        this.curve1 = c1;
        this.curve2 = c2;
        lines = new List<Vector3>();
    }

    public void GenerateLines()
    {
        for (int i = 0; i < curve1.points.Count; i++)
        {
            lines.Add(curve1.points[i].position);
            lines.Add(curve2.points[i].position);
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
