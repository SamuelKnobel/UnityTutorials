using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayTable 
{
    protected string Data_raw;
    GameControll game;


    public void CreatePlayGround()
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();

        string rawdata = LoadData();
        string[] Data = rawdata.Split('\n');

        string[] FirstLine = Data[0].Split(';');

        game.fieldList.Clear();
        Fields Field = new Fields();
        game.fieldList.Add(Field);
        for (int i = 0; i < Data.Length; i++) // Line is Variable Names   
        {
            string[] Words = Data[i].Split(';');
            //print(Data[i]);

            for (int i2 = 0; i2 < game.Hexagons.transform.childCount; i2++)
            {
                Transform child = game.Hexagons.transform.GetChild(i2);
                if (child.gameObject.name == ("Hexagon_" + game.parseInt(Words[0])))
                {
                    HexagonSettings hexchild = child.gameObject.GetComponent<HexagonSettings>();
                    Field = hexchild.field;
                    Field.FieldNb = game.parseInt(Words[0]);
                    Field.FieldName = Words[1];
                    Field.peoples = game.parsePeople(Words[10]);
                    Field.MainBuilding = game.parseBuilding(Words[2]);
                    Field.MainBuilding.BuildingPosition = Field.FieldNb;
                    Field.MainBuilding.BuildingFinished = true;
                    Race temprace = game.parseRace(Words[10]);
                    if (Field.MainBuilding.GetType() != typeof(Empty))
                    {
                        temprace.ownedBuildings.Add(Field.MainBuilding);
                    }
                    Field.MainBuilding.peoples = Field.peoples;

                    Field.MainBuilding.peoples = Field.peoples; 
                    Field.oreOutput = game.parseInt(Words[4]);
                    Field.fertility = game.parseInt(Words[5]);
                    bool upgraded= game.parseBool(Words[6]);
                    if (upgraded)
                    {
                        Field.UpgradeTower.Add(new UpgradeTower(Buildings.UpgradeType.Building));

                        foreach (Buildings tow in Field.UpgradeTower)
                        {
                        tow.SetSettings(tow, false, Field.FieldNb, Field.peoples);

                        }
                    }
                    // ToDo:  build it upgreaded
                    Field.population = game.parseInt(Words[7]);
                    Field.heroOrigine = Words[8];
                    Field.speciales = Words[9];
                    Field.trade = game.parseInt(Words[11]);
                    if (game.fieldList.Count > 0)
                    {
                        for (int i3 = game.fieldList.Count - 1; i3 >= 0; i3--)
                        {
                            if (game.fieldList[i3].FieldNb == Field.FieldNb)
                            {
                                game.fieldList.RemoveAt(i3);
                                Debug.LogError("Same Field number Twice! the old one is overwriten");
                            }
                        }
                    }
                    game.fieldList.Add(Field);
                    game.processSpecialities(Field.FieldNb, Field.speciales);

                }
            }
        }
    }
    public string LoadData()
    {
        //string path = "C:/Users/HP/Dropbox/Herr der Ringe/Regeln und Profile/Kampagne/Aktualisierter Ordner 2018/UNITY/SpielfeldTabelle.csv";
        string path = game.filepath_SpielfeldTabelle;

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                Data_raw = reader.ReadToEnd();
            }
            return Data_raw;
        }
        catch (System.Exception)
        {
            Debug.Log("Close the Spielfeld_Data File!");
            // ToDo: Show error in GUI or Infofield
            throw;
        }
    } 

    public void processSpecials()
    {

    }
}
[System.Serializable]
public class Fields : PlayTable
{
    public int FieldNb;
    public string FieldName;
    public Buildings MainBuilding;
    public int oreOutput;
    public int fertility;
    public int trade;
    public int population;
    public bool bridge;
    public bool upgraded_bridge;
    public Race.Peoples peoples = Race.Peoples.Nobody; 
    public List<Buildings> UpgradeTower;
    public bool unusable = false; //  false per default--> Field usable!
    public string heroOrigine = "none";
    public string speciales = "none";
    public int populationCounter;


}




