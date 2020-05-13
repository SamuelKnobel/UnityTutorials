using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransfereScene : MonoBehaviour {
    private Scene scene;
    void Start ()
    {
         
    }

    private void OnGUI()
    {
        scene = SceneManager.GetActiveScene();
        GUI.Label(new Rect(Screen.width/2 -50, Screen.height-80 ,200,30), "Current Scene" + SceneManager.GetActiveScene().name);

        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height - 50, 150, 40), "Load other Scene"))
        {
            Debug.Log("Button pressed");
            Debug.Log(scene.name);

            switch (scene.name)
            {
                case "Scene4":
                    Debug.Log(scene.name);
                    SceneManager.LoadScene("Scene4a");
                    break;

                case "Scene4a":
                    Debug.Log(scene.name);
                    SceneManager.LoadScene("Scene4");
                    break;

            }

        }

       // GUI.Button(new Rect(Screen.width/ 2 - 50, Screen.height - 50, 100, 40), "Load Scene" + SceneManager.GetSceneByBuildIndex(SceneManager.sceneCount+1).name);
    }

   /* public void Level_4()
    {
        SceneManager.LoadScene("Level_4");
    }


    public void Level_4a()
    {
        SceneManager.LoadScene("Level_4a");
    }
*/
    
}
