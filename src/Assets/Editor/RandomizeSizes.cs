using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Randomizes scale of items from 1 to 1.6 for x,y,z to make items look more natural
/// </summary>
public class RandomizeSizes : EditorWindow {

	[MenuItem("Tools/Randomize scale")]
    static void RandomScale()
    {
        GameObject[] gobjects = Selection.gameObjects;
        foreach(GameObject go in gobjects)
        {
            go.transform.localScale = new Vector3(Random.Range(1.0f, 1.6f), Random.Range(1.0f, 1.6f), Random.Range(1.0f,1.6f));
        }
    }
}
