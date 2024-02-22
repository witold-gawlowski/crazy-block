using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;

public class MapRandomizer: MonoBehaviour, MapGeneratorInterface
{
    public string folderPath = "Assets/Prefabs";

    [SerializeField] private List<GameObject> levelPrefabs;
    [SerializeField] private List<GameObject> prefabPool;

    private int currentMapIndex;
    private int _nextShift;

    public void Next()
    {
        var newMapIndex = Mathf.Clamp(currentMapIndex + _nextShift, 0, prefabPool.Count);
        _nextShift = Random.Range(-1, 10);

        prefabPool.RemoveAt(currentMapIndex);
        currentMapIndex = newMapIndex;
    }

    public int GetCurrentReward()
    {
        return 90;
    }

    public void Init()
    {
        prefabPool = new List<GameObject>(levelPrefabs);
        SortGameObjectsByLevelTileCount(prefabPool);
        currentMapIndex = 0;
        _nextShift = Random.Range(-1, 10);
        Next();
    }

    public GameObject GetCurrent()
    {
        return prefabPool[currentMapIndex];
    }

    void SortGameObjectsByLevelTileCount(List<GameObject> gameObjects)
    {
        // Use LINQ to sort the list based on the number of children with the tag "LevelTile"
        gameObjects.Sort((obj1, obj2) =>
        {
            int count1 = CountChildrenWithTag(obj1, "LevelTile");
            int count2 = CountChildrenWithTag(obj2, "LevelTile");
            return count1.CompareTo(count2); // Sort in descending order
        });
    }

    int CountChildrenWithTag(GameObject parent, string tag)
    {
        int count = 0;

        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                count++;
            }
        }

        return count;
    }

    public GameObject PeekNext()
    {
        return prefabPool[currentMapIndex + _nextShift];
    }
}
