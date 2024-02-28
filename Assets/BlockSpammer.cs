using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpammer : MonoBehaviour
{
    [SerializeField] private List<BlockScript> blocks;
    [SerializeField] private List<BlockScript> protectionPeriodBlocks;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] private AnimationCurve spawnCountCurve;

    public void HandleNewLevel()
    {
        //var spawnCount = Mathf.RoundToInt(spawnCountCurve.Evaluate(GlobalGameManager.Instance.GetLevel()));
        var  spawnCount = 0;
        if (GlobalGameManager.Instance.GetLevel() <= 5)
        {
            SpawnRandomBlocks(spawnCount, true);
        }
        else
        {
            SpawnRandomBlocks(spawnCount);
        }
    }

    void SpawnRandomBlocks(int numberOfBlocksToSpawn = 3, bool protectionPeriod = false)
    {
        if (numberOfBlocksToSpawn > spawnPoints.Count)
        {
            Debug.LogWarning("Number of blocks to spawn is greater than the number of spawn points. Adjusting to the number of spawn points.");
            numberOfBlocksToSpawn = spawnPoints.Count;
        }

        // Shuffle the spawn points array to randomize the order
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnPoints.Count);
            Transform temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }

        // Spawn blocks at the chosen locations
        for (int i = 0; i < numberOfBlocksToSpawn; i++)
        {
            // Randomly choose a block from the list
            BlockScript randomBlock;
            if (protectionPeriod)
            {
                randomBlock = protectionPeriodBlocks[Random.Range(0, protectionPeriodBlocks.Count)];
            }
            else
            {
                randomBlock = blocks[Random.Range(0, blocks.Count)];
            }

            // Instantiate the chosen block at the spawn point
            var newBlock = Instantiate(randomBlock, spawnPoints[i].position, Quaternion.identity);
            newBlock.GetComponent<BlockScript>().Init(true);
        }
    }
}
