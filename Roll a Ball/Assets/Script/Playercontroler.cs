using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroler : MonoBehaviour {


    public float speed; // appears as a edittable variable in the editor
    public Text CountText;
    public Text WinText;

    private Rigidbody rb;  // create a variable to hold the parameters of the rigid body
    private int count; // variable to store the counted pickups/ int da nur ganze zahlen

     void Start()
    {
        rb = GetComponent<Rigidbody>(); // import the rigidbody information into the script --> references to another component of the Minigame
        count = 0;
        SetCountText();
        WinText.text = "";
        
    }
  //  void Update() // called before rendering a Frame <-- GAME CODE
  //  {
    
  //  }
     void FixedUpdate() // Physics Code
    {
        float moveHorizonal = Input.GetAxis("Horizontal");  // grabs the Input from the player to the Keybord
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizonal, 0, moveVertical);

        rb.AddForce(movement*speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup")) 
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        CountText.text = "Count:" + count.ToString();
        if (count >= 12)
        {
            WinText.text = "You Win";
                }
    }




}


