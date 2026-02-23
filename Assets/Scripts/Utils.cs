using System.Collections.Generic;
using UnityEngine;

public class Utils
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

    public static List<T> TakeAndRemoveRandom<T>(List<T> source, int n)
    {
        List<T> result = new List<T>(); // khoi tao list de tra ve
        n = Mathf.Min(n, source.Count); // check de dam bao slg lay ve ko vuot qua slg list co san

        for(int i = 0; i < n; i++)
        {
            int ranIndex = Random.Range(0, source.Count);
            result.Add(source[ranIndex]);
            source.RemoveAt(ranIndex);
        }

        return result;
    }
}
