using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//gets attached to all XRRigs so that when the scene loads the game manager gets a new XRCameraReference
//it's easier than trying to have the game manager ask for an updated reference because small scene change scripts are what change the scenes (not the game manager)
//so the game manager doesn't necesesarily when the scene changes, but the XRRig does because it was just created
public class PassXRCameraReferenceToGameManagerOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.XRCameraReference = gameObject;
        
    }

    
}
