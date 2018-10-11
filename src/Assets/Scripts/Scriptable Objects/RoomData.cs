using UnityEngine;
using System.Collections.Generic;

namespace Cyan
{
    /// <summary>
    /// Scriptable object that holds name of object, vector3 spawn location, quaternion rotation and scale of objects in a room.
    /// Also holds list of objects that have been removed from the room- so that upon reentry, can be accessed for deactivation 
    /// </summary>
    [CreateAssetMenu()]
    public class RoomData : ScriptableObject
    {
        #region Scene Management
        public IList<string> objsinRoom = new List<string>();
        public IList<Vector3> objSpawns = new List<Vector3>();
        public IList<Quaternion> objSpawnRotations = new List<Quaternion>();
        public IList<Vector3> objScale = new List<Vector3>();

        public IList<InteractiveObjectDetails> sceneManagement = new List<InteractiveObjectDetails>();

        public IList<string> deactivatedObjects = new List<string>();
        #endregion

        public Dictionary<string, DrawerDetails> drawerDict;

        public IList<OnOffObjects> onOffList = new List<OnOffObjects>();
        public IList<TextureObjects> changedTexturesList = new List<TextureObjects>();

        /// <summary>
        /// Clear the contents of all the 
        /// </summary>
        public void ClearRoomData()
        {
            objsinRoom.Clear();
            objSpawns.Clear();
            objSpawnRotations.Clear();
            objScale.Clear();
            onOffList.Clear();
            changedTexturesList.Clear();
        }

    }
}