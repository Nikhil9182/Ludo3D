using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    Rigidbody rb;
    PhotonView pv;
    [SerializeField]
    bool Thrown;
    [SerializeField]
    bool hasLanded;
    [SerializeField]
    Vector3 Initposition;
    [SerializeField]
    int DiceValue;
    public DiceSide[] dicesides;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        Initposition = transform.position;
    }
    private void Start()
    {
        rb.useGravity = false;

        rb.isKinematic = true;
       
        dicesides = GetComponentsInChildren<DiceSide>();
    }

    Tween rotate;

    public void RollDice()
    {
        if (pv.IsMine)
        {
            
            ResetDice();
            if (!Thrown && !hasLanded)
            {
                Debug.Log("Roll Dice");

                //rb.AddTorque(Random.Range(450, 600), Random.Range(450, 600), Random.Range(450, 600));
                //Invoke(nameof(DiceFall), 2f);
                rotate = transform.DORotate(Random.rotation.eulerAngles, 0.2f, RotateMode.Fast).SetLoops(-1, LoopType.Incremental);
                Invoke(nameof(DiceFall), 2f);
            }
            else if (Thrown && hasLanded)
            {
                //RESET THE DICE
                ResetDice();
            }
        }
    }

    private void DiceFall()
    {
        rotate?.Kill();
        rb.AddTorque(Random.Range(250, 450), Random.Range(250, 450), Random.Range(250, 450));
        Thrown = true;
        rb.useGravity = true;
    }

    public void ResetDice()
    {
        rotate?.Kill();
        Debug.Log("Reset-Dice");
        transform.position = Initposition;
        transform.rotation = Random.rotation;
        Thrown = false;
        hasLanded = false;
        rb.isKinematic = false;
        rb.useGravity = false;
        
    }

    //private int DiceNumberGenerated = 0;

    private void Update()
    {
        if (pv.IsMine)
        {

            if (rb.IsSleeping() && !hasLanded && Thrown)
            {
                //Debug.Log("If Statement Update");
                hasLanded = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                Roll_DiceValueCheck();
            }
            else if (rb.IsSleeping() && hasLanded && DiceValue == 0)
            {
               // Debug.Log("else Statement Update");
                //ReRoll DICE
               ReRollDice();
            }


            //...........Testing Generate Required Number of Dice.............
            //if (Input.GetKeyDown(KeyCode.Keypad1))
            //{
            //    DiceNumberGenerated = 1;
            //}
            //else if(Input.GetKeyDown(KeyCode.Keypad2))
            //{
            //    DiceNumberGenerated = 2;
            //}
            //else if (Input.GetKeyDown(KeyCode.Keypad3))
            //{
            //    DiceNumberGenerated = 3;
            //}
            //else if (Input.GetKeyDown(KeyCode.Keypad4))
            //{
            //    DiceNumberGenerated = 4;
            //}
            //else if (Input.GetKeyDown(KeyCode.Keypad5))
            //{
            //    DiceNumberGenerated = 5;
            //}
            //else if (Input.GetKeyDown(KeyCode.Keypad6))
            //{
            //    DiceNumberGenerated = 6;
            //}

        }
    }

    void ReRollDice()
    {
        //Debug.Log("ReRollDice");
        ResetDice();
        Thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(100, 500), Random.Range(100, 500), Random.Range(100, 600));
    }

    void Roll_DiceValueCheck()
    {
        DiceValue = 0;
        //Debug.Log("Dice Value Checked Call ....");
        foreach(DiceSide side in dicesides)
        {
            if(side.onground())
            {
                DiceValue = side.SideValue;
                Debug.LogWarning(DiceValue);
                //SEND BACK TO THE GAMEMANAGER
                if(pv.IsMine)
                {
                    //Debug.Log("UPdate Dice Value Check");
                    pv.RPC("RPC_SetDiceValueInideGameManager", RpcTarget.All, DiceValue);
                }
            }
        }
    }

    [PunRPC]
    void RPC_SetDiceValueInideGameManager(int _DiceValue)
    {
        if((PhotonNetwork.LocalPlayer.ActorNumber-1)==GameManager.manager.activePlayer)
        {
            GameManager.manager.RollDice(_DiceValue);
            Debug.Log(_DiceValue);
        }
    }

}
