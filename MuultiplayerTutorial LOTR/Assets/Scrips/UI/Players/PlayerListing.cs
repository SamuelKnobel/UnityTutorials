using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _text;


    public Player Player { get; private set; }
    public bool Ready = false;
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        SetPlayerText(Player);


    }
    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (target!= null & target == Player)
        {
            if (changedProps.ContainsKey("RandomNumb"))
            {
                SetPlayerText(target);
            }
        }
    }
    private void SetPlayerText(Player player)
    {
        int result = -1;
        if (player.CustomProperties.ContainsKey("RandomNumb"))
        {
            result = (int)player.CustomProperties["RandomNumb"];
        }
        _text.text = player.NickName + " , " + result ;
        //TestClass testc = (TestClass)player.CustomProperties["Test"];

        //_text.text = player.NickName + " , " + testc.INT2;
    }

}
