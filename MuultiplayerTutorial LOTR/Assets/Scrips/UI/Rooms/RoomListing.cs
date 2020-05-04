using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.MaxPlayers + " , " + roomInfo.Name;
    }
    public void OnClick_Button()
    {
    // TODO Check for Identical Player Name (actually seems not to be a problem... )
        PhotonNetwork.JoinRoom(RoomInfo.Name);
    }
    
}
