using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
public class Helpers : MonoBehaviour
{
    public static Vector2Int RoundPosition(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public static IEnumerable<Vector2Int> GetBlockEnumerator(GameObject block, Vector2Int coordinates)
    {
        foreach (Transform t in block.transform)
        {
            if (t != block.transform && t.tag == "BlockTile")
            {
                yield return coordinates + Helpers.RoundPosition(t.position - block.transform.position);
            }
        }
    }

    public static Vector2 GetUIBlockPosition(int index)
    {
        const int horizontalScreenDivisorForBlockUI = 3;
        const int rows = 2, cols = 3;
        int width = Screen.width, height = Screen.height;
        var verticalGridSize = height / horizontalScreenDivisorForBlockUI / (rows + 1);
        var horizontalGridSize = width / (cols + 1);

        return new Vector2Int(horizontalGridSize * (index % cols + 1), verticalGridSize * (index / cols + 1));
    }

    public static int GetLevelSize(GameObject level)
    {
        int result = 0;
        foreach (Transform t in level.transform)
        {
            if (t != level.transform && t.CompareTag("LevelTile"))
            {
                result++;
            }
        }
        return result;
    }

    public static void Shuffle<T>(List<T> list)
    {

        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public static T GetRandomElement<T>(List<T> origin)
    {
        return GetRandomSubset<T>(origin, 1).First<T>();
    }

    public static List<T> GetRandomSubset<T>(List<T> origin, int count)
    {
        Assert.IsTrue(origin.Count >= count);
        return origin.OrderBy(x => Random.value).Take(count).ToList();
    }

    public static string GetProjectRelativePath(string absolutePath)
    {
        string assetsPath = Application.dataPath;

        if (absolutePath.StartsWith(assetsPath))
        {
            string relativePath = "Assets" + absolutePath.Substring(assetsPath.Length);
            return relativePath;
        }
        else
        {
            Debug.LogWarning("The provided path is not within the project folder.");
            return null;
        }
    }

    public static long GetTimestamp()
    {
        DateTime currentUTC = DateTime.UtcNow;
        return ((DateTimeOffset)currentUTC).ToUnixTimeSeconds();
    }

    public static T GetRandomWeightedElement<T>(List<T> weightedList) where T : IWeighted
    {
        // Calculate total weight
        float totalWeight = weightedList.Sum(item => item.GetWeight());

        // Generate a random value between 0 and totalWeight
        float randomValue = Random.value * totalWeight;

        // Iterate through the list and find the element
        float currentWeight = 0;
        foreach (var item in weightedList)
        {
            currentWeight += item.GetWeight();
            if (randomValue <= currentWeight)
            {
                return item;
            }
        }

        // This should not happen, but return null if it does
        return default(T);
    }
}
