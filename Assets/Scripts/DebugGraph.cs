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

        for (int i = SToAColor.Count - 1; i < meshUtility.Edges.Count * 2; ++i)
        {
            SToAColor.Add(Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));
        }
        for (int i = AToFColor.Count - 1; i < meshUtility.Triangles.Count * 3; ++i)
        {
            AToFColor.Add(Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));
        }

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        Vertex vert;
        for (int i = 0; i < meshUtility.Vertices.Count; ++i)
        {
            vert = meshUtility.Vertices[i];
            GL.Color(vert.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.05f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.05f, 0);
        }
        GL.End();

        Edge edge;
        for (int i = 0; i < meshUtility.Edges.Count; ++i)
        {
            edge = meshUtility.Edges[i];
            GL.Begin(GL.QUADS);
            GL.Color(edge.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0.15f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.15f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.20f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.20f, 0);
            GL.End();

            if (edge.Vertices[0] != null)
            {
                int iA = meshUtility.GetVertexIndex(edge.Vertices[0]);
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

            if (edge.Vertices[1] != null)
            {
                int iB = meshUtility.GetVertexIndex(edge.Vertices[1]);
                GL.Begin(GL.LINES);
                GL.Color(SToAColor[meshUtility.Edges.Count + i]);
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
        for (int i = 0; i < meshUtility.Triangles.Count; ++i)
        {
            triangle = meshUtility.Triangles[i];
            GL.Begin(GL.QUADS);
            GL.Color(triangle.GetColor());
            GL.Vertex3(0.05f * i, 1 - 0.30f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.30f, 0);
            GL.Vertex3(0.05f * i + 0.045f, 1 - 0.35f, 0);
            GL.Vertex3(0.05f * i, 1 - 0.35f, 0);
            GL.End();

            if (triangle.Edges[0] != null)
            {
                int iA = meshUtility.GetEdgeIndex(triangle.Edges[0]);
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

            if (triangle.Edges[1] != null)
            {
                int iB = meshUtility.GetEdgeIndex(triangle.Edges[1]);
                GL.Begin(GL.LINES);
                GL.Color(AToFColor[meshUtility.Triangles.Count + i]);
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


            if (triangle.Edges[2] != null)
            {
                int iB = meshUtility.GetEdgeIndex(triangle.Edges[2]);
                GL.Begin(GL.LINES);
                GL.Color(AToFColor[meshUtility.Triangles.Count * 2 + i]);
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