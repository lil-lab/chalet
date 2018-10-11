using UnityEngine;

namespace Cyan
{
    public class WebGLInputManager : MonoBehaviour
    {
        public bool loaded = false;

        void Awake()
        {
            if (loaded == false)
            {
                try
                {
                    Application.ExternalCall("WebGLLoaded");
                    loaded = true;
                }
                catch
                {

                }

            }
        }
        /// <summary>
        /// Funcion to access from browser to disable keyboard and mouse input for webgl
        /// </summary>
        public void DisableWebGLInput()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = false;
#endif
            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Function to access from browser to enable keyboard and mouse input for webGL
        /// </summary>
        public void EnableWebGLInput()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
