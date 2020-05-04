using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenue : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Transform _content;

    [SerializeField]
    private RoomListing _roomListing;

    private List<RoomListing> _listings = new List<RoomListing>();

    private RoomsCanvases _roomsCanvases;
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public override void OnJoinedRoom()
    {
        _roomsCanvases.CurrentRoomCanvas.Show();
        _content.DestroyChildern();
        _listings.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            //Removed from Roomlist
            if(info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            // added to Room List
            else
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                {
                    if (index ==-1) // nothing is found
                    {
                        RoomListing listing = (RoomListing)Instantiate(_roomListing, _content);
                        if (listing != null)
                        {
                            listing.SetRoomInfo(info);
                            _listings.Add(listing);

                        }
                    }
                    else
                    {
                        // Modify list here
                        //_listings[index]. ??
                    }
                }
            }
        }
    }
}
