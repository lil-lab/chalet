using UnityEngine;
using UnityEditor;


public class ObjProperties : EditorWindow {
    /// <summary>
    /// Gives selected object(s) a rigidbody, box collider and sets its tag to 'object'
    /// </summary>
    [MenuItem("Tools/Object Properties")]
    static void ObjectProperties()
    {
        GameObject[] gObjects = Selection.gameObjects;

        foreach (GameObject go in gObjects)
        {
            go.tag = "object";

            try
            {
               Rigidbody rB = go.GetComponent<Rigidbody>();
                Debug.LogFormat("Found Rigidbody :{0}", rB.name);
            }
            catch
            {
                go.AddComponent<Rigidbody>();
            }


            try
            {
                BoxCollider bC =  go.GetComponent<BoxCollider>();
                Debug.LogFormat("Found BoxCollider: {0}", bC.name);
            }

            catch
            {
                go.AddComponent<BoxCollider>();
            }

            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            foreach (Collider child_collider in colliders)
            {
                child_collider.gameObject.SetActive(false);
                Debug.Log("Collider is no longer enabled");
            }
        }
        Debug.LogFormat("Gave object properties to {0} items", gObjects.Length);
    }
}
