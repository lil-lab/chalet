using UnityEngine;
using System.Collections.Generic;
using System;
using Cyan.Elmer;

namespace Cyan
{
    /// <summary>
    /// Attached to a door. On trigger- serialize all of the interactable objects for the current room and goes into the next room
    /// </summary>
    public class LoadRoom : SceneLoader
    {
        private RoomManager roomManager;
        private IList<GameObject> interactableObjects = new List<GameObject>();
        [Tooltip("The fp screen in the scene")]
        public GameObject loadingScreen;
        private new Renderer renderer;
        private GeneralTimer genTimer;


        void OnEnable()
        {
            roomManager = FindObjectOfType<RoomManager>();
            loadingScreen = GetLoadingScreen();
            genTimer = FindObjectOfType<GeneralTimer>();
        }

        /// <summary>
        /// When a player enters the door:
        /// 1. Makes a list of all objects
        /// 2. Adds Drawers/cabinets to list
        /// 3. If a player is holding an item, removes item from interactable objects list
        /// 4. Adds all of the objects, their position, rotation and scale to the current room's roomdata scriptable object
        /// 5. Activate Loading screen
        /// 6. Tries to reset the player's rotation (currently doesn't work)
        /// 7. Loads next room (scene)
        /// </summary>
        /// <param name="c"></param>
        void OnTriggerEnter(Collider c)
        {
            if (c.name.Contains("Elmer"))
            {

                var pdata = c.GetComponent<ProcessTrajectory>().playData;
                var elmer = c.GetComponent<BotMovement>();

                try
                {
                    if (Math.Abs(pdata.sceneSwitches[elmer.sceneSwitchIndex] - genTimer.genTimer) <= 1.1f)
                    {
                        elmer.IncrementSceneSwitchIndex();
                        LoadNextScene(c, loadRoom);
                    }

                }
                catch (System.IndexOutOfRangeException)
                {
                }

            }

            else
            {
                LoadNextScene(c, loadRoom);
            }
        }

        public void LoadNextScene(Collider c, Room loadRoom)
        {
            if (c.tag == "Player" && !player.isLoadingRoom)
            {
                player.lastRoom = roomManager.currentRoom;
                Debug.Log("Load Room");
                player.isLoadingRoom = true;
                interactableObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("object"));
                AddDrawerstoList();
                RemoveHeldObject(c);
                AddDataForCurrentRoom(interactableObjects);
                loadingScreen.SetActive(true);
                c.gameObject.transform.Rotate(0f, 0f, 0f);
                StartCoroutine(LoadDestination(loadRoom));
                Input.ResetInputAxes();
                player.isLoadingRoom = false;
            }
        }


        #region RoomData recording functions
        /// <summary>
        ///  Checks FPSController for children gameobjects with "object" tag and removes it from interactableobjects scriptable object and also adds it to list of items that need to be deactivated upon reentry 
        /// </summary>
        /// <param name="FPSController"> player that enters trigger</param>
        private void RemoveHeldObject(Collider FPSController)
        {
            Transform[] children = FPSController.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.tag == "object")
                {
                    interactableObjects.Remove(child.gameObject);
                    AddToDeactivateList(child.gameObject);

                }
            }

            Rigidbody fpsBody = FPSController.gameObject.GetComponent<Rigidbody>();
            fpsBody.constraints = RigidbodyConstraints.FreezeAll;
        }
        /// <summary>
        /// Clears Room Data for current room, to replace with most recent data 
        /// For all the items in the list of interactableobjects list, add the gameobject's name, vector3 position, quaternion, and scale to the current room's RoomData 
        /// </summary>
        /// <param name="objs"></param>
        private void AddDataForCurrentRoom(IList<GameObject> objs)
        {
            var thisRoomData = player.GetAssociatedRoomData(roomManager.currentRoom);
            CatalogueRoom(thisRoomData, objs);
        }


        /// <summary>
        /// If a player is holding an object, add it to list of items that need to be deactivated upon rentry to the particular room.
        /// </summary>
        /// <param name="removedObj">item that player is holding</param>
        private void AddToDeactivateList(GameObject removedObj)
        {
            var thisRoomData = player.GetAssociatedRoomData(roomManager.currentRoom);
            thisRoomData.deactivatedObjects.Add(removedObj.name);
        }

        /// <summary>
        /// Resets the roomData scriptable obj for associated room. Adds in new objs and their placements
        /// </summary>
        /// <param name="roomData">associated roomData scriptable object</param>
        /// <param name="objs"> objs that are in current room that need to be catalogued</param>
        private void CatalogueRoom(RoomData roomData, IList<GameObject> objs)
        {

            roomData.ClearRoomData();
            foreach (GameObject obj in objs)
            {
                AddRoomData(roomData, obj);
            }

            AddToOnOffList(roomData);

        }

        /// <summary>
        /// Adds all of the objs in room that need to be catalogied into associated roomData. 
        /// Details added: name, position, rotation, scale 
        /// This information is required by the roomManager to recreate the scene upon rentry
        /// </summary>
        /// <param name="roomData"></param>
        /// <param name="obj"></param>
        private void AddRoomData(RoomData roomData, GameObject obj)
        {
            roomData.objsinRoom.Add(obj.name);
            roomData.objSpawns.Add(obj.transform.position);
            roomData.objSpawnRotations.Add(obj.transform.rotation);
            roomData.objScale.Add(obj.transform.localScale);
        }

        /// <summary>
        /// Adds all drawers/cabinets to the list of interactable items' positions/rotations for room preservation
        /// </summary>
        private void AddDrawerstoList()
        {
            var drawers = new List<GameObject>(GameObject.FindGameObjectsWithTag("object-openable"));
            foreach (GameObject drawer in drawers)
            {
                interactableObjects.Add(drawer);
            }
        }

        /// <summary>
        /// Items that can be turned "on" and "off" are documented for roomManager to recreate scene
        /// Particle Control - items that have particle effects and simply turn on and off
        /// TV Manager- currently TV only has one "channel", but in the future may have multiple channels 
        /// </summary>
        /// <param name="roomData">records are stored to associated RoomData</param>
        private void AddToOnOffList(RoomData roomData)
        {
            var particleObjs = FindObjectsOfType<ParticleControl>();
            var textureChangeObjs = FindObjectsOfType<TVManager>();

            try
            {
                foreach (ParticleControl particleObj in particleObjs)
                {
                    var pO = new OnOffObjects();
                    pO.objName = particleObj.name;
                    pO.isOff = particleObj.isOff;
                    roomData.onOffList.Add(pO);
                }

                foreach (TVManager tO in textureChangeObjs)
                {
                    var texturedObj = new TextureObjects();
                    texturedObj.objName = tO.name;
                    texturedObj.material = tO.renderer.material;
                    roomData.changedTexturesList.Add(texturedObj);
                }

            }
            catch
            {

            }
        }

        #endregion
        /// <summary>
        /// Gets the loading screen which is an inactive child in the FPS screen set 
        /// </summary>
        /// <returns></returns>
        private GameObject GetLoadingScreen()
        {
            var UIscreen = GameObject.Find("FPS Screen");
            var UIScreenChildren = UIscreen.GetComponentInChildren<Transform>();
            foreach (Transform screen in UIScreenChildren)
            {
                if (screen.name == "Loading Screen")
                {
                    return screen.gameObject;
                }
            }
            return null;
        }
    }
}
