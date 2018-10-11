using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cyan
{

    /// <summary>
    /// Attached to player. Keeps track of previous rooms and catalogues of each room's object placement
    /// </summary>
    public class RoomIdentifier : MonoBehaviour
    {
        public Room lastRoom;
        public RoomData livingRoomData;
        public RoomData kitchenData;
        public RoomData bathroomData;
        public RoomData bedroomData;
        public RoomData guestroomData;
        public RoomData hallwayData;

        public bool isLoadingRoom = false;

        /// <summary>
        /// Gets the RoomData Scriptable Object associated with a certain room type
        /// </summary>
        /// <param name="room">Type of room</param>
        /// <returns>Associated RoomData ScriptableObject for room type</returns>
        public RoomData GetAssociatedRoomData(Room room)
        {
            if (room == Room.livingRoom)
                return livingRoomData;
            else if (room == Room.kitchen)
                return kitchenData;
            else if (room == Room.bathroom)
                return bathroomData;
            else if (room == Room.guestroom)
                return guestroomData;
            else if (room == Room.hallway)
                return hallwayData;
            else
                return bedroomData;
        }
    }
}
