using UnityEngine;
using UnityEditor;
using System.Collections;

public class RotateBy90LR: Editor {
    [MenuItem("Tools/Rotate 90 %RIGHT")]
    static void Rotateby90()
    {
        GameObject[] gos = Selection.gameObjects;
        foreach(GameObject go in gos)
        {
            go.transform.Rotate(0f, 90f, 0f);
        }
    }


}


