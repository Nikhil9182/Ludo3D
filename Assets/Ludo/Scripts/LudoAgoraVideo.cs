using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using Photon.Pun;

public class LudoAgoraVideo : MonoBehaviour {
    [SerializeField]
    public string appId = "";
    [SerializeField]
    private string channel = "Ludo";
    [SerializeField]
    public GameObject[] userVideoSots;
    public GameObject videoPrefab;
    public IRtcEngine mRtcEngine;
    private PhotonView pv;

    public RectTransform spawnPoint;
    private string originalChannel;
    private uint myUID = 0;
    private float spaceBetweenUservideos = 0f;

    public List<GameObject> playerVideoList;
    public List<string> globalVideoList;
    //PlayerCardManager playerCardManager;
    //public List<string>


    public delegate void AgoraCustomEvent();
    public static event AgoraCustomEvent PlayerChatIsEmpty;
    public static event AgoraCustomEvent PlayerChatIsPopulated;

    void Start() {

        userVideoSots = GameObject.FindGameObjectsWithTag("Slots");
        spawnPoint = GameObject.Find("VideoSurfaces").GetComponent<RectTransform>();
        pv = GetComponent<PhotonView>();
        if (!pv.IsMine) {
            transform.GetChild(0).gameObject.SetActive(false);

            return;
        }

        if (mRtcEngine != null) {
            IRtcEngine.Destroy();
        }
        originalChannel = channel;
        Debug.Log(appId);

        mRtcEngine = IRtcEngine.GetEngine(appId);
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccessHandler;
        mRtcEngine.OnUserJoined = OnUserJoinedHandler;
        mRtcEngine.OnLeaveChannel = OnLeaveChannelHandler;
        mRtcEngine.OnUserOffline = OnUserOfflineHandler;

        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();

        mRtcEngine.JoinChannel(channel, null, 0);


    }

    public string GetCurrentChannel() => channel;


    public void JoinRemoteChannel(string remoteChannelName) {
        if (!pv.IsMine) {
            return;
        }

        mRtcEngine.LeaveChannel();

        mRtcEngine.JoinChannel(remoteChannelName, null, myUID);
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();

        channel = remoteChannelName;
    }


    public void JoinOriginalChannel() {
        if (!pv.IsMine) {
            return;
        }


        /* NOTE:
         * Say I'm in my original channel - "myChannel" - and someone joins me.
         * If I want to leave the party, and go back to my original channel, someone is already in it!
         * Therefore, if someone is inside "myChannel" and I want to be alone, I have to join a new channel that has the name of my unique Agora UID "304598093" (for example).
         */
        if (channel != originalChannel || channel == myUID.ToString()) {
            channel = originalChannel;
        }
        else if (channel == originalChannel) {
            channel = myUID.ToString();
        }

        JoinRemoteChannel(channel);
    }


    // Local user joins channel
    private void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed) {

        if (!pv.IsMine)
            return;

        //Debug.Log("Local user joined channel " + channelName + " uid " + uid);
        myUID = uid;

        Debug.Log(pv.Owner.ActorNumber - 1);

        string playerId = (pv.Owner.ActorNumber - 1).ToString();
        string playerVideoId = uid.ToString();
        pv.RPC("GlobalPlayerSurface", RpcTarget.AllBuffered, playerId, playerVideoId);

        CreateUserVideoSurface(uid, true);
    }

    // Remote user
    private void OnUserJoinedHandler(uint uid, int elapsed) {
        if (!pv.IsMine) {
            return;
        }
        //Debug.Log("Remote user joined channel " + uid);

        CreateUserVideoSurface(uid, false);
    }


    // Local user join a channel
    private void OnLeaveChannelHandler(RtcStats stats) {
        if (!pv.IsMine) {
            return;
        }

        foreach (GameObject player in playerVideoList) {
            Destroy(player.gameObject);
        }

        playerVideoList.Clear();
    }


    // Remote User Leaves the Channel.
    private void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason) {
        Debug.Log("useroffline reason" + reason);
        Debug.Log("OnUserOfflineHandler");
        if (!pv.IsMine)
            return;

        if (playerVideoList.Count <= 1) {
            // PlayerChatIsEmpty();
        }

        RemoveUserVideoSurface(uid);
    }



    private void RemoveUserVideoSurface(uint deletedUID) {
        foreach (GameObject player in playerVideoList) {
            if (player.name == deletedUID.ToString()) {
                playerVideoList.Remove(player);
                Destroy(player.gameObject);
                break;
            }
        }
    }


    private void CreateUserVideoSurface(uint uid, bool isLocalUser) {
        //Debug.Log("CreateVS");
        for (int i = 0; i < playerVideoList.Count; i++) {
            if (playerVideoList[i].name == uid.ToString()) {
                return;
            }
        }

       //Debug.Log("Inside CreateVideoASurface " + uid + isLocalUser);

        // Test
        float spawnY = playerVideoList.Count * spaceBetweenUservideos;
        Vector3 spawnPosition = new Vector3(0, spawnY, 0);
        GameObject newUserVideo = Instantiate(videoPrefab, spawnPosition, spawnPoint.rotation);
        Debug.Log(newUserVideo);
        if (newUserVideo == null) {
            Debug.Log("newUser null!!");
            return;
        }
        newUserVideo.name = uid.ToString();
        newUserVideo.transform.SetParent(spawnPoint, false);
        newUserVideo.transform.rotation = Quaternion.Euler(Vector3.right * -180);


        playerVideoList.Add(newUserVideo);
        


        // Update video surface
        VideoSurface newVideoSurface = newUserVideo.GetComponent<VideoSurface>();
        if (newVideoSurface == null) {
            Debug.Log("Video surface null");
            return;
        }

        if (isLocalUser == false) {
            newVideoSurface.SetForUser(uid);

        }

        newVideoSurface.SetGameFps(30);
        UpdatePlayerVideoPostions();


    }

    void UpdatePlayerVideoPostions() {
    }



    [PunRPC]
    public void GlobalPlayerSurface(string pid, string vid) {
        Debug.Log("RPC called for  " + pid);
        foreach(var i in GameManager.manager.playerVideoSurface) {
            Debug.Log(i);
        }
        GameManager.manager.playerVideoSurface.Add(pid+":"+vid);

    }
    


    private void OnApplicationQuit() {
        if (mRtcEngine != null) {
            mRtcEngine.LeaveChannel();
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }
    }
}
