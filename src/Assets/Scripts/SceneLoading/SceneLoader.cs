using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Cyan
{

    public enum Room { livingRoom, kitchen, bathroom, bedroom, guestroom, hallway, diningroom };

    public class SceneLoader : MonoBehaviour
    {
        //Scene Names 
        public string bedRoom = "bedroom";
        public string bathRoom = "bathroom";
        public string livingRoom = "livingroom";
        public string kitchen = "Kitchen3";
        public string guestroom = "guestroom";
        public string hallway = "hallway";
        public string diningroom = "diningroom";


        public Room loadRoom;
        public RoomIdentifier player;

        private HouseSetup houseSetup;

        void Awake()
        {
            try
            {
                houseSetup = FindObjectOfType<HouseIdentifier>().houseSetup;
                GetSceneNames();
            }
            catch
            {
                Debug.LogError("Assign a HouseSetup Scriptable Obj to agent!");
            }
            player = FindObjectOfType<RoomIdentifier>();


        }
        #region SceneNames Getter
        /// <summary>
        /// Associates scene names with different scenes in the house
        /// </summary>
        private void GetSceneNames()
        {
            bedRoom = houseSetup.bedroomSceneName;
            bathRoom = houseSetup.bathroomSceneName;
            livingRoom = houseSetup.LivingroomSceneName;
            kitchen = houseSetup.KitchenSceneName;
            guestroom = houseSetup.GuestroomSceneName;
            hallway = houseSetup.HallwaySceneName;
            diningroom = houseSetup.diningroomSceneName;

        }
        #endregion

        #region LoadScene
        /// <summary>
        /// Loads whatever room has been indicated to be loaded (in the enum dropdown, default is living room)
        /// </summary>
        /// <param name="loadRoom"></param>
        /// <returns></returns>
        public IEnumerator LoadDestination(Room loadRoom)
        {
            yield return new WaitForSeconds(1);
            AsyncOperation async;
            switch (loadRoom)
            {
                case (Room.bathroom):
                    async = SceneManager.LoadSceneAsync(bathRoom);
                    break;
                case (Room.bedroom):
                    async = SceneManager.LoadSceneAsync(bedRoom);
                    break;
                case (Room.kitchen):
                    async = SceneManager.LoadSceneAsync(kitchen);
                    break;
                case (Room.guestroom):
                    async = SceneManager.LoadSceneAsync(guestroom);
                    break;
                case (Room.hallway):
                    async = SceneManager.LoadSceneAsync(hallway);
                    break;
                case (Room.diningroom):
                    async = SceneManager.LoadSceneAsync(diningroom);
                    break;
                default:
                    async = SceneManager.LoadSceneAsync(livingRoom);
                    break;
            }
            while (!async.isDone)
            {
                yield return null;
            }
            player.isLoadingRoom = false;
        }
        #endregion

    }
}