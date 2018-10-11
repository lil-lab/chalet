using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Cyan
{
    /// <summary>
    /// Controls the color of the player pointer 
    /// </summary>
    public class ChangeImgColor : MonoBehaviour
    {
        public Image pointerImg;
        public GameObject player;
        public Hover hover;


        void OnEnable()
        {
            hover = FindObjectOfType<Hover>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        /// <summary>
        /// Changes color of the image to cyan
        /// </summary>
        public void ColorToCyan()
        {
            pointerImg.color = new Color(0f, 1f, 1f, .7f);
        }

        /// <summary>
        /// Changes color of the image to white
        /// </summary>
        public void ColorToWhite()
        {
            pointerImg.color = new Color(1f, 1f, 1f, .7f);
        }

        void Update()
        {
            if (hover.canInteract)
            {
                ColorToCyan();
            }
            else
            {
                ColorToWhite();
            }

        }

    }
}