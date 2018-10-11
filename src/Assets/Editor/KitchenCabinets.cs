using UnityEngine;
using System.Collections;
using UnityEditor;
using Cyan;

public class KitchenCabinets : EditorWindow {
    [MenuItem("Tools/Cabinet Scripts")]
    static void KitchCabProperties()
    {
        GameObject[] doors = Selection.gameObjects;

        foreach(GameObject door in doors)
        {
            

            try
            {
                
                        door.tag = "object-openable";
                        door.AddComponent<Open>();
                        //door.AddComponent<BoxCollider>();
                        door.AddComponent<MoveWithDrawer>();                
            }

            catch
            {

            }
            finally
            {

            }

        }
    }

	
}
