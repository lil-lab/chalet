using UnityEngine;
using UnityEditor;

public class SetChildrenInactive : EditorWindow {

    /// <summary>
    /// An Editor Tool to set set the children colliders of a model as inactive and also give a box collider to the main gameobject
    /// </summary>
    [MenuItem("Tools/Set children inactive and add box collider")]
    static void SetInactive()
    {
        GameObject[] gObjects = Selection.gameObjects;
        foreach (GameObject go in gObjects)
        {
            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            foreach (Collider child_collider in colliders)
            {
                child_collider.gameObject.SetActive(false);
                Debug.Log("Collider is no longer enabled");
            }

            Debug.LogFormat("Set {0} Colliders to not active?", gObjects.Length);
            go.SetActive(true);
            go.AddComponent<BoxCollider>();
        }
    }

}
