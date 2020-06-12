using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//get attached to objects with colliders set to 'is Trigger' for example the portals and the invisible walls where players transition between scenes
//the script asks for a string representing the scene that this object should send the player to upon collision
public class RequestSceneChangeAtCollision : MonoBehaviour
{
    //will give each change scene collider the correct string
    public string sceneToLoad;
    
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
       
    }

}
