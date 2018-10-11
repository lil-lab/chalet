using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Cyan.Elmer
{
    /// <summary>
    /// Manipulates the bot's movement for recreating actions 
    /// </summary>
    public class BotMovement : MonoBehaviour
    {
        public Camera playerCamera;
        private float recordingWaitTime = .2f;
        private PData playData;
        private GeneralTimer genTimer;
        private BotInteraction botInteraction;

        // Starting Locations/Rotations
        private Vector3 pStart;
        private Quaternion rStart;
        private Quaternion camStart;

        // Target Locations/Rotations
        private Vector3 pTarget;
        private Quaternion rTarget;
        private Quaternion camTarget;

        //this is for elmers movement updating to get percentage completion to goal position 
        private float timeStarted;
        public bool isExecuting = false;
        [SerializeField]
        public int diosIndex = 0;
        [SerializeField]
        public int sceneSwitchIndex = 2; // 2 because base scene load is 0. default living room load is 1, so next scene is 2

        void OnEnable()
        {
            genTimer = GetComponent<GeneralTimer>();
            botInteraction = GetComponent<BotInteraction>();
            ProcessTrajectory.OnRestartEvent += ResetIndices;

            ProcessTrajectory.OnNewTrajEvent += GetPlayData;
            ProcessTrajectory.OnNewTrajEvent += ElmerHold;
        }

        void OnDisable()
        {
            ProcessTrajectory.OnRestartEvent -= ResetIndices;

            ProcessTrajectory.OnNewTrajEvent -= GetPlayData;
            ProcessTrajectory.OnNewTrajEvent -= ElmerHold;
        }

        /// <summary>
        /// This function is registered to the OnNewTrajEvent.
        /// When a new trajectory is uploaded, bot movement waits until the trajectory has been processed to proceed with interpoloatin and updating actions 
        /// </summary>
        /// <param name="traj"></param>
        private void ElmerHold(string traj)
        {
            Debug.Log("Holding Elmer");
            StartCoroutine(WaitForTraj());
        }

        /// <summary>
        /// Waits for a new trajectory to be uploaded
        /// When a new traj is uploaded, then start start doing all the updating of elmer values for the recreation 
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForTraj()
        {
            isExecuting = false;
            yield return new WaitUntil(() => playData.dios != null);
            Debug.Log("Hold Released");
            recordingWaitTime = playData.dios[1].time - playData.dios[0].time;
            GeneralTimer.TimeInterval = recordingWaitTime;
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            StartCoroutine(ElmerUpdater());

        }

        /// <summary>
        /// Once ElmerPositionUpdater is called, bot elmer's goal destination will be updated every second that there is data 
        /// Updated goal values: bot location, bot rotation and camera rotation
        /// </summary>
        /// <returns></returns>
        private IEnumerator ElmerUpdater()
        {
            isExecuting = true;
            while (genTimer.genTimer < playData.dios[playData.dios.Length - 1].time - 1)
            {
                //Debug.LogFormat("Time {0}, pldata time{1}", genTimer.genTimer, playData.dios[diosIndex].time);
                yield return new WaitForSeconds(recordingWaitTime);
                var dios = playData.dios[diosIndex];
                diosIndex += 1;

                pStart = transform.position;
                pTarget = dios.playerLocation;

                rStart = transform.rotation;
                rTarget = dios.playerRotation;

                camStart = playerCamera.transform.rotation;
                camTarget = dios.cameraRotation;

                botInteraction.AllInteractionChecks(diosIndex);
                timeStarted = Time.time;
            }
            isExecuting = false;
            genTimer.PauseTimer();

        }

        /// <summary>
        /// In Lateupdate, moves the bot to its goal destination that is updated at every time collection interval  
        /// </summary>
        private void UpdateBotPosition()
        {
            if (isExecuting == true)
            {
                var timeSinceStarted = Time.time - timeStarted;
                var percentageComplete = timeSinceStarted / GeneralTimer.TimeInterval;
                if (percentageComplete < 1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, rTarget, Time.maximumDeltaTime * percentageComplete);
                    playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, camTarget, Time.maximumDeltaTime * percentageComplete);
                    transform.position = Vector3.Lerp(transform.position, pTarget, Time.maximumDeltaTime * percentageComplete);
                    botInteraction.DrawerInteractionCheck(diosIndex);
                }

            }


        }
        void FixedUpdate()
        {
            UpdateBotPosition();
        }

        #region Helper Functions/ incrementing/resetting
        /// <summary>
        /// Increments the sceneswitch index by 1
        /// </summary>
        public void IncrementSceneSwitchIndex()
        {
            sceneSwitchIndex += 1;
        }

        private void ResetIndices()
        {
            diosIndex = 0;
            sceneSwitchIndex = 0;
            isExecuting = false;
        }

        private void GetPlayData(string traj)
        {
            playData = GetComponent<ProcessTrajectory>().playData;
        }
        #endregion


    }
}