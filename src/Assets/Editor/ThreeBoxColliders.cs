using UnityEngine;
using UnityEditor;
using System.Collections;

public class ThreeBoxColliders : EditorWindow
{
    /// <summary>
    /// Gives selected object(s) 3 colliders needed for drawer
    /// </summary>
    [MenuItem("Tools/3 Colliders for kitchen drawers")]
    static void ThreeColliders()
    {
        GameObject[] gobjects = Selection.gameObjects;
        foreach(GameObject go in gobjects)
        {
            try
            {
                BoxCollider[] bcs = go.GetComponents<BoxCollider>();
                foreach(BoxCollider bc in bcs)
                {
                    DestroyImmediate(bc);
                }
            }

            catch
            {
                Debug.LogFormat("No colliders found in {0}", go.name);
            }


            BoxCollider outerBC = go.AddComponent<BoxCollider>();
            BoxCollider triggerBC = go.AddComponent<BoxCollider>();

            outerBC.center = new Vector3(.35f, .135f, 0f);
            outerBC.size = new Vector3(.0345f, .26f, .98f);
            outerBC.isTrigger = false;

            triggerBC.center = new Vector3(.0232f, .0137f, 0f);
            triggerBC.size = new Vector3(.67f, .01f, .982f);
            triggerBC.isTrigger = true;


            try
            {
                BoxCollider[] childColliders = go.GetComponentsInChildren<BoxCollider>(true);
                foreach (BoxCollider child in childColliders)
                {
                    if (child.gameObject.name == "collider" || child.gameObject.name == "Collider" || child.gameObject.name.Contains("1"))
                    {
                        child.gameObject.SetActive(true);
                    }

                    else if(child.gameObject.name != "collider" && child.gameObject.name.Contains("collider"))
                    {
                        child.gameObject.SetActive(false);
                    }
                        
                }

            }
            catch
            {
                Debug.LogFormat("Theres no child colliders in {0}", go.name);
            }


            try
            {
                MeshCollider uselessMesh = go.GetComponent<MeshCollider>();
                DestroyImmediate(uselessMesh);
            }
            catch
            {

            }
        }



    }
}