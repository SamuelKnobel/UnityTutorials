using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : MonoBehaviour
{

    //create an List to store vertices.  Note that it will only store 2001 of them
    public List<Vector3> currentVertexList = new List<Vector3>();

    public List<List<Vector3>> VertexLists = new List<List<Vector3>>();

    public int NbOfLists = 0;
    //store the last vertex (for comparison to see if the mouse has moved)
    public Vector3 lastVertex = new Vector3(0, 0, 0);

    //track the number of verteces
    public int vertexCount = 0;

    // vars used for drawing lines with GL
    static Material lineMaterial;
    public Vector3 startVertex;
    public Vector3 mousePos;


    private void Awake()
    {
        CreateLineMaterial();
    }

    void Start()
    {
        startVertex = Vector3.zero;
    }

    void Update()
    {
        NbOfLists = VertexLists.Count;

        mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseButtonDown();
        } 
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseButtonUp();
        }

        if (Input.GetMouseButton(0))
        {
            startVertex = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);

            //if the mouse has moved, add the new position to the vertexArr array
            if (lastVertex != startVertex)
            {
                currentVertexList.Add(startVertex);
                vertexCount++;
            }
        }

        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            DeleteDrawing();
        }

        
    }
    void DeleteDrawing()
    {
        VertexLists.Clear();
        currentVertexList.Clear();
    }

    void OnMouseButtonDown()
    {
            currentVertexList = new List<Vector3>();

    }
    void OnMouseButtonUp()
    {
        VertexLists.Add(currentVertexList);

    }
    private void OnGUI()
    {
        //CounterOnGUI++;


    }


    void OnPostRender()
    {





        //CounterOnPosRender++;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, 1, 0, 1);
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        for (int i = 0; i < currentVertexList.Count - 1; i++)
        {
            GL.Vertex(currentVertexList[i]);
            GL.Vertex(currentVertexList[i + 1]);
        }

        foreach (var list in VertexLists)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                GL.Vertex(list[i]);
                GL.Vertex(list[i + 1]);
            }
        }

        GL.End();
        GL.PopMatrix();

        //GL.PushMatrix();
        //lineMaterial.SetPass(0);
        //GL.LoadOrtho();
        //GL.Begin(GL.LINES);
        //GL.Vertex(new Vector3(1, 0, 0));
        //GL.Vertex(new Vector3(0, 1, 0));
        //GL.Vertex(new Vector3(.5f, .5f, 0));
        //GL.End();
        //GL.PopMatrix();




        //// 

        //GL.PushMatrix();
        //lineMaterial.SetPass(0);
        //GL.LoadOrtho();

        //GL.Begin(GL.LINES);
        //GL.Color(Color.red);
        //GL.Vertex(startVertex);
        //GL.Vertex(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0));
        //GL.End();

        //GL.PopMatrix();
    }
    //private void OnRenderObject()
    //{
    //    GL.PushMatrix();
    //    mat.SetPass(0);
    //    GL.LoadOrtho();

    //    GL.Begin(GL.LINES);
    //    GL.Color(Color.green);
    //    GL.Vertex(startVertex);
    //    GL.Vertex(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0));
    //    GL.End();

    //    GL.PopMatrix();
    //}
    //private void OnPreRender()
    //{
    //    GL.PushMatrix();
    //    mat.SetPass(0);
    //    GL.LoadOrtho();

    //    GL.Begin(GL.LINES);
    //    GL.Color(Color.black);
    //    GL.Vertex(startVertex);
    //    GL.Vertex(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0));
    //    GL.End();

    //    GL.PopMatrix();
    //}



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
            lineMaterial.color = Color.black;
        }
    }


}
