using UnityEngine;
using System.Collections;
using UnityEditor;

public class Rotateby90Up : Editor {

    [MenuItem("Tools/Rotate90 up %PageUp")]
    static void Rotateby90()
    {
        GameObject[] gos = Selection.gameObjects;
        foreach (GameObject go in gos)
        {
            go.transform.Rotate(90f, 0f, 0f);
        }
    }
}
