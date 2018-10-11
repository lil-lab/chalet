using UnityEngine;
using System.Collections;
using Cyan.Elmer;

namespace Cyan
{
    public class GeneralTimer : MonoBehaviour
    {
        public static float TimeInterval = .2f;
        [SerializeField]
        private float generalTimer = 0f;
        public float genTimer { get { return generalTimer; } }
        private bool timerRunning = false;


        void Awake()
        {
            timerRunning = true;
            StartCoroutine("timer");
        }

        void OnEnable()
        {
            ProcessTrajectory.OnNewTrajEvent += RestartTimer;
            ProcessTrajectory.OnRestartEvent += RestartTimer;
            ProcessTrajectory.OnRestartEvent += PauseTimer;
        }

        void OnDisable()
        {
            ProcessTrajectory.OnNewTrajEvent -= RestartTimer;
            ProcessTrajectory.OnRestartEvent -= RestartTimer;
            ProcessTrajectory.OnRestartEvent -= PauseTimer;


            StopCoroutine("timer");
            timerRunning = false;
        }

        private IEnumerator timer()
        {
            while (timerRunning)
            {
                yield return new WaitForSeconds(TimeInterval);
                generalTimer += TimeInterval;
            }

        }

        public void PauseTimer()
        {
            StopCoroutine("timer");
            timerRunning = false;
        }

        public void ResumeTimer()
        {
            StartCoroutine("timer");
            timerRunning = true;
        }

        public void RestartTimer()
        {
            if (timerRunning == true)
            {
                StopCoroutine("timer");
                timerRunning = false;
            }
            generalTimer = 0f;
            timerRunning = true;
            StartCoroutine("timer");
        }

        private void RestartTimer(string traj)
        {
            RestartTimer();
        }

        private void PauseTimer(string traj)
        {
            PauseTimer();
        }

    }
}
