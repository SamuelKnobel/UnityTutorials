using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[Serializable]
public class GameControll : MonoBehaviour
{
    // Test if less References are needed!
    //public static GameControll gameControll;
    // Scriptreferences
    public GUIHandler guiHandler;
    public Importer importer;
    public GameObject Hexagons;
    public Exporter exporter; 
    public PlayTable playTable;

    // Paths
    public string filepath_StreamingAssets;
    public string filepath_Dropbox;
    public string filepath_GeneralPlayerInformation;
    public string filepath_SpielfeldTabelle;
    public string filepath_GeneralGameInformation;
    public string filepath_Buildinginformation;

    // Filename in folder for settings and infomrations
    public string filename_GeneralPlayerInformation;

    // General Game Information
    public int currentRound;
    public InputField inputField_currentRound;

    // Error and PopUp Handling
    public bool showPopUp_FileError;
    public string PopUp_ErrorText;
    public bool showPopup;


    public Buildings BuildingsSkript = new Buildings();
    public List<Buildings> SamplebuildingList = new List<Buildings>();

    public PlayerSettings playerSettingsSkript = new PlayerSettings();
    public List<PlayerSettings> PlayersList = new List<PlayerSettings>();

    public Race RaceSkript = new Race();
    public List<Race> playableRaces;

    public List<Fields> fieldList;
    public List<Fields> NewfieldList;

    // TimerHandling
    [SerializeField] bool DoIt;
    [SerializeField] float DoItTimer;

    public enum Seasons
    {
        Spring =1, Summer=2, Fall = 3,Winter = 4,
    }

    // Use this for initialization
    void Awake()
    {
        SetDefaultContitions();
        importer.ImportSettings(filepath_GeneralPlayerInformation);
        RaceSkript.generateRaces();
        BuildingsSkript.createSampleBuilding();
        importer.ImportSettings(filepath_GeneralGameInformation);
        //guiHandler.GenerateRaceToggles(playableRaces);   
        guiHandler.GenerateRaceToggles(guiHandler.currentPlayer.playedRaces);
        guiHandler.inputfield_actualSeason.GetComponentInChildren<Text>().text=  guiHandler.DefineSeason();
        fieldList = new List<Fields>();
        BuildingsSkript.Awake();
        exporter.SavePath = filepath_Dropbox + "/SaveFile/";
    }

    private void Start()
    {

        LoadSpielfeldData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        DoSomethingAfterTime();

        if (Input.GetKeyDown(KeyCode.L))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        // Really Needed ??
        if (Input.GetKey(KeyCode.LeftShift) & Input.GetKeyDown(KeyCode.B))
        {
            BuildingsSkript.ResetBuildingDefinitions(BuildingsSkript);
        }



        if (Input.GetKeyDown(KeyCode.V))
        {

        }
        if (Input.GetKeyDown(KeyCode.J))
        {
        }

       
    }




    void DoSomethingAfterTime()
    {
        if (DoIt)
        {
            DoItTimer = DoItTimer - Time.deltaTime;

            if (false) // Boolean to Be true!
            {
                //if (DoItTimer<0)
                //{
                //    // Function to be called
                //}
            }
        }
    }

    void SetDefaultContitions()
    {
        filename_GeneralPlayerInformation = "GeneralPlayerInformation.txt";
        filepath_StreamingAssets = Application.streamingAssetsPath + "/";
        filepath_GeneralPlayerInformation = filepath_StreamingAssets + filename_GeneralPlayerInformation;
        currentRound = 0;
        showPopUp_FileError = false;
        PopUp_ErrorText = "";
        showPopup = false;
        guiHandler.SetDefault();
    }


