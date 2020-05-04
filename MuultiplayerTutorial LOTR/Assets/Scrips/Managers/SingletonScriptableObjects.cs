using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SingletonScriptableObjects<T> : ScriptableObject where T : ScriptableObject
{



    private static T _instance = null;
    public  static T Instance
    {
        get 
        {
        if (_instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();
                if (results.Length == 0)
                {
                    Debug.LogError("Results length is 0 of " + typeof(T).ToString());
                    return null;
                }
                if (results.Length > 1)
                {
                    Debug.LogError("Results length is greather then 1 of " + typeof(T).ToString());
                    return null;
                }
                _instance = results[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }
            return _instance;            
        }
    }


}
