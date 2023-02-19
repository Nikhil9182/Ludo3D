using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool IsTaken;

    public Stone stone;

    public List<Stone> stoneList = new List<Stone>();

    public bool SafePlace;


    public void AddStoneToList(Stone stone)
    {
        stoneList.Add(stone);
        if(IsShuffleRequired())
        {
            foreach (var _stone in stoneList)
            {
                _stone.Shuffle(stone.StoneID);
            }
        }
    }

    public void RemoveStoneToList(Stone stone)
    {
        stoneList.Remove(stone);
    }

    private bool IsShuffleRequired()
    {

        //if (stoneList.Count > 1)
        //{
        //    return true;
        //}
        //return false;

        if (!SafePlace || stoneList.Count<=1)
        {
            return false;
        }

        for (int i = 0; i < stoneList.Count - 1; i++)
        {
            if (stoneList[i].StoneID != stoneList[i + 1].StoneID)
            {
                return true;
            }
        }
        return false;
    }

}
