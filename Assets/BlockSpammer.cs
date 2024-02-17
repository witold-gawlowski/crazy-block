using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpammer : MonoBehaviour
{
    private ShopManager shopManager;
    private Camera mainCamera;
    private List<BlockScript> blocks;



    [SerializeField] List<Transform> spawnPoints;
    private void Awake()
    {
        shopManager = GetComponentInParent<ShopManager>();
        mainCamera = Camera.main;
        blocks = shopManager.GetAllBlocks();
    }

    public void HandleNewLevel()
    {
        SpawnRandomBlocks();
    }
    void SpawnRandomBlocks()
    {
        int numberOfBlocksToSpawn = 3;
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
            BlockScript randomBlock = blocks[Random.Range(0, blocks.Count)];

            // Instantiate the chosen block at the spawn point
            var newBlock =    Instantiate(randomBlock, spawnPoints[i].position, Quaternion.identity);
            newBlock.GetComponent<BlockScript>().Init(true);
        }
    }
}
