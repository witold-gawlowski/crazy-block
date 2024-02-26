using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;

public class MapRandomizer: MonoBehaviour, MapGeneratorInterface
{
    public string folderPath = "Assets/Prefabs";

    [SerializeField] private List<GameObject> initialPrefabs;
    [SerializeField] private List<GameObject> endgamePrefabs;

    private List<GameObject> initialPrefabPool;
    private List<GameObject> endgamePool;

    [SerializeField] private int _initialPrafabIndex;
    [SerializeField] private int _endPrafabIndex;
    [SerializeField] private bool _isEndgame;

    public void Next()
    {
        if (_isEndgame)
        {
            _endPrafabIndex++;
            if(_endPrafabIndex == endgamePool.Count)
            {
                _endPrafabIndex = 0;
            }
        }
        else
        {
            _initialPrafabIndex++;
            if(_initialPrafabIndex == initialPrefabPool.Count)
            {
                _isEndgame = true;
            }
        }
    }

    public int GetCurrentReward()
    {
        int targetPrice = CountChildrenWithTag(GetCurrent(), "LevelTile") * 4;
        return (targetPrice + 49 ) / 50 * 50 ;
    }

    public void Init()
    {
        int initialPoolCount = Mathf.Min(20, initialPrefabs.Count);
        initialPrefabPool = Helpers.GetRandomSubset(initialPrefabs, initialPoolCount);
        SortGameObjectsByLevelTileCount(initialPrefabPool);

        endgamePool = new List<GameObject>(endgamePrefabs);
        Helpers.Shuffle(endgamePool);

        _isEndgame = false;
        _initialPrafabIndex = -1;
        _endPrafabIndex = 0;
    }



    public GameObject GetCurrent()
    {
        if (_isEndgame)
        {
            return endgamePool[_endPrafabIndex];
        }
        else
        {
            return initialPrefabPool[_initialPrafabIndex];
        }
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
        int endPoolSIze = endgamePool.Count;
        int initialPoolSize = initialPrefabPool.Count;
        if (_isEndgame)
        {
            return endgamePool[(_endPrafabIndex + 1) % endPoolSIze];
        }
        else
        {
            if(_initialPrafabIndex == initialPoolSize -1)
            {
                return endgamePool[0];
            }
            else
            {
                return initialPrefabPool[_initialPrafabIndex + 1];
            }
        }
    }
}
