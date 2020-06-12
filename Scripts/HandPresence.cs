using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;


    //Add hands to the scene (called in start for 1st attempt and then in update called only if targetDevice has been made valid with instantiation)
    //tries to determine the type of controller the player is using, mostly for when we wish to display the controller
    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);


        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }

            else
            {
                //Debug.LogError("Did not find corresponding controller model");
                spawnedController = Instantiate(handModelPrefab, transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();

        }

    }

    void UpdateHandAnimation()
    {
        //set or unset the trigger animation
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }


        //set or unset the grip animation
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }



    // Start is called before the first frame update
    //see if it can detect two controllers
    void Start()
    {
        TryInitialize();
        
    }

    

    // Update is called once per frame
    //checks if the controllers were succesfully initialized already or not, if not it attemps to initialize
    //this makes sure the player can get their hands back after changing their batteries or if one controller wasn't on before starting the game
    void Update()
    {
        if(!targetDevice.isValid)
        {
            TryInitialize();
        }

        else
        {
            spawnedHandModel.SetActive(true);
            spawnedController.SetActive(false);
            UpdateHandAnimation();
        }
        
    }
    
}
