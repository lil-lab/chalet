using UnityEngine;
using System;

namespace Cyan
{
    /// <summary>
    /// Possible interaction states for player. 
    /// </summary>
    public enum InteractStates { isNotInteracting, isHolding, isPulling };


    /// <summary>
    /// Attached to player, player can interact with items in room that contain the tag "object"
    /// </summary>
    public class Interact : MonoBehaviour
    {
        private const float rayDistance = 2f;
        private const float holdDistance = 1.5f;
        private KeyCode rightClick = KeyCode.Mouse0;
        private KeyCode rotateKey = KeyCode.R;
        [SerializeField]
        public GameObject selectedItem;
        public bool greaterControl;

        public InteractStates currentAction;

        private Hover hover;
        //public CustomEvent interactionEvent;

        #region Rotation Events
        public delegate void OnRotateHandler();
        public static event OnRotateHandler OnRotateEvent;
        public delegate void OnTurnOnHandler();
        public static event OnTurnOnHandler OnTurnOnEvent;

        public static void InvokeRotation()
        {
            OnRotateEvent();
        }

        public static void InvokeTurnOnInteraction()
        {
            OnTurnOnEvent();
        }
        #endregion

        private void OnEnable()
        {
            OnRotateEvent += RotateItem;
            OnTurnOnEvent += TurnonInteraction;
        }

        private void OnDisable()
        {
            OnRotateEvent -= RotateItem;
            OnTurnOnEvent -= TurnonInteraction;
        }

        void Start()
        {
            currentAction = InteractStates.isNotInteracting;

            try
            {
                hover = GetComponent<Hover>();
            }

            catch
            {
                Debug.Log("Cant find Hover Script");
            }
        }


        /// <summary>
        /// Shoots out a Raycast if the player is not interacting with anything to look for an item to interact with. Otherwise stops any interactions AS long as the item won't clip through a wall
        /// </summary>
        public void ShootRay()
        {
            if (currentAction == InteractStates.isNotInteracting)
                InteractWith();
            else if ((currentAction == InteractStates.isHolding && hover.canInteract == true) || currentAction == InteractStates.isPulling)
                Drop();
        }


