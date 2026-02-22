using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static List<T> GetListInChild<T>(Transform parent)
    {
        List<T> result = new List<T>();

        for (int i = 0; i < parent.childCount; i++)
        {
            var component = parent.GetChild(i).GetComponent<T>();
            if(component != null)
                result.Add(component);
        }

        return result;
    }
}
