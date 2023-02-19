using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaitTimer : MonoBehaviour
{
    public static WaitTimer waittimer;
    public GameObject Timer;
    public Text TimerText;
    public Text PLayerCount;
    private void Awake()
    {
        if(WaitTimer.waittimer==null)
        {
            WaitTimer.waittimer = this;
        }
    }
}