    public void buildBuilding(Buildings building, bool fortified, int FieldNb, Race race)
    {
        if (guiHandler.b_openBuild || guiHandler.b_openFieldInfo)
        {
            bool CanYOuPay = guiHandler.payForBuilding(building);
            if (!CanYOuPay)
            {
                return;
            }          
        }
        HexagonSettings hex = GameObject.Find("Hexagon_" + FieldNb.ToString()).GetComponent<HexagonSettings>();
        building.RoundsExisting = -building.RoundsTillFinish;
        if (hex.field.MainBuilding!= null)
        {
            if (hex.field.MainBuilding.buildingType != Buildings.TypeOfBuildings.Empty)
            {
                race.ownedBuildings.Remove(hex.field.MainBuilding);
                hex.field.MainBuilding = null;
            }
        }

        if (building.buildingType == Buildings.TypeOfBuildings.Empty)
        {
            return;
        }
        else
        {
            hex.field.MainBuilding = building;
            if (building.buildingType == Buildings.TypeOfBuildings.City)
            {
                fortified= false;
                for (int i = hex.field.UpgradeTower.Count - 1; i >=0 ; i--)
                {
                    if (hex.field.UpgradeTower[i].upgradeType == Buildings.UpgradeType.Building)
                    {
                        hex.field.UpgradeTower.RemoveAt(i);
                    }
                }
            }
            building.SetSettings(building, fortified, FieldNb, race.peoples);

            if (guiHandler.b_openBuild || guiHandler.b_openFieldInfo)
            {
                guiHandler.updateCurrent(FieldNb,race);
            }            
            //print(race);
            //print(race.ownedBuildings);
            //print(race.ownedBuildings.Count);
            race.ownedBuildings.Add(building);
            //print(race.ownedBuildings.Count);

        }
    }

    public void buildUpgrade(int FieldNb, Race race, bool finished, Buildings.UpgradeType upType)
    {

        HexagonSettings hex = GameObject.Find("Hexagon_" + FieldNb.ToString()).GetComponent<HexagonSettings>();

        UpgradeTower UT = new UpgradeTower(upType);
        if (guiHandler.b_openBuild || guiHandler.b_openFieldInfo)
        {
            bool CanYOuPay = guiHandler.payForBuilding(UT);
            if (!CanYOuPay)
            {
                return;
            }
        }

        hex.field.UpgradeTower.Add(UT);
        UT.SetSettings(UT, false, FieldNb, race.peoples);
        UT.RoundsExisting = -UT.RoundsTillFinish;

        if (guiHandler.b_openBuild || guiHandler.b_openFieldInfo)
        {
            guiHandler.updateCurrent(FieldNb,race);
        }       
    }

    /// <summary>
    /// Removes all Builings from this Field and all lists if the counter for the Destruction reaches 0
    /// the downcounting is done in the NEXT- Function
    /// </summary>
    /// <param name="FieldNb"></param>
    public void destroyBuilding(int FieldNb)
    {        
        HexagonSettings hex = GameObject.Find("Hexagon_" + FieldNb.ToString()).GetComponent<HexagonSettings>();
        Race owner;
        if (hex.field.MainBuilding == null)        
            return;        
        if (hex.field.MainBuilding.buildingType == Buildings.TypeOfBuildings.Empty)        
            return;
        else
        {
            owner = parseRace(hex.field.peoples.ToString());
            owner.ownedBuildings.Remove(hex.field.MainBuilding);
            hex.field.MainBuilding = new Empty();
            hex.field.MainBuilding.SetSettings(hex.field.MainBuilding, false, FieldNb, owner.peoples);
        }
        hex.field.UpgradeTower.Clear();
        guiHandler.cS_BuildingScript = hex.field.MainBuilding;
        guiHandler.cS_BuildingType = Buildings.TypeOfBuildings.Empty;
        
    }
  
    void LoadSpielfeldData()
    {
        playTable = new PlayTable();
        playTable.CreatePlayGround();
    }

    public void processSpecialities(int FieldNb, string specialities)
    {
        Fields field = fieldList[FieldNb];
        string[] severalSpecialities = specialities.Split(',');
        if (severalSpecialities.Length != 0)
        {
            foreach (string item in severalSpecialities)
            {
                switch (item.Trim())
                {
                    case "Palantir":
                        break;
                    case "unbenutzbar":
                        field.unusable = true;
                        break;
                    case "Beorninger":
                        break;
                    case "Brücke":
                        field.bridge = true;
                        break;
                    case "Befestigte Brücke":
                        field.upgraded_bridge = true;
                        field.UpgradeTower.Add(new UpgradeTower(Buildings.UpgradeType.Bridge));
                        field.UpgradeTower.Add(new UpgradeTower(Buildings.UpgradeType.Bridge));
                        foreach (Buildings tow in field.UpgradeTower)
                        {
                            tow.SetSettings(tow, false, FieldNb, field.peoples);
                            tow.BuildingFinished = true;
                        }
                        break;
                    case "Ruine":
                        field.MainBuilding.ruine = true;
                        field.MainBuilding.BuildingFinished = false;
                        break;
                    case "Balrog":
                        break;
                    case "Ents":
                        break;
                    default:
                        if (item.Length > 1)
                        {
                            Debug.LogWarning("Unknown Speciality on Field "+ FieldNb+": " + item);
                        }
                        break;
                }
            }
        }
        else
            print(specialities + "," + FieldNb);
    }

