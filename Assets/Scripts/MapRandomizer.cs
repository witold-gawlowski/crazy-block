using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

public class MapRandomizer: MonoBehaviour, MapGeneratorInterface
{
    public string folderPath = "Assets/Prefabs";

    [SerializeField] private List<GameObject> levelPrefabs;

    private int currentMapIndex;

    public void Next()
    {
        var newMapIndex = Mathf.Clamp(currentMapIndex + Random.Range(-2, 4), 0, levelPrefabs.Count);
        
        levelPrefabs.RemoveAt(currentMapIndex);
        currentMapIndex = newMapIndex;
    }

    public int GetCurrentReward()
    {
        return 200;
    }

    public void Init()
    {
        SortGameObjectsByLevelTileCount(levelPrefabs);
        Debug.Log("init randomizer");
        currentMapIndex = 0;
        Next();
    }

    public GameObject GetCurrent()
    {
        return levelPrefabs[currentMapIndex];
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

        foreach (var gameObject in levelPrefabs)
        {
            Debug.Log(CountChildrenWithTag(gameObject, "LevelTile"));
        }
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
}
