using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Spawing_Stones : MonoBehaviour
{
    public Transform[] RedSpawnPos;
    public Transform[] YellowSpawnPos;
    public Transform[] GreenSpawnPos;
    public Transform[] BlueSpawnPos;
    public Transform[] CamPose;

    private void Start()
    {
        PhotonNetwork.Instantiate("LudoNetworkPlayer", transform.position, Quaternion.identity, 0);
        int PlayerNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        switch (PlayerNum)
        {
            case 0:
                {
                    Camera.main.transform.position = CamPose[0].position;
                    Camera.main.transform.rotation = CamPose[0].rotation;
                    for (int i = 0; i < RedSpawnPos.Length; i++)
                    {
                        GameObject stone = PhotonNetwork.Instantiate("RedStone", RedSpawnPos[i].position, Quaternion.identity);
                        stone.GetComponent<Stone>().BaseNode = RedSpawnPos[i].GetComponent<Node>();
                    }
                    break;
                }
            case 1:
                {
                    Camera.main.transform.position = CamPose[1].position;
                    Camera.main.transform.rotation = CamPose[1].rotation;
                    for (int i = 0; i < YellowSpawnPos.Length; i++)
                    {
                        GameObject stone = PhotonNetwork.Instantiate("YellowStone", YellowSpawnPos[i].position, Quaternion.identity);
                        stone.GetComponent<Stone>().BaseNode = YellowSpawnPos[i].GetComponent<Node>();
                    }
                    break;
                }
            case 2:
                {
                    Camera.main.transform.position = CamPose[2].position;
                    Camera.main.transform.rotation = CamPose[2].rotation;
                    for (int i = 0; i < GreenSpawnPos.Length; i++)
                    {
                        GameObject stone = PhotonNetwork.Instantiate("GreenStone", GreenSpawnPos[i].position, Quaternion.identity);
                        stone.GetComponent<Stone>().BaseNode = GreenSpawnPos[i].GetComponent<Node>();
                    }
                    break;
                }
            case 3:
                {
                    Camera.main.transform.position = CamPose[3].position;
                    Camera.main.transform.rotation = CamPose[3].rotation;
                    for (int i = 0; i < BlueSpawnPos.Length; i++)
                    {
                        GameObject stone = PhotonNetwork.Instantiate("BlueStone", BlueSpawnPos[i].position, Quaternion.identity);
                        stone.GetComponent<Stone>().BaseNode = BlueSpawnPos[i].GetComponent<Node>();
                    }
                    break;
                }
        }
    }

}
