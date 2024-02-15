using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapGenerator : MonoBehaviour, MapGeneratorInterface
{

    [SerializeField] private SuccessionNode staterMapNode;

    private SuccessionNode _currentMapNode;

    public GameObject GetCurrent()
    {
        return _currentMapNode.GetPrefab();
    }

    public int GetCurrentReward()
    {
        return 200;
    }

    public void Init()
    {
        _currentMapNode = staterMapNode;
    }

    public void Next()
    {
        _currentMapNode = _currentMapNode.GetNext();
    }

}
