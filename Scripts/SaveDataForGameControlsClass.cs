using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a storage place 1 per device for control preferences. Does not include anything that they cannot change
//game manager instantiates, includes many get and set functions so the options menu can reflect and alter the values
[System.Serializable]
public class SaveDataForGameControlsClass : MonoBehaviour
{
    //to prevent race conditions
    private bool busyMakingAChange;

    //Controller setting variables
    private bool autosaveEnabled;

    //joystick movement for movement or turn
    private bool leftPrimary2DAxisForTurning;
    private bool rightPrimary2DAxisForTurning;
    private bool leftPrimary2DAxisForMovement;
    private bool rightPrimary2DAxisForMovement;

    //primary buttons for teleport or pause
    private bool leftPrimaryForTeleport;
    private bool rightPrimaryForTeleport;
    private bool leftPrimaryForPause;
    private bool rightPrimaryForPause;

    //secondary buttons for pause
    private bool leftSecondaryForPause;
    private bool rightSecondaryForPause;

    //push joysticks for teleport
    private bool leftPrimary2DAxisClickForTeleport;
    private bool rightPrimary2DAxisClickForTeleport;

    //add settings for teleport limiting field of view settings
    //add settings for smooth or snap turning

    //default constructor that sets default control options 
    public SaveDataForGameControlsClass()
    {
        busyMakingAChange = false;
        autosaveEnabled = true;
        leftPrimary2DAxisForTurning = true;
        rightPrimary2DAxisForTurning = false;
        leftPrimary2DAxisForMovement = false;
        rightPrimary2DAxisForMovement = true;

        leftPrimaryForTeleport = true;
        rightPrimaryForTeleport = true;
        leftPrimaryForPause = false;
        rightPrimaryForPause = false;

        leftSecondaryForPause = true;
        rightSecondaryForPause = true;

        leftPrimary2DAxisClickForTeleport = true;
        rightPrimary2DAxisClickForTeleport = true;
    }

    public bool getAutosaveBool()
    {
        return autosaveEnabled;
    }

    public void setAutosaveBool(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        //just need to protect it long enough to stop it from getting written at the same time
        busyMakingAChange = true;
        autosaveEnabled = toggleOn;
        //so the GameManager can decide to ignore some function calls for autosave for a bit and then later check and fix the actual autosave status
        GameManager.Instance.UpdateAutosaveChangedInLast5MinutesHowManyTimes(1);
        busyMakingAChange = false;

        //in case of multiple sets/unsets this marks a flag (only once) and waits five minutes before checking what the last set was
        GameManager.Instance.CleanUpCheckIfAutosaveChanged(toggleOn);
    }

    public void setLeftPrimary2DAxisForTurning(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        leftPrimary2DAxisForTurning = toggleOn;
        if (toggleOn)
        {
            leftPrimary2DAxisForMovement = false;
        }
        busyMakingAChange = false;
    }

    public void setRightPrimary2DAxisForTurning(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        busyMakingAChange = true;
        rightPrimary2DAxisForTurning = toggleOn;
        if (toggleOn)
        {
            rightPrimary2DAxisForMovement = false;
        }

        busyMakingAChange = false;
    }

    public void setLeftPrimary2DAxisForMovement(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        busyMakingAChange = true;
        leftPrimary2DAxisForMovement = toggleOn;
        if (toggleOn)
        {
            leftPrimary2DAxisForTurning = false;
        }

        busyMakingAChange = false;
    }

    public void setRightPrimary2DAxisForMovement(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        busyMakingAChange = true;
        rightPrimary2DAxisForMovement = toggleOn;
        if (toggleOn)
        {
            rightPrimary2DAxisForTurning = false;
        }
        busyMakingAChange = false;
    }

    public void setLeftPrimaryForTeleport(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        leftPrimaryForTeleport = toggleOn;
        if (toggleOn)
        {
            leftPrimaryForPause = false;
        }
        busyMakingAChange = false;
    }

    public void setRightPrimaryForTeleport(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        rightPrimaryForTeleport = toggleOn;
        if (toggleOn)
        {
            rightPrimaryForPause = false;
        }
        busyMakingAChange = false;
    }

    public void setLeftPrimaryForPause(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        busyMakingAChange = true;
        leftPrimaryForPause = toggleOn;
        if (toggleOn)
        {
            leftPrimaryForTeleport = false;
        }
        busyMakingAChange = false;
    }

    public void setRightPrimaryForPause(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }

        busyMakingAChange = true;
        rightPrimaryForPause = toggleOn;
        if (toggleOn)
        {
            rightPrimaryForTeleport = false;
        }

        busyMakingAChange = false;
    }

    public void setLeftSecondaryForPause(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        leftSecondaryForPause = toggleOn;
        busyMakingAChange = false;
    }

    public void setRightSecondaryForPause(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        rightSecondaryForPause = toggleOn;
        busyMakingAChange = false;
    }

    public void setLeftPrimary2DAxisClickForTeleport(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        leftPrimary2DAxisClickForTeleport = toggleOn;
        busyMakingAChange = false;
    }

    public void setRightPrimary2DAxisClickForTeleport(bool toggleOn)
    {
        while (busyMakingAChange)
        {
            //just need this to spin until it's not busy anymore
        }
        busyMakingAChange = true;
        rightPrimary2DAxisClickForTeleport = toggleOn;
        busyMakingAChange = false;
    }

}


