using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

[SerializeField]
public class Spieler : MonoBehaviourPunCallbacks, IPunObservable
{

    public Player player;
    public PlayerSettings playerS;
    public int PlayerID;
    public GameControll gameControll;
    public GUIHandler guiHandler;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    [SerializeField]
    public static GameObject LocalPlayerInstance;

    public int tes = 4;
    public Vector2 vec = new Vector2(1f,1f);
    public GameObject Map;
    public TestClass testClass;

    // Start is called before the first frame update
    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            
        }
        player = this.GetComponent<PhotonView>().Owner;
        Map = GameObject.Find("Map");

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("02"))
        {
            gameControll = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
            guiHandler = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GUIHandler>();

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerS.PlayerName.Length < 2 & photonView.IsMine)
        {
            playerS = guiHandler.currentPlayer;
        }

        if (SceneManager.GetActiveScene().name.Contains("02"))
        {
            if (gameControll == null)
            {
                gameControll = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();

            }
            if (guiHandler== null)
            {
                guiHandler = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GUIHandler>();

            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            photonView.RPC("test", RpcTarget.All, playerS.PlayerNumber);
        }
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    photonView.RPC("test2", RpcTarget.All, 0, 100);
        //}
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    photonView.RPC("test3", RpcTarget.All, guiHandler.InfoText.text);
        //}

        // if (player != null)
        {
            //gameObject.name = playerS.PlayerName;
        }
        //if (testclass != null)
        //{
        //    this.name = testclass.playername;
        //}
        //if (photonView.IsMine)
        //{
        //    player = gameControll.guiHandler.currentPlayer;
        //}
    }
    [PunRPC]
    void test(int t)
    {

        playerS = gameControll.PlayersList[t - 1];

    }
    [PunRPC]
    void test2(int raceInt, int addmoney )
    {

        playerS.playedRaces[raceInt].actualMoney = playerS.playedRaces[raceInt].actualMoney + addmoney;
    }
    //[PunRPC]
    //void test3(string textt)
    //{

    //    guiHandler.InfoText.text = textt;
    //}

    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerS.PlayerNumber);
            stream.SendNext(guiHandler.InfoText.text);
            stream.SendNext(testClass);

            //print(1);
            // We own this player: send the others our data
            //stream.SendNext(gameControll.fieldList);
            //stream.SendNext(gameControll.playableRaces);
            //stream.SendNext(guiHandler.InfoText.text);
            stream.SendNext(tes);
            stream.SendNext(vec);

        }
        else
        {
            // Network player, receive data
            this.playerS.PlayerNumber = (int)stream.ReceiveNext();
            this.guiHandler.InfoText.text = (string)stream.ReceiveNext();
            this.testClass = (TestClass)stream.ReceiveNext();

            //this.gameControll.fieldList = (List<Fields>)stream.ReceiveNext();
            //this.gameControll.playableRaces = (List<Race>)stream.ReceiveNext();
            this.tes = (int)stream.ReceiveNext();
            this.vec = (Vector2)stream.ReceiveNext();
        }
        //photonView.RPC("Lel", RpcTarget.All, 0);
    }

 




    #endregion
}
