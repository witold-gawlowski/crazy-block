using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private ShopUIScript ui;

    [SerializeField] private Color mapColor;

    [SerializeField] private GameObject mapGeneratorObject;

    [SerializeField] private AnimationCurve blockLifeLengthByLevel;

    private MapGeneratorInterface mapGenerator;
    private GameObject _currentMapObj;

    private int level;
    public static GlobalGameManager Instance { get; private set; }

    private float restartCooldown;

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
        restartCooldown = 0;
    }

    private void Start()
    {
        level = 0;
        mapGenerator.Init();
        StartNewLevel();
    }

    private void Update()
    {
        UpdateRestart();
    }

    private void UpdateRestart()
    {
        if (restartCooldown > 0)
        {
            restartCooldown -= Time.deltaTime;
        }

        if (restartCooldown <= 0)
        {
            ui.ShowRestartVisible(false);
        }
    }

    public void HandleRestart()
    {
        if(restartCooldown > 0)
        {
            {
                level = 0;
                DestroyLooseBlocks();
                mapGenerator.Init();
                StartNewLevel();
                ShopManager.Instance.Reroll();
                ShopManager.Instance.ResetCash();
            }
            restartCooldown = 0;
            return;
        }
        ui.ShowRestartVisible(true);
        restartCooldown += 3.0f;
    }

    public void FinalizeLevel()
    {
        StartCoroutine(FinalizeLevelCOrouitne());
    }

    public float GetBlockLifeLength()
    {
        return blockLifeLengthByLevel.Evaluate(level);
    }

    private IEnumerator FinalizeLevelCOrouitne()
    {
        ui.HandleLevelCompleted(mapGenerator.GetCurrentReward());

        yield return new WaitForSeconds(2);

        ShopManager.Instance.AddCash(mapGenerator.GetCurrentReward());
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        level++;

        ui.HandleNewLevel(level);

        DestroyPlacedBLocks();

        Destroy(_currentMapObj);

        mapGenerator.Next();

        LoadCurrentNode();

        MapManager.GetInstance().AddCleanMap();

        ShopManager.Instance.HandleNewLevel();
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

    private void DestroyLooseBlocks()
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in blocks)
        {
            var script = block.GetComponent<BlockScript>();
            if (script.IsBought() && !MapManager.GetInstance().IsPlaced(block))
            {
                Destroy(block);
            }
        }
    }

    private void DestroyPlacedBLocks()
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
