using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

namespace Cyan
{
    /// <summary>
    /// Attached to player, collects player data every second and when instructed, data is compiled into json format
    /// </summary>
    public class DataCollection : MonoBehaviour
    {
        private float coroutineWaitTime = GeneralTimer.TimeInterval;
        private GeneralTimer genTimer;
        // DIOS
        public Camera playerCamera;
        List<PData.DataInOneSecond> DIOS = new List<PData.DataInOneSecond>();

        // Scene switches
        public IList<float> sceneSwitches = new List<float>();

        // Misc interaction Data
        private IList<PData.TurnOnInteractRecord> miscInteractions = new List<PData.TurnOnInteractRecord>();
        private HouseSetup housesetup;

        // Rotation Data
        private IList<float> rotationInteractions = new List<float>();

        // DATA STRINGS 
        private string finalData;

        // MISC.
        [Header("Misc values")]
        private bool isRecording = true;
        private Interact player;
        private int decimalPointMax = 4;

        void OnEnable()
        {
            genTimer = FindObjectOfType<GeneralTimer>();
            Interact.OnTurnOnEvent += TurnOnInteractionRecord;
            Interact.OnRotateEvent += RecordRRotation;
            SceneManager.sceneLoaded += SceneJumpRecord;

        }

        void OnDisable()
        {
            Interact.OnTurnOnEvent -= TurnOnInteractionRecord;
            Interact.OnRotateEvent -= RecordRRotation;
            SceneManager.sceneLoaded -= SceneJumpRecord;

        }

        void Start()
        {

            player = FindObjectOfType<Interact>();
            playerCamera = FindObjectOfType<Camera>();
            housesetup = FindObjectOfType<HouseIdentifier>().houseSetup;
            StartCoroutine("DataCollector");
        }

        /// <summary>
        /// Coroutine for data collection at specified interval
        /// Data Collected: Player coordinates/rotations/action state/object being interacted with/objects position/rotation - all collected with delegate 
        /// </summary>
        /// <returns>Appends to 6 data lists and also adds to recording time</returns>
        public IEnumerator DataCollector()
        {
            while (isRecording)
            {
                AddOneSecondData();
                yield return new WaitForSeconds(coroutineWaitTime);
            }
        }


        #region Data collected at every interval
        /// <summary>
        /// Appends the data in one second to a PData DataInOneSecond serialized class and adds that to the List DIOS
        /// </summary>
        private void AddOneSecondData()
        {
            PData.DataInOneSecond oneSecondData = new PData.DataInOneSecond();
            oneSecondData.time = genTimer.genTimer;
            oneSecondData.playerLocation = RoundVect3(transform.position);
            oneSecondData.playerRotation = RoundQuaternion(transform.rotation);
            oneSecondData.cameraRotation = playerCamera.transform.rotation;
            oneSecondData.playerInteractState = player.currentAction;

            if (player.currentAction != InteractStates.isNotInteracting)
                oneSecondData.objName = player.selectedItem.name;
            else
                oneSecondData.objName = ("**null**");
            if (player.currentAction != InteractStates.isNotInteracting)
            {
                oneSecondData.objRotation = (RoundQuaternion(player.selectedItem.transform.rotation));
                oneSecondData.objPosition = player.selectedItem.transform.position;
            }
            else
            {
                oneSecondData.objRotation = (Quaternion.Euler(0, 0, 0));
                oneSecondData.objPosition = new Vector3(0, 0, 0);
            }
            DIOS.Add(oneSecondData);

        }

        #endregion

        #region Scene Switch Data
        /// <summary>
        /// Adds scene changes with timestamp to sceneSwitches Dictionary
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode">idk</param>
        public void SceneJumpRecord(Scene scene, LoadSceneMode mode)
        {
            Debug.LogWarning(scene.name);
            sceneSwitches.Add(genTimer.genTimer);
            Debug.LogWarning("Jumpscene record");
        }
        #endregion

        #region Misc Interaction Data
        private void TurnOnInteractionRecord()
        {
            try
            {
                var toR = new PData.TurnOnInteractRecord();
                toR.second = genTimer.genTimer;
                toR.interactedObject = player.selectedItem.name;
                miscInteractions.Add(toR);
            }
            catch
            {

            }

        }

        private void RecordRRotation()
        {
            rotationInteractions.Add(genTimer.genTimer);
        }
        #endregion