    public void Buttonfunctions()
    {
        string currentname = EventSystem.current.currentSelectedGameObject.name;
        switch (currentname)
        {
            case "Next":
                int i = 0;
                while (i < guiHandler.currentPlayer.playedRaces.Count)
                {
                    guiHandler.currPlaying_RaceScript = guiHandler.currentPlayer.playedRaces[i];

                    GameObject.Find("T_" + guiHandler.currPlaying_RaceScript).GetComponent<Toggle>().isOn = true;

                    if (guiHandler.currPlaying_RaceScript.peoples != Race.Peoples.Nobody)
                        if (!guiHandler.currPlaying_RaceScript.b_GoldCollected || !guiHandler.currPlaying_RaceScript.b_TaxCollected)
                        {
                            print(guiHandler.currPlaying_RaceScript.ToString() + " has not collected all Money");

                            guiHandler.CollectMoney(guiHandler.currPlaying_RaceScript.peoples);
                            return;
                        }
                        else
                            i++;
                }
                exporter.EndofRoundSave();
               
                currentRound++;
                guiHandler.defineTextInfoField("Round: " + currentRound, false);
                guiHandler.defineTextInfoField("--------------", true);


                inputField_currentRound.text = (currentRound.ToString());
                guiHandler.inputfield_actualSeason.GetComponentInChildren<Text>().text = guiHandler.DefineSeason();

                foreach (Fields field in fieldList)
                {
                    if (field.MainBuilding != null)
                    {                    
                        if (field.MainBuilding.buildingType != Buildings.TypeOfBuildings.Empty)
                        {
                            field.MainBuilding.b_moneyCollected = false;

                            if (field.MainBuilding.underDestruction)
                                field.MainBuilding.RoundsExisting--;
                            else
                                field.MainBuilding.RoundsExisting++;

                            field.MainBuilding.BuildingFinished = field.MainBuilding.checkIfFinished(field.MainBuilding);

                            if (field.MainBuilding.BuildingFinished && field.MainBuilding.underDestruction)
                            {
                                destroyBuilding(field.FieldNb);
                            }
                            else
                            {
                                // Rise the Population Number to the base value if the building is finished
                                // without changing the initial Population
                                if (field.MainBuilding.BuildingFinished)
                                {
                                    field.MainBuilding.ruine = false;
                                    if (field.population < field.MainBuilding.BasePopulation)
                                    {
                                        field.populationCounter = 0;
                                        field.population = field.MainBuilding.BasePopulation;
                                    }
                                }
                            }

                            if (field.MainBuilding.buildingType != Buildings.TypeOfBuildings.Mine && field.MainBuilding.buildingType != Buildings.TypeOfBuildings.Fort )                            
                                field.populationCounter++;                            

                            if (field.populationCounter >= 5 + field.population)
                            {
                                field.population++;
                                if (field.population > field.fertility * 2)
                                {
                                    field.population = field.fertility * 2;
                                    guiHandler.defineTextInfoField("Maximum Population reached at Field: " + field.FieldNb, true);
                                }
                                else
                                    guiHandler.defineTextInfoField("Population +1 on Field: " + field.FieldNb, true);
                                field.populationCounter = 0;
                            }
                        }
                        if (field.UpgradeTower != null)
                        {
                            if (field.UpgradeTower.Count > 0)
                            {
                                foreach (UpgradeTower tow in field.UpgradeTower)
                                {
                                    tow.RoundsExisting++;
                                    tow.BuildingFinished = tow.checkIfFinished(tow);
                                }
                            }
                        }   
                    }
                }
                foreach (Race race in playableRaces)
                {
                    race.MoneyLastRound = race.actualMoney - race.MoneyLastRound;
                    race.MoneyLastRound = race.actualMoney;
                    race.b_GoldCollected = false;
                    race.b_TaxCollected = false;                    
                }
                ResetPopUp();
                guiHandler.updateCurrent(guiHandler.cS_FieldNbr, guiHandler.currPlaying_RaceScript);
                guiHandler.changeButtonState("CollectAllMoney", true);


                break;
            case "Refresh":
               
                print("Refresh");
                print("TODO check if there is a file to load Data from (identified by Player and Round Name" +
                "Define Boolean-Array for number of other players to check if the file has been integrated" +
                 "Function to check if 2 Field were changed by different people and if the changes differ or not.");
                break;
            case "ChangeMoney":
                guiHandler.b_openMoneyAddGUI = true;
                break;
            case "CollectMoney":
                guiHandler.CollectMoney(guiHandler.currPlaying_Race);
                break;  
            case "CollectAllMoney":
                foreach (Race race in guiHandler.currentPlayer.playedRaces)
                {
                    guiHandler.defineTextInfoField("<b>" + race+ "</b>" + ":", true);
                    int earnedMoney = 0;
                    string stringCollectMoney = "";
                    List<Buildings> MineList = new List<Buildings>();

                    foreach (Buildings build in race.ownedBuildings)
                    {
                        if (!build.b_moneyCollected)
                        {
                            bool b_money;
                            int money;
                            if (build.BuildingFinished)
                            {
                                b_money = int.TryParse(build.getIncomeInfo(build.buildingType, build.BuildingPosition), out money);
                            }
                            else if (!build.BuildingFinished & build.buildingType == Buildings.TypeOfBuildings.FortressMine)
                            {
                                b_money = false;
                                money = 0;
                            }
                            else
                            {
                                b_money = int.TryParse(build.getIncomeInfo(build.previousState, build.BuildingPosition), out money);
                            }

                            if (b_money && money != 0)
                            {
                                earnedMoney = earnedMoney + money;
                                stringCollectMoney = stringCollectMoney + build + " (" + build.BuildingPosition + "): " + money + "\n";
                                build.b_moneyCollected = true;
                            }
                            else if (!b_money)
                            {
                                MineList.Add(build);
                                stringCollectMoney = stringCollectMoney + build + " (" +
                                    build.BuildingPosition + "): " + build.getIncomeInfo(build.buildingType, build.BuildingPosition) + "\n";
                            }
                        }
                        build.b_moneyCollected = true;                       
                    }
                    race.b_TaxCollected = true;

                    guiHandler.defineTextInfoField(stringCollectMoney, true);
                    if (MineList.Count == 0)
                    {
                        race.b_GoldCollected = true;
                    }
                    else
                    {
                        int Income = 0;
                        foreach (Buildings mine in MineList)
                        {
                            string IncomePerDice = "";
                            int tempIncome = 0;
                            int Dices = mine.NbOfDices;
                            string singleNumbers = "";
                            for (int i2 = 0; i2 < Dices; i2++)
                            {
                                tempIncome = Mathf.RoundToInt(2 * UnityEngine.Random.Range(1, 6));
                                Income += tempIncome;
                                IncomePerDice += tempIncome.ToString() + " ";
                                singleNumbers += tempIncome + ",";
                            }

                            string stringCollectMine = Income.ToString();

                            guiHandler.defineTextInfoField(race + " diced " + singleNumbers + " in Mine" + "\n", true);
                            IncomePerDice = "";
                            earnedMoney += Income;
                            mine.b_moneyCollected = true;
                        }
                        race.b_GoldCollected = true;
                        
                    }
                    race.actualMoney += earnedMoney;
                    guiHandler.defineTextInfoField("Totally earned Money: " +earnedMoney+ "\n" + "___________________\n", true);
                    guiHandler.updateCurrent(1, guiHandler.currPlaying_RaceScript);
                    guiHandler.changeButtonState("CollectAllMoney", false);
                }

                break;

            default:
                print("Error with:" +currentname +" Button doesnt exist");
                break;
        }
    }
 
