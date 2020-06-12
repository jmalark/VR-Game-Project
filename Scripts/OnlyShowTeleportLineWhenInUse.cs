using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


//the teleportation scripts that the Unity XR toolkit provides always has the raycaster showing
//this makes sure to disable the raycaster(controls haptic feedback) and two pieces that make up with line displayed when not in use
public class OnlyShowTeleportLineWhenInUse : MonoBehaviour
{
    //references to the XRController that's checking for teleporation button press
    public XRController controller;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;
    public GameObject reticle;


    // Update is called once per frame
    //check to see teleportation is activated
    void Update()
    {
        //see if the line/etc should be active/if the button is pushed
        bool isTeleportActive = CheckIfActivated(controller);

        //set line and raycaster active or inactive depending on whether the button is pushed
        XRRayInteractor teleportRay = controller.GetComponent<XRRayInteractor>();
        LineRenderer teleportLineRenderer = controller.GetComponent<LineRenderer>();
        XRInteractorLineVisual teleportLineVisual = controller.GetComponent<XRInteractorLineVisual>();
        teleportRay.enabled = isTeleportActive;
        teleportLineRenderer.enabled = isTeleportActive;
        teleportLineVisual.enabled = isTeleportActive;
        reticle.SetActive(isTeleportActive);


    }

    //checks controller in question to see if the button in question is pressed
    private bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }

}