using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    public List<Transform> points;
    private float size;

    public Curve()
    {
        points = new List<Transform>();
        size = 0;
    }

    public Curve(List<Transform> curve)
    {
        points = curve;
        setSize();
    }

    public float GetSize()
    {
        setSize();
        return size;
    }

    public void setSize()
    {
        float n = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            
            n += Vector3.Distance(points[i].position, points[i + 1].position);
        }
        size = n;
    }

    public void AddPoint(Vector3 coords, GameObject point_representation)
    {
        GameObject new_point = Object.Instantiate(point_representation, coords, Quaternion.identity);
        new_point.transform.position = coords;
        points.Add(new_point.transform);
        if (points.Count > 1)
            setSize();
    }

    public void Draw(Color color)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Debug.DrawLine(points[i].position, points[i + 1].position, color);
        }
    }
    
    public void ShowPointsOrNot(bool show)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].gameObject.SetActive(show);
        }
    }
}
