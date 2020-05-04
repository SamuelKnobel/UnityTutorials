using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class Buildings {


    public Buildings.TypeOfBuildings buildingType;

    [System.NonSerialized]
    public GameControll game;
    
    public int Price;
    public int RoundsExisting;
    public int RoundsTillFinish;
    public string GeneratedIncome;
    public int baseIncome;
    public int BuildingPosition;
    public bool BuildingFinished;
    public bool underDestruction;
    public Race.Peoples peoples;
    public bool fortified;
    public bool ruine;
    public int NbOfDices;
    public int AdditionalDices = 0;
    public int MinimalPopulation;
    public int BasePopulation;
    public Buildings.TypeOfBuildings nextBuildingType = TypeOfBuildings.Empty;
    public Buildings.TypeOfBuildings previousState = TypeOfBuildings.Empty;
    public bool b_Farm_data { get; set; }
    public bool b_Village_data { get; set; }
    public bool b_Fort_data { get; set; }
    public bool b_Castel_data { get; set; }
    public bool b_Tower_data { get; set; }
    public bool b_Stronghold_data { get; set; }
    public bool b_City_data { get; set; }
    public bool b_Mine_data { get; set; }
    public bool b_Citadel_data { get; set; }
    public bool b_FortressMine_data { get; set; }
    public bool b_UpgradeTower_data { get; set; }

    public bool b_moneyCollected;

    public enum TypeOfBuildings
    {
        Empty= 0,
        Farm = 1,
        Village = 2,
        Fort = 3,
        Castle = 4,
        Tower = 5,
        Stronghold = 6,
        City = 7,
        Mine = 8,
        Citadel = 10,
        FortressMine = 11,
        UpgradeTower = 12,
    }

    public enum UpgradeType
    {
        Building = 1,
        Bridge = 2,
    }
   public  UpgradeType upgradeType;

    public void Awake()
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
    }

    public void createSampleBuilding()
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
        string RawData = LoadData();      

        string[] Data = RawData.Split('\n');
        foreach (string Lines in Data)
        {
            for (int i = 0; i < Data.Length; i++) // Line is Variable Names   
            {
                string[] Words = Data[i].Split(';');
                if (Words.Length > 5)
                {
                    Buildings building = new Empty(); ;
                    switch (Words[0])
                    {
                        case "Farm":
                            if (!b_Farm_data)
                            {
                                building = new Farm("");                                  
                                b_Farm_data = true;
                            }
                            //else Debug.Log("Farm already Defined, Press Shift+ B to reset!");
                            break;

                        case "Village":
                            if (!b_Village_data)
                            {
                                building = new Village("");
                                b_Village_data = true;
                            }
                            //else Debug.Log("Village already Defined, Press Shift+ B to reset!");
                            break;

                         case "Fort":
                            if (!b_Fort_data)
                            {
                                building = new Fort("");
                                b_Fort_data = true;
                            }
                            //else Debug.Log("Fort already Defined, Press Shift+ B to reset!");
                            break;

                        case "Castle":                            
                            if (!b_Castel_data)
                            {
                                building = new Castle("");
                                b_Castel_data = true;
                            }
                            //else Debug.Log("Castle already Defined, Press Shift+ B to reset!");
                            break;

                    case "Tower":
                            if (!b_Tower_data)
                            {
                                building = new Tower("");
                                b_Tower_data = true;
                            }
                            //else Debug.Log("Tower already Defined, Press Shift+ B to reset!"); 
                            break;

                   case "Stronghold":
                            if (!b_Stronghold_data)
                            {
                                building = new Stronghold("");
                                b_Stronghold_data = true;
                            }
                            //else Debug.Log("Stronghold already Defined, Press Shift+ B to reset!");
                            break;

                    case "Mine":
                            if (!b_Mine_data)
                            {
                                building = new Mine("");
                                b_Mine_data = true;
                            }
                            //else Debug.Log("Mine already Defined, Press Shift+ B to reset!"); 
                            break;
                    case "City":
                            if (!b_City_data)
                            {
                                building = new City("");
                                b_City_data = true;
                            }
                            //else Debug.Log("City already Defined, Press Shift+ B to reset!");
                            break;
                    case "Citadel":
                            if (!b_Citadel_data)
                            {
                                building = new Citadel("");
                                b_Citadel_data = true;
                            }
                            //else Debug.Log("Citadel already Defined, Press Shift+ B to reset!"); 
                            break;
                    case "fortressMine":
                            if (!b_FortressMine_data)
                            {
                                building = new FortressMine("");
                                b_FortressMine_data = true;
                            }
                            //else Debug.Log("fortressMine already Defined, Press Shift+ B to reset!");
                            break;
                        case "UpgradeTower":
                            if (!b_UpgradeTower_data)
                            {
                                building = new UpgradeTower("");
                                b_UpgradeTower_data = true;
                            }
                            //else Debug.Log("UpgradeTower already Defined, Press Shift+ B to reset!"); 
                            break;

                        default:                           
                          //  Debug.Log("no Building Defined: " + Words[0]);
                            break;
                    }

                    building.game = game;
                    building.Price = game.parseInt(Words[1]);
                    building.RoundsTillFinish = game.parseInt(Words[2]);
                    building.baseIncome = game.parseInt(Words[3]);
                    building.BasePopulation = game.parseInt(Words[4]);
                    building.MinimalPopulation = game.parseInt(Words[5]);
                    //building.BuildingFinished = true;
                    building.RoundsExisting = 0;
                    if (building.buildingType != TypeOfBuildings.Empty)
                        game.SamplebuildingList.Add(building);                    
                }
                else
                {
                    if (Data[i].Length > 0)
                    {
                        Debug.LogWarning("unreadable Line in CostFile: " + Data[i]);
                    }
                }
            }
        }        
    }
    public string LoadData()
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
        string Data_raw;
        string path = game.filepath_Buildinginformation;
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
            Debug.Log("Close the CostBuilding File!");
            // ToDo: Show error in GUI or Infofield
            throw;
        }
    }

    public void CopyValues(Buildings newBuilding)
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
        foreach (var item in game.SamplebuildingList)
        {
            if (newBuilding.GetType() == item.GetType())
            {
                newBuilding.Price = item.Price;
                newBuilding.RoundsTillFinish = item.RoundsTillFinish;
                newBuilding.baseIncome = item.baseIncome;
                newBuilding.BasePopulation = item.BasePopulation;
                newBuilding.MinimalPopulation = item.MinimalPopulation;
                newBuilding.RoundsExisting = item.RoundsExisting;
            }
        }
    }

    public void  ResetBuildingDefinitions(Buildings script)
    {
        script.b_Farm_data = false;
        script.b_Village_data = false;
        script.b_Fort_data = false;
        script.b_Castel_data = false;
        script.b_Tower_data = false;
        script.b_Stronghold_data = false;
        script.b_City_data = false;
        script.b_Mine_data = false;
        script.b_Citadel_data = false;
        script.b_FortressMine_data = false;
        script.b_UpgradeTower_data = false;
        game.SamplebuildingList.Clear();
    } // Check if really needed!

    public bool checkIfFinished(Buildings building)
    {
        if (!building.BuildingFinished)
        {
            if (building.RoundsExisting == 0 )
            {
                game.guiHandler.defineTextInfoField("Building (" + building.buildingType + ") finished at Field: " + building.BuildingPosition, true);
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

    public void SetSettings(Buildings newBuilding,bool Upgraded,  int FieldNb, Race.Peoples peoples)
    {
        newBuilding.BuildingPosition = FieldNb;
        newBuilding.peoples = peoples;
        newBuilding.fortified = Upgraded;
    }
    
    public string getIncomeInfo(TypeOfBuildings building, int fieldNb)         // TODO!
    {
        game = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();

        string incomeString = "0";
        int NbOfDices_temp=0;
             
        Fields field = GameObject.Find("Hexagon_" + fieldNb.ToString()).GetComponent<HexagonSettings>().field;
        int fert = 1;
        int pop = 1;
        int ore = 0;
        int trade = 0;
        trade = field.trade;
        switch (trade)
        {
        case 0:
            break;
        case 1:     trade = 20;
                break;
        case 2:     trade = 30;
                break;
         default:   Debug.LogWarning("Unknown trade income on field:" + fieldNb);
                break;
        }

        fert = field.fertility;
        pop = field.population;
        ore = field.oreOutput;                
        int Fert_income = 0;
        switch (game.guiHandler.currSeason)
        {
            case GameControll.Seasons.Spring:
                Fert_income = 5;
                break;
            case GameControll.Seasons.Summer:
                Fert_income = 10;
                break;
            case GameControll.Seasons.Fall:
                Fert_income = 5;
                break;
            case GameControll.Seasons.Winter:
                Fert_income = 2;
                break;
            default:
                Fert_income = 0;
                break;
        }
        int TotalDiceCount = 0;
        switch (building)
        {
            case TypeOfBuildings.Empty:
                break;
            case TypeOfBuildings.Farm:                       
                incomeString = (fert * Fert_income + pop * 3 + baseIncome).ToString();
                break;
            case TypeOfBuildings.Village:                       
                incomeString = (fert * Fert_income + baseIncome + pop * 10 + trade).ToString();
                break;
            case TypeOfBuildings.Fort:
                break;
            case TypeOfBuildings.Castle:
                break;
            case TypeOfBuildings.Tower:
                break;
            case TypeOfBuildings.Stronghold:
                break;
            case TypeOfBuildings.City:
                incomeString = (pop * 10 + baseIncome + trade).ToString();
                break;
            case TypeOfBuildings.Mine:
                NbOfDices_temp = 0;
                switch (ore)
                {
                    case 0:
                        NbOfDices_temp = 0;
                        break;
                    case 1:
                        NbOfDices_temp = 5;
                        if (field.peoples == Race.Peoples.Khazad)
                            NbOfDices_temp = NbOfDices_temp + 1;
                        break;
                    case 2:
                        NbOfDices_temp = 6;
                        if (field.peoples == Race.Peoples.Khazad)
                            NbOfDices_temp = NbOfDices_temp + 1;
                        break;
                    case 3:
                        NbOfDices_temp = 8;
                        break;
                    case 4:
                        NbOfDices_temp = 12;
                        break;
                    default:
                        Debug.LogError("WARNING Unknown OreOutput on Field:" + fieldNb);
                        break;
                }
                TotalDiceCount = NbOfDices_temp + AdditionalDices;
                if (TotalDiceCount != 0)
                    incomeString = TotalDiceCount.ToString() + " W6 x 2";

                break;
            case TypeOfBuildings.Citadel:
                incomeString = (baseIncome + pop * 10 + trade).ToString();
                break;
            case TypeOfBuildings.FortressMine:
                incomeString = "8 W6 x 2";
                TotalDiceCount = 8;
                break;
            default:
                TotalDiceCount = 0;
                break;
        }
        NbOfDices = TotalDiceCount;


        return incomeString;
    }

    public virtual Buildings BuildNextType()
    {
        return new Buildings();
    }

}
public class Empty : Buildings
{
    public Empty()
    {
        buildingType = TypeOfBuildings.Empty;
    }
}

public class Farm : Buildings
{
    public Farm()
    {
        nextBuildingType = TypeOfBuildings.Village;
        buildingType = TypeOfBuildings.Farm;
        CopyValues(this);
    }
    public override Buildings BuildNextType()
    {
        return new Village();
    }
    public Farm(string t)
    {
        nextBuildingType = TypeOfBuildings.Village;
        buildingType = TypeOfBuildings.Farm;
    }
}

public class Fort : Buildings
{
    public Fort()
    {
        BuildingFinished = true;
        nextBuildingType = TypeOfBuildings.Castle;
        buildingType = TypeOfBuildings.Fort;
        CopyValues(this);
    }
    public Fort(string t)
    {
        nextBuildingType = TypeOfBuildings.Castle;
        buildingType = TypeOfBuildings.Fort;
    }
    public override Buildings BuildNextType()
    {
        //Debug.Log(3);
        return new Castle();
    }
}

public class Castle : Buildings
{
    public Castle()
    {
        nextBuildingType = TypeOfBuildings.Stronghold;
        buildingType = TypeOfBuildings.Castle;
        previousState = TypeOfBuildings.Fort;
        CopyValues(this);
    }
    public Castle(string t)
    {
        nextBuildingType = TypeOfBuildings.Stronghold;
        buildingType = TypeOfBuildings.Castle;
        previousState = TypeOfBuildings.Fort;
    }
    public override Buildings BuildNextType()
    {
        //Debug.Log(3);
        return new Stronghold();

    }

}

public class Tower : Buildings
{
    public Tower()
    {
        nextBuildingType = TypeOfBuildings.Stronghold;
        buildingType = TypeOfBuildings.Tower;
        CopyValues(this);
    }
    public Tower(string t)
    {
        nextBuildingType = TypeOfBuildings.Stronghold;
        buildingType = TypeOfBuildings.Tower;
    }
    public override Buildings BuildNextType()
    {
        //Debug.Log(3);
        return new Stronghold();

    }
}

public class Stronghold : Buildings
{
    public Stronghold()
    {
        buildingType = TypeOfBuildings.Stronghold;
        previousState = TypeOfBuildings.Castle;
        CopyValues(this);
    }
    public Stronghold(string t)
    {
        buildingType = TypeOfBuildings.Stronghold;
        previousState = TypeOfBuildings.Castle;
    }
}

public class City : Buildings
{
    public City()
    {
        buildingType = TypeOfBuildings.City;
        previousState = TypeOfBuildings.Village;
        fortified = false;
        CopyValues(this);

    }
    public City(string t)
    {
        buildingType = TypeOfBuildings.City;
        previousState = TypeOfBuildings.Village;
        fortified = false;
    }
}

public class Village : Buildings
{
    public Village()
    {
        nextBuildingType = TypeOfBuildings.City;
        buildingType = TypeOfBuildings.Village;
        previousState = TypeOfBuildings.Farm;
        CopyValues(this);
    }
    public Village(string t)
    {
        nextBuildingType = TypeOfBuildings.City;
        buildingType = TypeOfBuildings.Village;
        previousState = TypeOfBuildings.Farm;
    }
    public override Buildings BuildNextType()
    {
        //Debug.Log(3);
        return new City();

    }
}

public class Mine : Buildings
{
    public Mine()
    {
        buildingType = TypeOfBuildings.Mine;
        CopyValues(this);
    }
    public Mine(string t)
    {
        buildingType = TypeOfBuildings.Mine;
    }
}

public class Citadel : Buildings
{
    public Citadel()
    {
        buildingType = TypeOfBuildings.Citadel;
        CopyValues(this);
    }
    public Citadel(string t)
    {
        buildingType = TypeOfBuildings.Citadel;
    }
}

public class FortressMine : Buildings
{
    public FortressMine()
    {
        buildingType = TypeOfBuildings.FortressMine;
        CopyValues(this);

    }
    public FortressMine(string t)
    {
        buildingType = TypeOfBuildings.FortressMine;
    }
}

public class UpgradeTower : Buildings
{

    public UpgradeTower(UpgradeType upType)
    {

        buildingType = TypeOfBuildings.UpgradeTower;
        upgradeType = upType;
        CopyValues(this);
    }
    public UpgradeTower(string t)
    {
        buildingType = TypeOfBuildings.UpgradeTower;
    }
}



