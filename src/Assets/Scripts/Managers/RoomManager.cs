using UnityEngine;
using System.Collections.Generic;

namespace Cyan
{
    /// <summary>
    /// Attached to each scene/room. When the room is loaded, items are placed/instaitated according to room data for room and player is placed in the spawn corresponding to the room that they last came from.
    /// If the room has been never been visited, scene will load normally without placing items. 
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public RoomIdentifier player;
        public GameObject fpsController;

        public Room currentRoom;
        private Room lastRoom;
        private RoomData thisRoomData;

        public GameObject spawns;
        private Transform[] spawnPoints;

        public PrefabDB prefabsDB;

        /// <summary>
        /// When the scene is loaded, place all of the items based on room data for loaded room.
        /// Place the player in front of the door of the room that they came from
        /// </summary>
        void OnEnable()
        {
            //fpsController = FindObjectOfType<FirstPersonController>();
            fpsController = GameObject.FindGameObjectWithTag("Player");
            player = FindObjectOfType<RoomIdentifier>();
            lastRoom = player.lastRoom;
            thisRoomData = player.GetAssociatedRoomData(currentRoom);

            spawns = GameObject.Find("SpawnPoints");

            spawnPoints = spawns.GetComponentsInChildren<Transform>();

            SetRoom();
            SpawnPlayer();


        }

        /// <summary>
        /// Depending on which room the player is in, sets the objects in place or instantiates new ones if they were brought in from another room
        /// </summary>
        private void SetRoom()
        {
            placeObjects(thisRoomData);
            DeactivateRemoved();
            TurnOnOff();

        }

        #region Player Management
        /// <summary>
        /// Spawn the player in front of the door to mimic that they just came from the last room
        /// note that the rooms are all lowercased- don't worry about casing when setting spawns. it could be all caps doesnt matter bc its lowered later 
        /// </summary>
        public void SpawnPlayer()
        {
            switch (lastRoom)
            {
                case Room.livingRoom:
                    SpawnInPoint("livingroom");
                    break;
                case Room.bathroom:
                    SpawnInPoint("bath");
                    break;
                case Room.bedroom:
                    SpawnInPoint("bedroom");
                    break;
                case Room.kitchen:
                    SpawnInPoint("kitchen");
                    break;
                case Room.guestroom:
                    SpawnInPoint("guestroom");
                    break;
                case Room.hallway:
                    SpawnInPoint("hallway");
                    break;
                case Room.diningroom:
                    SpawnInPoint("dining");
                    break;
                default:
                    fpsController.transform.position = spawnPoints[1].transform.position;
                    fpsController.transform.Rotate(spawnPoints[1].transform.rotation.eulerAngles);
                    break;
            }
        }

        /// <summary>
        /// spawns player at point indicated
        /// </summary>
        /// <param name="room">spawn point, should be "livingroom", "bathroom", "bedroom","kitchen"</param>
        private void SpawnInPoint(string room)
        {

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].name.ToLower().Contains(room))
                {
                    fpsController.transform.position = spawnPoints[i].transform.position;
                    fpsController.transform.Rotate(spawnPoints[i].transform.rotation.eulerAngles);

                }

            }
        }
        #endregion

        #region Prop Management

        /// <summary>
        /// Room manager goes through the associated room's roomData and either moves existing objects
        /// or instantiates new ones to mimic the objects being brought from room to room
        /// </summary>
        /// <param name="roomData"></param>
        private void placeObjects(RoomData roomData)
        {
            for (int i = 0; i < roomData.objsinRoom.Count; i++)
            {
                try
                {
                    GameObject objInScene = GameObject.Find(roomData.objsinRoom[i]);
                    objInScene.transform.position = roomData.objSpawns[i];
                    objInScene.transform.rotation = roomData.objSpawnRotations[i];
                    objInScene.transform.localScale = roomData.objScale[i];
                }
                catch
                {
                    GameObject objFromOtherRoom = prefabsDB.getPrefab(roomData.objsinRoom[i]);
                    var instantiatedPrefab = Instantiate(objFromOtherRoom, roomData.objSpawns[i], roomData.objSpawnRotations[i]) as GameObject;
                    instantiatedPrefab.transform.localScale = roomData.objScale[i];
                }
            }

        }
        /// <summary>
        /// if an item was removed from the current room previously, it will be active in the scene on reload, so removed items are deactivated
        /// </summary>
        private void DeactivateRemoved()
        {
            IList<string> deactivateObjs = new List<string>();
            deactivateObjs = thisRoomData.deactivatedObjects;

            foreach (string removed in deactivateObjs)
            {
                try
                {
                    GameObject removedObj = GameObject.Find(removed);
                    removedObj.SetActive(false);
                }
                catch
                {
                    //Debug.Log("Item didnt belong in room");
                }
                finally
                {

                }

            }
        }

        /// <summary>
        /// Checks the associated roomData for current room and turns particle effects on/off 
        /// depending on the state they were left in 
        /// </summary>
        private void TurnOnOff()
        {
            if (thisRoomData.onOffList.Count > 0 || thisRoomData.changedTexturesList.Count > 0)
            {
                var pEffects = FindObjectsOfType<ParticleControl>();
                var screens = FindObjectsOfType<TVManager>();

                for (int i = 0; i < pEffects.Length; i++)
                {
                    pEffects[i].isOff = thisRoomData.onOffList[i].isOff;
                }
                foreach (TVManager screen in screens)
                {
                    for (int i = 0; i < thisRoomData.changedTexturesList.Count; i++)
                    {
                        if (screen.name == thisRoomData.changedTexturesList[i].objName)
                        {
                            screen.isOff = thisRoomData.changedTexturesList[i].isOff;
                            screen.GetComponent<Renderer>().material = thisRoomData.changedTexturesList[i].material;
                            break;
                        }
                    }
                }
            }

        }
        #endregion


    }
}
