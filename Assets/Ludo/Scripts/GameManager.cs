using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Entity
    {
        public string PlayerName;
        public bool hasTurn;

        public List<Stone> MyStones = new List<Stone>();

        public List<Stone> MovableStone = new List<Stone>();

        public bool hasWon;
    }

    
    public static GameManager manager;
    public Entity[] PlayerList = new Entity[1];
    public Dice dice;

    [Header("Timer")]
    public double PerPlayerTime;
    float TimerMultiplierFraction;
    public bool FreezeTimer = true;
    double StarTime;
    [SerializeField]
    float FillAmount;

    [Header("UI")]
    public GameObject GameOverPanel;

    public Image TimerUI;

    [Header("Game Mode")]
    public GameMode Mode;
    

    //State-Machine
    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER,
        GAMEOVER
    }
    public States state;

    public enum Variation
    {
        PLAY_ROLL,
        ROLL_PLAY
    }
    public Variation PlayMode;

    public int activePlayer;
    bool SwitchingPlayer;
    bool PossibleTurn = true;


    //Human Input.................................
    [Header("HUMAN INPUTS")]
    int hummanrolldice;
    public GameObject RollButton;

    //---------------------------MULTIPLAYER STUF-------------------------
    PhotonView pv;
    public List<string> playerVideoSurface;
    //public List<>

    private void Awake()
    {
        if(manager==null)
        {
            manager = this;
        }
        else
        {
            Debug.Log("GameManager Script Has Another Instance");
            Destroy(this.gameObject);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;


        SetGameMode();

    }

    private void SetGameMode()
    {
        Mode=(GameMode)(int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["GMS"].ToString()));
    }

    private void Start()
    {
        
        pv = GetComponent<PhotonView>();

        ActivateButton(false);

        TimerMultiplierFraction = (float)(1 / PerPlayerTime);

        //Test Purpose
        if (activePlayer==(PhotonNetwork.LocalPlayer.ActorNumber-1))
        {
            state = States.SWITCH_PLAYER;
        }
    }

    private void Update()
    {

        switch(state)
        {
            case States.WAITING:
                {
                    //IDLE
                    break;
                }
            case States.GAMEOVER:
                {
                    //IDLE
                    break;
                }
            case States.ROLL_DICE:
                {
                    if(PossibleTurn)
                    {
                        if (activePlayer == (PhotonNetwork.LocalPlayer.ActorNumber - 1))
                        {
                            ActivateButton(true);
                            //StartCoroutine(DiceDelay());
                            
                        }
                        state = States.WAITING;
                    }
                    
                }
                break;

            case States.SWITCH_PLAYER:
                {
                    if (PossibleTurn)
                    {
                        //DEACTIVATE BUTTON
                        //DEACTIVATE HIGHLIGHT
                        if (pv.IsMine)
                        {
                            StartCoroutine(SwitchThePlayer());
                        }
                        
                        state = States.WAITING;
                    }
                   
                }
                break;
        }

        StartTimer();
    }

    void TimerINIT(double time)
    {
        FillAmount = 0.0f;
        //UiTimer.fillAmount = 0.0f;
        FreezeTimer = false;
        StarTime = time;
        //Debug.LogError(FreezeTimer);
    }

    public float UITimerFillAmount()
    {
        return (1 - FillAmount);//Replace This with Fill Amount Variable
    }

    void StartTimer()
    {
        if (state==States.GAMEOVER)
        {
            return;
        }
        if (!FreezeTimer)
        {
            double CurrentTime = PhotonNetwork.Time - StarTime;
            FillAmount = ((float)CurrentTime) * TimerMultiplierFraction;
            //Debug.Log(FillAmount);
            TimerUI.fillAmount=UITimerFillAmount();
            if (CurrentTime > PerPlayerTime)
            {
                FreezeTimer = true;
                PlayerTimeOut();
            }
        }
    }

    private void PlayerTimeOut()
    {
        StartMasterSwitching(PhotonNetwork.MasterClient);
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Player-TimeOut");

            pv.RPC("RPC_PlayerTimeOUT", RpcTarget.AllViaServer,0);
        }
    }

    public void OnMasterClientSwitched()
    {
        StopMasterSwitching();
        SetAllSelectorDeactivate();
        ActivateButton(false);
        state = States.SWITCH_PLAYER;
    }

    private bool MasterSwitching = false;
    private Coroutine MyCorutine;

    public void StartMasterSwitching(Player player)
    {
        if(MyCorutine!=null)
        {
            StopCoroutine(MyCorutine);
        }
       
        MasterSwitching = true;
        MyCorutine = StartCoroutine(TimeToSwitchMaster(player.GetNext()));
    }

    public void StopMasterSwitching()
    {
        MasterSwitching = false;
        if(MyCorutine!=null)
        {
            StopCoroutine(MyCorutine);
        }
       
    }

    IEnumerator TimeToSwitchMaster(Player player)
    {
        //yield return new WaitForSeconds(3.0f);

        if (player.ActorNumber==PhotonNetwork.LocalPlayer.ActorNumber)
        {
            SwitchMaster.instance.MasterSwitchEvent(player);
        }
        yield return new WaitForSeconds(5.0f);
        if(!MasterSwitching)
        {
            yield break;
        }
        StartMasterSwitching(player);
    }

    public void RollDice(int _dicenumber)
    {
        /* CPU INPUT DATA...........
         
        int DiceNumber = Random.Range(0, 7);

        if(DiceNumber==6)
        {
            //check start node
            CheckStartNode(DiceNumber);
        }

        if(DiceNumber<6)
        {
            //Check For Move
            MoveAStone(DiceNumber);
        }
        */

        hummanrolldice = _dicenumber;
        RollHumanDice();
    }

    IEnumerator DiceDelay()
    {
        yield return new WaitForSeconds(1);
        //RollDice();
    }

    void CheckStartNode(int dicenumber)
    {
        bool startNodeFull=false;

        for(int i=0;i<PlayerList[activePlayer].MyStones.Count; i++)
        {
            if(PlayerList[activePlayer].MyStones[i].CurrentNode==PlayerList[activePlayer].MyStones[i].StartNode)
            {
                // Start Node Is Full
                startNodeFull = true;
                break;
            }
            
        }
        if(startNodeFull)
        {
            //Move The Stone
            MoveAStone(dicenumber);
        }
        else
        {
            for (int i = 0; i < PlayerList[activePlayer].MyStones.Count; i++)
            {

                if (!PlayerList[activePlayer].MyStones[i].ReturnIsOut())
                {
                    //Leave The Base
                    PlayerList[activePlayer].MyStones[i].LeaveBase();
                    state = States.WAITING;
                    return;
                }
            }
            //Move The Stone
            MoveAStone(dicenumber);

        }
    }

    void MoveAStone(int dicenumber)
    {

        List<Stone> MoveableStone = new List<Stone>();
        List<Stone> MoveKickableStone = new List<Stone>();

        for(int i=0;i<PlayerList[activePlayer].MyStones.Count;i++)
        {
            if(PlayerList[activePlayer].MyStones[i].ReturnIsOut())
            {
                //CHECK FOR POSSIBLE KICK
                if(PlayerList[activePlayer].MyStones[i].CheckPossibleKick(PlayerList[activePlayer].MyStones[i].StoneID,dicenumber))
                {
                    MoveKickableStone.Add(PlayerList[activePlayer].MyStones[i]);
                    continue;
                }

                //CHECK FOR POSSIBLE MOVE
                if (PlayerList[activePlayer].MyStones[i].CheckPossibleMove(PlayerList[activePlayer].MyStones[i].StoneID,dicenumber))
                {
                    MoveableStone.Add(PlayerList[activePlayer].MyStones[i]);
                }
            }
        }

        //IF POSSIBLE KICK THE STONE
        if(MoveKickableStone.Count>0)
        {
            Debug.LogError("Possible Kick stone");
            int num = UnityEngine.Random.Range(0, MoveKickableStone.Count);
            MoveKickableStone[num].StartTheMove(dicenumber);
            state = States.WAITING;
            return;
        }

        //IF POSSOBLE MOVE THE STONE
        if (MoveableStone.Count > 0)
        {
            Debug.LogError("Possible Move stone");
            int num = UnityEngine.Random.Range(0, MoveableStone.Count);
            MoveableStone[num].StartTheMove(dicenumber);
            state = States.WAITING;
            return;
        }
        Debug.LogError("None Is Possible");
        //IF NONE IS POSSIBLE SWITCH THE PLAYER
        state = States.SWITCH_PLAYER;
    }

    IEnumerator SwitchThePlayer()
    {
        if(SwitchingPlayer)
        {
            yield break;
        }
        SwitchingPlayer = true;

        yield return new WaitForSeconds(2f);

        //SET THE NEXT PLAYER
        SetTheNextPlayer();

        SwitchingPlayer = false;
    }

    void SetTheNextPlayer()
    {
        int PlayerCount = 0;
        activePlayer++;
        PlayerCount = PhotonNetwork.PlayerList.Length;
        activePlayer %= PlayerCount;

        int AvaliablePlayers = 0;


        if(Mode==GameMode.CLASSIC)
        {
            //CHECK IF AT LEST TWO PLAYERS ARE LEFT IN THIS GAME
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                int PlayerNumber = PhotonNetwork.PlayerList[i].ActorNumber - 1;
                if (!PlayerList[PlayerNumber].hasWon)
                {
                    AvaliablePlayers++;
                }
            }

            if (PlayerList[activePlayer].hasWon && AvaliablePlayers > 1)
            {

                SetTheNextPlayer();
                return;
            }
            else if (AvaliablePlayers <= 1)
            {

                pv.RPC("RPC_GameOver", RpcTarget.All);

                return;
            }
            pv.RPC("RPC_ActivePlayer", RpcTarget.All, activePlayer, PhotonNetwork.Time);
        }
        else if(Mode==GameMode.QUICK || Mode == GameMode.MASTER)
        {
           
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                int PlayerNumber = PhotonNetwork.PlayerList[i].ActorNumber - 1;
                if (PlayerList[PlayerNumber].hasWon)
                {
                    pv.RPC("RPC_GameOver", RpcTarget.All);

                    return;
                }
            }
           
            pv.RPC("RPC_ActivePlayer", RpcTarget.All, activePlayer, PhotonNetwork.Time);
        }
        
    }

    public void ReportPossibleTurn(bool possibleturn)
    {
        pv.RPC("RPC_Possibleturn", RpcTarget.All, possibleturn);
    }

    //-------------------------------HUMAN INPUT--------------------------------
    void ActivateButton(bool on)
    {
        RollButton.SetActive(on);
    }

    public void SetAllSelectorDeactivate()
    {
        for(int i=0;i<PlayerList.Length;i++)
        {
            for(int j=0;j<PlayerList[i].MyStones.Count;j++)
            {
                PlayerList[i].MyStones[j].SetSelector(false);
            }
        }
    }

    //SET ON ROLLDICE BUTTON
    public void HumanRoll()
    {
        //Debug.Log("Human Roll");
        pv.RPC("RPC_HumanRollDice", RpcTarget.All);
        ActivateButton(false);
    }

    public void RollHumanDice()
    {
       
        //hummanrolldice = Random.Range(1, 7);

        //Debug.Log(hummanrolldice);

        List<Stone> MoveableStone = new List<Stone>();
        

        //CHECK START NODE IS FULL OR NOT
        bool startNodeFull = false;

        for (int i = 0; i < PlayerList[activePlayer].MyStones.Count; i++)
        {
            if (PlayerList[activePlayer].MyStones[i].CurrentNode == PlayerList[activePlayer].MyStones[i].StartNode)
            {
                // Start Node Is Full
                startNodeFull = true;
                break;
            }

        }

        if(hummanrolldice<6)
        {
            MoveableStone.AddRange(PossibleStones());
        }
        if(hummanrolldice==6 && !startNodeFull)
        {
            
            //INSDIE BASE CHECK
            for (int i = 0; i < PlayerList[activePlayer].MyStones.Count; i++)
            {
                if (!PlayerList[activePlayer].MyStones[i].ReturnIsOut())
                {
                    MoveableStone.Add(PlayerList[activePlayer].MyStones[i]);
                }
            }
            //OUTSIDE BASE CHECK
            MoveableStone.AddRange(PossibleStones());
        }
        else if(hummanrolldice==6 && startNodeFull)
        {
            //INSDIE BASE CHECK
            for (int i = 0; i < PlayerList[activePlayer].MyStones.Count; i++)
            {
                if (!PlayerList[activePlayer].MyStones[i].ReturnIsOut())
                {
                    MoveableStone.Add(PlayerList[activePlayer].MyStones[i]);
                }
            }
            
            Debug.Log("StartNode Is Full");
            MoveableStone.AddRange(PossibleStones());
        }

        if(PlayMode==Variation.PLAY_ROLL)
        {

        }
        if(MoveableStone.Count>0)
        {
            Debug.Log("Inside set selector  "+MoveableStone.Count);
            pv.RPC("RPC_ResetTimer", RpcTarget.All,PhotonNetwork.Time);
            //TimerINIT();

            for(int i=0;i<MoveableStone.Count;i++)
            {
                MoveableStone[i].SetSelector(true);
            }
        }
        else
        {
            pv.RPC("RPC_SwitchPlayer", RpcTarget.AllViaServer);
            //state = States.SWITCH_PLAYER;
        }
    }

    public int RequestDiceNumber()
    {
        return hummanrolldice;
    }
    List<Stone>PossibleStones()
    {
        List<Stone> Templist = new List<Stone>();

        for (int i = 0; i < PlayerList[activePlayer].MyStones.Count; i++)
        {
            if (PlayerList[activePlayer].MyStones[i].ReturnIsOut())
            {
                
                //CHECK FOR POSSIBLE KICK
                if (PlayerList[activePlayer].MyStones[i].CheckPossibleKick(PlayerList[activePlayer].MyStones[i].StoneID, hummanrolldice))
                {
                    Templist.Add(PlayerList[activePlayer].MyStones[i]);
                    continue;
                }

                //CHECK FOR POSSIBLE MOVE
                if (PlayerList[activePlayer].MyStones[i].CheckPossibleMove(PlayerList[activePlayer].MyStones[i].StoneID,hummanrolldice))
                {
                    Templist.Add(PlayerList[activePlayer].MyStones[i]);
                }
            }
        }

        return Templist;
    }

    [PunRPC]
    void RPC_ActivePlayer(int Activeplayer,double Time)
    {
        Debug.Log("RPC Active player");

        //Ui-Timer INIT
        TimerINIT(Time);

        activePlayer = Activeplayer;
        state = States.ROLL_DICE;
    }
    [PunRPC]
    void RPC_GameOver()
    {
        Debug.LogError("GameOver....");
        //SET THE GAME OVER SCREEN
        GameOverPanel.SetActive(true);
        state = States.GAMEOVER;
    }
    [PunRPC]
    void RPC_Possibleturn(bool possibleturn)
    {
        PossibleTurn = possibleturn;
    }
    [PunRPC]
    void RPC_PlayerTimeOUT(int code)
    {
        if(!FreezeTimer)
        {
            return;
        }
        StopMasterSwitching();
        Debug.Log("Switch Player "+code);
        SetAllSelectorDeactivate();
        ActivateButton(false);
        state = States.SWITCH_PLAYER;
    }
    [PunRPC]
    void RPC_ResetTimer(double time)
    {
        TimerINIT(time);
    }

    [PunRPC]
    void RPC_SwitchPlayer()
    {
        Debug.Log("Switch Player");
        
        state = States.SWITCH_PLAYER;
    }
    [PunRPC]
    void RPC_RollDice(double time)
    {
        Debug.Log("Roll Dice");
        TimerINIT(time);
        state = States.ROLL_DICE;
    }

    [PunRPC]
    void RPC_HumanRollDice()
    {
        FreezeTimer = true;
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Button Press");
            dice.RollDice();
        }
        
    }

}
