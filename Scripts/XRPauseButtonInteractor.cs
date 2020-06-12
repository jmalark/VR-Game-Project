using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for handling pausing/unpausing and activating/deactivating the pause menu on valid button press
    /// Inherits from XRBaseControllerInteractor that is provided by the Unity XR toolkit
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/XR Pause Button Interactor")]
    public class XRPauseButtonInteractor : XRBaseControllerInteractor
    {
        //references to the menus and the raycasters so they can be activated/deactivated at the approprate times
        public GameObject MenuGrouping;
        public GameObject PauseMenuObject;
        public GameObject LeftUIRaycaster;
        public GameObject RightUIRaycaster;

        public bool pauseButtonStillHeld = false;

        //inheritence required implementing the abstract 
        protected override List<XRBaseInteractable> ValidTargets { get { return m_ValidTargets; } }
        
        
        List<XRBaseInteractable> m_ValidTargets = new List<XRBaseInteractable>();


        //clears targets then always add the game manager (which I attached an XRBaseInteractable to
        //because this is an interactor it needed to have an interactable even though it doesn't directly interact with anything
        protected override void Awake()
        {
            ValidTargets.Clear();
            ValidTargets.Add(GameManager.Instance.GetComponent<XRBaseInteractable>());

            base.Awake();
            ValidTargets.Clear();
            ValidTargets.Add(GameManager.Instance.GetComponent<XRBaseInteractable>());

            
        }

        //when you hit the pause button
        protected override void OnSelectEnter(XRBaseInteractable interactable)
        {
            //prevents flashing of pause menu by ignoring after a button press and before release
            if (!pauseButtonStillHeld)
            {
                //press was just initiated, so anything after before release is ignored
                pauseButtonStillHeld = true;

                base.OnSelectEnter(GameManager.Instance.GetComponent<XRBaseInteractable>());

                //checks if it's paused or unpause currently (menu grouping is invisible but parent to the other menus and used to check pause state)
                bool pauseActive = MenuGrouping.activeSelf;
                

                //if pausing, menu always starts on basic pause menu
                if (!pauseActive)
                {
                    //turn the grouping on, though this does not visibly change anything (so the children can be visible and for tracking purposes)
                    MenuGrouping.gameObject.SetActive(true);

                    //have GameManager calculate the menu position and player rotation (updates GameManager public variables), then use those values to put menu in correct place
                    GameManager.Instance.CalculateMenuPosition();

                    //this will move the whole stack of menus into position for when they need to appear
                    MenuGrouping.transform.SetPositionAndRotation(GameManager.Instance.menuPosition, GameManager.Instance.playerRotation);

                    //Vector3 testVector = new Vector3(0, 1.5f, 0);
                    //PauseMenuObject.transform.SetPositionAndRotation(testVector, GameManager.Instance.playerRotation);
                    //PauseMenuObject.transform.SetPositionAndRotation(GameManager.Instance.menuPosition, GameManager.Instance.playerRotation);

                    //now that position is set, activate the base pause menu
                    PauseMenuObject.gameObject.SetActive(true);

                }
                //if unpausing
                else
                {
                    

                    //set all of the menus in menu grouping to inactive
                    for (int i = 0; i < MenuGrouping.transform.childCount; i++)
                    {
                        MenuGrouping.transform.GetChild(i).gameObject.SetActive(false);

                    }

                    //now deactivate the group
                    MenuGrouping.gameObject.SetActive(false);

                    //unpause time/reactivate ability to move/etc

                    

                }

                //activate or deactivate the raycasters accordingly
                LeftUIRaycaster.SetActive(!pauseActive);
                RightUIRaycaster.SetActive(!pauseActive);
            }

        }

        //make sure to mark that the button is no longer held
        protected override void OnSelectExit(XRBaseInteractable interactable)
        {
            base.OnSelectExit(interactable);
            pauseButtonStillHeld = false;
        }

        //clears targets then always add the game manager (which I attached an XRBaseInteractable to
        //because this is an interactor it needed to have an interactable even though it doesn't directly interact with anything
        public override void GetValidTargets(List<XRBaseInteractable> validTargets)
        {
            
            ValidTargets.Clear();
            ValidTargets.Add(GameManager.Instance.GetComponent<XRBaseInteractable>());

        }
      

        //we want this to always be true so that it can always pause
		public override bool CanSelect(XRBaseInteractable interactable)
        {
            return true;
        }

        
    }
}