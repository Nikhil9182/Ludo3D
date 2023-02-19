
using System.Collections.Generic;
using UnityEngine;
// Start is called before the first frame update
#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif
public class CheckAndroidPermission : MonoBehaviour
{
    
void Start() {
#if (UNITY_2018_3_OR_NEWER)
    if(Permission.HasUserAuthorizedPermission(Permission.Microphone)) {

        }
    else {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        if (Permission.HasUserAuthorizedPermission(Permission.Camera)) {

        }
        else {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
    }

}
