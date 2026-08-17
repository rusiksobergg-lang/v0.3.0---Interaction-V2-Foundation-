using UnityEngine;
using UnityEditor;

public class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts")]
    static void Find()
    {
        GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);

        int count = 0;

        foreach (GameObject go in objects)
        {
            Component[] components = go.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    count++;
                    Debug.Log($"Missing Script на: {go.name}", go);
                }
            }
        }

        Debug.Log($"Знайдено Missing Script: {count}");
    }
}