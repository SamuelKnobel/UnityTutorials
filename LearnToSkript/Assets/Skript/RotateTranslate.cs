
using UnityEngine;
using System.Collections;

public class RotateTranslate : MonoBehaviour
{
    
    private Renderer rendT, rendNT;

    private bool rayhit;

    public int i=0;

    void Start()
    {
       // StartCoroutine(SpawnCubes());

        GameObject CubeT = GameObject.FindWithTag("Target");
        TargetScript TargetComponent = CubeT.GetComponent<TargetScript>();

        GameObject CubeNT = GameObject.FindWithTag("NoTarget");
        NonTargetScript NonTargetComponent = CubeNT.GetComponent<NonTargetScript>();

       // rendT = TargetComponent.CubeT.GetComponent<Renderer>();
        Debug.Log(rendT);
      //  rendNT = NonTargetComponent.CubeNT.GetComponent<Renderer>();
    }

    void Update()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.transform.tag == "Target")
                {
                    print("Hit right!");
                    rendT = hit.transform.GetComponent<Renderer>();
                    rendT.material.color = Color.red;

                  //  MuzzleFlash();
                }
                else if (hit.transform.tag == "NoTarget" | hit.transform.tag == "Untagged")
                {
                    print("Wrong target");
                    //rendT.material.color = Color.blue;
                }
            }
            else
            {
                print("Hit nothing!");
              
            }
        }
    }

    public void MuzzleFlash()
    {
        StartCoroutine(MuzzleFlashCR());
    }

    private IEnumerator MuzzleFlashCR()
    {
        yield return new WaitForSeconds(1f);
        rendT.enabled = false;

    }
}





