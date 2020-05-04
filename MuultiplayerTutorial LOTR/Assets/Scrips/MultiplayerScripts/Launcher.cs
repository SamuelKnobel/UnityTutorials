using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks  
{
    [SerializeField]
    private Text ShowConnection;
    [SerializeField]
    Button ConnectToServer;

    private void Start()
    {
        CustomTypes.Register();

        ShowConnection.text = "starts connection";
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.SendRate = 40; // Default is 20
        PhotonNetwork.SerializationRate = 40; // Default is 10
        // only use the Mastersetting Gamesetting part if Name not set by Playerprefs
        if (PhotonNetwork.NickName.Length ==0)
        {
            PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        }

        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (PhotonNetwork.IsConnected)
        {
            ShowConnection.color = Color.green;
            ConnectToServer.gameObject.SetActive(false);
        }
        else
        {
            ShowConnection.color = Color.red;
            ConnectToServer.gameObject.SetActive(true);
        }
    }

    public void RetryConnection()
    {
        ShowConnection.text = "starts connection";
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;

        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        ShowConnection.text = "Connected to server";
        print(PhotonNetwork.LocalPlayer.NickName);
        if (!PhotonNetwork.InLobby)        
            PhotonNetwork.JoinLobby();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowConnection.text = "disconnected from server " + cause.ToString();
        ConnectToServer.gameObject.SetActive(true);


    }





}


