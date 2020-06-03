using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Chaikin : MonoBehaviour
{
    private Curve C1;
    private Curve C2;
    private Curve d1;
    private Curve d2;
    private Surface s1;
    private Surface s2;
    [SerializeField]
    private List<Transform> C1_scene;
    [SerializeField]
    private List<Transform> C2_scene;
    [SerializeField]
    private List<Transform> d1_scene;
    [SerializeField]
    private List<Transform> d2_scene;
    [SerializeField]
    private GameObject point;
    [SerializeField]
    private GameObject surface_point;
    [SerializeField]
    private bool show_points;
    private Curve chaikin_curve_C1;
    private Curve chaikin_curve_C2;
    private Curve chaikin_curve_d1;
    private Curve chaikin_curve_d2;
    [SerializeField]
    private bool show_C1 = true;
    [SerializeField]
    private bool show_C2 = true;
    [SerializeField]
    private bool show_d1 = true;
    [SerializeField]
    private bool show_d2 = true;
    [SerializeField]
    private bool show_chaikin_curve_C1 = true;
    [SerializeField]
    private bool show_chaikin_curve_C2 = true;
    [SerializeField]
    private bool show_chaikin_curve_d1 = true;
    [SerializeField]
    private bool show_chaikin_curve_d2 = true;

    private void Start()
    {
        C1 = new Curve(C1_scene);
        C2 = new Curve(C2_scene);
        d1 = new Curve(d1_scene);
        d2 = new Curve(d2_scene);
        chaikin_curve_C1 = new Curve(CreateChaikinCurve(C1, 0.33f, 0.25f, 2).points);
        chaikin_curve_C2 = new Curve(CreateChaikinCurve(C2, 0.33f, 0.25f, 2).points);
        chaikin_curve_d1 = new Curve(CreateChaikinCurve(d1, 0.25f, 0.25f, 2).points);
        chaikin_curve_d2 = new Curve(CreateChaikinCurve(d2, 0.25f, 0.25f, 2).points);
        s1 = new Surface(chaikin_curve_C1, chaikin_curve_C2);
        s1.GenerateLines();
        s2 = new Surface(chaikin_curve_d1, chaikin_curve_d2);
        s2.GenerateLines();
    }

    private void Update()
    {
        C1.Draw(Color.yellow);
        chaikin_curve_C1.Draw(new Color(0.5f, 0.5f, 0));
        C2.Draw(Color.red);
        chaikin_curve_C2.Draw(new Color(0.5f, 0, 0));
        d1.Draw(Color.blue);
        chaikin_curve_d1.Draw(new Color(0, 0, 0.5f));
        d2.Draw(Color.green);
        chaikin_curve_d2.Draw(new Color(0, 0.5f, 0));
        s1.Draw(new Color(0.5f, 0, 0.5f));
        s2.Draw(new Color(0, 0.5f, 0.5f));
    }

    public Curve CreateChaikinCurve(Curve initial_curve, float u, float v, int nb_iteration)
    {
        Vector3 p1, p2, u_director, new_p1, new_p2;
        Curve tmp_polygon_line = initial_curve;
        Curve curve = new Curve();
        curve.AddPoint(tmp_polygon_line.points[0].position, point);
        for (int i = 0; i < tmp_polygon_line.points.Count - 1; i++)
        {
            p1 = tmp_polygon_line.points[i].position;
            p2 = tmp_polygon_line.points[i + 1].position;
            u_director = p2 - p1;
            //Debug.Log("u_director : " + u_director);
            new_p1 = new Vector3(p1.x + u_director.x * u, p1.y + u_director.y * u, p1.z + u_director.z * u);
            //Debug.Log("new_p1 = " + new_p1);
            //AddPointOnChaikinCurve(], new_p1, "P");
            AddPointOnChaikinCurve(curve, new_p1, "P");
            new_p2 = new Vector3(p1.x + u_director.x * (1 - v), p1.y + u_director.y * (1 - v), p1.z + u_director.z * (1 - v));
            //Debug.Log("new_p2 = " + new_p2);
            //AddPointOnChaikinCurve(tmp_chaikin_curve, new_p2, "P");
            AddPointOnChaikinCurve(curve, new_p2, "P");
        }
        curve.AddPoint(tmp_polygon_line.points[tmp_polygon_line.points.Count-1].position, point);
        nb_iteration--;
        if (nb_iteration != 0)
        {
            curve = CreateChaikinCurve(curve, u, v, nb_iteration);
        }
        return curve;
        //tmp_polygon_line = curve;
    }

    public void AddPointOnChaikinCurve(Curve curve, Vector3 position, string name)
    {
        GameObject new_point = Instantiate(point, position, Quaternion.identity);
        new_point.transform.position = position;
        curve.AddPoint(new_point.transform.position, point);
    }


    //public void ClearChaikinCurve(Curve curve)
    //{
    //    for (int i = 0; i < curve.points.Count; i++)
    //    {
    //        curve.RemoveAt(i);
    //    }
    //}

    //public void DrawLine(Vector3 p1, Vector3 p2, Color color)
    //{
    //    Debug.DrawLine(p1, p2, color);
    //}
}
