using UnityEngine;

namespace Cyan
{
    /// <summary>
    /// Indicates all of the scenes that should be in each house
    /// Put the name of each room that should be in the house in their respective string places
    /// Scene values are defaulted to the first of each kind.
    /// If a house doesn't contain a room of a certain type, just leave the string at its default, it will not effect how the house is loaded
    /// Note that if a new room type is added, this scriptable object will need to be updated to check for the name of the room that needs to be loaded
    /// </summary>
    [CreateAssetMenu()]
    public class HouseSetup : ScriptableObject
    {
        [SerializeField, Header("Put the string name of each room that should be in the house")]
        public string bedroomSceneName = "bedroom";
        public string bathroomSceneName = "bathroom";
        public string LivingroomSceneName = "livingroom";
        public string KitchenSceneName = "Kitchen";
        public string GuestroomSceneName = "guestroom";
        public string HallwaySceneName = "hallway";
        public string diningroomSceneName = "diningroom";

    }
}
