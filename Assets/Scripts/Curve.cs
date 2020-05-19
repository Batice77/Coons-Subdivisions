using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    public List<Transform> points;

    public Curve()
    {
        points = new List<Transform>();
    }

    public Curve(List<Transform> curve)
    {
        points = curve;
    }

    public void AddPoint(Vector3 coords, GameObject point_representation)
    {
        GameObject new_point = GameObject.Instantiate(point_representation, coords, Quaternion.identity);
        new_point.transform.position = coords;
        points.Add(new_point.transform);
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
