using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class QstartLobby : MonoBehaviourPunCallbacks
{
    public const string MAP_PROP_KEY = "MPL";
    public const string GAME_MODE_PROP_KEY = "GMS";

    public int GameVariationKey = 0;
    public int GameVariationMode = 0;

    [SerializeField]
    Button ExitButton;
    [SerializeField]
    Button StartButton;
    [SerializeField]
    Button MuffleButton;
    //[SerializeField]
    //Text PlayerName;
    [SerializeField]
    GameObject InputPannel;
    public static QstartLobby qstartlobby;
    private byte maxPlayer=0;

    [Header("Canvas Mode selection UI")]

    [SerializeField] GameObject GameModeSelectionPanel;
    [SerializeField] GameObject PlayerSelectionPanel;
    [SerializeField] GameObject LoadingPage;

    [SerializeField] GameJoin _gameJoin;

    private void Awake()
    {
        if (QstartLobby.qstartlobby == null)
        {
            QstartLobby.qstartlobby = this;
        }
        else
        {
            Destroy(this);
            QstartLobby.qstartlobby = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {   
        //ExitButton.gameObject.SetActive(true);
        //StartButton.gameObject.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnClickMasterMode()
    {
        GameVariationMode = 2;
        GameModeSelectionPanel.SetActive(false);
        OnClickStartButton();
    }
    public void OnClickQuickMode()
    { 
        GameVariationMode = 1;
        GameModeSelectionPanel.SetActive(false);
        OnClickStartButton();
    }
    public void OnClickClassicMode()
    {
        GameVariationMode = 0;
        GameModeSelectionPanel.SetActive(false);
        OnClickStartButton();
    }

    public void OnClickTwoPlayer()
    {
        maxPlayer = 2;
        GameModeSelectionPanel.SetActive(true);
        PlayerSelectionPanel.SetActive(false);
        LoadingPage.SetActive(true);
    }


    public void OnClickFourPlayer()
    {
        maxPlayer = 4;
        GameModeSelectionPanel.SetActive(true);
        PlayerSelectionPanel.SetActive(false);
        LoadingPage.SetActive(true);
    }

    public void OnClickStartButton()
    {
        if(maxPlayer==0)
        {
            return;
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickExitButton()
    {
        StartButton.gameObject.SetActive(false);
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected with master");
        StartCoroutine(_gameJoin.JoinContest());
        ExitGames.Client.Photon.Hashtable RequiredRoomProp = new ExitGames.Client.Photon.Hashtable { { MAP_PROP_KEY, GameVariationKey }, { GAME_MODE_PROP_KEY, GameVariationMode } };
        PhotonNetwork.JoinRandomRoom(RequiredRoomProp, maxPlayer);

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("RoomJoining Fails...");
        RoomOptions veerRoom = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = maxPlayer };
        string[] LobbyProp = { MAP_PROP_KEY, GAME_MODE_PROP_KEY};
        veerRoom.CustomRoomPropertiesForLobby =LobbyProp;
        veerRoom.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MAP_PROP_KEY, GameVariationKey }, { GAME_MODE_PROP_KEY, GameVariationMode } };
        veerRoom.CleanupCacheOnLeave = true;
        //veerRoom.PlayerTtl = 10000;
        int RandomNumber = Random.Range(1, 1000);
        PhotonNetwork.CreateRoom("VeerMalik05" + RandomNumber, veerRoom);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("RoomCreation Failed..!!");
        ExitGames.Client.Photon.Hashtable RequiredRoomProp= new ExitGames.Client.Photon.Hashtable { { MAP_PROP_KEY, GameVariationKey }, { GAME_MODE_PROP_KEY, GameVariationMode } };
        PhotonNetwork.JoinRandomRoom(RequiredRoomProp,maxPlayer);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("RoomCreat");
    }

}
