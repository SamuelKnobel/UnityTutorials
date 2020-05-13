using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    
    public GameObject CubeT;

    public void Start()
    {
   CubeT = GameObject.FindGameObjectWithTag("Target");
        


    }      
}
 