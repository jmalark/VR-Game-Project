using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using System.IO;
using System.ComponentModel.Design.Serialization;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;

//a singleton class that gets created and set to be persistent in the pre scene
//handles tracking save data and generating other persistent systems

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //number of times player has needed to loop
    private int numberOfTimesLooped = 0;

    //autosave variables
    private bool autosaveEnabled = true;
    private bool autosaveHandlerStateChangeBusy = false;
    private int autosaveChangedInLast5MinutesHowManyTimes = 0;

    //save data variables
    private int currentSaveSlotNumber;
    private bool gamePlayedBefore = false;
    private SaveDataClass saveDataClassInstanceForSaving;
    private BaseSaveSlotDataClass baseSaveSlotDataClassInstance;
    private SaveDataForGameControlsClass saveDataForGameControlsClassInstanceForSaving;
    private string basePersistentFilePath;
    private string fileExtensionType = ".json";
    private string controlsFileNameMinusExtension = "controlsFile";
    private string combinedBaseSaveSlotDataFilePath = string.Empty;
    private string fullControlsFilePath = string.Empty;

    //on start the XR Rigs should trigger a script to update this variable
    public GameObject XRCameraReference;

    //references to prefabs it may need to set active or inactive
    public GameObject MenuGrouping;
    public GameObject PauseMenuUIPrefab;
    public GameObject SaveMenuUIPrefab;
    public GameObject LoadMenuUIPrefab;
    public GameObject QuitOptionsUIPrefab;
    public GameObject GameSavedMessagePrefab;

    //just to get used temporarily whenever UI pops up, Vector3's were not happy with how I was trying to declare them before
    private Vector3 playerPosition;
    private Vector3 playerForward;
    public Vector3 menuPosition;
    public Quaternion playerRotation;

    //variable to check against in case someone literally just saved and those hit quit (so the 'are you sure' message won't pop up needlessly)
    public float lastSaveTimestamp = 0.0f;

    //marks GameManager so it persists/is the only instance, load into the first scene of the game
    private void Awake()
    {
        //if this is the first (and only) instance, set up the game manager
        if (Instance == null)
        {
            //make it persistent
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //checks if there's a file storing gameslot basic information and if not creates and saves a default one
            basePersistentFilePath = Application.persistentDataPath;
            string baseSaveSlotDataFileName = "baseSaveSlotDataFileName" + fileExtensionType;
            combinedBaseSaveSlotDataFilePath = Path.Combine(basePersistentFilePath, baseSaveSlotDataFileName);
            gamePlayedBefore = File.Exists(combinedBaseSaveSlotDataFilePath);

            if (gamePlayedBefore)
            {   //load the file storing the names of the save slots becuase we know it exists
                baseSaveSlotDataClassInstance = SaveGame.Load<BaseSaveSlotDataClass>(combinedBaseSaveSlotDataFilePath, new BaseSaveSlotDataClass());

                //load the file storing user control preferences because we know they've played before
                string controlsFileName = controlsFileNameMinusExtension + fileExtensionType;
                fullControlsFilePath = Path.Combine(basePersistentFilePath, controlsFileName);
                saveDataForGameControlsClassInstanceForSaving = SaveGame.Load<SaveDataForGameControlsClass>(fullControlsFilePath, new SaveDataForGameControlsClass());

                //user save data class doesn't get instatiated yet because we don't know what save slot they want to load
            }

            //game was never played before so it doesn't have a file for the save file names created yet
            else
            {
                //spawn and save a file to keep track of the save slot names, set to default "Empty" for all of them
                baseSaveSlotDataClassInstance = new BaseSaveSlotDataClass();
                SaveGameSlotNames();

                //spawn and save a file to store user control preferences, set to control defaults so the game knows how to interpret buttons to start
                saveDataForGameControlsClassInstanceForSaving = new SaveDataForGameControlsClass();
                SaveGameControls();

                //spawn a location for save data, the save slot number and name will come once the player actually hits New game
                saveDataClassInstanceForSaving = new SaveDataClass();


            }

        }
        //somehow another game manager already exists so destroy this one
        else
        {
            Destroy(gameObject);
        }



    }

    private void Start()
    {
        //get it past the pre scene
        SceneManager.LoadSceneAsync(1);
    }

    //returns the full file path name used for a specified save slot
    public string GetFullSaveDataPath(int saveSlotNumber)
    {
        string nameWithExtension = baseSaveSlotDataClassInstance.getSaveSlotName(saveSlotNumber);
        string fullPathName = Path.Combine(basePersistentFilePath, nameWithExtension);
        return fullPathName;
    }

    //is protected in SaveDataForGameControlsClass from being overwritten at the same time
    //either resets to 0 or updates by one a variable that tracks how many times the autosave feature was toggled back and forth in the last 5 minutes
    //is needed so that the user doesn't cause mulitple autosave coroutines going
    public void UpdateAutosaveChangedInLast5MinutesHowManyTimes(int zeroMeansResetOneMeansUpdateByOne)
    {
        switch (zeroMeansResetOneMeansUpdateByOne)
        {
            case 0:
                autosaveChangedInLast5MinutesHowManyTimes = 0;
                break;
            case 1:
                autosaveChangedInLast5MinutesHowManyTimes += 1;
                break;
            default:
                //it should never get here but it does we are going to ignore that the function was called
                break;
        }
    }

    //determines whether it is safe to toggle autosave right now (repeated toggling could cause multiple autosave coroutines)
    //if it's safe it sends it on to the function that handles the toggle, if it's not it takes care of the wait and check needed
    public void CleanUpCheckIfAutosaveChanged(bool toggleOn)
    {
        //it's always fine to run the option to turn it off so that just runs
        if (!toggleOn)
        {
            //turn off the enabled variable immediately
            HandleTurningAutosaveOnOrOff(toggleOn);
        }

        //if trying to turn autosave on
        else
        {
            //can just run because this call was the only one to toggle the value in the last ~5 minutes (or it's the first in as series of spamming and we want to run it). implies it used to be off
            if (autosaveChangedInLast5MinutesHowManyTimes == 1)
            {
                //safe to send it right through before autosave had been off for at least 5 minutes
                HandleTurningAutosaveOnOrOff(toggleOn);


                //wait ~5 minutes. In case there was an off after the first on, check what the behavior should actually be. if on, it sends to the handler, otherwise it doesn't need to do anything because all of the off commands went right through
                StartCoroutine(WaitAbout5MinutesThenCheckAutosaveToggle());

            }

            //was just turned off and now 1st request to turn it back on
            else if (autosaveChangedInLast5MinutesHowManyTimes == 2)
            {
                //not safe to turn it on right this minute, but also no guarantee that the 'off' command didn't get lucky and hit right before the autosave was going to loop again\
                //need to wait ~5 minutes so the original autosave stops running and check and see if it needs turned on at that point

                //wait 5 minutes and 15 seconds, check the save for controls class for correct toggle, resets count to 0, if true sends it to the autosave handler, otherwise does nothing
                StartCoroutine(WaitAbout5MinutesThenCheckAutosaveToggle());

            }

            else
            {
                //do nothing because either whatever the 1st 'on' call was will take care of making sure to check back in 5 minutes for the true value
            }

        }

    }

    //only gets called by cleanUpCheckIfAutosaveGetsChanged, either if it can immediately be set or if the ~5 minutes have passed and it wasn't recently changed 
    public void HandleTurningAutosaveOnOrOff(bool toggleOn)
    {
        while (autosaveHandlerStateChangeBusy)
        {
            //wait until it's not in use
        }

        autosaveHandlerStateChangeBusy = true;

        //only do something if it is actually changing states (aka first off in a spam or an on after 5 minutes or possibly a single on from off from a while ago
        if (toggleOn != autosaveEnabled)
        {
            autosaveEnabled = toggleOn;

            //turning autosave on when it wasn't before
            if (toggleOn)
            {

                //get the autosave full file path including the slot name and extension into one string
                string nameWithExtension = baseSaveSlotDataClassInstance.getSaveSlotName(currentSaveSlotNumber) + " - Autosave" + fileExtensionType;
                string fullFilePathWithName = Path.Combine(basePersistentFilePath, nameWithExtension);

                //disable the busy state first because it could get stuck in StartAutosaving for as long as it's autosaving
                autosaveHandlerStateChangeBusy = false;
                StartCoroutine(StartAutosaving(fullFilePathWithName));

                //it may have gotten 'stuck' on StartAutosaving because theoretically that function never ends, but the funcion will end as soon as that function stops running

            }

            //turning autosave off when it was on before
            else
            {
                //we already put autosaveEnabled to false above, so we just have to clear the 'busy'
                autosaveHandlerStateChangeBusy = false;

            }
        }

        else
        {   //need to make sure this gets unlocked since nothing needs done
            autosaveHandlerStateChangeBusy = false;
        }



    }

    //tells the game to save every 5 minutes (300 seconds). Gets called by the launch of a new game, load of an old game with autosave enabled, or flip over to autosave being on in the options menu
    IEnumerator StartAutosaving(string fullFilePathWithName)
    {

        //keeps repeating for the entire game once started unless player turns off autosave
        while (autosaveEnabled)
        {
            //wait 5 minutes first (so the player can fix their mistake if they started a new game without properly saving their old game)
            yield return new WaitForSecondsRealtime(300.0f);



            //save into the autosave saveslot 3 (0 indexed)
            SaveUnwovenFile(3, fullFilePathWithName);
        }

    }

    //exists to wait out any repeated toggling of autosave so that we don't accidentally start multiple autosave corountines
    IEnumerator WaitAbout5MinutesThenCheckAutosaveToggle()
    {
        //wait 
        yield return new WaitForSecondsRealtime(310.0f);

        //check what the behavior should actually be. if on, it sends to the handler, otherwise it doesn't need to do anything because all of the off commands went right through
        bool toggleOn = saveDataForGameControlsClassInstanceForSaving.getAutosaveBool();
        autosaveChangedInLast5MinutesHowManyTimes = 0;
        HandleTurningAutosaveOnOrOff(toggleOn);
    }

    //the funtion that literally performs the save given the save slot and the file path, returns true to allow calling functions to know it completed
    public bool SaveUnwovenFile(int saveSlotNumber, string desiredSaveFileFullPathWithName)
    {

        SaveGame.Save<SaveDataClass>(desiredSaveFileFullPathWithName, saveDataClassInstanceForSaving);


        return true;
    }

    //save game controls settings
    public void SaveGameControls()
    {
        string fileNameWithExtension = controlsFileNameMinusExtension + fileExtensionType;
        string desiredSaveFileFullPathWithName = Path.Combine(basePersistentFilePath, fileNameWithExtension);
        SaveGame.Save<SaveDataForGameControlsClass>(desiredSaveFileFullPathWithName, saveDataForGameControlsClassInstanceForSaving);
    }

    //saves the save slot names
    public void SaveGameSlotNames()
    {
        SaveGame.Save<BaseSaveSlotDataClass>(combinedBaseSaveSlotDataFilePath, baseSaveSlotDataClassInstance);
    }

    public void LoadUnwovenFile()
    {
        //
    }


    //may become obsolete if I am able to get the correct menus to activate/deactivate using UI buttons
    public int DisplaySlotsToSaveIn()
    {
        //pop up a canvas listing the save slot options (not including the autosave slot)


        //if they hit back return a unique value so it knows to return to the pause menu
        return -1;
    }

    //set number of times looped to the value provided (so typically if the player looped once more or we needed to set it to 0)
    public void UpdateNumberOfTimesLoopedTo(int newNumberOfTimesLooped)
    {
        numberOfTimesLooped = newNumberOfTimesLooped;
    }

    //uses camera references from the XR rig to make the menus show up in front of the player
    public void CalculateMenuPosition()
    {
        float distanceFromPlayer = 3.0f;

        //declares the Vector3's and Quaternion's and gets information from XR Rig in order to make the menu appear in front of the player
        playerPosition = new Vector3(XRCameraReference.transform.position.x, XRCameraReference.transform.position.y, XRCameraReference.transform.position.z);
        playerForward = new Vector3(XRCameraReference.transform.forward.x, XRCameraReference.transform.forward.y, XRCameraReference.transform.forward.z);
        Quaternion playerRotation = new Quaternion(XRCameraReference.transform.rotation.w, XRCameraReference.transform.rotation.x, XRCameraReference.transform.rotation.y, XRCameraReference.transform.rotation.z);
        var holdVectorMath = playerPosition + playerForward * distanceFromPlayer;
        menuPosition = new Vector3(holdVectorMath.x, holdVectorMath.y + 1, holdVectorMath.z);

        


    }

    //may become obsolete if I can get the correct menus to activate/deactivate with press of UI buttons
    public void InstantiateUICanvas(string UIType)
    {

        CalculateMenuPosition();
        
        //menuPosition = playerPosition + playerForwardDirection * distanceFromPlayer;
        //Instantiate(prefabReference, Vector3 for location, quaternion for rotation) 
        switch (UIType)
        {
                   
            case "PauseMenu":
                Instantiate(PauseMenuUIPrefab, menuPosition, playerRotation);
                break;
            case "SaveMenu":
                Instantiate(SaveMenuUIPrefab, menuPosition, playerRotation);
                break;
            case "LoadMenu":
                Instantiate(LoadMenuUIPrefab, menuPosition, playerRotation);
                break;
            case "QuitOptions":
                Instantiate(QuitOptionsUIPrefab, menuPosition, playerRotation);
                break;
            case "GameSavedMessage":
                Instantiate(GameSavedMessagePrefab, menuPosition, playerRotation);
                break;
            default:
                break;
        }
    

    }

    //may become obsolete but was orignally intended so that the pause menu could set itself and its parent inactive
    public void SetPauseAndMenuGroupingInactive()
    {
        PauseMenuUIPrefab.gameObject.SetActive(false);
        MenuGrouping.gameObject.SetActive(false);

    }
    

}
