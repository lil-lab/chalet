using UnityEngine;
using System.Collections;

namespace Cyan
{
    public class TVManager : MonoBehaviour
    {
        public Material[] screenMaterials;
        public new Renderer renderer;
        public bool isOff = true;

        // Use this for initialization
        void OnEnable()
        {
            renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Changes material of screen to mimic changing channels 
        /// </summary>
        public void ChangeScreen()
        {
            if (isOff)
            {
                renderer.material = screenMaterials[1];
                isOff = true;
            }
            else
            {
                renderer.material = screenMaterials[0];
                isOff = false;
            }
        }
    }

}
