using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    public Node(bool _wakable, Vector3 _wp , int _gridX,  int _gridY)
    {
        walkable = _wakable;
        worldPosition = _wp;
        gridX = _gridX;
        gridY = _gridY;
    }


}
