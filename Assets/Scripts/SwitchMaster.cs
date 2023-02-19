using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SwitchMaster : MonoBehaviourPun,IInRoomCallbacks
{
    // Network Event-Codes
    private const byte SwitchCode = 101;

    public static SwitchMaster instance;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        //PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void SetMaster()
    {
        Debug.LogError("Set-Master");
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        //throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.LogError("Master Client Switched "+newMasterClient.NickName);
        HasSwitched = true;

        if(MyCorutine!=null)
        {
            StopCoroutine(MyCorutine);
        }
       
        GameManager.manager.OnMasterClientSwitched();
    }

    private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    {
        switch (obj.Code)
        {
            case 101:
                {
                    HasSwitched = false;

                    object[] data = (object[])obj.CustomData;

                    //Cast the data in same order and data structure in which you have send

                    GameManager.manager.StopMasterSwitching();

                    Player player = (Player)data[0];

                    MyCorutine = StartCoroutine(ISwitchMaster(player));

                    Debug.Log("Switch Event Catch");

                    break;
                }
        }
    }
    //End Raise Events Callbacks

    private float SwitchTimer=3f;

    private bool HasSwitched = false;

    public void MasterSwitchEvent(Player player)
    {
        Debug.Log("Switch Event Raise");

        player = player.GetNext();

        object[] data = new object[1];

        data[0] = player;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(SwitchCode, data, raiseEventOptions, SendOptions.SendReliable);

    }

    private Coroutine MyCorutine;

    IEnumerator ISwitchMaster(Player player)
    {
        if(player.ActorNumber==PhotonNetwork.LocalPlayer.ActorNumber)
        {
            SetMaster();
        }

        yield return new WaitForSeconds(SwitchTimer);

        if(HasSwitched)
        {
            yield break;
        }

        MasterSwitchEvent(player);
    }


}
