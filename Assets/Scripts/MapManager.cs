using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    private Map map;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    public static MapManager GetInstance()
    {
        return instance;
    }

    public void Init()
    {
        map = new Map();
        map.Init();
    }

    public void Place(GameObject block, Vector2Int coordinates)
    {
        map.Place(block, coordinates);
    }


    public void Remove(GameObject block)
    {
        map.Remove(block);
    }

    public bool CanBePlaced(GameObject block, Vector2Int coordinates)
    {
        return map.CanBePlaced(block, coordinates);
    }

    public bool IsPlaced(GameObject block)
    {
        return map.IsPlaced(block);
    }
}
