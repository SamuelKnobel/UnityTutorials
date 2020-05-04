using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RandomCustomPropertyGenerator : MonoBehaviour
{
    [SerializeField]

    private Text _text;

    private ExitGames.Client.Photon.Hashtable _myCustomProp = new ExitGames.Client.Photon.Hashtable();

    private void SetCustomNumber()
    {
        System.Random rnd = new System.Random();
        int result = rnd.Next(0, 99);
        _text.text = result.ToString();
        _myCustomProp["RandomNumb"] = result;
        _myCustomProp["Test"] = new TestClass();

        PhotonNetwork.SetPlayerCustomProperties(_myCustomProp);
        //PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProp;
    }
        


    public void OnClick_Generator()
    {
        SetCustomNumber();

    }
}
