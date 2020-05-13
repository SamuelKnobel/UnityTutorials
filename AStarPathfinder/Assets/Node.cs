using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    int heapIndex;
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

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare ==0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;

    }

    public int HeapIndex 
    {
        get
        {
            return heapIndex;
        }
        set { heapIndex = value; }
    }

}
