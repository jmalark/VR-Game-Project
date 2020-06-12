using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.UI;

//gets placed in all pauseable scenes so that it is seperate from the groupings of menus (so that the menus can be set inactive)
public class PauseSystem : MonoBehaviour
{
    //used when trying to make sure saving is done before player continues and possibly quits the game
    private bool savingDone = false;
    private bool saveConfirmationTimeDone = false;


    //references to the different menus and the XRPauseButtonInteractor (what checks to see if the player hits the pause button) in the scene
    public XRPauseButtonInteractor PauseInteractor;
    public GameObject MenuGrouping;
    public GameObject PauseMenu;
    public GameObject OptionsMenu;
    public GameObject SaveMenu;
    public GameObject LoadMenu;
    public GameObject ExitToMainMessage;
    public GameObject QuitOptionsMessage;
    public GameObject saveConfirmedMessagePrefab;

    



    //was paused, now resuming, deactivates menus and UI raycasters/controllers, reactivates game time and teleport/etc game functionality
    public void Resume()
    {
        //if they hit the resume button make sure the raycasters get turned off, then inactivate the pausemenu and the menu grouping for tracking purposes
        PauseInteractor.LeftUIRaycaster.gameObject.SetActive(false);
        PauseInteractor.RightUIRaycaster.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
        MenuGrouping.gameObject.SetActive(false);

        //unpause game time/reactivate ability to interact with non UI items


    }



        /*
         * 
    //became obsolete after having UI buttons activate and deactivate the correct menus, but parts may still be needed for certain save and quit options
    public void SaveAndQuit()
    {
        //pop up the menu asking which of the 3 user save files they wish to use
        int saveSlot = GameManager.Instance.DisplaySlotsToSaveIn();

        if (saveSlot == -1)
        {
            //if the user hits the back button saveSlot gets set to -1, so they should get sent back to the pause menu and the rest of the code below 
            //does not get executed because of break statement
            
            return;

        }
        savingDone = false;

        //ask the GameManager to save the file and have 'true' return into savingDone when complete
        savingDone = GameManager.Instance.SaveUnwovenFile(saveSlot, GameManager.Instance.GetFullSaveDataPath(saveSlot));

        //wait until save is done so it doesn't quit too soon
        while (!savingDone)
        {
            //just wait until the save is done, this loop may not be necesary but shouldn't hurt
        }
        
        //give indication that the game has been saved and leave the message up long enough to read it
        StartCoroutine(DisplaySaveConfirmationMessage());

        while (!saveConfirmationTimeDone)
        {
            //wait for the confirmation message to be up long enough to read
        }

        //quit the game now that the user knows their game has been saved
        Application.Quit();

    }

        */



    /*
    //needs updated, originally was going to take care of menu activation but now activation is typcially done via UI buttons
    //however, if they gets called both for the regular save button and the section of save and quit, it needs to be able to handle that
    public void ChoseToSave(int saveSlot)
    {
        bool savingDone = false;

        //SaveMenu.gameObject.SetActive(false);

        //save and store if it's done saving
        savingDone = GameManager.Instance.SaveUnwovenFile(saveSlot, GameManager.Instance.GetFullSaveDataPath(saveSlot));

        while (!savingDone)
        {
            //wait until saving is done, may not be necesary to have this while loop but shouldn't hurt anything
        }

        //get timestamp for 'are you sure?' calculations, only really necessary for saving from pause menu but oh well it won't hurt to have it for at quit/exit
        GameManager.Instance.lastSaveTimestamp = Timeout.realtimeSinceStartup;


        //display a brief confirmation of save message, is a coroutine so it stays up long enough
        StartCoroutine(DisplaySaveConfirmationMessage());

        while(!saveConfirmationTimeDone)
        {
            //wait for the save confirmation message to be up for the time designated in the coroutine
        }
        saveConfirmationTimeDone = false;

    }
    
    */

        //pops up a small window that says the save was succesful, hold it up for long enough to read but not very long
    IEnumerator DisplaySaveConfirmationMessage()
    {
        //display the message
        saveConfirmedMessagePrefab.gameObject.SetActive(true);

        //wait just long enough for them to read it
        yield return new WaitForSecondsRealtime(0.5f);
        saveConfirmationTimeDone = true;
        saveConfirmedMessagePrefab.gameObject.SetActive(false);
    }

}
