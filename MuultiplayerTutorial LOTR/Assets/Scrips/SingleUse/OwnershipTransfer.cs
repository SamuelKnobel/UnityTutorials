using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OwnershipTransfer : MonoBehaviourPun ,IPunOwnershipCallbacks
{

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this); // Registers the Callbacks internally
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView)
            return;

        // Add Checks here

        base.photonView.TransferOwnership(requestingPlayer); // Changes the Ownership!to new player

    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView)
            return;
    }

    private void OnMouseDown()
    {
        base.photonView.RequestOwnership();

        // Can be changed on any event... 
    }
}
