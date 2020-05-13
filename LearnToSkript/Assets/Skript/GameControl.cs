using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {  // Klasse welche sicherstellt, dass nur ein Gamobject dieser Sorte exisiert

    public static GameControl Control;

    public float health;
    public float experience;

    private void Awake()
    {
        if (Control == null)  // wenn kein GameControll vorhanden, dann wird dieses zur Gamecontroll
        {
            DontDestroyOnLoad(gameObject); // jedes Gameobject welches dieses Script angehängt bekommt wird nicht zerstör beim wechseln der Scene
            Control = this;
        }
        else if (Control != this) // wenn bereits eines vorhendenzusätzlich zu diesem, wird dieses zerstört, da wir nur eines wollen
        {
            Destroy(gameObject);
        }

    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "Health: " + health);
        GUI.Label(new Rect(10, 40, 100, 30), "Experience: " + experience);
    }

}
