using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Chaikin : MonoBehaviour
{
    [SerializeField]
    private List<Transform> polygon_line;
    private List<Transform> chaikin_curve;

    private void Start()
    {
        chaikin_curve = new List<Transform>();
        CreateChaikinCurve(0.33f, 0.25f);
    }

    private void Update()
    {
        DrawPolygonLine();
        DrawChaikinCurve();
    }

    public void DrawPolygonLine()
    {
        for (int i = 0; i < polygon_line.Count - 1; i++)
            DrawLine(polygon_line[i].position, polygon_line[i + 1].position, Color.yellow);
    }

    public void DrawChaikinCurve()
    {
        for (int i = 0; i < chaikin_curve.Count - 1; i++)
            DrawLine(chaikin_curve[i].position, chaikin_curve[i + 1].position, Color.red);
    }

    public void CreateChaikinCurve(float u, float v)
    {
        Vector3 p1, p2, u_director, new_p1, new_p2;
        for (int i = 0; i < polygon_line.Count - 1; i++)
        {
            p1 = polygon_line[i].position;
            p2 = polygon_line[i+1].position;
            u_director = p2 - p1;
            Debug.Log("u_director : " + u_director);
            new_p1 = new Vector3(p1.x + u_director.x * u, p1.y + u_director.y * u, p1.z + u_director.z * u);
            Debug.Log("new_p1 = " + new_p1);
            AddPointOnChaikinCurve(new_p1, "P");
            new_p2 = new Vector3(p1.x + u_director.x * (1-v), p1.y + u_director.y * (1-v), p1.z + u_director.z * (1-v));
            Debug.Log("new_p2 = " + new_p2);
            AddPointOnChaikinCurve(new_p2, "P");
        }
    }

    public void AddPointOnChaikinCurve(Vector3 position, string name)
    {
        GameObject new_point = Instantiate(polygon_line[0].gameObject, position, Quaternion.identity);
        new_point.transform.position = position;
        chaikin_curve.Add(new_point.transform);
    }

    public void DrawLine(Vector3 p1, Vector3 p2, Color color)
    {
        Debug.DrawLine(p1, p2, color);
    }
}
