using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//provides the functions that the various main menu buttons need to call
public class MainMenu : MonoBehaviour
{
    //New button
    public void StartNewGame()
    {
        //pop up the save slots menu so they can pick a slot and name their game


        SceneManager.LoadSceneAsync("CampingForestScene");

        //make sure to save as soon as they pick a save file so that if their game crashes they have valid data instead of corrupted date


    }

    //Load button

    public void RequestToLoadGame()
    {
        //if all slots empty do something?

        //pop up the load save slots screen and then close it, it will only allow loading from non empty slots

    }

    //Options button

    //Quits immediately because there is nothing to save if you are on the menu screen
    public void QuitButtonPressed()
    {
        Application.Quit();
    }

}
