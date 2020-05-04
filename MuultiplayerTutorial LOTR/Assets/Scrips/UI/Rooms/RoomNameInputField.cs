using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;



/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// </summary>
[RequireComponent(typeof(InputField))]
public class RoomNameInputField : MonoBehaviour
{

    [SerializeField]
    private Text _roomName;
    [SerializeField]
    private Button _createRoomButton;

    public void SetRoomName(string value)
    {
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (_roomName.text.Length == 0)
            {
                Debug.LogError("No Name for the Room has been chosen");
                _roomName.text = "Default";

            }
            if (_roomName.text.Length < 1)
                CreateRoomMenu.ChangeButtonState(_createRoomButton, false);
            else
                CreateRoomMenu.ChangeButtonState(_createRoomButton, true);

            _roomName.text = value;
        }
    }



}
