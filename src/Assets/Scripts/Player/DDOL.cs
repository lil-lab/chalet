using UnityEngine;
using System.Collections;

namespace Cyan
{
    /// <summary>
    /// Don't Destroy on Load when loading new scenes. Attached to FPS Controller
    /// </summary>
    public class DDOL : MonoBehaviour
    {

        public static DDOL instance;

        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

    }
}
