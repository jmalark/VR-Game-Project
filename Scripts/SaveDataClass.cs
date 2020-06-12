using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

//a class used to store user data and instanciated by the game manager
[System.Serializable]
public class SaveDataClass : MonoBehaviour
{
    
    //location information
    private string currentSceneName;
    private float currentPositionX;
    private float currentPositionZ;

    //0 is Sunday 6 is Saturday, will be -1 if prior to traveling to the past or otherwise not applicable
    private int dayOfWeekInt;

    //in game time, military time, undecided yet how much I'll display of the partial hours, -1 is if not applicable
    //ranges from 8 to 28, yes I know that's not how real days work
    private float hourOfDay;

    //something for inventory


    //number of times player has had to loop
    private int numberOfTimesLooped;

    //save file number and name, this may end up stored elsewhere instead
    private int saveFileSlotNumber;
    private string saveFileName;

    //to track journal entries
    private int numberOfJournalEntriesUnlocked;
    //the array will be created in a function so the size gets declared properly, this information will come from a journal manager, I would like to use floats to represent ranges to avoid wasting space when recording 
    //which pages should be filled in





    //add any booleans for early or late game progress markers

    //constructor, should only get called by the game manager
    public SaveDataClass()
    {
        //default values for a new game, will be used if it's a new game will be overwritten otherwise
        currentSceneName = "CampingForestScene";
        currentPositionX = 0;
        currentPositionZ = 0;
        dayOfWeekInt = -1;
        hourOfDay = -1.0f;
        numberOfTimesLooped = 0;
        numberOfJournalEntriesUnlocked = 0;
        

        
    }


    
}
