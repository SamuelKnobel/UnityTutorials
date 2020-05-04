using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Photon.Pun;
using ExitGames.Client.Photon;

//public class GUIHandler : MonoBehaviour
public class GUIHandler : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //print(1);
            // We own this player: send the others our data
            //stream.SendNext(gameControll.fieldList);
            //stream.SendNext(gameControll.playableRaces);
            //stream.SendNext(gameControll.guiHandler.InfoText.text);
        }
        else
        {
            // Network player, receive data
            //this.gameControll.fieldList = (List<Fields>)stream.ReceiveNext();
            //this.gameControll.playableRaces = (List<Race>)stream.ReceiveNext();
            //this.gameControll.guiHandler.InfoText.text = (string)stream.ReceiveNext();
        }
    }


    #endregion


    // References
    public GameControll gameControll;
    public Importer importer;
    public Exporter exporter;

    // Current Playing
    public PlayerSettings currentPlayer;
    public Race currPlaying_RaceScript;
    public Race.Peoples currPlaying_Race; 

    // current Selected
    public int cS_FieldNbr;
    public Fields cS_FieldScript;
    public Buildings cS_BuildingScript;
    public Buildings.TypeOfBuildings cS_BuildingType;
    public Race cS_RaceScript;
    public Race.Peoples cS_Race;

    // Current State
    public GameControll.Seasons currSeason;

    public Buildings currentMine = null;

    // GUI Elements
    public Slider Transparency;
    public InputField inputfield_currentRound;
    public InputField inputfield_currentRace;
    public InputField inputfield_actualMoney;
    public InputField inputfield_actualSeason;
    public ToggleGroup ToggleGroupe_Race;
    public List<Toggle> Toggles_Race;
    public GameObject TogglePrefab;
    public Text InfoText;
    public string stringEarnMoney = "";
    public string stringCollectMine = "";
    public string stringCollectMoney = "";
    Font font;
    int fontSize;
    public Texture diceImage;
    public string IncomePerDice;

    public int collectedMoney;
    public List<Buildings> MineList;

    // GUI Boolean
    public bool b_raceSelected;
    public bool b_showColor { get; set; }
    public bool b_openFieldInfo;
    public bool b_clicked;
    public bool b_clickable; // only turns true if mouse over a Hex;
    public bool b_openBuild;
    public bool b_notEnoughMoney;
    public bool b_openMoneyAddGUI;      
    public bool b_openMoneyCollectGUI;
    public bool b_showInterfaceMineMoney = false;
    public bool b_showInputfieldMineMoney = true;
    public bool b_showDiceButton = true;
    public bool b_ShowDiceIncomeLabel = false;

    public Button[] allButtons;


    // Race Colors    
    Color color_Angmar = Color.black;
    Color color_Gondor = Color.cyan;
    Color color_Harad = Color.blue;
    Color color_Isengart = Color.white;
    Color color_Khazad = new Color(192 / 255f, 192 / 255f, 192 / 255f, 1);
    Color color_Mordor = Color.red;
    Color color_Noldor = Color.grey;
    Color color_Numenor = Color.magenta;
    Color color_Rhun = Color.green;
    Color color_Rohan = Color.yellow;

    private Vector3 MouseOrigin;


    public Text text;
    public void Awake()
    {
        CustomTypes.Register();

    }

    // Use this for initialization
    void Start()
    {


        Camera.main.GetComponent<Transform>().position = new Vector3(Screen.width / 2, Screen.height / 2, -120);

        // Finds only the active Buttons, if one is activated later --> add it and then deactivate it
        allButtons = FindObjectsOfType<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseZoom();


        if (Input.GetMouseButtonDown(0) && b_clickable)
        {
            resetCurrent();
            b_clicked = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            resetCurrent();
            b_openFieldInfo = false;
            b_openBuild = false;
            b_openMoneyCollectGUI = false;

        }
        if (cS_FieldNbr != 0)
        {
            if (Input.GetKeyDown(KeyCode.N)) changeField(cS_FieldNbr, gameControll.parseRace("Nobody"));
            if (Input.GetKeyDown(KeyCode.Alpha1)) changeField(cS_FieldNbr, gameControll.parseRace("Angmar"));
            if (Input.GetKeyDown(KeyCode.Alpha2)) changeField(cS_FieldNbr, gameControll.parseRace("Gondor"));
            if (Input.GetKeyDown(KeyCode.Alpha3)) changeField(cS_FieldNbr, gameControll.parseRace("Harad"));
            if (Input.GetKeyDown(KeyCode.Alpha4)) changeField(cS_FieldNbr, gameControll.parseRace("Isengart"));
            if (Input.GetKeyDown(KeyCode.Alpha5)) changeField(cS_FieldNbr, gameControll.parseRace("Khazad"));
            if (Input.GetKeyDown(KeyCode.Alpha6)) changeField(cS_FieldNbr, gameControll.parseRace("Mordor"));
            if (Input.GetKeyDown(KeyCode.Alpha7)) changeField(cS_FieldNbr, gameControll.parseRace("Noldor"));
            if (Input.GetKeyDown(KeyCode.Alpha8)) changeField(cS_FieldNbr, gameControll.parseRace("Numenor"));
            if (Input.GetKeyDown(KeyCode.Alpha9)) changeField(cS_FieldNbr, gameControll.parseRace("Rhun"));
            if (Input.GetKeyDown(KeyCode.Alpha0)) changeField(cS_FieldNbr, gameControll.parseRace("Rohan"));

            if (Input.GetKeyDown(KeyCode.D)) gameControll.destroyBuilding(cS_FieldNbr);
            if (Input.GetKeyDown(KeyCode.A)) I_GotAttacked(cS_FieldNbr);
            if (Input.GetKeyDown(KeyCode.G)) I_GotAttackedHard(cS_FieldNbr);
            if (Input.GetKeyDown(KeyCode.R)) I_GotDestroyed(cS_FieldNbr);
            if (Input.GetKeyDown(KeyCode.F)) FinishImediately(cS_FieldNbr);

            if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.KeypadPlus)) ChangeNbrofDices(cS_FieldNbr, 1);
            if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.KeypadMinus)) ChangeNbrofDices(cS_FieldNbr, - 1);            
        }       
    }
    /// <summary>
    /// Handle Mouse Movement and Zoom
    /// </summary>
    private void mouseZoom()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");

        if (d > 0f & Camera.main.GetComponent<Transform>().position.z < -10)
        {
            Camera.main.GetComponent<Transform>().position = Camera.main.GetComponent<Transform>().position + new Vector3(0, 0, 5f);
        }
        else if (d < 0f & Camera.main.GetComponent<Transform>().position.z > -400)
        {
            Camera.main.GetComponent<Transform>().position = Camera.main.GetComponent<Transform>().position - new Vector3(0, 0, 5f);
        }
        if (Input.GetMouseButton(1))
        {
            mouseMove();
        }
    }
    private void mouseMove()
    {
        float screenwidth = Screen.width;
        float screenhight = Screen.height;

        if (Input.GetMouseButtonDown(1))
        {
            MouseOrigin = Input.mousePosition;


        }

        if (Input.mousePosition.x > 0 & Input.mousePosition.x < Screen.width & Input.mousePosition.y > 0 & Input.mousePosition.y < Screen.height)
        {
            //Vector3 MouseOrigin = Input.mousePosition;

            Vector3 posCam = Camera.main.GetComponent<Transform>().position;
            //Debug.Log(posCam);

            Vector3 pos = (Input.mousePosition - MouseOrigin) / 100;


            if (Camera.main.GetComponent<Transform>().position.x <= Screen.width - 0.2 * Screen.width & Camera.main.GetComponent<Transform>().position.x >= 0 + 0.2 * Screen.width)
            {
                Camera.main.GetComponent<Transform>().position = Camera.main.GetComponent<Transform>().position + pos;
            }
            else if (Camera.main.GetComponent<Transform>().position.x < 0 + 0.2 * Screen.width)
            {
                Camera.main.GetComponent<Transform>().position = new Vector3(0 + 0.2f * Screen.width, Camera.main.GetComponent<Transform>().position.y, Camera.main.GetComponent<Transform>().position.z);
            }
            else if (Camera.main.GetComponent<Transform>().position.x > Screen.width - 0.2 * Screen.width)
            {
                Camera.main.GetComponent<Transform>().position = new Vector3(Screen.width - 0.2f * Screen.width, Camera.main.GetComponent<Transform>().position.y, Camera.main.GetComponent<Transform>().position.z);
            }
            else
            {
                Debug.Log(posCam);
                Debug.Log(MouseOrigin);
                Debug.Log(screenwidth);
                Debug.Log(screenhight);
                Debug.Log("Error in Camera Handling");
            }

            if (Camera.main.GetComponent<Transform>().position.y <= Screen.height - 0.2f * Screen.height & Camera.main.GetComponent<Transform>().position.y >= 0 + 0.2f * Screen.height)
            {
                Camera.main.GetComponent<Transform>().position = Camera.main.GetComponent<Transform>().position + pos;
            }
            else if (Camera.main.GetComponent<Transform>().position.y < 0 + 0.2f * Screen.height)
            {
                Camera.main.GetComponent<Transform>().position = new Vector3(Camera.main.GetComponent<Transform>().position.x, 0 + 0.2f * Screen.height, Camera.main.GetComponent<Transform>().position.z);
            }
            else if (Camera.main.GetComponent<Transform>().position.y > Screen.height - 0.2f * Screen.height)
            {
                Camera.main.GetComponent<Transform>().position = new Vector3(Camera.main.GetComponent<Transform>().position.x, Screen.height - 0.2f * Screen.height, Camera.main.GetComponent<Transform>().position.z);
            }
            else
            {
                Debug.Log(posCam);
                Debug.Log(MouseOrigin);
                Debug.Log(screenwidth);
                Debug.Log(screenhight);
                Debug.Log("Error in Camera Handling");
            }



            /* Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
             Vector3 move = new Vector3(pos.x * dragSpeed,  pos.y * dragSpeed,0);

             Camera.main.GetComponent<Transform>().Translate(move, Space.World);
             */
        }
        else
        {
            Debug.Log("Invalid click");
        }


    }

    public void SetDefault()
    {
        Transparency.value = 0.5f;
        b_showColor = false;
        text = InfoText;
    }
    void resetCurrent()
    {
        cS_BuildingScript = null;
        cS_FieldScript = null;
        cS_Race = Race.Peoples.Nobody;
        cS_RaceScript = gameControll.parseRace(cS_Race.ToString());
        cS_BuildingType = Buildings.TypeOfBuildings.Empty;
        cS_FieldNbr = 0;
    }

    public void updateCurrent(int fieldNbr, Race race)
    {
        if (fieldNbr> 0 & fieldNbr < 282)
        {
            HexagonSettings hex = GameObject.Find("Hexagon_" + fieldNbr.ToString()).GetComponent<HexagonSettings>();

            Buildings tempBuilding = hex.GetComponent<HexagonSettings>().field.MainBuilding;
            if (tempBuilding != null && tempBuilding.buildingType.ToString() != typeof(Empty).ToString())
            {
                cS_BuildingScript = hex.field.MainBuilding;
                cS_BuildingType = cS_BuildingScript.buildingType;
                if (tempBuilding.buildingType == Buildings.TypeOfBuildings.Mine)
                {
                    cS_BuildingScript.GeneratedIncome = cS_BuildingScript.getIncomeInfo(Buildings.TypeOfBuildings.Mine, fieldNbr);
                }
            }
            inputfield_actualMoney.GetComponentInChildren<Text>().text = cS_RaceScript.actualMoney.ToString();
            cS_RaceScript = race;
            cS_Race = race.peoples;
        }
    }

    // Handle Race Toggels
    public void GenerateRaceToggles(List<Race> raceList)
    {
        int ind = 1;
        foreach (Race itemR in raceList)
        {
            string item = itemR.GetType().ToString();
            if (item!="Nobody")
            {            
                Vector3 firstposition = new Vector3(20, -20, 0);
                string togglename = "T_" + item;

                if (GameObject.Find(togglename) ==null)
                {           
                    GameObject ToggleGO = Instantiate(TogglePrefab, ToggleGroupe_Race.gameObject.transform, false);
                    ToggleGO.transform.SetParent(ToggleGroupe_Race.gameObject.transform, false);
                    ToggleGO.transform.localPosition = firstposition - new Vector3(0, (ind - 1) * 30, 0);
                    ToggleGO.name = togglename;
                    ToggleGO.GetComponentInChildren<Text>().text = item.ToString();
                    Toggle newToggle = ToggleGO.GetComponent<Toggle>();
                    newToggle.group = ToggleGroupe_Race;
                    newToggle.isOn = false;
                    Toggles_Race.Add(newToggle);
                    newToggle.onValueChanged.AddListener(delegate {addFunctionToggle(newToggle.gameObject, itemR); });            
                }
                ind++;
            }
        }
        ToggleGroupe_Race.allowSwitchOff = false;
    }
    void addFunctionToggle(GameObject toggle, Race race)
    {
        string[] tempname = toggle.name.Split('_');
        string togglename = tempname[1];
        inputfield_currentRace.GetComponentInChildren<Text>().text = togglename;

        currPlaying_RaceScript = race;
        currPlaying_Race = (Race.Peoples)System.Enum.Parse(typeof(Race.Peoples), togglename);
        inputfield_actualMoney.GetComponentInChildren<Text>().text = currPlaying_RaceScript.actualMoney.ToString();
        b_raceSelected = checkRaceToggles(Toggles_Race);
        b_openMoneyCollectGUI = false;

    }
    bool checkRaceToggles(List<Toggle> togglelist)
    {
        foreach (var item in togglelist)
        {
            if (item.isOn)
                return true;
        }
        inputfield_currentRace.GetComponentInChildren<Text>().text = "Nobody";
        currPlaying_Race = Race.Peoples.Nobody;
        currPlaying_RaceScript = null;
        return false;
    }

    // Handle Area coloring
    public void ChangeSliderTransparency()
    {
        GameObject[] AllHexagons = GameObject.FindGameObjectsWithTag("Hexagon");

        foreach (GameObject hex in AllHexagons)
        {
            Color temp = hex.GetComponent<SpriteRenderer>().color;
            temp.a = Transparency.value;
            hex.GetComponent<SpriteRenderer>().color = temp;
        }
    }
    public void ToggleShowAreas()
    {
        GameObject[] AllHexagons = GameObject.FindGameObjectsWithTag("Hexagon");

        foreach (GameObject hex in AllHexagons)
        {
            Race.Peoples race = hex.GetComponent<HexagonSettings>().field.peoples;
            Color temp = hex.GetComponent<SpriteRenderer>().color;
            if (b_showColor)
            {
                switch (race)
                {
                    case Race.Peoples.Angmar:
                        temp = color_Angmar;
                        break;
                    case Race.Peoples.Gondor:
                        temp = color_Gondor;
                        break; ;
                    case Race.Peoples.Harad:
                        temp = color_Harad;
                        break; ;
                    case Race.Peoples.Isengart:
                        temp = color_Isengart;
                        break; ;
                    case Race.Peoples.Khazad:
                        temp = color_Khazad;
                        break; ;
                    case Race.Peoples.Mordor:
                        temp = color_Mordor;
                        break; ;
                    case Race.Peoples.Noldor:
                        temp = color_Noldor;
                        break; ; ;
                    case Race.Peoples.Numenor:
                        temp = color_Numenor;
                        break; ;
                    case Race.Peoples.Rhun:
                        temp = color_Rhun;
                        break; ;
                    case Race.Peoples.Rohan:
                        temp = color_Rohan;
                        break; ;
                    default:
                        temp = Color.white;
                        temp.a = Transparency.value;
                        break;
                }
            }
            else
            {
                temp = Color.white;
                temp.a = Transparency.value;
            }
            hex.GetComponent<SpriteRenderer>().color = temp;
            temp.a = Transparency.value;

        }
    }

    // Handel Change of Fields
    public void changeField(int FieldNb, Race newRace)
    {
        GameObject hex = GameObject.Find("Hexagon_" + FieldNb.ToString());
        Race.Peoples oldPeople = hex.GetComponent<HexagonSettings>().field.peoples;
        Race oldRace = gameControll.parseRace(oldPeople.ToString());

        Race.Peoples newPeople = newRace.peoples;
        if (oldPeople.Equals(newPeople))
            return;

        Buildings tempBuilding = hex.GetComponent<HexagonSettings>().field.MainBuilding;

        if (tempBuilding != null && tempBuilding.buildingType.ToString() != typeof(Empty).ToString())
        {
                oldRace.ownedBuildings.Remove(tempBuilding);
                Race newOwner = gameControll.parseRace(newRace.peoples.ToString());
                newOwner.ownedBuildings.Add(tempBuilding);
                tempBuilding.peoples = newPeople;
            if (tempBuilding.buildingType == Buildings.TypeOfBuildings.Farm || tempBuilding.buildingType == Buildings.TypeOfBuildings.Village)
            {
                I_GotDestroyed(FieldNb);
            }
            else if (tempBuilding.buildingType == Buildings.TypeOfBuildings.Mine)
            {
                tempBuilding.GeneratedIncome = tempBuilding.getIncomeInfo(Buildings.TypeOfBuildings.Mine, FieldNb);
            }
            else
                I_GotAttacked(FieldNb);
        }
        if (hex.GetComponent<HexagonSettings>().field.UpgradeTower.Count > 0)
        {
            foreach (var tower in hex.GetComponent<HexagonSettings>().field.UpgradeTower)
            {
                tower.peoples = newPeople;
            }
        }

        hex.GetComponent<HexagonSettings>().field.peoples = newPeople;

        updateCurrent(cS_FieldNbr,newRace);
        ToggleShowAreas();
    }

    public void I_GotAttacked(int FieldNb)
    {
        cS_FieldScript.populationCounter = 0;
        if (cS_FieldScript.population >=0)
        {
            cS_FieldScript.population--;
            if (cS_FieldScript.population<0)
            {
                cS_FieldScript.population = 0;
            }
        }       
    }
    public void I_GotAttackedHard(int FieldNb)
    {
        cS_FieldScript.populationCounter = 0;     
    }
    public void I_GotDestroyed(int FieldNb)
    {
        cS_BuildingScript.ruine = true;
        cS_FieldScript.population = 0;
        cS_FieldScript.populationCounter = 0;
    }
    public void FinishImediately(int FieldNb)
    {
        if (cS_BuildingScript!= null)
        {
            cS_BuildingScript.RoundsExisting = 0;
            cS_BuildingScript.BuildingFinished = cS_BuildingScript.checkIfFinished(cS_BuildingScript);
        }
    }
    public void ChangeNbrofDices(int FieldNb, int Nbr)
    {
        if (cS_BuildingScript != null && cS_BuildingScript.NbOfDices > 0)
        {
            cS_BuildingScript.AdditionalDices = cS_BuildingScript.AdditionalDices + Nbr;
            cS_BuildingScript.GeneratedIncome = cS_BuildingScript.getIncomeInfo(gameControll.fieldList[FieldNb].MainBuilding.buildingType, FieldNb);
            updateCurrent(FieldNb, cS_RaceScript);
        }
    }
    public void CollectMoney(Race.Peoples peoples )
    {
        MineList = new List<Buildings>();

        Race race = gameControll.parseRace(peoples.ToString());
        stringCollectMoney = "Your Buildings taxes\n";
        collectedMoney = 0; ;
        foreach (Buildings build in race.ownedBuildings)
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
                collectedMoney = collectedMoney + money;
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
        b_openMoneyCollectGUI = true;

    }
    public bool payForBuilding(Buildings newBuilding)
    {
        int leftover = currPlaying_RaceScript.actualMoney - newBuilding.Price;
        if (leftover > 0)
        {
            currPlaying_RaceScript.actualMoney = leftover;
            return true;
        }
        else
        {
            b_notEnoughMoney = true;
            return false;
        }
    } 
    public void startDestruction(int FieldNb)
    {
        if (FieldNb > 0 && FieldNb< 281)
        {
            Fields field = gameControll.fieldList[FieldNb];
        
            if (field.MainBuilding != null)
            {
                field.MainBuilding.underDestruction = true;
                field.MainBuilding.BuildingFinished = false;
                field.MainBuilding.RoundsExisting = Mathf.CeilToInt(cS_BuildingScript.RoundsTillFinish / 2);
            }
        }
    }
    string defineTextToShow()
    {
        string textToShow = "";
        if (cS_FieldNbr != 0)
        {
            if (cS_BuildingScript.buildingType != Buildings.TypeOfBuildings.Empty)
            {
                if (!cS_BuildingScript.ruine)
                {
                    textToShow = textToShow + "Here stands a Building: " + cS_BuildingType.ToString();
                    int generatedIncome;
                    bool b_income = int.TryParse(cS_BuildingScript.getIncomeInfo(cS_BuildingType, cS_FieldNbr), out generatedIncome);
                    if (b_income)
                    {
                        if (generatedIncome > 0)
                            textToShow = textToShow + " (" + cS_BuildingScript.getIncomeInfo(cS_BuildingType, cS_FieldNbr) + ")";
                    }
                    else
                        textToShow = textToShow + " (" + cS_BuildingScript.getIncomeInfo(cS_BuildingType, cS_FieldNbr) + ")";
                    if (!cS_BuildingScript.BuildingFinished & !cS_BuildingScript.underDestruction)
                    {
                        textToShow = "";
                        textToShow = textToShow + "\nBuild in Progress ("+ cS_BuildingType + "): Rounds to go: " + (-1 * cS_BuildingScript.RoundsExisting) + "\n\n";

                    }
                    else if (!cS_BuildingScript.BuildingFinished & cS_BuildingScript.underDestruction)
                    {
                        textToShow = "";
                        textToShow = textToShow + "\nDestruction in Progress: Rounds to go: " + (1 * cS_BuildingScript.RoundsExisting) + "\n\n";
                    }
                    else  
                        textToShow = textToShow + "\nRounds existing: " + cS_BuildingScript.RoundsExisting + "\n";
                    if (cS_BuildingScript.fortified)
                    {
                        if (cS_FieldScript.UpgradeTower[0].BuildingFinished)
                        {
                            textToShow = textToShow + "\n" + "The building is fortified\n";
                        }
                        else
                            textToShow = textToShow + "\n" + "The fortification of the building is under construction. " +
                             (-1 * cS_FieldScript.UpgradeTower[0].RoundsExisting) + " rounds till finished\n";
                    }
                }
                else
                     textToShow = textToShow + "Here stands a ruine of a " + cS_BuildingType.ToString() + "\n\n";
            }
            else
                textToShow = textToShow + "\n";
            textToShow = textToShow + "The field has the following properties\n" +
              "Fertility: " + cS_FieldScript.fertility + "\n" +
              "Ore Output: " + cS_FieldScript.oreOutput + "\n" +
              "Trade: " + cS_FieldScript.trade + "\n"+
              "Population: " + cS_FieldScript.population + "\n\n";

            if (cS_FieldScript.heroOrigine.Length > 1)           
                textToShow = textToShow + "\n" + "Heros Origine: " + cS_FieldScript.heroOrigine;
           
            if (cS_FieldScript.speciales.Length > 1)            
                textToShow = textToShow + "\n" + "Specials: " + cS_FieldScript.speciales;
            if (cS_FieldScript.bridge)            
                textToShow = textToShow + "\n" + "Here stands a bridge";

            if (cS_FieldScript.upgraded_bridge)
            {
                if (cS_FieldScript.UpgradeTower[0].BuildingFinished)
                {
                    textToShow = textToShow + "\n" + "Here stands a fortified bridge";
                }
                else
                    textToShow = textToShow + "\n" + "A fortified bridge is under construction. " +
                       (-1 * cS_FieldScript.UpgradeTower[0].RoundsExisting) + " rounds till finished";
            }                             
        }
        return textToShow;
    }

    // Todo: fix size issue! resize it if new Round has been started
    public void defineTextInfoField(string newText, bool add)
    {
        if (add)
        {
            InfoText.text = InfoText.text + "\n" + newText;      
        }
        else
        {
            exporter.LogFileSave(InfoText.text + "\n");
            InfoText.text = "";
            InfoText.text = newText;
        }

        // TODO: Jumt to the end of the Field
        InfoText.gameObject.transform.parent.gameObject.GetComponentInChildren<Scrollbar>().value = 1;
        //if (!add)
        //    InfoText.gameObject.transform.parent.gameObject.GetComponentInChildren<Scrollbar>().value = 1;
        //else
        //{
        //    InfoText.gameObject.transform.parent.gameObject.GetComponentInChildren<Scrollbar>().value = 0;
        //}

        //print(InfoText.cachedTextGenerator.characterCountVisible);
        //Canvas.ForceUpdateCanvases();

    }

    public string DefineSeason()
    {
        string curSeas = "";
        int season = gameControll.currentRound % 4;
        switch (season)
        {
            case 0:
                currSeason = GameControll.Seasons.Spring;
                break;
            case 1:
                currSeason = GameControll.Seasons.Summer;
                break;
            case 2:
                currSeason = GameControll.Seasons.Fall;
                break;
            case 3:
                currSeason = GameControll.Seasons.Winter;
                break;
            default:
                Debug.LogError("Error in SeasonHandling");
                break;
        }
        curSeas = currSeason.ToString();
        return curSeas;
    }

    // Activate or deactivate Buttons
    public void changeButtonState(string buttonName, bool state)
    {
        foreach (var item in allButtons)
        {
            if (item.name.Equals(buttonName))
            {
                item.gameObject.SetActive(state);
            }          
        }
    }



    public bool checkIfGUIactive()
    {
        if (b_openMoneyCollectGUI || b_openMoneyAddGUI || b_notEnoughMoney || !b_raceSelected || b_openBuild|| b_openFieldInfo)
            return true;
        else
            return false;

    }
    void OnGUI()
    {
        fontSize = 16;
        GUI.skin.label.font = GUI.skin.button.font = GUI.skin.box.font = GUI.skin.window.font = font;
       
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = GUI.skin.window.fontSize= fontSize;
        if (!b_raceSelected)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 110, 320, 210), ShowGUI1, "Warning");
        }

        if (b_openFieldInfo)// Open Field to decide if one want to build or concuer
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 110, 320, 300), ShowGUI2, "Field Menu of: "+
                 cS_FieldNbr+ " " + cS_Race);
        }

        if (b_openBuild && cS_FieldNbr !=0)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 110, 320, 300), ShowGUI3,
                "Build Options for Field: " + cS_FieldNbr);
        }
        if (b_notEnoughMoney)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 110, 320, 210), ShowGUI4, "Warning");
        }
        if (b_openMoneyAddGUI)
        {
            GUI.Window(1, new Rect((Screen.width / 2) -150, (Screen.height / 2) - 110, 200, 180), ShowGUI5, "Change Gold");
        }
        if (b_openMoneyCollectGUI)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 190, (Screen.height / 2) - 110, 360, 220), ShowGUI6, "Tax Report for " + currPlaying_RaceScript);
        }
    }

    void ShowGUI1(int windowID) // GUI for Start to select Race;
    {
        GUI.Label(new Rect(10, 20, 280, 150), "There is currently no race selected!\nPlease select race for continuing");         
    }
    void ShowGUI2(int windowID)
    {
        // RECT Positions
        Rect pos1 = new Rect(10, 215, 80, 30);
        Rect pos2 = new Rect(110, 215, 80, 30);
        Rect pos3 = new Rect(210, 215, 80, 30);
        Rect pos4 = new Rect(10, 250, 80, 30);
        Rect pos5 = new Rect(110, 250, 80, 30);
        Rect pos6 = new Rect(210, 250, 80, 30);

        GUI.Label(new Rect(15, 15, 280, 220), defineTextToShow());
        if (cS_FieldScript!= null)
        {      
            if (cS_Race != currPlaying_Race)
            {
                if (!cS_FieldScript.unusable)
                {
                    if (GUI.Button(pos5, "Conquer"))
                    {
                        changeField(cS_FieldNbr, currPlaying_RaceScript);
                    }
                }
            }

            if (GUI.Button(pos6, "Close"))
            {
                resetCurrent();
                b_openFieldInfo = false;
            }

            if (b_openFieldInfo && cS_Race == currPlaying_Race)
            {
                bool show_DestroyButton= false;
                bool show_DevelopButton = false;
                bool show_RepairButton = false;
                bool show_FortifyBuilding = false;
                bool show_FortifyBridge = false;

                switch (cS_BuildingType)
                {              
                    case Buildings.TypeOfBuildings.Empty:
                        if (GUI.Button(pos1, "Build"))
                        {
                            b_openFieldInfo = false;
                            b_openBuild = true;
                        }
                        break;
                    case Buildings.TypeOfBuildings.Farm:
                        show_DestroyButton = true;
                        show_DevelopButton = true;
                        show_FortifyBuilding = true;
                        break;
                    case Buildings.TypeOfBuildings.Village:
                        show_DestroyButton = true;
                        show_DevelopButton = true;
                        show_FortifyBuilding = true;
                        break;
                    case Buildings.TypeOfBuildings.Fort:
                        show_DevelopButton = true;
                        break;
                    case Buildings.TypeOfBuildings.Castle:
                        show_DestroyButton = true;
                        show_DevelopButton = true;
                        break;
                    case Buildings.TypeOfBuildings.Tower:
                        show_DevelopButton = true;
                        break;
                    case Buildings.TypeOfBuildings.Stronghold:
                        break;
                    case Buildings.TypeOfBuildings.City:
                        break;
                    case Buildings.TypeOfBuildings.Mine:
                        show_DestroyButton = true;
                        show_FortifyBuilding = true;
                        break;
                    case Buildings.TypeOfBuildings.Citadel:
                        break;
                    case Buildings.TypeOfBuildings.FortressMine:
                        break;
                    default:
                        break;
                }

                if (cS_FieldScript.bridge)
                    show_FortifyBridge = true;
                if (cS_BuildingScript.buildingType != Buildings.TypeOfBuildings.Empty & cS_BuildingScript != null)
                {
                    if (cS_BuildingScript.ruine)
                        show_RepairButton = true;
                    if (!cS_BuildingScript.BuildingFinished)
                    {
                        show_DevelopButton = false;
                    }
                }
                if (cS_BuildingType == Buildings.TypeOfBuildings.Farm)
                    if (cS_BuildingScript.BuildingFinished)
                        if (cS_FieldScript.fertility < 2)
                            show_DevelopButton = false;

                if (show_RepairButton)
                {
                    if (GUI.Button(pos4, "Repair"))
                    {
                        gameControll.buildBuilding(cS_BuildingScript, false, cS_FieldNbr, currPlaying_RaceScript);
                        cS_BuildingScript.BuildingFinished = false;
                        cS_BuildingScript.ruine = false;
                        cS_BuildingScript.RoundsExisting++;
                    }
                }
                if (show_DevelopButton)
                {
                    if (GUI.Button(pos1, "Develop"))
                    {
                        bool upgraded = false;
                        if (cS_BuildingScript.fortified)
                        {
                            upgraded = true;
                        }
                        gameControll.buildBuilding(cS_BuildingScript.BuildNextType(), upgraded, cS_FieldNbr, currPlaying_RaceScript);
                        updateCurrent(cS_FieldNbr,currPlaying_RaceScript);
                        b_openBuild = false;                                    
                    }
                }
                if (show_DestroyButton && !cS_BuildingScript.underDestruction)
                {
                    if (GUI.Button(pos3, "Destroy"))
                    {
                        startDestruction(cS_FieldNbr);
                        
                        b_openFieldInfo = false;
                    }
                }
                if (show_FortifyBridge && cS_FieldScript.bridge)
                {
 
                        if (GUI.Button(pos5, "Bridge +"))
                        {
                            gameControll.buildUpgrade(cS_FieldNbr, currPlaying_RaceScript, false, Buildings.UpgradeType.Bridge);
                            gameControll.buildUpgrade(cS_FieldNbr, currPlaying_RaceScript, false, Buildings.UpgradeType.Bridge);
                        cS_FieldScript.bridge = false;
                        cS_FieldScript.upgraded_bridge = true;
                        }               
                }
                if (show_FortifyBuilding && !cS_BuildingScript.fortified)
                {
                    if (GUI.Button(pos2, "Fortify"))
                    {
                        gameControll.buildUpgrade(cS_FieldNbr, currPlaying_RaceScript, false, Buildings.UpgradeType.Building);                    
                        cS_BuildingScript.fortified = true;
                    }
                }
            }
        }       
    }
    void ShowGUI3(int windowID)
    {
        Rect pos1 = new Rect(10, 215, 80, 30);
        Rect pos2 = new Rect(110, 215, 80, 30);
        Rect pos3 = new Rect(210, 215, 80, 30);
        Rect pos4 = new Rect(10, 250, 80, 30);
        Rect pos5 = new Rect(110, 250, 80, 30);
        Rect pos6 = new Rect(210, 250, 80, 30);

        GUI.Label(new Rect(15, 20, 280, 220), defineTextToShow());

        if (GUI.Button(pos6, "Cancel"))
        {
            b_openFieldInfo = true;
            b_openBuild = false;
        }
        if (b_openBuild)
        {
            if (cS_FieldScript.fertility > 0)
            {
                if (GUI.Button(pos1, "Farm"))
                {
                    gameControll.buildBuilding(new Farm(), false, cS_FieldNbr, currPlaying_RaceScript);
                    updateCurrent(cS_FieldNbr, currPlaying_RaceScript);
                    b_openBuild = false;
                }
            }
            if (GUI.Button(pos2, "Fort"))
            {
                gameControll.buildBuilding(new Fort(), false, cS_FieldNbr, currPlaying_RaceScript);
                defineTextInfoField("Building (" + cS_BuildingScript.buildingType + ") finished at Field: " + cS_BuildingScript.BuildingPosition, true);

                updateCurrent(cS_FieldNbr, currPlaying_RaceScript);
                b_openBuild = false;
            }
            if (GUI.Button(pos3, "Tower"))
            {
                gameControll.buildBuilding(new Tower(), false, cS_FieldNbr, currPlaying_RaceScript);
                updateCurrent(cS_FieldNbr, currPlaying_RaceScript);
                b_openBuild = false;
            }
            if (cS_FieldScript.oreOutput > 0)
            {
                if (GUI.Button(pos4, "Mine"))
                {
                    gameControll.buildBuilding(new Mine(), false, cS_FieldNbr, currPlaying_RaceScript);
                    updateCurrent(cS_FieldNbr, currPlaying_RaceScript);
                    b_openBuild = false;
                }
            }

        }
    }
    void ShowGUI4(int windowID)
    {
        GUI.Label(new Rect(10, 20, 280, 150), "Cannot be bought: not enough money!");
        if (GUI.Button(new Rect(100, 160, 100, 30), "OK"))
        {
            b_notEnoughMoney = false;
        }
    }
    void ShowGUI5(int windowID)
    {
        GUI.Label(new Rect(10, 20, 180, 150), "Please enter the amount of gold you want to add or draw (-)");
        stringEarnMoney = GUI.TextField(new Rect(10, 80, 180, 30), stringEarnMoney, 25);
        bool validInput;
        int newMoney;
        if (GUI.Button(new Rect(10, 130, 80, 30), "add/draw"))
        {
            validInput = int.TryParse(stringEarnMoney, out newMoney);
            if (validInput)
            {
                if (currPlaying_RaceScript.actualMoney + newMoney < 0)
                {
                    defineTextInfoField("Not Enough Money! Transaction has benn cancelled\n", true);
                }
                else
                {
                    currPlaying_RaceScript.actualMoney = currPlaying_RaceScript.actualMoney + newMoney;
                    inputfield_actualMoney.GetComponentInChildren<Text>().text = currPlaying_RaceScript.actualMoney.ToString();
                    defineTextInfoField(currPlaying_Race + " added some money: " +newMoney+"\n" , true);
                    stringEarnMoney = "";
                }

            }
            else
                stringEarnMoney = "";
            b_openMoneyAddGUI = false;
        }
        if (GUI.Button(new Rect(110, 130, 80, 30), "Close"))
        {
            b_openMoneyAddGUI = false;
        }
    }
    void ShowGUI6(int windowID) // Collect Taxes
    {
        GUI.Label(new Rect(10, 20, 300, 200), stringCollectMoney);

        if (!currPlaying_RaceScript.b_TaxCollected)
        {
            if (GUI.Button(new Rect(10, 180, 70, 30), "add Tax"))
            {
                currPlaying_RaceScript.actualMoney = currPlaying_RaceScript.actualMoney + collectedMoney;
                inputfield_actualMoney.GetComponentInChildren<Text>().text = currPlaying_RaceScript.actualMoney.ToString();
                defineTextInfoField(currPlaying_Race + " collected " + collectedMoney + " Gold from Taxes" + "\n", true);
                collectedMoney = 0;
                currPlaying_RaceScript.b_TaxCollected = true;
                if (MineList.Count == 0)
                {
                    currPlaying_RaceScript.b_GoldCollected = true;
                }
                b_showDiceButton = true;
                b_showInputfieldMineMoney = true;
            }
        }

        if (GUI.Button(new Rect(90, 180, 70, 30), "Cancel"))
        {
            foreach (Buildings build in currPlaying_RaceScript.ownedBuildings)
            {
                int money;
                bool b_money = int.TryParse(build.getIncomeInfo(build.buildingType, build.BuildingPosition), out money);
                if (b_money && money != 0)
                {
                    build.b_moneyCollected = false;
                }
            }
            collectedMoney = 0;
            MineList.Clear();
            b_openMoneyCollectGUI = false;
        }
        int mineIndex = 0;
        
        foreach (Buildings mine in MineList)
        {
            if (!mine.b_moneyCollected)
            {          
                if (GUI.Button(new Rect(220, (35* mineIndex + 20), 135, 30), mine +" "  + mine.BuildingPosition ))
                {
                    int position = MineList[mineIndex].BuildingPosition;
                    currentMine = mine;
                    b_showInputfieldMineMoney = true;
                    b_showDiceButton = true;
                    b_showInterfaceMineMoney = true;
                }
                mineIndex++;
            }
        }

        if (b_showInterfaceMineMoney)
        {
            if (b_showInputfieldMineMoney)
            {
                stringCollectMine = GUI.TextField(new Rect(170, 180, 40, 30), stringCollectMine, 25);
            }
            bool validInput;
            int newMoney;
            if (GUI.Button(new Rect(220, 180, 80, 30), "add Mine"))
            {
                validInput = int.TryParse(stringCollectMine, out newMoney);
                if (validInput)
                {
                    currPlaying_RaceScript.actualMoney = currPlaying_RaceScript.actualMoney + newMoney;
                    inputfield_actualMoney.GetComponentInChildren<Text>().text = currPlaying_RaceScript.actualMoney.ToString();
                    if (currentMine != null)
                    {
                        currentMine.b_moneyCollected = true;
                        currentMine = null;
                    }
                    stringCollectMine = "";

                    defineTextInfoField(currPlaying_Race + " collected " + newMoney + " from Mine" + "\n", true);

                    b_showInputfieldMineMoney = false;
                    b_showDiceButton = false;
                    b_showInterfaceMineMoney = false;
                    b_ShowDiceIncomeLabel = false;
                }
                else
                {
                    stringCollectMine = "";
                }
            }

            if (stringCollectMine == "" & b_showDiceButton)
            {
                if (GUI.Button(new Rect(310, 180, 40, 30), diceImage))
                {
                    IncomePerDice = ""; 
                    int Income = 0;
                    int tempIncome = 0;
                    int Dices = currentMine.NbOfDices;
                    string singleNumbers = "";
                    for (int i = 0; i < Dices; i++)
                    {
                        tempIncome = Mathf.RoundToInt(2 * UnityEngine.Random.Range(1, 6));
                        Income = Income + tempIncome;
                        IncomePerDice = IncomePerDice + tempIncome.ToString() + " ";
                        //   print(tempIncome);
                        singleNumbers = singleNumbers + tempIncome + ",";
                    }
                  
                    stringCollectMine = Income.ToString();

                    defineTextInfoField(currPlaying_Race + " diced " + singleNumbers + " in Mine"+ "\n", true);
                    IncomePerDice = "";
                    b_showDiceButton = false;
                    b_showInputfieldMineMoney = false;
                    b_ShowDiceIncomeLabel = true;
                }
            }
            if (b_ShowDiceIncomeLabel)
            {
                GUI.Label(new Rect(190, 185, 40, 30), stringCollectMine);
            }
        }

        foreach (var mine in MineList)
        {
            if (mine.b_moneyCollected)
            {
                currPlaying_RaceScript.b_GoldCollected = true;
            }
            else
            {
                currPlaying_RaceScript.b_GoldCollected = false;
                break;
            }
        }
    }
}

