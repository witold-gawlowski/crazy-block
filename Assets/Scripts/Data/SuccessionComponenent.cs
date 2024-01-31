using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SuccessionNode", order = 1)]
public class SuccessionNode : ScriptableObject
{
    [System.Serializable]
    private class Successor
    {
        public SuccessionNode Node;
        public float Chance;
    }

    [SerializeField] private GameObject mapPrefab;

    [SerializeField] private List<Successor> successors;

    [SerializeField] private int reward = 200;

    public GameObject GetPrefab() { return mapPrefab; }

    public int GetReward() { return reward; }
    public SuccessionNode GetNext()
    {
        float totalChance = 0f;
        foreach (var successor in successors)
        {
            totalChance += successor.Chance;
        }

        float randomValue = Random.Range(0f, totalChance);

        foreach (var successor in successors)
        {
            if (randomValue < successor.Chance)
            {
                return successor.Node;
            }

            randomValue -= successor.Chance;
        }

        return null;
    }
}