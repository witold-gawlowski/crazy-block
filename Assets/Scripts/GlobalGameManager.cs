using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private SuccessionNode staterMapNode;

    [SerializeField] private ShopUIScript ui;

    [SerializeField] private Color mapColor;

    private GameObject _currentMapObj;
    private SuccessionNode _currentMapNode;

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
    }

    private void Start()
    {
        HandleNewGame();
    }

    public void HandleNewGame()
    {
        _currentMapNode = staterMapNode;
        LoadCurrentNode();
    }

    //public void HandleMapCompleted()
    //{
    //    ClearSnappedBlocks();

    //    Destroy(_currentMapObj);

    //    ShopManager.Instance.AddCash(_currentMapNode.GetReward());

    //    _currentMapNode = _currentMapNode.GetNext();
    //    LoadCurrentNode();

    //    ui.HandleLevelCompleted(_currentMapNode.GetReward());

    //    MapManager.GetInstance().Init();
    //}

    public void FinalizeLevel()
    {
        StartCoroutine(FinalizeLevelCOrouitne());
    }

    private IEnumerator FinalizeLevelCOrouitne()
    {
        ui.HandleLevelCompleted(_currentMapNode.GetReward());

        yield return new WaitForSeconds(2);
        StartNewLevel();
    }

    private void StartNewLevel()
    {

        ui.HandleNewLevel();

        ClearSnappedBlocks();

        Destroy(_currentMapObj);

        ShopManager.Instance.AddCash(_currentMapNode.GetReward());

        _currentMapNode = _currentMapNode.GetNext();
        LoadCurrentNode();

        MapManager.GetInstance().Init();
    }

    public GameObject GetCurrentMapObject()
    {
        return _currentMapObj;
    }

    private void LoadCurrentNode()
    {
        _currentMapObj = Instantiate(_currentMapNode.GetPrefab());
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

    private void ClearSnappedBlocks()
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
