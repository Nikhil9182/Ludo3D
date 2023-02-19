using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    Transform[] ChildNodes;
    public List<Transform> ChildNodeList = new List<Transform>();


    void Start()
    {
        FillNodes();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        FillNodes();

        for(int i=0;i<ChildNodeList.Count;i++)
        {
            Vector3 pos = ChildNodeList[i].position;
            
            if(i>0)
            {
                Vector3 prev = ChildNodeList[i - 1].position;

                Gizmos.DrawLine(prev, pos);
            }
        }
    }

    void FillNodes()
    {
        ChildNodeList.Clear();

        ChildNodes = this.GetComponentsInChildren<Transform>();

        foreach(Transform child in ChildNodes)
        {
            if(child!=this.transform)
            {
                ChildNodeList.Add(child);
            }
        }
    }

    public int RequestedPostion(Transform node)
    {
        return ChildNodeList.IndexOf(node);
    }
}
