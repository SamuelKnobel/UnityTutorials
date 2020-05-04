using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        // Idea: change the rotation position of x y z for every frame

        transform.Rotate(new Vector3(15, 30, 45)*Time.deltaTime);

	}
}
