using UnityEngine;
using System.Collections;

public class ExampleBehaviourScript : MonoBehaviour
    
{
    public Vector3Int  higth;
    public Quaternion rot;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<Renderer>().material.color = Color.blue;
            GetComponent<Transform>().SetPositionAndRotation(higth, rot);
        }
    }
}