using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Stone : MonoBehaviour
{
    [Header("COLOR")]
    public string RouteColor;
    public string StartNodeColor;
    //public string BaseNodeColor;

    [Header("IGNORE-INDEX")]
    public int IgnoredBackNodeIndex;

    [Header("ROUTES..")]
    public Route CommonRoute;
    public Route FinalRoute;

    public List<Node> FullRoute = new List<Node>();
    public List<int> DiceNumbers = new List<int>();

    [Header("NODES..")]
    public Node StartNode;
    public Node BaseNode;
    public Node CurrentNode;
    public Node GoalNode;

    int RoutePosition;
    [SerializeField]
    int startnodeIndex;
    int steps;
    int doneSteps;

    [Header("BOOL")]
    bool IsOut;
    public bool IsMoving;
    public bool PossibleMove;
    public bool HasReached = false;
    float amplitude = 0.5f;
    float cTime = 0f;

    [Header("SELECTOR..")]
    public GameObject Selector;

    [Header("Player")]
    int PlayerNumber=0; //Actor Number Of PhotonPlayer
    public int StoneID;// Actor or team name here later we decide what we can do so we can recogaize our team stone
    private bool HaskickedAnotherPlayer = false;

    //-------------MUTLIPLAYER STUFF-------------------
    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

       transform.position = new Vector3(transform.position.x, 2.006f, transform.position.z);
        //WE NEED TO REPLACE THIS WITH ACTOR NUMBER
        StoneID = (pv.Owner.ActorNumber-1);
        PlayerNumber = StoneID;
        SetSelector(false);
        CommonRoute = GameObject.FindGameObjectWithTag("CommonRoute").GetComponent<Route>();
        FinalRoute = GameObject.FindGameObjectWithTag(RouteColor).GetComponent<Route>();
        StartNode = GameObject.FindGameObjectWithTag(StartNodeColor).GetComponent<Node>();
        startnodeIndex = CommonRoute.RequestedPostion(StartNode.gameObject.transform);
        CreateFullRoute();

        //Later We Change It With ActorNumber
        //GameManager.manager.PlayerList[PlayerNumber].PlayerName = "RedStone";
        GameManager.manager.PlayerList[PlayerNumber].MyStones.Add(this);
    }

    void CreateFullRoute()
    {
        for(int i=0;i<CommonRoute.ChildNodeList.Count;i++)
        {
            
            int TempPos = startnodeIndex+i;
            TempPos %= CommonRoute.ChildNodeList.Count;
            if(CommonRoute.ChildNodeList[TempPos].gameObject.layer!= IgnoredBackNodeIndex)
            {
                FullRoute.Add(CommonRoute.ChildNodeList[TempPos].GetComponent<Node>());
            }
            
        }

        for (int i = 0; i < FinalRoute.ChildNodeList.Count; i++)
        {
            FullRoute.Add(FinalRoute.ChildNodeList[i].GetComponent<Node>());
        }
    }


    IEnumerator Move(int dicenumber)
    {
        if(IsMoving)
        {
            yield break;
        }
        IsMoving = true;

        while(steps>0)
        {
            RoutePosition++;
            Vector3 StartPos= FullRoute[RoutePosition-1].gameObject.transform.position;
            Vector3 nextPos = FullRoute[RoutePosition].gameObject.transform.position;
            nextPos.y = 1.98f;

            while(MoveInArcToNextNode(StartPos,nextPos,5f))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }
        GoalNode = FullRoute[RoutePosition];

        bool IsKicked = false;

        if (GoalNode.IsTaken && GoalNode.stoneList.Count==1 && GoalNode.stone.StoneID != StoneID && !GoalNode.SafePlace)
        {
            //KICK THE STONE
            //GoalNode.stone.ReturnToBase();
            Debug.LogWarning("Stone Get kiced :" + GoalNode.stone.GetComponent<PhotonView>().ViewID);
            IsKicked = true;
            
        }

        HasReached = HasReachedAtHome(GoalNode);

        if(pv.IsMine)
        {
            pv.RPC("RPC_MoveStoneInfo", RpcTarget.AllViaServer, RoutePosition,IsKicked,HasReached,CheckForWinning());
        }

        //REPORT TO GAME MANAGER

        //SWITCH THE PLAYER AFTER MOVEMENT COMPLETE
        if(dicenumber<6 && !IsKicked)
        {
            GameManager.manager.gameObject.GetComponent<PhotonView>().RPC("RPC_SwitchPlayer", RpcTarget.AllViaServer);
            //GameManager.manager.state = GameManager.States.SWITCH_PLAYER;
        }
        else
        {
            GameManager.manager.gameObject.GetComponent<PhotonView>().RPC("RPC_RollDice", RpcTarget.AllViaServer,PhotonNetwork.Time);

            //GameManager.manager.state = GameManager.States.ROLL_DICE;
        }

        IsMoving = false;
    }

    bool MoveTONextNode(Vector3 goalPos,float Speed)
    {
        return goalPos!=(transform.position = Vector3.MoveTowards(transform.position, goalPos, Speed * Time.deltaTime));
        
    }
    bool MoveInArcToNextNode(Vector3 startPos, Vector3 goalPos, float Speed)
    {
        cTime += Speed * Time.deltaTime;
        Vector3 myPos = Vector3.Lerp(startPos, goalPos, cTime);

        myPos.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPos, cTime));
    }

    public bool ReturnIsOut()
    {
        return IsOut;
    }

    public void LeaveBase()
    {
        steps = 1;
        IsOut = true;
        RoutePosition = 0;

        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if (IsMoving)
        {
            yield break;
        }
        IsMoving = true;

        while (steps > 0)
        {
            //RoutePosition++;
            Vector3 StartPos = BaseNode.gameObject.transform.position;
            Vector3 nextPos = FullRoute[RoutePosition].gameObject.transform.position;
            nextPos.y = 1.98f;

            while (MoveInArcToNextNode(StartPos, nextPos, 5f))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }

        
        //Update Node
        GoalNode = FullRoute[RoutePosition];

        bool IsKicked = false;

        if (GoalNode.IsTaken && !GoalNode.SafePlace && GoalNode.stoneList.Count == 1 && GoalNode.stone.StoneID != StoneID)
        {
            //Return To the Start Node
            //GoalNode.stone.ReturnToBase();
            IsKicked = true;
        }

        if (pv.IsMine)
        {
            pv.RPC("RPC_MoveOutInfo", RpcTarget.All,RoutePosition,IsKicked);
        }
        //GoalNode.IsTaken = true;
        //GoalNode.stone = this;
        //CurrentNode = GoalNode;
        //GoalNode = null;

        // Update GameManager Back
        GameManager.manager.gameObject.GetComponent<PhotonView>().RPC("RPC_RollDice", RpcTarget.AllViaServer,PhotonNetwork.Time);
        //GameManager.manager.state = GameManager.States.ROLL_DICE;

        IsMoving = false;
    }

    public bool CheckPossibleMove(int StoneID,int dicenumber)
    {
        int TempPos = RoutePosition + dicenumber;
        if(TempPos>=FullRoute.Count)
        {
            return false;
        }

        if (GameManager.manager.Mode == GameMode.MASTER)
        {
            if (!HasKicked() && FullRoute.Count-FinalRoute.ChildNodeList.Count<=TempPos)
            {
               // Debug.LogError("Meet the condition Final Pos"+TempPos +"FullRoute Count "+FullRoute.Count+"  Final Route Count "+FinalRoute.ChildNodeList.Count);
                return false;
            }
        }

        if (FullRoute[TempPos].IsTaken && (FullRoute[TempPos].SafePlace || FullRoute[TempPos].stone.StoneID == StoneID))
        {
            return true;
        }
        return !FullRoute[TempPos].IsTaken;

    }

    public bool CheckPossibleKick(int StoneID,int dicenumber)
    {
        int TempPos = RoutePosition + dicenumber;
        if (TempPos >= FullRoute.Count || FullRoute[TempPos].SafePlace)
        {
            return false;
        }

        if(FullRoute[TempPos].IsTaken && FullRoute[TempPos].stoneList.Count==1 && FullRoute[TempPos].stone.StoneID != StoneID)
        {
            return true;
        }

        return false;

    }

    public void StartTheMove(int dicenumber)
    {
        steps = dicenumber;
        StartCoroutine(Move(dicenumber));

    }

    IEnumerator Return()
    {
        Debug.LogWarning("start-Corutine Return to it's Base");
        GameManager.manager.ReportPossibleTurn(false);
        Vector3 BaseNodePos = BaseNode.gameObject.transform.position;
        BaseNodePos.y = 2.006f;
        while (MoveTONextNode(BaseNodePos, 6f))
        {
            yield return null;
        }
        if (pv.IsMine)
        {
            pv.RPC("RPC_ReturnInfo", RpcTarget.All);
        }
    }

    private bool HasKicked()
    {
        foreach (var stone in GameManager.manager.PlayerList[GameManager.manager.activePlayer].MyStones)
        {
            if (stone.HaskickedAnotherPlayer)
            {
                return true;
            }
        }

        return false;
    }

    //----------------------HUMAN INPUT---------------------------------

    public void SetSelector(bool on)
    {
        //Clear the Dice-Number list & MovableStone list Here
        if(GameManager.manager.PlayMode==GameManager.Variation.ROLL_PLAY && on==false)
        {
            DiceNumbers.Clear();
            GameManager.manager.PlayerList[PlayerNumber].MovableStone.Clear();
        }

        Selector.SetActive(on);
        PossibleMove = on;
    }

    public bool HasReachedAtHome(Node _CurrentNode)
    {
        if(FinalRoute.ChildNodeList[FinalRoute.ChildNodeList.Count-1].GetComponent<Node>()== _CurrentNode)
        {
            return true;
        }
        return false;
    }

    private bool CheckForWinning()
    {
        if(GameManager.manager.Mode==GameMode.CLASSIC || GameManager.manager.Mode == GameMode.MASTER)
        {
            foreach (var stone in GameManager.manager.PlayerList[GameManager.manager.activePlayer].MyStones)
            {
                if (!stone.HasReached)
                {
                    return false;
                }
            }

            return true;
        }
        else if(GameManager.manager.Mode == GameMode.QUICK)
        {
            foreach (var stone in GameManager.manager.PlayerList[GameManager.manager.activePlayer].MyStones)
            {
                if (stone.HasReached)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    public void Shuffle(int stoneID)
    {
        if(pv.IsMine)
        {
            if(StoneID== stoneID)
            {
                this.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }
            else
            {
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            Debug.LogError(transform.position+" Before");

            switch(StoneID)
            {
                case 0:
                    {
                        Vector3 NewPos= CurrentNode.transform.position+new Vector3(-0.07f, 0, -0.06f);
                        NewPos.y = transform.position.y;
                        this.transform.position = NewPos;
                        break;
                    }
                case 1:
                    {
                        Vector3 NewPos = CurrentNode.transform.position + new Vector3(0.07f, 0, 0.06f);
                        NewPos.y = transform.position.y;
                        this.transform.position =NewPos;
                        break;
                    }
                case 2:
                    {
                        Vector3 NewPos = CurrentNode.transform.position + new Vector3(0.07f, 0, -0.06f);
                        NewPos.y = transform.position.y;
                        this.transform.position += NewPos;
                        break;
                    }
                case 3:
                    {
                        Vector3 NewPos = CurrentNode.transform.position + new Vector3(-0.07f, 0, 0.06f);
                        NewPos.y = transform.position.y;
                        this.transform.position += NewPos;
                        break;
                    }
            }

            Debug.LogError(transform.position + " After");


        }
    }

    public void ReShuffle()
    {
        if (pv.IsMine)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void StoneGetKicked()
    {
        Debug.LogWarning("Stone Kick RPC " + pv.ViewID);
        if(pv.IsMine)
        {
            StartCoroutine(Return());
        }
    }

    private void OnMouseDown()
    {
        if(PossibleMove)
        {
            pv.RPC("StoneMovement", RpcTarget.All);

            ReShuffle();

            if(!IsOut)
            {
                LeaveBase();
            }
            else
            {
                StartTheMove(GameManager.manager.RequestDiceNumber());
            }

            GameManager.manager.SetAllSelectorDeactivate();
        }
    }

    [PunRPC]
    void RPC_MoveStoneInfo(int RoutePosition,bool IsKicked,bool hasReached,bool HasWon)
    {
        GoalNode = FullRoute[RoutePosition];
        CurrentNode.IsTaken = false;

        if(IsKicked)
        {
            HaskickedAnotherPlayer = IsKicked;
            GoalNode.stone.StoneGetKicked();
           
        }

        if(CurrentNode.stoneList.Count>0)
        {
            CurrentNode.stoneList.Remove(CurrentNode.stone);
        }
        
        CurrentNode.stone = null;

        GoalNode.stone = this;
        GoalNode.IsTaken = this;
        CurrentNode = GoalNode;
        GoalNode.AddStoneToList(this);
       
        GoalNode = null;

        HasReached = hasReached;

        GameManager.manager.PlayerList[GameManager.manager.activePlayer].hasWon = HasWon;

    }
    [PunRPC]
    void RPC_MoveOutInfo(int RoutePosition,bool IsKicked)
    {
       
        GoalNode = FullRoute[RoutePosition];

        if (IsKicked)
        {
            HaskickedAnotherPlayer = IsKicked;

            GoalNode.stone.StoneGetKicked();
        }

        if (CurrentNode!=null && CurrentNode.stoneList.Count > 0)
        {
            CurrentNode.stoneList.Remove(CurrentNode.stone);
        }

        GoalNode.IsTaken = true;
        GoalNode.stone = this;
        CurrentNode = GoalNode;
        GoalNode.AddStoneToList(this);
       
        GoalNode = null;

    }
   
    [PunRPC]
    void RPC_ReturnInfo()
    {
        RoutePosition = 0;
        GoalNode = null;
        doneSteps = 0;
        IsOut = false;
        if(CurrentNode.stoneList.Count>0)
        {
            CurrentNode.stoneList.Remove(this);
        }
        CurrentNode = null;
        GameManager.manager.ReportPossibleTurn(true);
    }
    [PunRPC]
    void StoneMovement()
    {
        GameManager.manager.FreezeTimer=true;
    }
}
