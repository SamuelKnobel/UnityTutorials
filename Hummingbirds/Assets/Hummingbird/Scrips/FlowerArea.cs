using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages a collection of flower plants and attatched flowers
/// </summary>
public class FlowerArea : MonoBehaviour
{

    // The diameter of the area where the agend and flowers can be used for 
    // observing relative distance from agent to flower
    public const float AreaDiameter = 20f;


    // The list of all Flower plants in this flower area (Flower Plants have multiple flowers)
    private List<GameObject> flowerPlants;

    // A lookup dictionary for looking up a flower from a nectar collider
    private Dictionary<Collider, Flower> nectarFlowerDictionary;

    /// <summary>
    /// The list of all flwoers in the flower area
    /// </summary>
    public List<Flower> Flowers { get; private set; }


    /// <summary>
    /// Reset the flowers and flowerplants
    /// </summary>
    public void ResetFlower()
    {
        //Rotate each flower plant around Y axix and subtly around X and Z
        foreach (GameObject flowerPlant in flowerPlants)
        {
            float xRotation = UnityEngine.Random.Range(-5f, 5f);
            float yRotation = UnityEngine.Random.Range(-180f, 180f);
            float zRotation = UnityEngine.Random.Range(-5f, 5f);

            flowerPlant.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        // Reset each flower
        foreach (Flower flower in Flowers)
        {
            flower.ResetFlower();
        }
    }

    /// <summary>
    /// Gets the <see cref="Flower"/> that a nectar collider belongs to
    /// </summary>
    /// <param name="collider">The nectar collider</param>
    /// <returns>The matching flower</returns>
    public Flower GetFlowerFromNectar(Collider collider)
    {
        return nectarFlowerDictionary[collider];
    }
    /// <summary>
    /// Called when the area wakes up
    /// </summary>
    private void Awake()
    {
        // initilaizes variables
        flowerPlants = new List<GameObject>();
        nectarFlowerDictionary = new Dictionary<Collider, Flower>();
        Flowers = new List<Flower>();

    }

    /// <summary>
    ///  called when the game starts
    /// </summary>
    private void Start()
    {
        //Find all flowers that are chlidren of this GameObject/Transform
        FindChildFlower(transform);
    }


    /// <summary>
    /// Recursevly findes alle flowers and flower plants that are childeren of a parent transform
    /// </summary>
    /// <param name="parent">the parent of the childeren to check</param>
    private void FindChildFlower(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag("flower_plant"))
            {
                // found  flower plant, add it to the flowerPlants list
                flowerPlants.Add(child.gameObject);

                // Look for flowers within the flower plants
                FindChildFlower(child);
            }
            else
            {
                //not a flowerplant, look for a flower component
                Flower flower = child.GetComponent<Flower>();
                if (flower != null)
                {
                    // Found a flower , add it ot the Flower list
                    Flowers.Add(flower);

                    // Add Nectar Collider to the Lookup Dict
                    nectarFlowerDictionary.Add(flower.nectarCollider, flower);

                    // note: there are no flowers that are chlindern of other flowers
                }
                else
                {
                    // Flower component not found,so check childern
                    FindChildFlower(child);


                }
            }
        }
    }


}
