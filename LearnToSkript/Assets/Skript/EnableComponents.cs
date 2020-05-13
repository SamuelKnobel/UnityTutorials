using UnityEngine;
using System.Collections;

public class EnableComponents : MonoBehaviour
{
    private Light myLight;
    public GameObject Sphere;
    private bool log = false;


    void Start()
    {
        myLight = GetComponent<Light>();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //  myLight.enabled = !myLight.enabled;
            log = !log;

            Sphere.SetActive(log);
            Debug.Log("Status:" + log);
        }
    }
}