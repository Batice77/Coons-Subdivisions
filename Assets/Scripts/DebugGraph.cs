using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebugGraph : MonoBehaviour
{
    Material mat;

    public static MeshUtility meshUtility;

    List<Color> SToAColor;
    List<Color> AToFColor;

    bool isHide = false;

    // Start is called before the first frame update
    void Start()
    {
        SToAColor = new List<Color>();
        AToFColor = new List<Color>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SToAColor.Clear();
            AToFColor.Clear();
        }
        if (Input.GetKeyDown(KeyCode.H)) isHide = !isHide;
    }

    void OnPostRender()
    {
        if (isHide) return;
        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            //mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            /*Debug.LogError("Please Assign a material on the inspector");
            return;*/
        }
        if (meshUtility == null) return;

        for (int i = SToAColor.Count - 1; i < meshUtility.edges.Count * 2; ++i)
        {
            SToAColor.Add(Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));
        }
        for (int i = AToFColor.Count - 1; i < meshUtility.triangles.Count * 3; ++i)
        {
            AToFColor.Add(Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));
        }

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        Vertex vert;
        for (int i = 0; i < meshUtility.vertices.Count; ++i)
        {
            vert = meshUtility.vertices[i];
            GL.Color(vert.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.05f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.05f, 0);
        }
        GL.End();

        Edge edge;
        for (int i = 0; i < meshUtility.edges.Count; ++i)
        {
            edge = meshUtility.edges[i];
            GL.Begin(GL.QUADS);
            GL.Color(edge.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0.15f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.15f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.20f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.20f, 0);
            GL.End();

            if (edge.vertices[0] != null)
            {
                int iA = meshUtility.GetVertexIndex(edge.vertices[0]);
                GL.Begin(GL.LINES);
                GL.Color(SToAColor[i]);
                GL.Vertex3(0.05f * iA + 0.025f, 1 - 0.05f, 0);
                GL.Vertex3(0.05f * i + 0.0125f, 1 - 0.15f, 0);
                GL.End();

                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.white);
                GL.Vertex3(0.05f * i + 0.0125f, 1 - 0.15f, 0);
                GL.Vertex3(0.05f * i + 0.0125f - 0.002f, 1 - (0.15f + 0.005f), 0);
                GL.Vertex3(0.05f * i + 0.0125f + 0.002f, 1 - (0.15f + 0.005f), 0);
                GL.End();
            }

            if (edge.vertices[1] != null)
            {
                int iB = meshUtility.GetVertexIndex(edge.vertices[1]);
                GL.Begin(GL.LINES);
                GL.Color(SToAColor[meshUtility.edges.Count + i]);
                GL.Vertex3(0.05f * iB + 0.025f, 1 - 0.05f, 0);
                GL.Vertex3(0.05f * i + 0.0375f, 1 - 0.15f, 0);
                GL.End();

                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.white);
                GL.Vertex3(0.05f * i + 0.0375f, 1 - 0.15f, 0);
                GL.Vertex3(0.05f * i + 0.0375f - 0.002f, 1 - (0.15f + 0.005f), 0);
                GL.Vertex3(0.05f * i + 0.0375f + 0.002f, 1 - (0.15f + 0.005f), 0);
                GL.End();
            }
        }

        Triangle triangle;
        for (int i = 0; i < meshUtility.triangles.Count; ++i)
        {
            triangle = meshUtility.triangles[i];
            GL.Begin(GL.QUADS);
            GL.Color(triangle.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0.30f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.30f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.35f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.35f, 0);
            GL.End();

            if (triangle.edges[0] != null)
            {
                int iA = meshUtility.GetEdgeIndex(triangle.edges[0]);
                GL.Begin(GL.LINES);
                GL.Color(AToFColor[i]);
                GL.Vertex3(0.05f * iA + 0.025f, 1 - 0.20f, 0);
                GL.Vertex3(0.05f * i + 0.0125f, 1 - 0.30f, 0);
                GL.End();

                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.white);
                GL.Vertex3(0.05f * i + 0.0125f, 1 - 0.30f, 0);
                GL.Vertex3(0.05f * i + 0.0125f - 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.Vertex3(0.05f * i + 0.0125f + 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.End();
            }

            if (triangle.edges[1] != null)
            {
                int iB = meshUtility.GetEdgeIndex(triangle.edges[1]);
                GL.Begin(GL.LINES);
                GL.Color(AToFColor[meshUtility.triangles.Count + i]);
                GL.Vertex3(0.05f * iB + 0.025f, 1 - 0.20f, 0);
                GL.Vertex3(0.05f * i + 0.025f, 1 - 0.30f, 0);
                GL.End();

                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.white);
                GL.Vertex3(0.05f * i + 0.025f, 1 - 0.30f, 0);
                GL.Vertex3(0.05f * i + 0.025f - 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.Vertex3(0.05f * i + 0.025f + 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.End();
            }


            if (triangle.edges[2] != null)
            {
                int iB = meshUtility.GetEdgeIndex(triangle.edges[2]);
                GL.Begin(GL.LINES);
                GL.Color(AToFColor[meshUtility.triangles.Count * 2 + i]);
                GL.Vertex3(0.05f * iB + 0.025f, 1 - 0.20f, 0);
                GL.Vertex3(0.05f * i + 0.0375f, 1 - 0.30f, 0);
                GL.End();

                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.white);
                GL.Vertex3(0.05f * i + 0.0375f, 1 - 0.30f, 0);
                GL.Vertex3(0.05f * i + 0.0375f - 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.Vertex3(0.05f * i + 0.0375f + 0.002f, 1 - (0.30f + 0.005f), 0);
                GL.End();
            }
        }

        GL.PopMatrix();
    }
}