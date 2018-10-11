using UnityEngine;

namespace Cyan
{
    /// <summary>
    /// Catches players and items that fall through floors and respawns them at a spawn point
    /// </summary>
    public class SafetyNet : MonoBehaviour
    {
        private RoomManager rm;

        void OnTriggerEnter(Collider c)
        {
            if (c.tag == "Player")
            {
                rm.SpawnPlayer();
                Debug.Log("RespawnedPLayer");
            }
            else
            {
                c.transform.position = rm.transform.position;
            }
        }

        // Use this for initialization
        void Start()
        {
            rm = FindObjectOfType<RoomManager>();
        }

    }
}
