using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
///  TODO: Add more Players !
///  TODO: Integrate this code into Gamecontroll or into other classes Or Integrat the Loading from Buildings and Race in this File
///  ToDO: does this need the Monobehavior 
/// </summary>


public class Importer : MonoBehaviour
{
    public GameControll game;
    public GUIHandler guiHandler;



    public void ImportSettings(string path)
    {
        //try// Open the text file using a stream reader.
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string Data_raw = reader.ReadToEnd();
                if (Data_raw == null)
                    return;
                string[] Data = Data_raw.Split('\n');
                foreach (var item in Data)
                {
                    string[] lineParts = item.Split('=');
                    if (lineParts.Length == 2)
                    {
                        switch (lineParts[0].Trim())
                        {
                            case "Dropboxfolder":
                                game.filepath_Dropbox = lineParts[1].Trim();
                                break;
                            case "Path_Buildings":
                                game.filepath_Buildinginformation = lineParts[1].Trim();
                                break;
                            case "Path_GeneralInfo":
                                game.filepath_GeneralGameInformation = lineParts[1].Trim();
                                break;
                            case "Path_Spielfeld":
                                game.filepath_SpielfeldTabelle = lineParts[1].Trim();
                                break;
                            case "NbOfPlayers":
                                game.playerSettingsSkript.NbOfPlayer = game.parseInt(lineParts[1].Trim());
                                break;
                            case "PlayerNumber":
                                game.playerSettingsSkript.PlayerNumber = game.parseInt(lineParts[1].Trim());
                                break;
                            case "CurrentRound":
                                game.inputField_currentRound.text = lineParts[1];
                                break;
                            case "PlayersName":
                                string[] nameArray = lineParts[1].Split(',');
                                game.playerSettingsSkript.PlayerNames = nameArray;
                                int index = 1;
                                foreach (var Pl_name in nameArray)
                                { 
                                    game.playerSettingsSkript.generatePlayer(Pl_name.Trim(), index);
                                    index++;
                                }
                                game.guiHandler.currentPlayer = game.PlayersList[game.playerSettingsSkript.PlayerNumber - 1];
                                break;
                            case "Player_1":
                                string[] races = lineParts[1].Split(',');
                                foreach (var race in races)
                                {
                                    for (int i = 0; i < game.playableRaces.Count; i++)
                                    {
                                        if (game.playableRaces[i].GetType() == Type.GetType(race.Trim()))
                                        {
                                            game.PlayersList[0].playedRaces.Add(game.playableRaces[i]);
                                            i = game.playableRaces.Count;
                                        }                                     
                                    }    
                                }
                                break;
                            case "Player_2":
                                string[] races2 = lineParts[1].Split(',');
                                foreach (var race in races2)
                                {
                                    for (int i = 0; i < game.playableRaces.Count; i++)
                                    {
                                        if (game.playableRaces[i].GetType() == Type.GetType(race.Trim()))
                                        {
                                            game.PlayersList[1].playedRaces.Add(game.playableRaces[i]);
                                            i = game.playableRaces.Count;
                                        }
                                    }
                                }
                                break;
                            case "Player_3":
                                string[] races3  = lineParts[1].Split(',');
                                foreach (var race in races3)
                                {
                                    for (int i = 0; i < game.playableRaces.Count; i++)
                                    {
                                        if (game.playableRaces[i].GetType() == Type.GetType(race.Trim()))
                                        {
                                            game.PlayersList[2].playedRaces.Add(game.playableRaces[i]);
                                            i = game.playableRaces.Count;
                                        }
                                    }
                                }
                                break;


                            case "Startgeld":
                                game.playerSettingsSkript.StartGold = game.parseInt(lineParts[1].Trim());
                                foreach (var item_r in game.playableRaces)
                                {
                                item_r.actualMoney = game.playerSettingsSkript.StartGold;
                                    item_r.MoneyLastRound = game.playerSettingsSkript.StartGold;
                                }
                                break;

                        }
                    }
                }
            }
        }
        //catch (Exception e)
        //{
        //    game.showPopUp_FileError = true;
        //    game.PopUp_ErrorText =
        //        "The file could not be read: " +
        //         e.Message + "\n" +
        //        "Please check file! and reload programm:" +
        //        path;
        //}
    }
   
}
