using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


public class GetChildren : MonoBehaviour {
    private IList<Transform> listOfTransforms;
    private IList<GameObject> listofPrefabs = new List<GameObject>();
    private const string prefabStr = "PRE_";
    
    void Start()
    {        
        Debug.Log("Start");
        listOfTransforms = gameObject.GetComponentsInChildren<Transform>();
        Debug.Log("pls");
        foreach(Transform obj in listOfTransforms)
        {
            if (obj.name.Contains(prefabStr))
            {
                var objName = obj.name;
                Debug.Log(objName);
                listofPrefabs.Add(obj.gameObject);
            }
        }
        foreach (GameObject go in listofPrefabs)
        {
            Debug.Log(go.name);
            //var prefabName = go.name + ".prefab";
            foreach(string s in AssetDatabase.GetAllAssetPaths())
            {
                if (s.Contains(go.name) && s.Contains("Complete_Home_Interior_Pack"))
                    Debug.Log(s);
                    
            }
            
            
            //Debug.Log(Path.Combine(Directory.GetCurrentDirectory(), fileName));

        }

    }


}
	
	

    
	

