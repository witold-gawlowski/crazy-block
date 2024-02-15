using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private ShopUIScript ui;

    [SerializeField] private Color mapColor;

    [SerializeField] private GameObject mapGeneratorObject;

    private MapGeneratorInterface mapGenerator;
    private GameObject _currentMapObj;

    private int level;
    public static GlobalGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        mapGenerator = mapGeneratorObject.GetComponent<MapGeneratorInterface>();
    }

    private void Start()
    {
        HandleNewGame();
    }

    public void HandleNewGame()
    {
        mapGenerator.Init();
        LoadCurrentNode();
        level = 1;
    }

    public void FinalizeLevel()
    {
        StartCoroutine(FinalizeLevelCOrouitne());
    }

    private IEnumerator FinalizeLevelCOrouitne()
    {
        ui.HandleLevelCompleted(mapGenerator.GetCurrentReward());

        yield return new WaitForSeconds(2);
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        level++;

        ui.HandleNewLevel(level);

        DestroyBlocksFromTopLevel();

        Destroy(_currentMapObj);

        ShopManager.Instance.AddCash(mapGenerator.GetCurrentReward());

        mapGenerator.Next();

        LoadCurrentNode();

        MapManager.GetInstance().AddCleanMap();
    }

    public GameObject GetCurrentMapObject()
    {
        return _currentMapObj;
    }

    private void LoadCurrentNode()
    {
        _currentMapObj = Instantiate(mapGenerator.GetCurrent());
        UpdateMapColor();
    }
    
    private void UpdateMapColor()
    {
        var srs = _currentMapObj.GetComponentsInChildren<SpriteRenderer>();
        foreach(var r in srs)
        {
            r.color = mapColor;
        }
    }

    private void DestroyBlocksFromTopLevel()
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in blocks)
        {
            var script = block.GetComponent<BlockScript>();
            if (script.IsBought() && MapManager.GetInstance().IsPlaced(block))
            {
                Destroy(block);
            }
        }
    }
}
