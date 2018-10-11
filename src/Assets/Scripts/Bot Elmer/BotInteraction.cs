using UnityEngine;
using System;
using System.Collections.Generic;

namespace Cyan.Elmer
{
    /// <summary>
    /// Handles the interaction moments by keeping track of when things are grabbed, dropped etc
    /// </summary>
    public class BotInteraction : MonoBehaviour
    {
        private Interact interact;
        public GeneralTimer genTimer;

        private PData playData;
        private List<float> grabTimes = new List<float>();
        private List<float> dropTimes = new List<float>();

        [SerializeField]
        private int grabIndex = 0;
        [SerializeField]
        private int dropIndex = 0;
        [SerializeField]
        private int rotationInteractionIndex = 0;
        [SerializeField]
        private int miscInteractionIndex = 0;
        [SerializeField]
        private bool hasShotDrawerRay = false;
        [SerializeField]
        private bool isHolding = false;

        // Use this for initialization, interact and genTimer can be found anytime so this is okay 
        void Start()
        {
            genTimer = GetComponent<GeneralTimer>();
            interact = GetComponentInChildren<Interact>();
        }

        void OnEnable()
        {
            ProcessTrajectory.OnRestartEvent += ResetIndices;

            ProcessTrajectory.OnNewTrajEvent += GetNewPlayData;
            ProcessTrajectory.OnNewTrajEvent += GetInteractionLists;
        }

        void OnDisable()
        {
            ProcessTrajectory.OnRestartEvent -= ResetIndices;

            ProcessTrajectory.OnNewTrajEvent -= GetNewPlayData;
            ProcessTrajectory.OnNewTrajEvent -= GetInteractionLists;
        }

        #region Grab(Base) Interactions 

