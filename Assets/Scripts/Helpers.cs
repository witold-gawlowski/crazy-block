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

    public static List<T> GetRandomWeightedSubset<T>(List<T> weightedItems, int subsetSize) where T: IWeighted
    {
        List<T> randomizedSubset = new List<T>();
        HashSet<T> selectedItems = new HashSet<T>();

        float totalWeight = weightedItems.Sum(item => item.GetWeight());
        var probabilities = weightedItems.Select(item => item.GetWeight() / totalWeight).ToList();

        while (randomizedSubset.Count < subsetSize && selectedItems.Count < weightedItems.Count)
        {
            float randomValue = Random.value;
            int selectedIndex = Enumerable.Range(0, weightedItems.Count)
                .FirstOrDefault(i => randomValue <= probabilities.Take(i + 1).Sum() && selectedItems.Add(weightedItems[i]));

            if (selectedIndex != -1)
            {
                randomizedSubset.Add(weightedItems[selectedIndex]);
            }
        }

        return randomizedSubset;
    }

    public static Vector3 GetCenterOffset(Transform parent)
    {
        Vector3 sum = Vector3.zero;

        foreach (Transform t in parent)
        {
            // Assuming tiles have a Transform component, you can use localPosition
            if (t != null && t.transform != null)
            {
                sum += t.transform.localPosition;
            }
        }

        // Calculate the average by dividing the sum by the number of elements
        Vector3 averageLocalPosition = sum / parent.transform.childCount;

        return averageLocalPosition;
    }

    public static int CustomRound(int x)
    {
        if (x < 50)
        {
            return (int)(Math.Round(x / 5.0) * 5);
        }
        else if (x < 100)
        {
            return (int)(Math.Round(x / 10.0) * 10);
        }
        else if (x < 200)
        {
            return (int)(Math.Round(x / 25.0) * 25);
        }
        else
        {
            return (int)(Math.Round(x / 50.0) * 50);
        }
    }
}
