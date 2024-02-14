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

    private List<Map> maps;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        maps = new List<Map>();
    }

    private void Start()
    {
        AddCleanMap();
    }

    public static MapManager GetInstance()
    {
        return instance;
    }

    public void AddCleanMap()
    {
        var newMap = new Map();
        newMap.Init();
        maps.Add(newMap);
    }

    public void DestroyLastMap()
    {
        maps.RemoveAt(maps.Count - 1);
    }

    public void Place(GameObject block, Vector2Int coordinates)
    {
        maps.Last().Place(block, coordinates);
    }

    public void Remove(GameObject block)
    {
        maps.Last().Remove(block);
    }

    public bool CanBePlaced(GameObject block, Vector2Int coordinates)
    {
        return maps.Last().CanBePlaced(block, coordinates);
    }

    public bool IsPlaced(GameObject block)
    {
        return maps.Last().IsPlaced(block);
    }
}
