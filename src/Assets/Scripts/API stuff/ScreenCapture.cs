using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cyan {
    public class ScreenCapture : MonoBehaviour {
        private IList<string> pixelList = new List<string>();

        /// <summary>
        /// Get a capture of the window and append pixels to a list
        /// </summary>
        public void WindowCapture()
        {
            pixelList.Clear();
            StartCoroutine("GrabPixels");
            
        }

        /// <summary>
        /// Convert the list to a string and return the string of pixels
        /// </summary>
        /// <returns>string of pixels of the window capture </returns>
        public string GetPixelString()
        {
            var pixelString = string.Join(",", pixelList.ToArray());
            return pixelString;
        }

        /// <summary>
        /// Coroutine to capture an image of the window and append the pixels of the texture to a list 
        /// </summary>
        /// <returns></returns>
        private IEnumerator GrabPixels()
        {
            yield return new WaitForEndOfFrame();
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
            tex.Apply();
            var pixelList = new List<string>();

            foreach (Color pix in tex.GetPixels())
            {
                pixelList.Add(pix.ToString());
            }
        }
    }
}