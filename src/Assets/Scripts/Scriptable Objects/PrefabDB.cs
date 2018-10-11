using UnityEngine;
using System;

namespace Cyan
{
    /// <summary>
    /// Scriptable object that acts as a DB for all interactable objects. 
    /// </summary>
    [CreateAssetMenu()]
    public class PrefabDB : ScriptableObject
    {
        public GameObject[] prefabs;

        /// <summary>
        /// Retrieves Gameobject by name 
        /// </summary>
        /// <param name="itemString"> Name of the Gameobject that will be searched in DB</param>
        /// <returns>Gameobject from DB</returns>
        public GameObject getPrefab(string itemString)
        {
            var prefabName = parseToPrefabName(itemString);
            int index = Array.IndexOf(prefabs, searchPrefabsFor(prefabName));
            //Debug.LogFormat("Index of prefab {0}", index);
            return prefabs[index];
        }

        /// <summary>
        /// Parses the name of an object in scene to be just that of the reference name
        /// </summary>
        /// <param name="itemString">name of item in scene</param>
        /// <returns>Name of prefab that will be used to search in prefabDB</returns>
        private string parseToPrefabName(string itemString)
        {
            string prefabName;
            if (itemString.Contains(" ("))
            {
                prefabName = itemString.Substring(0, itemString.IndexOf(" ("));
            }
            else if (itemString.Contains("("))
            {
                prefabName = itemString.Substring(0, itemString.IndexOf("("));
            }
            else
                prefabName = itemString;
            return prefabName;
        }

        /// <summary>
        /// Looks for the Prefab in DB by name
        /// </summary>
        /// <param name="prefabName">The name of the prefab being looked for</param>
        /// <returns>Gameobject that needs to be created in scene from DB</returns>
        private GameObject searchPrefabsFor(string prefabName)
        {
            GameObject p = prefabs[0];
            foreach (GameObject prefab in prefabs)
            {
                var prefabString = prefab.name;
                if (prefabString == prefabName)
                {
                    p = prefab;
                    break;
                }
            }

            return p;
        }

    }

}