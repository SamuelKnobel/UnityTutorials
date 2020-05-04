using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PlayerSettings 
{
    [SerializeField]
    public int NbOfPlayer;
    [SerializeField]
    public string[] PlayerNames;
    
    [NonSerialized]
    public GameControll gameControll;

    [SerializeField]
    public int StartGold;
    [SerializeField]
    public string PlayerName = "" ;
    [SerializeField]
    public int PlayerNumber;

    //[NonSerialized]
    public List<Race> playedRaces = new List<Race>();


    public static byte[] Serialize(object obj)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, obj);

            return memoryStream.ToArray();
        }
    }


    public static PlayerSettings Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(data, 0, data.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (PlayerSettings)binaryF.Deserialize(memoryStream);
        }
    }









    public void generatePlayer(string stringname, int playernumber)
    {
        gameControll = GameObject.FindGameObjectWithTag("Script_Container").GetComponent<GameControll>();
        PlayerSettings player = new PlayerSettings();
        player.PlayerName = stringname;
        player.PlayerNumber = playernumber;
        gameControll.PlayersList.Add(player);

    }
    public PlayerSettings()
    {
    }



}



