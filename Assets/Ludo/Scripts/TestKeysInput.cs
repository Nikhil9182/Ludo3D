using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestKeysInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                Debug.Log("Key was Pressed :" + vKey);

            }
            
        }
    }
}
