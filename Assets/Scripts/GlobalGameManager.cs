using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private SuccessionNode staterMapNode;

    [SerializeField] private ShopUIScript ui;

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

    public void HandleMapCompleted()
    {
        ClearSnappedBlocks();

        Destroy(_currentMapObj);

        ShopManager.Instance.AddCash(_currentMapNode.GetReward());

        _currentMapNode = _currentMapNode.GetNext();
        LoadCurrentNode();

        ui.HandleLevelCompleted(_currentMapNode.GetReward());

        MapManager.GetInstance().Init();
    }

    public GameObject GetCurrentMapObject()
    {
        return _currentMapObj;
    }

    private void LoadCurrentNode()
    {
        _currentMapObj = Instantiate(_currentMapNode.GetPrefab());
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