        /// <summary>
        /// Shoots out raycast-if item is interactable- interacts with the item. 
        /// </summary>
        private void InteractWith()
        {
            RaycastHit targetHit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out targetHit, rayDistance))
            {
                selectedItem = GetObject(Camera.main.transform.position, Camera.main.transform.forward, targetHit);
                var itemTag = selectedItem.tag.ToString();

                if (itemTag.Contains("object"))
                {

                    if (itemTag.Contains("openable"))
                    {
                        currentAction = InteractStates.isPulling;
                    }
                    else if (itemTag.Contains("effects"))
                    {
                        InvokeTurnOnInteraction();
                    }
                    else // Normal object
                    {
                        Freeze(selectedItem);
                        currentAction = InteractStates.isHolding;
                    }
                }

            }
        }


        #region Object picking
        /// <summary>
        /// Releases held object- changes player state to not interacting 
        /// </summary>
        public void Drop()
        {
            if (selectedItem.GetComponent<Rigidbody>())
            {
                Unfreeze(selectedItem);
            }
            currentAction = InteractStates.isNotInteracting;
            selectedItem = null;
        }


        /// <summary>
        /// Returns the gameObject that is clicked
        /// </summary>
        /// <param name="origin">where you shoot ur ray from </param>
        /// <param name="outray">the direction that the ray goes out (the center of the camera)</param>
        /// <param name="r">the hit</param>
        /// <returns></returns>
        GameObject GetObject(Vector3 origin, Vector3 outray, RaycastHit r)
        {
            if (Physics.Raycast(origin, outray, out r, rayDistance))
            {
                return r.collider.gameObject;
            }
            return null;
        }


        /// <summary>
        /// Once grabbable item is found- moves the item `hold_distance` from the main camera, freezes its position and then becomes child of the player
        /// Also adds the frozen item to active object layer so it is rendered on top of everything else
        /// </summary>
        /// <param name="obj">selected object</param>
        private void Freeze(GameObject obj)
        {
            Vector3 freeze_position = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
            obj.transform.position = freeze_position;
            obj.transform.parent = Camera.main.transform;
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            obj.layer = 13;
        }

        public void Freeze(string objName)
        {
            var goalObj = GameObject.Find(objName);
            selectedItem = goalObj;
            Vector3 freeze_position = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
            goalObj.transform.position = freeze_position;
            goalObj.transform.parent = Camera.main.transform;
            Rigidbody rigidbody = goalObj.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            goalObj.layer = 13;
            currentAction = InteractStates.isHolding;
        }

        /// <summary>
        /// Unparents the grabbed item and releases freeze constraints
        /// Puts object back into default layer
        /// </summary>
        /// <param name="obj">selected object</param>
        private void Unfreeze(GameObject obj)
        {
            obj.transform.parent = null;
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.None;
            obj.layer = 0;
        }

        /// <summary>
        /// Takes the selected item and rotates it 90 degrees around the x axis 
        /// </summary>
        private void RotateItem()
        {
            if (selectedItem != null)
            {
                selectedItem.transform.Rotate(Vector3.right * 90);
            }
        }
        #endregion

        #region Drawer Interactions
        /// <summary>
        /// Pulls a door based on hingetype.
        /// Checks hinge type of door and takes the mouse input axis necessary for hinge
        /// Calls different open functionality based on hingetype
        /// </summary>
        /// <param name="axis">String indicating the axis that you are recieving mouse input from </param>
        /// <param name="mouseInput">float value of mouse input that will change the rotation of the door per frame</param>
        private void Pull(string axis, float mouseInput)
        {
            try
            {
                Open door = selectedItem.GetComponent<Open>();
                if (axis == "vertical" && door.hType == HingeType.slideOut)
                    door.slideOut(mouseInput);

                else if (axis == "vertical" && (door.hType == HingeType.slideOutBedLeft || door.hType == HingeType.slideOutBedRight))
                    door.slideOutZ(mouseInput);

                else if (axis == "horizontal" && (door.hType == HingeType.leftHinge || door.hType == HingeType.rightHinge))
                    door.swingOut(mouseInput);

                else if (axis == "vertical" && door.hType == HingeType.bottomHinge)
                    door.swingDown(mouseInput);

                else if (axis == "vertical" && door.hType == HingeType.toilet)
                {
                    door.toiletSeat(mouseInput);
                }



            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        private void AutoPull()
        {
            try
            {
                Open door = selectedItem.GetComponent<Open>();
                if (door.hType == HingeType.slideOut)
                {
                    if (door.isClosed)
                    {
                        //door.AutoSlideOut(2f);
                    }
                    else
                        currentAction = InteractStates.isNotInteracting;


                }

            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        #endregion

        #region Misc  Interactions
        /// <summary>
        /// Meant for activating particle effects- turns effects on and off
        /// If interacting item does not have a Particle Control Script, it means it is a screen and change the material for the screen to mimic the screen turning off
        /// </summary>
        /// <param name="selectedItem">collider that user clicks to interact with</param>
        private void TurnonInteraction()
        {
            //interactionEvent.InvokeEvent();
            try
            {
                ParticleControl interactableSwitch = selectedItem.GetComponent<ParticleControl>();
                SwitchOnOff(interactableSwitch);
            }
            catch
            {
                var materials = selectedItem.GetComponent<TVManager>();
                materials.ChangeScreen();
            }


        }

        /// <summary>
        /// Changes isOff bool in Particle control script
        /// </summary>
        /// <param name="p">Particle control script attached to selected item</param>
        private void SwitchOnOff(ParticleControl p)
        {
            if (p.isOff)
                p.isOff = false;
            else
                p.isOff = true;
        }

        #endregion

        void Update()
        {
            if (Input.GetKeyDown(rightClick))
            {
                ShootRay();
            }
            else if (currentAction == InteractStates.isPulling && greaterControl)
            {
                // calls the function twice sending both mouse input axis information- Pull will determine which input to take based on hinge type
                Pull("vertical", Input.GetAxis("Mouse Y"));
                Pull("horizontal", Input.GetAxis("Mouse X"));
            }

            else if (currentAction == InteractStates.isPulling && !greaterControl) // Automatic control is not yet implented and may never be
            {
                AutoPull();
            }

            else if (currentAction == InteractStates.isHolding && Input.GetKeyUp(rotateKey))
            {
                InvokeRotation();
            }


        }
    }
}