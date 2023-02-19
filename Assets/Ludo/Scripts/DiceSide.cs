using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{
    bool OnGround;
    public int SideValue;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnGround = false;
        }
    }

    public bool onground()
    {
        return OnGround;
    }
}
