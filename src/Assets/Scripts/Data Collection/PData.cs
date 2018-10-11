using UnityEngine;
using System.Collections;

namespace Cyan
{
    [System.Serializable]
    public class PData
    {
        [System.Serializable]
        public class DataInOneSecond
        {
            public float time;
            public Vector3 playerLocation;
            public Quaternion playerRotation;
            public InteractStates playerInteractState;
            public Quaternion cameraRotation;
            public string objName;
            public Quaternion objRotation;
            public Vector3 objPosition;
        }


        [System.Serializable]
        public class TurnOnInteractRecord
        {
            public float second;
            public string interactedObject;
        }

        public DataInOneSecond[] dios;
        public float[] sceneSwitches;
        public TurnOnInteractRecord[] miscInteractions;
        public float[] rRotations;
        public string houseSetup;
    }
}