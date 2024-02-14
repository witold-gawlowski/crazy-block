using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private Dictionary<Vector2Int, GameObject> levelMap;
    private HashSet<Vector2Int> blockedCoords;
    private HashSet<GameObject> placedBlocks;
    private int mapSize;

    private int _coveredArea;
    private int CoveredArea { get { return _coveredArea; } set { _coveredArea = value; CheckForLevelCompleted(); } }


    public void Init()
    {
        levelMap = new Dictionary<Vector2Int, GameObject>();
        blockedCoords = new HashSet<Vector2Int>();
        placedBlocks = new HashSet<GameObject>();
        mapSize = 0;
        _coveredArea = 0;

        var levelTiles = GlobalGameManager.Instance.GetCurrentMapObject().transform;
        foreach (Transform tile in levelTiles)
        {
            levelMap.TryAdd(Helpers.RoundPosition(tile.position), tile.gameObject);
            mapSize++;
        }
    }

    public void Place(GameObject block, Vector2Int coordinates)
    {
        var script = block.GetComponent<BlockScript>();

        foreach (Vector2Int tileCoords in Helpers.GetBlockEnumerator(block, coordinates))
        {
            blockedCoords.Add(tileCoords);
        }

        placedBlocks.Add(block);
        CoveredArea = CoveredArea + script.GetSize();
    }


    public void Remove(GameObject block)
    {
        var script = block.GetComponent<BlockScript>();

        foreach (Transform tileTransform in block.transform)
        {
            if (tileTransform != block.transform)
            {
                var coords = Helpers.RoundPosition(tileTransform.position); //TODO: fix relative positon
                blockedCoords.Remove(coords);
            }
        }

        placedBlocks.Remove(block);
        CoveredArea = CoveredArea - script.GetSize();
    }

    public bool CanBePlaced(GameObject block, Vector2Int coordinates)
    {
        foreach (Vector2Int tileCoords in Helpers.GetBlockEnumerator(block, coordinates))
        {
            if (!levelMap.ContainsKey(tileCoords) || blockedCoords.Contains(tileCoords))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsPlaced(GameObject block)
    {
        return placedBlocks.Contains(block);
    }

    private void CheckForLevelCompleted()
    {
        if (_coveredArea == mapSize)
        {
            GlobalGameManager.Instance.FinalizeLevel();
        }
    }
}
