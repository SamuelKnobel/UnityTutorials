using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;


public class Exporter : MonoBehaviour
{
    public GameControll gC;
    public GUIHandler guiHandler;
    public List<GameObject> HexList;
    public List<string> NewList;



    string json;
    string[] alldataAsJson;

    public List<Fields> fieldList;
    public List<Fields> NewfieldList;
    public string SavePath;
    public string fullPath;
    public string PathLatestSave;

    // Use this for initialization
    void Start()
    {
        NewList = new List<string>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // CreateNewTable();            

        }
        /// Write to JSON
        if (Input.GetKeyDown(KeyCode.M))
        {
            EndofRoundSave();
        }

        /// Read From JSON
        if (Input.GetKeyDown(KeyCode.H))
        {
            NewfieldList = new List<Fields>();

            alldataAsJson = File.ReadAllLines(PathLatestSave);
            foreach (var item in alldataAsJson)
            {
                print(item);
                if (item.Length > 5)
                {
                    Fields loadedData = JsonUtility.FromJson<Fields>(item);
                    NewfieldList.Add(loadedData);
                }
            }
        } 

    }
    public void EndofRoundSave()
    {
        
        string Path = SavePath + "Fields_";
        if (!Directory.Exists(Path = SavePath))
        {
            Directory.CreateDirectory(Path = SavePath);
        } 
        int counter = 1;
        string fullPath;

        while (File.Exists(Path + gC.currentRound + "_" + counter + ".json"))
        {
            counter++;
        }

        fullPath = Path + "Round_" + gC.currentRound + "_" + counter + ".json";

        foreach (var item in gC.fieldList)
        {
            json = JsonUtility.ToJson(item);

            if (File.Exists(fullPath))
            {
                using (StreamWriter streamWriter = File.AppendText(fullPath))
                {
                    streamWriter.WriteLine(json);
                }
            }
            else
            {
                using (StreamWriter streamWriter = File.CreateText(fullPath))
                {
                    streamWriter.WriteLine(json);
                }
            }
        }
        //PathLatestSave = fullPath;

         Path = SavePath + "Races_";
         counter = 1;

        while (File.Exists(Path + gC.currentRound + "_" + counter + ".json"))
        {
            counter++;
        }

        fullPath = Path + "Round_" + gC.currentRound + "_" + counter + ".json";

        foreach (var item in gC.playableRaces)
        {
            json = JsonUtility.ToJson(item);

            if (File.Exists(fullPath))
            {
                using (StreamWriter streamWriter = File.AppendText(fullPath))
                {
                    streamWriter.WriteLine(json);
                }
            }
            else
            {
                using (StreamWriter streamWriter = File.CreateText(fullPath))
                {
                    streamWriter.WriteLine(json);
                }
            }
        }
        //PathLatestSave = fullPath;




    }

    public void LogFileSave(string LogToSave)
    {
        string Path = SavePath + "LogFile.txt";

        if (File.Exists(Path))
        {
            using (StreamWriter streamWriter = File.AppendText(Path))
            {
                streamWriter.WriteLine(LogToSave);
            }
        }
        else
        {
            using (StreamWriter streamWriter = File.CreateText(Path))
            {
                streamWriter.WriteLine(LogToSave);
            }
        }
    }

    public void CreateNewTable()
    {
        string path = gC.filepath_Dropbox + "/Rounds/Spielfeld_Round_" +
            gC.currentRound + ".csv";
        if (File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path);

            HexList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Hexagon"));
            foreach (GameObject item in HexList)
            {
                HexagonSettings HexSet = item.GetComponent<HexagonSettings>();

                string[] fieldname_Array = item.name.Split('_');
                string FieldNbr = fieldname_Array[1];
                if (FieldNbr != HexSet.field.FieldNb.ToString())
                    Debug.LogError("Error in Hexagon:" + item.name);

                string FieldName = HexSet.field.FieldName;
                string Building = HexSet.field.MainBuilding.ToString();

                string BuildingLifeTime = ""; // TODO!
                string Ore = HexSet.field.oreOutput.ToString();
                string Fert = HexSet.field.fertility.ToString();


                string upgraded = "";
                string pop = HexSet.field.population.ToString();
                string Heros = HexSet.field.heroOrigine;
                string specials = HexSet.field.speciales;
                string race = HexSet.field.peoples.ToString();

                //createList(FieldNbr,FieldName, Building, BuildingLifeTime, Ore, Fert, upgraded,pop,Heros,specials,race);
                writer.WriteLine(createList(FieldNbr, FieldName, Building, BuildingLifeTime,
                    Ore, Fert, upgraded, pop, Heros, specials, race));
            }
            writer.Close();
        }
        else
        {
            //TODO: If File Already exists --> Combine Inputs from different sources
            // Idee: gezielt überschreiben nciht ganze Liste neu schreiben!
        }


    }

    public string createList(string string0, string string1, string string2, string string3, string string4, string string5, 
        string string6, string string7, string string8, string string9, string string10)
    {
        NewList.Add(string0 +";"+string1 + ";" + string2 + ";" + string3 + ";" + string4 + ";" + string5 + ";" + string6+
            ";"+ string7 + ";" + string8 + ";" + string9 + ";" + string10);
        return (string0 + ";" + string1 + ";" + string2 + ";" + string3 + ";" + string4 + ";" + string5 + ";" + string6 +
            ";" + string7 + ";" + string8 + ";" + string9 + ";" + string10);
    }


    public void CollectDatapoints(int FieldNbr)
    {

    }


}
