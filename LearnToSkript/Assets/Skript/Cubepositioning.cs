using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Cubepositioning : MonoBehaviour
{
    public GameObject[] Cubes_T;
    public TextAsset csvFile; // Reference of CSV file
    public Text contentArea; // Reference of contentArea where records are displaye
    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter
    private Vector3 spawnposition;


    void Start()
    {
        readData_Target();
    }
    
    // Read data from CSV file
    private void readData_Target()
    {
        List<int> iList = new List<int>();
        string[] records = csvFile.text.Split(lineSeperater);
        foreach (string record in records)

        {
            string[] fields = record.Split(fieldSeperator);
            foreach (string field in fields)
            {
                int fieldi = int.Parse(field);
                iList.Add(fieldi);
            }
        }
        for (int i= 0; i < 40; i = i + 2)
        {
            float x_cor = iList[i];
            float y_cor = iList[i + 1];
            x_cor =  x_cor /  3508f * 0.6f;
            y_cor = y_cor / 2552f * 0.4f;
            // Umrechnen der Koordinaten auf die Fläche der Plane!
            // Breite = 0.6, Höhe= 0.4--> 2552x3508

            spawnposition.x = y_cor;
            spawnposition.y = 0.4f-x_cor ;
            spawnposition.z = 0;

            Quaternion spawnrotation = Quaternion.identity;
            GameObject Cube_T = Cubes_T[Random.Range(0, Cubes_T.Length)];
            Instantiate(Cube_T, spawnposition, spawnrotation);
            Debug.Log(spawnposition);
            



        }
    }
}

