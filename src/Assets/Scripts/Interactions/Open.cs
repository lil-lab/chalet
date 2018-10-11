using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cyan
{
    public enum HingeType { rightHinge, leftHinge, bottomHinge, slideOut, slideOutBedRight, slideOutBedLeft, toilet }

    /// <summary>
    /// Attached to anything that can open. Drawers/cabinets are clamped to not over close/open
    /// </summary>
    public class Open : MonoBehaviour
    {
        public HingeType hType;
        public bool isClosed = true;
        private bool isManual = true;
        private bool drawerActivated = false;

        private float slideMax = 0.5f; // translate x or z 
        private float rHingeMax = -95.0f; // rotate y
        private float lHingeMax = 95.0f; // rotate y
        private float bHingeMax = -75.0f; //rotate z

        private RoomData thisRoomData;
        private Room thisRoom;

        #region stored in RoomData Scriptable object
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        #endregion

        void Start()
        {
            thisRoom = FindObjectOfType<RoomManager>().currentRoom;
            thisRoomData = FindObjectOfType<RoomIdentifier>().GetAssociatedRoomData(thisRoom);
            try
            {
                if (!thisRoomData.drawerDict.ContainsKey(name))
                {
                    DrawerDetails d = new DrawerDetails();
                    d.initialPosition = transform.localPosition;
                    d.initialRotation = transform.localRotation;
                    thisRoomData.drawerDict.Add(name, d);

                }

                initialPosition = thisRoomData.drawerDict[name].initialPosition;
                initialRotation = thisRoomData.drawerDict[name].initialRotation;
            }
            catch
            {

            }


        }


        #region Manual Control
        /// <summary>
        /// Translates door based on mouseinput from vertical axis (x axis)
        /// Clamps the door from pulling past its original location and the slide max
        /// </summary>
        /// <param name="mouseInput">vertical axis mouse input</param>
        public void slideOut(float mouseInput)
        {
            transform.Translate(Vector3.right * -mouseInput * Time.deltaTime);
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, initialPosition.x, slideMax + initialPosition.x), transform.localPosition.y, transform.localPosition.z);

        }

        /// <summary>
        /// Translates drawer based on mouseinput from vertical axis on the z axis 
        /// Clamps the door from pulling past its original location and the slide max
        /// </summary>
        /// <param name="mouseInput">vertical axis mouse input</param>
        public void slideOutZ(float mouseInput)
        {
            if (hType == HingeType.slideOutBedLeft)
            {
                transform.Translate(Vector3.forward * mouseInput * Time.deltaTime);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, initialPosition.z, slideMax + initialPosition.z));
            }

            else if (hType == HingeType.slideOutBedRight)
            {
                transform.Translate(Vector3.forward * mouseInput * Time.deltaTime);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, initialPosition.z - slideMax, initialPosition.z));

            }
        }

        /// <summary>
        /// Rotates door by right/left hinge  (y axis)
        /// Clamps door from swinging past original location and hingemax
        /// </summary>
        /// <param name="mouseInput">horizontal mouse input</param>
        public void swingOut(float mouseInput)
        {
            float higherInput = mouseInput * 100;
            if (hType == HingeType.rightHinge)
            {
                transform.Rotate(Vector3.up * -higherInput * Time.deltaTime);
                var currentYRot = transform.localEulerAngles.y;

                if (currentYRot > 0 && currentYRot < (360f + rHingeMax))
                {
                    if (currentYRot < (360 - rHingeMax) / 2)
                    {
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                        transform.localRotation = Quaternion.Euler(0, 360f + rHingeMax, 0);
                }
            }

            else if (hType == HingeType.leftHinge)
            {
                transform.Rotate(Vector3.up * -higherInput * Time.deltaTime);
                var currentYRot = transform.localEulerAngles.y;
                if (currentYRot > lHingeMax)
                {
                    if (currentYRot > (360 - lHingeMax) / 2)
                    {
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                        transform.localRotation = Quaternion.Euler(0, lHingeMax, 0);
                }

            }
        }

        /// <summary>
        /// Rotates door on its bottom hinge (z axis)
        /// Clamps door from swinging past original location and hinge max
        /// </summary>
        /// <param name="mouseInput">vertical mouse input</param>
        public void swingDown(float mouseInput)
        {
            float higherInput = mouseInput * 100;
            transform.Rotate(Vector3.forward * higherInput * Time.deltaTime);
            var currentZRot = transform.localEulerAngles.z;

            if (currentZRot > 0 && currentZRot < (360 + bHingeMax))
            {
                if (currentZRot < (360 - bHingeMax) / 2)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                    transform.localRotation = Quaternion.Euler(0, 0, 360 + bHingeMax);
            }
        }

        /// <summary>
        /// Rotates toilet seat cover z axis
        /// Clamps from exceeding 95 degrees and below 0
        /// </summary>
        /// <param name="mouseInput">vertical mouse input</param>
        public void toiletSeat(float mouseInput)
        {
            float higherInput = mouseInput * 100;
            transform.Rotate(Vector3.forward * higherInput * Time.deltaTime);
            var currentZRot = transform.localEulerAngles.z;

            transform.localRotation = Quaternion.Euler(initialRotation.x, initialRotation.y, Mathf.Clamp(currentZRot, .4f, 95f));

            if (currentZRot > (360 - 95f))
            {
                if (currentZRot > (360 - 95) / 2)
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.localRotation = Quaternion.Euler(0, 0, 95f);
            }

        }
        #endregion

        #region Automatic Control 
        //Might never be added

        public void ActivateDrawer()
        {
            isManual = false;
            drawerActivated = true;
        }

        // TODO AutoBottomHinge
        // TODO AutoLeftHinge
        // TODO AutoRightHinge

        private void AutoOpen()
        {
            switch (hType)
            {
                case HingeType.bottomHinge:
                    AutoBSwing();
                    break;
                case HingeType.leftHinge:
                    AutoLSwing();
                    break;
                case HingeType.slideOut:
                    //AutoSlideOut(2f);
                    break;
                default: // rhinge and anything else (will have rhinge functionality)
                    AutoRSwing();
                    break;
            }
        }
        private void AutoRSwing()
        {

        }

        private void AutoLSwing()
        {

        }

        private void AutoBSwing()
        {

        }

        //public void AutoSlideOut(float speedDirection)
        //{
        //    Vector3 MaxSlideOut = initialPosition + new Vector3(slideMax, 0, 0);
        //    Debug.Log(MaxSlideOut);
        //    Vector3 MaxROut = initialRotation + Quaternion.Euler(0, rHingeMax, 0);
        //    Vector3 MaxLOut = initialRotation + new Vector3(0, lHingeMax, 0);
        //    Vector3 MaxBOut = initialRotation + new Vector3(0, 0, bHingeMax);
        //    if (isClosed)
        //    {
        //        if (Mathf.Round(transform.localPosition.x * 100) / 100 != Mathf.Round(MaxSlideOut.x * 100) / 100)
        //        {
        //            transform.localPosition += transform.right * Time.deltaTime * speedDirection;
        //        }
        //        //transform.localPosition = Vector3.Lerp(transform.localPosition, MaxSlideOut, 0);
        //        else
        //            isClosed = false;

        //    }

        //    //else // is open
        //    //{
        //    //    if (Mathf.Round(transform.localPosition.x * 100) / 100 != Mathf.Round(initialPosition.x * 100) / 100)
        //    //    {
        //    //        transform.localPosition -= transform.right * Time.deltaTime * 2f;
        //    //    }
        //    //    else
        //    //        isClosed = true;

        //    //}

        //}

        private void MovementCheck()
        {
            Vector3 MaxSlideOut = initialPosition + new Vector3(slideMax, 0, 0);


            if (hType == HingeType.slideOut && isClosed && (transform.localPosition == MaxSlideOut))
            {
                isClosed = false;
            }
            else if (hType == HingeType.slideOut && !isClosed && (transform.localPosition == initialPosition))
            {
                isClosed = true;
            }
        }

        #endregion

    }
}
