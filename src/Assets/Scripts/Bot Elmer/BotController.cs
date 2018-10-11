using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Cyan.Elmer;

namespace Cyan
{
    public class BotController : MonoBehaviour
    {
        private ScreenCapture capture;
        private Rewards reward;
        private RoomData roomBefore;
        private RoomData roomAfter;
        private SocketManager socketManager;
        private StandaloneHouseSetup setupControl;

        private bool isExecutingInstruction = false;

        private Vector3 target = Vector3.zero;

        void OnEnable()
        {
            capture = GetComponent<ScreenCapture>();
            reward = GetComponent<Rewards>();
            socketManager = GetComponent<SocketManager>();
            setupControl = GetComponent<StandaloneHouseSetup>();

            roomBefore = ScriptableObject.CreateInstance(typeof(RoomData)) as RoomData;
            roomAfter = ScriptableObject.CreateInstance(typeof(RoomData)) as RoomData;
        }

        /// <summary>
        /// Checks the action that is meant to be executed. 
        /// and after execution gives reward.
        /// To add new instructions just create a new case
        /// NOTE: the instruction should be all lowercase
        /// </summary>
        /// <param name="instructions">the queue for instructions</param>
        private void CheckInstruction(Queue<string> instructions)
        {
            //TODO - before and after roomDatas 
            try
            {
                Scene thisScene = SceneManager.GetActiveScene(); 
                if ( instructions.Count > 0 && isExecutingInstruction == false)
                {
                    isExecutingInstruction = true;
                    var instr = instructions.Dequeue();
                    switch (instr)
                    {
                        case "forward":
                            Forward(instr);
                            break;
                  
                        default:
                            break;
                    }
                }
            }
            catch { }

        }

        #region Actions
        /// <summary>
        /// Moves the player forward by one unit 
        /// </summary>
        /// <param name="instruction"></param>
        private void Forward(string instruction)
        {
            transform.Translate(Vector3.forward);
            Debug.Log("Moved Forward at " + Time.time);
            SendReward(instruction);
            isExecutingInstruction = false;
        }
        #endregion

        #region Reward
        private void SendReward(string instruction)
        {
            Debug.Log("Send reward");
            var rwd = reward.GiveReward(instruction, roomBefore, roomAfter);
            socketManager.sendMessage(rwd.ToString());
            socketManager.gotMessage = false;
        }
        #endregion

        void Update()
        {
            CheckInstruction(socketManager.instructionsQueue);
        }
    }
}