    void ResetPopUp()
    {
        showPopup = false;
    }


    public int parseInt(string inputString)
    {
        int result;
        bool Bool = int.TryParse(inputString, out result);
        if (!Bool)
        {
            result = 0;
            inputString = inputString.Trim();
            if (inputString != "Nr" & 
                inputString != "MinimalPopulation" &
                inputString != "BasePopulation" &
                inputString != "RoundsTillFinish" &
                inputString != "BaseIncome" &
                inputString != "Price" &
                inputString.Length < 3)
            {
                Debug.LogWarning("WRONG FORMAT!! : " + inputString + " cannot be parsed into an INT");
            }
        }
        return result;
    }
    public Buildings parseBuilding(string inputString)
    {
        Buildings result = new Empty();
        Buildings.TypeOfBuildings temp;

        bool Bool = Enum.IsDefined(typeof(Buildings.TypeOfBuildings), inputString);

        if (!Bool)
            Debug.LogWarning("WRONG FORMAT!! : " + inputString + " cannot be parsed into an Building");
        else
        {
            temp = (Buildings.TypeOfBuildings)Enum.Parse(typeof(Buildings.TypeOfBuildings), inputString);

            switch (temp)
            {
                case Buildings.TypeOfBuildings.Empty:
                    result = new Empty();
                    break;
                case Buildings.TypeOfBuildings.Farm:
                    result = new Farm();
                    break;
                case Buildings.TypeOfBuildings.Village:
                    result = new Village();
                    break;
                case Buildings.TypeOfBuildings.Fort:
                    result = new Fort();
                    break;
                case Buildings.TypeOfBuildings.Castle:
                    result = new Castle();
                    break;
                case Buildings.TypeOfBuildings.Tower:
                    result = new Tower();
                    break;
                case Buildings.TypeOfBuildings.Stronghold:
                    result = new Stronghold();
                    break;
                case Buildings.TypeOfBuildings.City:
                    result = new City();
                    break;
                case Buildings.TypeOfBuildings.Mine:
                    result = new Mine();
                    break;
                case Buildings.TypeOfBuildings.Citadel:
                    result = new Citadel();
                    break;
                case Buildings.TypeOfBuildings.FortressMine:
                    result = new FortressMine();
                    break;
                case Buildings.TypeOfBuildings.UpgradeTower:
                    Debug.Log(1);
                    result = new UpgradeTower("");
                    break;
                default:
                    break;
            }
        }

        return result;
    }
    public bool parseBool(string inputString)
    {
        if (inputString == "Ja")
            return true;
        else if (inputString.Length > 1)
        {
            Debug.LogWarning("WRONG FORMAT!! : " + inputString + " cannot be parsed into an Boolean");
            return false;
        }
        return false;
    }
    public Race.Peoples parsePeople(string inputString)
    {
        Race.Peoples result = Race.Peoples.Nobody;
        if (inputString.Length <= 1)
            inputString = "Nobody";
        bool Bool = Enum.IsDefined(typeof(Race.Peoples), inputString);
        if (!Bool)
            Debug.LogWarning("WRONG FORMAT!! : " + inputString + " cannot be parsed into an Race");
        else
            result = (Race.Peoples)System.Enum.Parse(typeof(Race.Peoples), inputString);
        return result;
    }
    public Race parseRace(string inputString)
    {
        if (inputString.Length <= 1)
            inputString = "Nobody";
        Race result = playableRaces[0];
        Race.Peoples temp = Race.Peoples.Nobody;

        bool Bool = Enum.IsDefined(typeof(Race.Peoples), inputString);
        if (!Bool)
        {
            Debug.LogWarning("WRONG FORMAT!! : " + inputString + " cannot be parsed into an Race");
            return result;
        }
        else
        {
            temp = (Race.Peoples)System.Enum.Parse(typeof(Race.Peoples), inputString);
            switch (temp)
            {
                case Race.Peoples.Nobody:
                    result = playableRaces[0];
                    break;
                case Race.Peoples.Angmar:
                    result = playableRaces[1];

                    break;
                case Race.Peoples.Gondor:
                    result = playableRaces[2];

                    break;
                case Race.Peoples.Harad:
                    result = playableRaces[3];

                    break;
                case Race.Peoples.Isengart:
                    result = playableRaces[4];

                    break;
                case Race.Peoples.Khazad:
                    result = playableRaces[5];

                    break;
                case Race.Peoples.Mordor:
                    result = playableRaces[6];

                    break;
                case Race.Peoples.Noldor:
                    result = playableRaces[7];

                    break;
                case Race.Peoples.Numenor:
                    result = playableRaces[8];

                    break;
                case Race.Peoples.Rhun:
                    result = playableRaces[9];

                    break;
                case Race.Peoples.Rohan:
                    result = playableRaces[10];

                    break;
                default:
                    result = playableRaces[0];

                    break;
            }
        }
        if (result.GetType() != Type.GetType(inputString.Trim()))
        {
            Debug.LogWarning("Error in Order of Races... check:" + result+ "," + inputString);
        }
        return result;
    }


    void OnGUI()
    {
        if (showPopUp_FileError)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 150, (Screen.height / 2) - 100, 300, 320),ShowGUI2, "ERROR");
        }
    }

    void ShowGUI2(int windowID) // GUI for FileError
    {
        GUI.Label(new Rect(10, 20, 280, 300), PopUp_ErrorText);
        
        if (GUI.Button(new Rect(100, 270, 100, 30), "OK"))
        {
            showPopUp_FileError = false;
        }
    }
}

