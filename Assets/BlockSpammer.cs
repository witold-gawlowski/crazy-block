using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpammer : MonoBehaviour
{
    private ShopManager shopManager;
    private Camera mainCamera;
    private List<BlockScript> blocks;
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
        for (int i = 0; i < 5; i++)
        {
            // Generate random position within the camera view on the 0 plane
            Vector3 randomPosition = new Vector3(
                Random.Range(-mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * mainCamera.aspect),
                Random.Range(-mainCamera.orthographicSize, mainCamera.orthographicSize),
                0f
            );

            // Randomly choose a block from the list
            BlockScript randomBlock = blocks[Random.Range(0, blocks.Count)];

            // Instantiate the chosen block at the random position
            Instantiate(randomBlock.gameObject, randomPosition, Quaternion.identity);
        }
    }
}
