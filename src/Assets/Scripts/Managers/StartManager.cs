using UnityEngine;
using System.Collections;
using Cyan.Elmer;

namespace Cyan
{
    public class StartManager : SceneLoader
    {
        private ProcessTrajectory p;

        void OnEnable()
        {
            if (player.transform.parent.name.Contains("Control"))
            {
                StartCoroutine(LoadDestination(loadRoom));
            }

            else
                StartCoroutine(WaitForTrajectory());

            //StartCoroutine(LoadDestination(loadRoom));
        }

        /// <summary>
        /// Coroutine holds on the base scene until a new trajectory has been recieved to be processed
        /// </summary>
        /// <param name="gotTraj">bool for if a new tajectory was accessed </param>
        /// <returns></returns>
        private IEnumerator WaitForTrajectory()
        {
            p = FindObjectOfType<ProcessTrajectory>();
            yield return new WaitUntil(() => p.gotTrajectory == true);
            StartCoroutine(LoadDestination(loadRoom));
        }

    }
}