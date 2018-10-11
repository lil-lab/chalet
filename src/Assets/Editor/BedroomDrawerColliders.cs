using UnityEngine;
using UnityEditor;

public class BedroomDrawerColliders : EditorWindow {

    [MenuItem("Tools/BedroomDrawerColliders")]
    static void BedroomColliders()
    {
        GameObject[] gobjects = Selection.gameObjects;
        foreach (GameObject go in gobjects)
        {
            try
            {
                BoxCollider[] bcs = go.GetComponents<BoxCollider>();
                foreach (BoxCollider bc in bcs)
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

            outerBC.center = new Vector3(0f, .12f, -.05f);
            outerBC.size = new Vector3(1.178f, .218f, .04f);
            outerBC.isTrigger = false;

            triggerBC.center = new Vector3(0f, .0236f, -.018f);
            triggerBC.size = new Vector3(1.178f, .047f, .987f);
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

                    else if (child.gameObject.name != "collider" && child.gameObject.name.Contains("collider"))
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
