using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

namespace Cyan
{
    /// <summary>
    /// Controls where the player indicator goes on the map
    /// </summary>
    public class MapControl : MonoBehaviour
    {
        public RoomManager roomManager;
        public GameObject HouseLayout_1;
        public GameObject HouseLayout_2;
        public GameObject HouseLayout_3;


        private Image[] firstHousepieces;
        private Image[] secondHousePieces;
        private Image[] thirdHousePieces;


        public Image playerCircle;
        public Image mapLivingroom;
        public Image mapKitchen;
        public Image mapBathroom;
        public Image mapBedroom;
        public Image mapHallway;
        public Image mapGuestroom;
        public Image mapDiningroom;


        private HouseSetup houseSetup;

        void Awake()
        {
            roomManager = FindObjectOfType<RoomManager>();
            houseSetup = FindObjectOfType<HouseIdentifier>().houseSetup;
            string b = string.Empty;
            for (int i = 0; i < houseSetup.name.Length; i++)
            {
                if (Char.IsDigit(houseSetup.name[i]))
                {
                    b += (houseSetup.name[i]);
                    determineMap(Convert.ToInt32(b));
                }
            }


            setMap();
        }

        /// <summary>
        /// Determines which map to set active.
        /// House number is determined from the housesetup name number, so if new houses are added to older styles, the ranges will need to be updated
        /// </summary>
        /// <param name="housenum">number from the houseSetup name </param>
        private void determineMap(int housenum)
        {
            if (housenum > 3 && housenum <= 6)
                HouseLayout_2.SetActive(true);
            else if (housenum > 6)
                HouseLayout_3.SetActive(true);
            else
                HouseLayout_1.SetActive(true);
        }

        /// <summary>
        /// checks which room the scene is and places the indicator image over the respective room on the map. 
        /// </summary>
        private void setMap()
        {
            switch (roomManager.currentRoom)
            {
                case (Room.livingRoom):
                    playerCircle.transform.position = getAssociatedMapPiece("living").transform.position;
                    break;
                case (Room.kitchen):
                    playerCircle.transform.position = getAssociatedMapPiece("kitchen").transform.position;
                    break;
                case (Room.bathroom):
                    playerCircle.transform.position = getAssociatedMapPiece("bathroom").transform.position;
                    break;
                case (Room.bedroom):
                    playerCircle.transform.position = getAssociatedMapPiece("bedroom").transform.position;
                    break;
                case (Room.guestroom):
                    playerCircle.transform.position = getAssociatedMapPiece("guestroom").transform.position;
                    break;
                case (Room.hallway):
                    playerCircle.transform.position = getAssociatedMapPiece("hallway").transform.position;
                    break;
                case (Room.diningroom):
                    playerCircle.transform.position = getAssociatedMapPiece("dining").transform.position;
                    break;
            }
        }

        private Image getAssociatedMapPiece(string room)
        {
            Image[] mapSquares = GetComponentsInChildren<Image>();

            foreach (Image mapSquare in mapSquares)
            {
                if (mapSquare.name.ToLower().Contains(room))
                {
                    return mapSquare;
                }
            }
            return null;
        }

    }
}
