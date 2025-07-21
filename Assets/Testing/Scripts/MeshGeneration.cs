using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibTessDotNet;
using drawing = System.Drawing;

public class MeshGenerator : MonoBehaviour
{
    public int xSize;
    public int zSize;
    public int lastxSize;
    public Mesh mesh;

    public float y;

    Vector3[] vertices;
    int[] triangles;
    public float raise;
    [SerializeField] private GameObject meshObject;
    [SerializeField] private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh = meshObject.GetComponent<MeshFilter>().mesh;
        meshObject.GetComponent<Renderer>().material = mat;
        CreateMesh();
    }

    // Update is called once per frame
    private void Update()
    {
        RenderMesh();

    }

    // The data array contains 4 values, it's the associated data of the vertices that resulted in an intersection.
    private static object VertexCombine(LibTessDotNet.Vec3 position, object[] data, float[] weights)
    {
        // Fetch the vertex data.
        var colors = new drawing.Color[] { (drawing.Color)data[0], (drawing.Color)data[1], (drawing.Color)data[2], (drawing.Color)data[3] };
        // Interpolate with the 4 weights.
        var rgba = new float[] {
            (float)colors[0].R * weights[0] + (float)colors[1].R * weights[1] + (float)colors[2].R * weights[2] + (float)colors[3].R * weights[3],
            (float)colors[0].G * weights[0] + (float)colors[1].G * weights[1] + (float)colors[2].G * weights[2] + (float)colors[3].G * weights[3],
            (float)colors[0].B * weights[0] + (float)colors[1].B * weights[1] + (float)colors[2].B * weights[2] + (float)colors[3].B * weights[3],
            (float)colors[0].A * weights[0] + (float)colors[1].A * weights[1] + (float)colors[2].A * weights[2] + (float)colors[3].A * weights[3]
        };
        // Return interpolated data for the new vertex.
        return drawing.Color.FromArgb((int)rgba[3], (int)rgba[0], (int)rgba[1], (int)rgba[2]);
    }

    void CreateMesh()
    {
        float rotAngle = 360 / xSize;


        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {


            for (int x = 0; x <= xSize; x++)
            {

                vertices[i] = new Vector3(x, 0, z);
                i++;

                //GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //newSphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                //newSphere.transform.position = transform.position + transform.up * 5;

                //transform.Rotate(0, 0, rotAngle);

            }
            //transform.Rotate(0, 0, rotAngle / 2);
            //transform.Translate(0, 0, 1);
        }
        var contour = new ContourVertex[vertices.Length];
        for(int i = 0;i < vertices.Length;i++)
        {
            contour[i].Position = new LibTessDotNet.Vec3(vertices[i].x, vertices[i].z, 0);
            contour[i].Data = drawing.Color.Azure; 

        }

        var tess = new LibTessDotNet.Tess();

        tess.AddContour(contour, ContourOrientation.Clockwise);

        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3, VertexCombine);

        int numTriangles = tess.ElementCount;
        for (int i = 0; i < numTriangles; i++)
        {
            var a = tess.Elements[i * 3 + 0];
            var b = tess.Elements[i * 3 + 1];
            var c = tess.Elements[i * 3 + 2];


            triangles[i * 3 + 0] = a;
            triangles[i * 3 + 1] = b;
            triangles[i * 3 + 2] = c;
        }
    }

    void RenderMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        meshObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}