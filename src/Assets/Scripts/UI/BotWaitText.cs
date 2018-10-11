using UnityEngine;
using UnityEngine.UI;

namespace Cyan
{
    public class BotWaitText : MonoBehaviour
    {
        public Text waitText;
        private GameObject player;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (waitText != null && !player.name.Contains("Control"))
            {
                waitText.gameObject.SetActive(true);
            }
        }
    }
}
