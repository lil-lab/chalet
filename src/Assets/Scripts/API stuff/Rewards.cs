using UnityEngine;
using System.Collections;

namespace Cyan
{
    public class Rewards : MonoBehaviour
    {
        /// <summary>
        /// Gives Reward
        /// </summary>
        /// <param name="action"> the action called from client</param>
        /// <param name="before">RoomData record beforeaction was taken</param>
        /// <param name="after">RoomData record after action was taken</param>
        /// <returns>some double value</returns>
        public double GiveReward(string action, RoomData before, RoomData after)
        {
            return 0;
        }

    }
}
