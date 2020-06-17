using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages a Single Flower with Nectar
/// </summary>
public class Flower : MonoBehaviour
{
    [Tooltip("The Color when the flower is full")]
    public Color fullFlowerColor = new Color(1, 0, .3f);   

    [Tooltip("The Color when the flower is empty")]
    public Color emptyFlowerColor = new Color(.5f, 0, 1f);

    /// <summary>
    /// Trigger Collider representing the nectar
    /// </summary>
    [HideInInspector]
    public Collider nectarCollider;

    // solid Collider representing the flower pedals
    private Collider flowerCollider;

    // flower material
    private Material flowerMaterial;


    /// <summary>
    ///  A Vector pointing straight out of the Flower
    /// </summary>
    public Vector3 FlowerUpVector
    {
        get
        {
            return nectarCollider.transform.up;
        }
    }
    /// <summary>
    /// The center position of a nectar collider
    /// </summary>
    public Vector3 FlowerCenterPosition
    {
        get
        {
            return nectarCollider.transform.position;
        }
    }
    /// <summary>
    /// The amount of Nectar remaining in the flower
    /// </summary>
    public float NectarAmount { get; private set; }
    /// <summary>
    /// Whether the flower has any nectar remaining
    /// </summary>
    public bool HasNectar
    {
        get
        {
            return NectarAmount > 0f;
        }
    }

    private void Awake()
    {
        //Finde the flower Meshrenderenr and the main Material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        flowerMaterial = meshRenderer.material;

        // find flower an nectar colliders
        flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }


    /// <summary>
    /// Attemots to remove nectar from the flower
    /// </summary>
    /// <param name="amount"The amount of nectar to remove></param>
    /// <returns>the actual amount successfully removed</returns>
    public float Feed(float amount)
    {
        // Track how much Nectar was successfully taken (cannot take more than is availible)
        float nectarTaken = Mathf.Clamp(amount, 0f, NectarAmount);

        //Subtract the nectar

        NectarAmount -= amount;
        if (NectarAmount <=0)
        {
            // No Nectar Remaining
            NectarAmount = 0;

            // Disable the flower an nectar colliders
            flowerCollider.gameObject.SetActive(false);
            nectarCollider.gameObject.SetActive(false);

            //change Flower Color to indicate that it is emtpty
            flowerMaterial.SetColor("_BaseColor", emptyFlowerColor);
        }
        //retunr the amount of nectar that was taken
        return nectarTaken;
    }

    /// <summary>
    /// Reset the flower
    /// </summary>
    public void ResetFlower()
    {
        // refill nectar
        NectarAmount = 1f;
        // Disable the flower an nectar colliders
        flowerCollider.gameObject.SetActive(true);
        nectarCollider.gameObject.SetActive(true);

        //Change the FlowerColor to indicate that it is full
        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);

    }
}