        #region misc helper functions
        /// <summary>
        /// Returns a Vector3 with all of the points rounded to the nearest x(can be changed at the top) decimal points
        /// </summary>
        /// <param name="vect3">The vect3 to be rounded</param>
        /// <returns>vector 3 with x,y,z rounded </returns>
        private Vector3 RoundVect3(Vector3 vect3)
        {
            var x = RoundToMaxDecimalPoints(vect3.x);
            var y = RoundToMaxDecimalPoints(vect3.y);
            var z = RoundToMaxDecimalPoints(vect3.z);
            var roundedVect = new Vector3(x, y, z);
            return roundedVect;
        }

        /// <summary>
        /// Returns a Quaternion with all of the points rounded to the nearest x(can be changed at the top) decimal points
        /// </summary>
        /// <param name="quart"> THe quaterion being rounded</param>
        /// <returns>quaternion with x,y,z,w being rounded</returns>
        private Quaternion RoundQuaternion(Quaternion quart)
        {
            var x = RoundToMaxDecimalPoints(quart.x);
            var y = RoundToMaxDecimalPoints(quart.y);
            var z = RoundToMaxDecimalPoints(quart.z);
            var w = RoundToMaxDecimalPoints(quart.w);
            var roundedQuart = new Quaternion(x, y, z, w);
            return roundedQuart;
        }

        /// <summary>
        /// Rounds a float to the nearest x(indicated at top) decimal points
        /// </summary>
        /// <param name="val">the float val to be rounded</param>
        /// <returns>float to the nearest x decimal points</returns>
        private float RoundToMaxDecimalPoints(float val)
        {
            var pointsToRound = 10.0f * decimalPointMax;
            return Mathf.Round(val * pointsToRound) / pointsToRound;
        }

        /// <summary>
        /// Generates a string that has all the formatting done for the data string compilation
        /// Ex: {0},{1},....{paramsForString-1}
        /// </summary>
        /// <param name="paramsForString">total number of data points you are collecting per second</param>
        /// <returns>formatted string "{0},{1}...{paramsForString-1}"</returns>
        private string GenerateStringFormat(int paramsForString)
        {
            List<string> stringComponents = new List<string>();
            for (int i = 0; i < paramsForString; i++)
            {
                stringComponents.Add("{" + i + "}");
            }
            var stringFormatted = string.Join(",", stringComponents.ToArray());
            return stringFormatted;
        }

        /// <summary>
        /// For testing, writes data to a file in resources 
        /// </summary>
        private void WriteStringToFile()
        {
            var FileName = "JSONTrajectory.txt";
            var JSONFile = File.CreateText("Assets/Resources/" + FileName);
            JSONFile.Write(finalData);
            JSONFile.Close();
        }
        #endregion

        #region Data Compilation/Sending
        /// <summary>
        /// Creates a PData and converts DIOS and sceneswitches to an array
        /// DIO and sceneSwitches are converted so that the data can be converted to JSON format
        /// </summary>
        private void CompileData()
        {
            PData nestedData = new PData();

            nestedData.dios = DIOS.ToArray();
            nestedData.sceneSwitches = sceneSwitches.ToArray();
            nestedData.miscInteractions = miscInteractions.ToArray();
            nestedData.rRotations = rotationInteractions.ToArray();
            nestedData.houseSetup = housesetup.name;
            finalData = JsonUtility.ToJson(nestedData);

        }

        public string GetData()
        {
            StopAllCoroutines();
            CompileData();
            return finalData;
        }

        /// <summary>
        /// Stops all coroutines, compiles the data that has been collecting into json formation 
        /// and sends an external call to the browser with the final data in JSON.
        /// </summary>
        public void SendData()
        {
            StopAllCoroutines();
            CompileData();
            Application.ExternalCall("SayHello", finalData);
        }

        /// <summary>
        /// ACCESS THIS FUNCTION FROM BROWSER. Calls the sendData Function. 
        /// </summary>
        public void BrowserToC()
        {
            gameObject.SendMessage("SendData");
        }

        #endregion

        #region notinuse
        /// <summary>
        ///[NOT IN USE]
        /// collects the time that player clicked and the item that the player interacted with
        /// Appends data into a string and adds to clickData
        /// string is concatenated to: Ex "3, GameObject_Mug"
        /// </summary>
        private void CollectClicks()
        {
            var click = "";
            try
            {
                click = string.Format("{0},{1}", genTimer.genTimer, player.selectedItem.ToString());
            }
            catch
            {
                click = genTimer.genTimer + "Error";
            }
        }
        #endregion

        #region TESTING
        //void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.P))
        //    {
        //        SendData();
        //        WriteStringToFile();
        //        Debug.Log("Recorded new playData");
        //    }

        //}
        #endregion
    }
}
