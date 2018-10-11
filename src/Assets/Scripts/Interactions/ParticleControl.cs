using UnityEngine;
using System.Collections;

namespace Cyan
{
    /// <summary>
    /// Turns particle effects on and off 
    /// </summary>
    public class ParticleControl : MonoBehaviour
    {

        public bool isOff;
        ParticleSystem particles;

        // Use this for initialization
        void Start()
        {
            try
            {
                particles = GetComponentInChildren<ParticleSystem>();
            }
            catch
            {
                Debug.Log("Couldn't find particle system");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isOff)
            {
                particles.gameObject.SetActive(false);
            }
            else
            {
                particles.gameObject.SetActive(true);

            }
        }
    }
}
