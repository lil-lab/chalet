using UnityEngine;
using UnityEditor;
using Cyan;

public class GiveMeshCollider : EditorWindow {

	[MenuItem("Tools/Set with Drawer Settings")]
    static void GiveMesh()
    {
        GameObject[] gobjects = Selection.gameObjects;
        
        foreach(GameObject go in gobjects)
        {
            try
            {
                BoxCollider bc = go.GetComponent<BoxCollider>();
                bc.enabled = true;
                bc.isTrigger = true;
                Debug.LogFormat("Enabled Box Colldier on {0}", go.name);
            }
            catch
            {
                BoxCollider bc = go.AddComponent<BoxCollider>();
                bc.isTrigger = true;
                Debug.LogFormat("Added Box Collider on {0}", go.name);
            }
            try
            {
                MeshCollider mC = go.GetComponent<MeshCollider>();
                mC.enabled = false;
                Debug.LogFormat("Found MeshCollider {0}", mC.ToString());
            }
            catch
            {
                Debug.Log("Did not have mesh collider");

            }
            try
            {
                MoveWithDrawer mWD = go.GetComponent<MoveWithDrawer>();
                Debug.LogFormat("Found move with drawer script in {0},{1}", mWD.ToString(), go.name);
            }
            catch
            {
                MoveWithDrawer mWS = go.AddComponent<MoveWithDrawer>();
                Debug.LogFormat("Added Move with Drawer script {0}", mWS.ToString());
            }
        }

        Debug.LogFormat("Updated {0} objects with Drawer Functionality", gobjects.Length);
    }

}
