using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleClass : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    //public void OnRenderObject()
    //{
    //    CreateLineMaterial();
    //    // Apply the line material
    //    lineMaterial.SetPass(0);

    //    GL.PushMatrix();
    //    // Set transformation matrix for drawing to
    //    // match our transform
    //    GL.MultMatrix(transform.localToWorldMatrix);

    //    // Draw lines
    //    GL.Begin(GL.LINES);
    //    for (int i = 0; i < lineCount; ++i)
    //    {
    //        float a = i / (float)lineCount;
    //        float angle = a * Mathf.PI * 2;
    //        // Vertex colors change from red to green
    //        GL.Color(new Color(a, 1 - a, 0, 0.8F));
    //        // One vertex at transform position
    //        GL.Vertex3(0, 0, 0);
    //        // Another vertex at edge of circle
    //        GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
    //    }
    //    GL.End();
    //    //GL.PopMatrix();
    //    Vector3 OldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    if (Input.GetMouseButton(0))
    //    {
    //        print(1);

    //        GL.Begin(GL.LINES);
    //        GL.Vertex3(OldPos.x, OldPos.y,0);
    //        GL.Vertex3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,0);
    //        GL.End();
           
    //    }
    //    GL.PopMatrix();
    //}
    Vector3 OldPos;
    Vector3 NewPos;



    // vars used for drawing lines with GL
    public Material mat;
    public Vector3 startVertex;
    public Vector3 mousePos;

    //create an array to store vertices.  Note that it will only store 1000 of them
    public Vector3[] vertexArr = new Vector3[1000];

    //store the last vertex (for comparison to see if the mouse has moved)
    public Vector3 lastVertex = new Vector3(0, 0, 0);

    //track the number of verteces
    public int vertexCount = 0;


    public List<Vector3[]> vector3ArrayList;
    List<Vector3> Vector3List;
    public int ArraySize;
    public int ArraySizeFirstEntry;
    public Vector3 FirstEntry;
    public int drawingindex=0;
    void Start()
    {
        vertexArr = new Vector3[1000];
           vector3ArrayList = new List<Vector3[]>();
        Vector3List = new List<Vector3>();

    }

void Update()
    {
        ArraySize = vector3ArrayList.Count;
        if (vector3ArrayList.Count>0)
        {
            ArraySizeFirstEntry= vector3ArrayList[0].Length;
            FirstEntry = vector3ArrayList[0][0];
        }

        mousePos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            vector3ArrayList.Add(vertexArr);
            startVertex = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);

            //if the mouse has moved, add the new position to the vertexArr array
            if (lastVertex != startVertex && vertexCount < 1000)
            {
                Vector3List.Add(startVertex);
                vector3ArrayList[drawingindex][vertexCount] = startVertex;
                //vertexArr[vertexCount] = startVertex;
                vertexCount++;
                lastVertex = startVertex;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            vertexArr = new Vector3[1000];
            vertexCount = 0;
            drawingindex++;
        }

    }

    void OnRenderObject()
    {
        foreach (var item in vector3ArrayList)
        {
            if (item[0] != Vector3.zero& item[1] != Vector3.zero)
            {

                int ind = 0;
                GL.PushMatrix();
                mat.SetPass(0);
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                GL.Color(Color.red);

                //loop through all the vertices in the vertexArr array.
                for (int i = 1; i < item.Length - 1; i++)
                {
                    
                    GL.Vertex(vector3ArrayList[drawingindex][i - 1]);
                    GL.Vertex(vector3ArrayList[drawingindex][i]);
                }
                //while (item[ind]!= Vector3.zero)
                //{
                //    GL.Vertex(vector3ArrayList[drawingindex][i - 1]);
                //    GL.Vertex(vector3ArrayList[drawingindex][i]);
                //}


                GL.End();
                GL.PopMatrix();
            }
        }
    }

}


//using UnityEngine;
//using System.Collections;

//// Attach this script to a Camera
//public class ExampleClass : MonoBehaviour
//{
//    public Mesh mesh;
//    public Material mat;
//    public void OnPostRender()
//    {
//        // set first shader pass of the material
//        mat.SetPass(0);
//        // draw mesh at the origin
//        Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
//    }
//}