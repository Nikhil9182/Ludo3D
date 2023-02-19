using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPositions : MonoBehaviour {
    PhotonView pv;
    int sceneNumber;
    public GameObject[] userVideoSots;
    // Start is called before the first frame update
    void Start() {
        pv = GetComponent<PhotonView>();


        //userVideoSots = GameObject.FindGameObjectsWithTag("Slots");
        if (pv.IsMine) {
            sceneNumber = pv.Owner.ActorNumber - 1;
            Debug.Log(sceneNumber);
        }
    }

    // Update is called once per frame
    // Clean it with tide
    void Update() {
        sceneNumber = pv.Owner.ActorNumber - 1;
        if (pv.IsMine) {
            if (sceneNumber == 0 && (GameManager.manager.playerVideoSurface.Count > 0)) {
                for (int i = 0; i < GameManager.manager.playerVideoSurface.Count; i++) {
                    string[] data = GameManager.manager.playerVideoSurface[i].Split(':');
                    int pid = Int16.Parse(data[0]);
                    string vsurface = data[1];
                    if (pid == 0 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[0].transform, false);
                    }
                    else if (pid == 1 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[1].transform, false);
                    }
                    else if (pid == 2 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[2].transform, false);
                    }
                    else if (pid == 3 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[3].transform, false);
                    }
                }

            }

            else if (sceneNumber == 1 && (GameManager.manager.playerVideoSurface.Count > 0)) {
                for (int i = 0; i < GameManager.manager.playerVideoSurface.Count; i++) {
                    string[] data = GameManager.manager.playerVideoSurface[i].Split(':');
                    int pid = Int16.Parse(data[0]);
                    string vsurface = data[1];
                    if (pid == 0 && GameObject.Find(vsurface)!=null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[3].transform, false);
                    }
                    else if (pid == 1 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[0].transform, false);
                    }
                    else if (pid == 2 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[1].transform, false);
                    }
                    else if (pid == 3 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[2].transform, false);
                    }
                }
            }

            else if (sceneNumber == 2 && (GameManager.manager.playerVideoSurface.Count > 0)) {
                for (int i = 0; i < GameManager.manager.playerVideoSurface.Count; i++) {
                    string[] data = GameManager.manager.playerVideoSurface[i].Split(':');
                    int pid = Int16.Parse(data[0]);
                    string vsurface = data[1];
                    if (pid == 0 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[2].transform, false);
                    }
                    else if (pid == 1 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[3].transform, false);
                    }
                    else if (pid == 2 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[0].transform, false);
                    }
                    else if (pid == 3 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[1].transform, false);
                    }
                }
            }

            else if (sceneNumber == 3 && (GameManager.manager.playerVideoSurface.Count > 0)) {
                for (int i = 0; i < GameManager.manager.playerVideoSurface.Count; i++) {
                    string[] data = GameManager.manager.playerVideoSurface[i].Split(':');
                    int pid = Int16.Parse(data[0]);
                    string vsurface = data[1];
                    if (pid == 0 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[1].transform, false);
                    }
                    else if (pid == 1 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[2].transform, false);
                    }
                    else if (pid == 2 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[3].transform, false);
                    }
                    else if (pid == 3 && GameObject.Find(vsurface) != null) {
                        GameObject.Find(vsurface).transform.SetParent(userVideoSots[0].transform, false);
                    }
                }
            }




        }

        //Todo
        /////// Generalize videopos
        ///

    }
}
