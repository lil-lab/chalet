using UnityEngine;
using System.Collections;

namespace Cyan
{
    /// <summary>
    /// In the case of standalone builds, this script overwrites the current Housesetup and tells the agent which house it is and which rooms to load
    /// </summary>
    public class StandaloneHouseSetup : MonoBehaviour
    {
        private HouseIdentifier houseIdentifier;
        public HouseSetup[] houseSetups;
        public int setupsIndex = 5;
        
        void Awake()
        {
            houseIdentifier = GetComponent<HouseIdentifier>();
            houseIdentifier.houseSetup = houseSetups[setupsIndex];
        }

        public void ChangeSetup(int index)
        {
            houseIdentifier.houseSetup = houseSetups[index];

        }

    }
}
