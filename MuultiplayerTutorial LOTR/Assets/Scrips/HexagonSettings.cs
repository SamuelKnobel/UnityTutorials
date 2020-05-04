using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexagonSettings : MonoBehaviour {


    // Script References
    public GameControll gameControll;
    public GUIHandler guiHandler;
    public Fields field;

 
    // GUI Handler
    public bool displayInfo= false;
    public bool displayInfo_full= false;
    public bool openPopUp1 = false;
     string textToShow = "";
    string infoText_Header = "";

    // Use this for initialization
    void Awake () {

        string Objectname = transform.name;
        var arr = Objectname.Split('_');
        transform.Find("FieldNb").GetComponent<TextMesh>().text = arr[1];
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKey(KeyCode.Mouse1))
        {
            infoText_Header = "Additional Info";
            displayInfo_full = true;
            openPopUp1 = false;
        }
        else
        {
            infoText_Header = "Basic Info";
            displayInfo_full = false;
        }
    }

    private void OnMouseOver()
    {
        guiHandler.b_clickable = true;                
            if (guiHandler.b_clicked)
            {
            guiHandler.cS_FieldScript = field;
            guiHandler.cS_FieldNbr = field.FieldNb;
            guiHandler.cS_Race = field.peoples;
            guiHandler.cS_RaceScript = gameControll.parseRace(field.peoples.ToString());
            
            guiHandler.cS_BuildingScript = field.MainBuilding;
            guiHandler.cS_BuildingType = field.MainBuilding.buildingType;

              displayInfo_full = false;
                displayInfo = false;
                guiHandler.b_openFieldInfo = true;
                guiHandler.b_clicked = false;
            }

        if (!guiHandler.checkIfGUIactive())
        {                   
            if (!openPopUp1 & !displayInfo_full)
            {
                displayInfo = true;
            }        
            if (field.MainBuilding.buildingType == Buildings.TypeOfBuildings.Empty)
            {
                textToShow =
                    "Field Number = " + field.FieldNb + "\n" +
                    "Field Name = "+ field.FieldName + "\n" +
                    "Owner = " + field.peoples.ToString() + "\n" +
                    "Building = Empty";
            }
            else
            {
                if (field.MainBuilding.BuildingFinished)
                {
                    textToShow =
                        "Field Number = " + field.FieldNb + "\n" +
                        "Field Name = " + field.FieldName + "\n" +
                        "Owner = " + field.peoples.ToString() + "\n" +
                        "Building = " + field.MainBuilding.ToString() + "\n" +
                        "Finished" + "\n" +
                        "Rounds Existing = " + field.MainBuilding.RoundsExisting + "\n";
                    if (field.MainBuilding.getIncomeInfo(field.MainBuilding.buildingType, field.FieldNb)!= "0")
                    {
                        textToShow= textToShow   +
                        "Income = " + field.MainBuilding.getIncomeInfo(field.MainBuilding.buildingType, field.FieldNb); 
                    }               
                }              
                else
                {
                    textToShow =
                        "Field Number = " + field.FieldNb + "\n" +
                        "Field Name = " + field.FieldName + "\n" +
                        "Owner = " + field.peoples.ToString() + "\n" +
                        "Building = " + field.MainBuilding.ToString();
                    if (field.MainBuilding.underDestruction)
                    {
                        textToShow= textToShow+ "\n"+
                            "Destruction in Progress" + "\n" +
                        "Rounds to go = " + (Math.Abs(field.MainBuilding.RoundsExisting)).ToString();
                    }
                    else if (field.MainBuilding.ruine)
                    {
                        textToShow = textToShow + "\n" + "Building is a ruine";
                    }
                    else
                        textToShow = textToShow + "\n" +
                            "Construction in Progress" + "\n" +
                            "Rounds to go = " + (Math.Abs(field.MainBuilding.RoundsExisting)).ToString();
                }
            }
            if(displayInfo_full)
            {
                textToShow =
                    "Ore Output = " + field.oreOutput + "\n" +
                    "Fertility = " + field.fertility + "\n" +
                    "Population = " + field.population + "\n"+
                    "Trade = " + field.trade + "\n";

                if (field.heroOrigine.Length >1)
                {
                    textToShow = textToShow + "\n" +
                    "Heros Origine = " + field.heroOrigine + "\n";
                }
                if (field.speciales.Length >1)
                {
                    textToShow = textToShow + "\n" +
                    "Specials = " + field.speciales;
                }
                    

            }          
        }
    }

    private void OnMouseExit()
    {
        displayInfo = false;
        displayInfo_full = false;
        guiHandler.b_clickable = false;
    }

    void OnGUI()
    {
        if (displayInfo)
        {
            GUI.Window(0, new Rect(Input.mousePosition.x+5, Screen.height - Input.mousePosition.y+5, 260, 200), ShowGUI1, infoText_Header);
        }
 
    }

    void ShowGUI1(int windowID)
    {       
        GUI.Label(new Rect(15, 20, 240, 175), textToShow);
    }



}
