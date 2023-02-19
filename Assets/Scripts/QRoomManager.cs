using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class QRoomManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static QRoomManager qroommanager;
    [SerializeField]
    int MultiplyerSceneIndex;
    float StartTime;
    private int PlayerCount;
    public int countRequired;
    bool GameStart = false;

    [SerializeField]
    Text PlayerName;

    private void Awake()
    {
        if (QRoomManager.qroommanager == null)
        {
            QRoomManager.qroommanager = this;
        }

    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);

    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);

    }
    private void Start()
    {
        StartTime = 0;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PlayerCount = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.PlayerList.Length == countRequired)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnClickStartButton();
            }
        }

    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PlayerCount = PhotonNetwork.PlayerList.Length;
       // PhotonNetwork.LocalPlayer.NickName = PlayerName.text;

        //if (PhotonNetwork.PlayerList.Length == 1)
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        PhotonNetwork.CurrentRoom.IsOpen = false;
        //        PhotonNetwork.LoadLevel(MultiplyerSceneIndex);
        //    }
        //}

    }

    public void OnClickStartButton()
    {
        GameStart = true;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(MultiplyerSceneIndex);
    }



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        PlayerCount = PhotonNetwork.PlayerList.Length;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
                Application.Quit();
            }
        }

        if (!GameStart && PhotonNetwork.InRoom)
        {
            WaitTimer.waittimer.Timer.SetActive(true);
            WaitTimer.waittimer.PLayerCount.text = "No. of PLayers: " + PlayerCount.ToString();
            //StartTime += Time.deltaTime;
            //WaitTimer.waittimer.TimerText.text = StartTime.ToString("f2");
        }
        else
        {
            WaitTimer.waittimer.Timer.SetActive(false);
        }
    }



}
