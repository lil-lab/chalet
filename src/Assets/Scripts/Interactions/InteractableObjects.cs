using UnityEngine;
using System.Collections;

namespace Cyan
{
    [System.Serializable]
    public class DrawerDetails : InteractiveObjectDetails
    {
        public Vector3 initialPosition;
        public Quaternion initialRotation;
    }

    [System.Serializable]
    public class InteractiveObjectDetails
    {
        public string objName;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }

    [System.Serializable]
    public class OnOffObjects : InteractiveObjectDetails
    {
        public bool isOff;
    }

    [System.Serializable]
    public class TextureObjects : OnOffObjects
    {
        public Material material;
    }
}