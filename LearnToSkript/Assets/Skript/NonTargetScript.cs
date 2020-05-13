using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTargetScript : MonoBehaviour
{
    public GameObject CubeNT;

    public void Start()
    {
         CubeNT = GameObject.FindGameObjectWithTag("NoTarget");
        
    }      
}
 
   

        