        /// <summary>
        /// If it is time to pick something up, shoot a ray, checking if the item you want to pick up is the one that elmer is supposed to pick up
        /// If interpolation causes the pickup to never happen in the time frame, skip the drop index 
        /// </summary>
        /// <param name="diosIndex"></param>
        private void CheckForGrabs(int diosIndex)
        {
            try
            {
                if (grabTimes[grabIndex] == genTimer.genTimer && interact.currentAction == InteractStates.isNotInteracting) // if it is time to pick something up 
                {
                    interact.Freeze(playData.dios[diosIndex].objName);

                    if (interact.currentAction != InteractStates.isNotInteracting)
                    {
                        isHolding = true;
                        grabIndex += 1;
                    }

                    else if (isHolding == false)
                    {
                        dropIndex += 1;
                    }
                }
                else if (grabTimes[grabIndex] < genTimer.genTimer)
                {
                    ForceGrab(playData.dios[diosIndex].objName);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        /// <summary>
        /// If it is time to drop something, keep trying to drop w.e it is that is being held. 
        /// If the time passes for an item to be safely dropped- forcefully unparent the item 
        /// </summary>
        private void CheckForDrops()
        {
            try
            {
                if (dropTimes[dropIndex] <= genTimer.genTimer && isHolding == true) // if it is time for a drop try to drop it
                {
                    interact.ShootRay();
                    if (interact.currentAction == InteractStates.isNotInteracting)
                    {
                        isHolding = false;
                        dropIndex += 1;

                    }
                    if ((dropIndex == dropTimes.Count - 1))
                    {
                        // Following lines are attempting to drop the item normally at the time the item is meant to be dropped - and if successfully droppped then incremented 
                        interact.ShootRay();
                        if (interact.currentAction == InteractStates.isNotInteracting)
                        {
                            isHolding = false;
                            dropIndex += 1;

                        }
                    }
                }

            }
            catch (System.ArgumentOutOfRangeException)
            {

            }
        }

        /// <summary>
        /// If bot is holding something, drop it and try to pickup objName
        /// </summary>
        /// <param name="objName">name of the obj you want the bot to grab in the scene </param>
        private void ForceGrab(string objName)
        {
            if (interact.currentAction == InteractStates.isHolding)
            {
                ForceDrop();
            }
            interact.Freeze(objName);
            grabIndex += 1;
            isHolding = true;
        }

        /// <summary>
        /// If the bot is holding something - drop it - increment the drop index and set isholding to false 
        /// </summary>
        private void ForceDrop()
        {
            if (interact.currentAction == InteractStates.isHolding)
            {
                interact.Drop();
                dropIndex += 1;
                isHolding = false;
            }

        }

        #endregion

        #region Misc Interactions On/Off

        /// <summary>
        ///  misc interactions list is not needed for this script- the playdata has a array with the seconds already 
        ///  Just check the index of the misc interaction and get the seconds and compare to timer, if the time is right shoot the ray 
        ///  Also if the screen interaction fails - and elmer happens to pick up an object - forcefully drop it 
        /// </summary>
        private void MiscItemCheck()
        {
            if (playData.miscInteractions != null && miscInteractionIndex < playData.miscInteractions.Length)
            {
                var miscInteractionArr = playData.miscInteractions;
                if (miscInteractionArr[miscInteractionIndex].second == genTimer.genTimer)
                {
                    if (interact.currentAction == InteractStates.isHolding)
                    {
                        ForceDrop();
                    }
                    var OnOffItem = GameObject.Find(miscInteractionArr[miscInteractionIndex].interactedObject);
                    interact.selectedItem = OnOffItem;
                    Interact.InvokeTurnOnInteraction();
                    interact.selectedItem = null;
                    Debug.Log("Tried to turn on item ");

                    miscInteractionIndex += 1;

                }
            }
        }
        #endregion
        #region Rotations
        /// <summary>
        /// A rotations list is not needed in this script, bc the playData already has a single list with all of the times that rotations occurred. 
        /// As a result, all we need is an index counter to increment whenever one roration is completed
        /// Checks the index of the rotations list in playdata and if the time is equal to the time on the general timer, invokes the rotation event 
        /// </summary>
        private void RotateItemCheck()
        {
            if (playData.rRotations != null && rotationInteractionIndex < playData.rRotations.Length)
            {
                if (playData.rRotations[rotationInteractionIndex] == genTimer.genTimer)
                {
                    Interact.InvokeRotation();
                    rotationInteractionIndex += 1;
                }
            }

        }
        #endregion

        #region Drawer Interactions

        /// <summary>
        /// checks if the player was interacting with a drawer, from the running playData. and updates the position of the drawer if the state is pulling
        /// drawers does not have a drawer index bc i am just always checking if a drawer is being interacted with - and if so - updating the position of it from pdata
        /// Note that a drawer only need to be interacted with once, to make it the selected item, and then interacted with once again to release it as the selected item
        /// </summary>
        /// <param name="diosIndex"></param>
        public void DrawerInteractionCheck(int diosIndex)
        {
            try
            {
                // if the current state ispulling - and the next state is pulling , shoot a ray to grab the drawer and then manipulate its position
                if (playData.dios[diosIndex].playerInteractState == InteractStates.isPulling && playData.dios[diosIndex + 1].playerInteractState == InteractStates.isPulling)
                {
                    if (hasShotDrawerRay == false)
                    {
                        interact.selectedItem = GameObject.Find(playData.dios[diosIndex].objName);
                        interact.currentAction = InteractStates.isPulling;
                        hasShotDrawerRay = true;

                    }
                    var next_dios = playData.dios[diosIndex + 1];
                    ManipulateObjectPositionRotation(interact.selectedItem, next_dios.objPosition, next_dios.objRotation);

                }
                //IF THE NEXT STATE IS NOT INTERACTING - shoot ray to release drawer - logic should be fine bc only way you would be pulling is if ur holding onto a drawer
                else if (playData.dios[diosIndex].playerInteractState == InteractStates.isPulling && playData.dios[diosIndex + 1].playerInteractState == InteractStates.isNotInteracting)
                {
                    interact.ShootRay();
                }
                else // for any other case like basic interactions- just a safety to always say the drawer ray has not been shot and that no drawer is being interacted with 
                {
                    hasShotDrawerRay = false;
                }
            }

            catch (NullReferenceException)
            {
                hasShotDrawerRay = false;
            }

        }

        /// <summary>
        /// Updated the position and rotation of the indicated item - should be the selecteditem that is being held (really for drawer interactions )
        /// </summary>
        /// <param name="selectedItem">item whose position is being updated </param>
        /// <param name="goalPosition">goal position of the item</param>
        /// <param name="goalRotation">goal rotation of the item</param>
        private void ManipulateObjectPositionRotation(GameObject selectedItem, Vector3 goalPosition, Quaternion goalRotation)
        {
            if (interact.currentAction == InteractStates.isHolding)
            {
                ForceDrop();
            }
            if (selectedItem != null)
            {
                selectedItem.transform.position = Vector3.Lerp(selectedItem.transform.position, goalPosition, 1f);
                selectedItem.transform.rotation = Quaternion.Slerp(selectedItem.transform.rotation, goalRotation, 1f);
            }

        }
        #endregion

        #region All Functions Accesser
        /// <summary>
        /// Calls for all the interaction checks - if something is to be grabbed, dropped, rotated, pulled, or turned on/off 
        /// Purpose is to be called from BotMovement in ElmerUpdater Coroutine - its shitty design I know - idk how to code nicely 
        /// </summary>
        /// <param name="diosIndex"></param>
        public void AllInteractionChecks(int diosIndex)
        {
            CheckForGrabs(diosIndex);
            CheckForDrops();
            RotateItemCheck();
            DrawerInteractionCheck(diosIndex);
            MiscItemCheck();
        }
        #endregion

        #region Helper Reset functions
        /// <summary>
        /// When a reset is called- resets all the indices and other things in preperation for a new trajectory recreation 
        /// </summary>
        private void ResetIndices()
        {
            grabIndex = 0;
            dropIndex = 0;
            rotationInteractionIndex = 0;
            miscInteractionIndex = 0;
            hasShotDrawerRay = false;
            isHolding = false;
        }

        public void GetNewPlayData(string newTraj)
        {
            playData = GetComponent<ProcessTrajectory>().playData;
        }

        private void GetInteractionLists(string newTraj)
        {
            var pt = GetComponent<ProcessTrajectory>();
            grabTimes = pt.grabTimes;
            dropTimes = pt.dropTimes;
        }
        #endregion
    }
}