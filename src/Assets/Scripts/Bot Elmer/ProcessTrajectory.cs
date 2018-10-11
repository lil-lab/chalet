using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Cyan.Elmer
{
    /// <summary>
    /// Takes a trajectory and does all the basic parsing needed for interactions
    /// </summary>
    public class ProcessTrajectory : MonoBehaviour
    {
        #region TESTING
        //JSON stuff
        public TextAsset trajectoryFile;
        private string JSONTrajectory;
        #endregion

        [Tooltip("playData holds the trajectory for the Bot")]
        public PData playData = new PData();

        // All the interactions lists
        private List<float> objectInteractionTimes = new List<float>();
        [Tooltip("Holds all the times that the bot grabs an item")]
        public List<float> grabTimes = new List<float>();
        [Tooltip("Holds all the times that the bot drops an item")]
        public List<float> dropTimes = new List<float>();

        public bool gotTrajectory = false;

        #region Events 

        public delegate void NewTrajHandler(string traj);
        public static event NewTrajHandler OnNewTrajEvent;

        public delegate void OnRestartHandler();
        public static event OnRestartHandler OnRestartEvent;

        public static void InvokeNewProcessing(string trajectory)
        {
            if (SceneManager.GetActiveScene().name == "Base")
                OnNewTrajEvent(trajectory);
            else
                Debug.LogWarning("Need to be in Base Scene");
        }

        public static void InvokeRestart()
        {
            OnRestartEvent();
        }
        #endregion

        #region TESTING

        // Use this for initialization
        //void Awake()
        //{
        //    JSONTrajectory = trajectoryFile.text;
        //}
        #endregion

        private void OnEnable()
        {
            OnNewTrajEvent += RegisterBaseInteractions;

            OnRestartEvent += ClearInteractions;
            OnRestartEvent += LoadBase;
        }

        private void OnDisable()
        {
            OnNewTrajEvent -= RegisterBaseInteractions;

            OnRestartEvent -= ClearInteractions;
            OnRestartEvent -= LoadBase;
        }

        #region Registering Interactions 
        /// <summary>
        /// Taken from original Register Base interaction, but instead takes the new string trajectory from the events
        /// </summary>
        /// <param name="trajectory"></param>
        private void RegisterBaseInteractions(string trajectory)
        {
            gotTrajectory = true;
            Debug.Log(gotTrajectory);
            try
            {
                playData = JsonUtility.FromJson<PData>(trajectory);
                var lastState = InteractStates.isNotInteracting;

                foreach (PData.DataInOneSecond dios in playData.dios)
                {
                    if (lastState != dios.playerInteractState && dios.playerInteractState != InteractStates.isPulling)
                    {
                        lastState = dios.playerInteractState;
                        objectInteractionTimes.Add(dios.time);
                    }
                }
                SplitBaseInteractions();
            }
            catch
            {
                Debug.LogError("Incorrect JSON Format");
            }
            
        }


        /// <summary>
        /// The list, objectInteraction is a list of when a person did a basic grab/drop action 
        /// This function splits the objectinteractions list into a grab and drop list - for more elaborate action checking 
        /// </summary>
        public void SplitBaseInteractions()
        {
            if (objectInteractionTimes != null)
            {
                // All the even numbered indices in object interaction are grab moments- split them into a new list
                var grabInteractions = objectInteractionTimes.Where((grab, index) => index % 2 == 0);
                grabTimes.AddRange(grabInteractions);

                // All the odd numbers indices are drop moments - split it into drop list 
                var dropInteractions = objectInteractionTimes.Where((drop, index) => index % 2 != 0);
                dropTimes.AddRange(dropInteractions);
            }
        }

        #endregion

        #region Helper Functions
        /// <summary>
        /// Resets all of the lists and etc that existed before in preperation for a new trajectory 
        /// </summary>
        private void ClearInteractions()
        {
            gotTrajectory = false;
            playData = null;
            objectInteractionTimes.Clear();
            dropTimes.Clear();
            grabTimes.Clear();
        }

        /// <summary>
        /// if a reset is called - stop everything and Load the base scene 
        /// </summary>
        private void LoadBase()
        {
            StopAllCoroutines();
            SceneManager.LoadSceneAsync(0);
        }
        #endregion

        #region Browser Calls

        public void BrowserTrajCall(string trajectory)
        {
            gameObject.SendMessage("InvokeNewProcessing", trajectory);
        }

        public void BrowserRestartCall()
        {
            gameObject.SendMessage("InvokeRestart");
        }

        #endregion

        #region TESTING
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.N))
        //    {
        //        InvokeRestart();
        //    }

        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        InvokeNewProcessing(JSONTrajectory);
        //    }
        //}
        #endregion

    }
}
