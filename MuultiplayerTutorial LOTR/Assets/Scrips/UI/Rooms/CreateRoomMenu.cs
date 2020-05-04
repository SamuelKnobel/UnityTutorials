using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;

    private RoomsCanvases _roomsCanvases;
    

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public static void ChangeButtonState(Button button, bool state)
    {
        button.gameObject.SetActive(state);
    }

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
              return;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;
        options.BroadcastPropsChangeToAll = true;

        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Sucesfully:");
        _roomsCanvases.CurrentRoomCanvas.Show();

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation failed:"+ message);

    }


}
