using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//a class that exists for the GameManager to instanciate to store the names of the 3 user save slots and 1 autosave save slot
[System.Serializable]
public class BaseSaveSlotDataClass : MonoBehaviour
{
    //user save slot names
    private string saveSlot0 = "Empty";
    private string saveSlot1 = "Empty";
    private string saveSlot2 = "Empty";

    //autosave only save slot name
    private string saveSlot3 = "Empty";

    //given the save slot number this will return the name (a string)
    public string getSaveSlotName(int slotNumber)
    {
        switch(slotNumber)
        {
            case 0:
                return saveSlot0;
            case 1:
                return saveSlot1;
            case 2:
                return saveSlot2;
            case 3:
                return saveSlot3;
            default:
                //it should never get here 
                return "defaultSaveFileName";


        }
    }

    //given the save slot number and desired name, it sets the save slot name
    public void setSaveSlotName(int slotNumber, string desiredSlotName)
    {
        switch (slotNumber)
        {
            case 0:
                saveSlot0 = desiredSlotName;
                break;
            case 1:
                saveSlot1 = desiredSlotName;
                break;
            case 2:
                saveSlot2 = desiredSlotName;
                break;
            case 3:
                saveSlot3 = desiredSlotName;
                break;
            default:
                //it should never get here, but I guess we just aren't making a new name
                break;
                

        }
    }
    
}
