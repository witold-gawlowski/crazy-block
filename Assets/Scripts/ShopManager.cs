using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    class OfferElement
    {
        public void DestroyInstance()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
        }

        public GameObject _prefab;
        public float _lifeTime;
        public int _price;
        public GameObject _instance;
        public Transform _shopMarker;
        public Color _color;
    }

    [SerializeField] private ShopUIScript ui;
    [SerializeField] private List<BlockScript> blockPrefabs;
    [SerializeField] private List<Transform> positionMarkers;

    [SerializeField] private int initialCash = 100;
    [SerializeField] private int baseRerollPrice = 10;
    [SerializeField] private AnimationCurve lifetimeToPrice;
    [SerializeField] private float debetDuration = 15;

    public System.Action<int, int, bool> CashChanged;

    public int Cash { get { return _cash; } private set { _oldCash = _cash; _cash = value; CashChanged(_cash, _oldCash, CanAffordReroll()); } }

    private int _cash;
    private int _oldCash;
    private bool _debetCountdownStarted;
    private float _debetCountdownStartTime;

    private List<OfferElement> offer;
    private BlockSpammer spammer;

    public static ShopManager Instance { get; private set; }
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
        spammer = GetComponentInChildren<BlockSpammer>();
    }

    private void Update()
    {
        if (!GlobalGameManager.Instance.IsPaused())
        {
            UpdateDebetTimer();
        }
    }

    public void Init()
    {
        Reroll();
        ResetCash();
    }

    private void OnEnable()
    {
        ui.RerollPressedEvent += HandleRerollPressedEvent;
    }

    private void OnDisable()
    {
        ui.RerollPressedEvent -= HandleRerollPressedEvent;
    }

    public void HandleNewLevel()
    {
        spammer.HandleNewLevel();
        ui.HandleNewShopOffer(Cash, GetRerollPrice());
    }

    public void HandleRerollPressedEvent()
    {
        if (!GlobalGameManager.Instance.IsPaused())
        {
            Reroll();
            Cash = Cash - GetRerollPrice();
        }
    }

    public void HandleRestart()
    {
        ResetCash();
        Reroll();
        _debetCountdownStarted = false;
    }

    public void HandleBlockRelease(BlockScript draggedScript)
    {
        if (!draggedScript.IsBought())
        {
            if (!draggedScript.IsOverShop())
            {
                Buy(draggedScript);
            }
        }
    }

    public List<BlockScript> GetAllBlocks()
    {
        return blockPrefabs;
    }

    public int GetPrice(float lifeTime, int initialPrice)
    {
        var level = GlobalGameManager.Instance.GetLevel();
        return initialPrice * Mathf.RoundToInt(Mathf.Pow(1.1f, level + 3));
    }

    public int GetRerollPrice()
    {
        var level = GlobalGameManager.Instance.GetLevel();
        var targetPrice = baseRerollPrice * Mathf.RoundToInt(Mathf.Pow(1.1f, level - 1));
        return (targetPrice + 24) / 25 * 25;
    }

    public void ResetCash()
    {
        Cash = initialCash;
    }

    public void AddCash(int value)
    {
        Cash = Cash + value;
    }

    public bool CanAffordReroll()
    {
        return GetRerollPrice() <= Cash;
    }

    public void Reroll()
    {
        CleanOffer();

        var offerItems = Helpers.GetRandomWeightedSubset(blockPrefabs, positionMarkers.Count);
        foreach ((Transform t, BlockScript prefabScript) in positionMarkers.Zip(offerItems, (a, b) => (a, b)))
        {
            var blockObject = Instantiate(prefabScript.gameObject);
            var blockScript = blockObject.GetComponent<BlockScript>();

            blockScript.Init();
            blockScript.SetPosition(t.transform.position);

            var newOfferElem = new OfferElement()
            {
                _instance = blockObject,
                _lifeTime = blockScript.GetLifeTime(),
                _prefab = prefabScript.gameObject,
                _price = blockScript.GetPrice(),
                _shopMarker = t,
                _color = blockScript.GetInitialColor()
            };

            offer.Add(newOfferElem);
        }

        ui.HandleNewShopOffer(Cash, GetRerollPrice());

        SoundManager.Instance.PlaySwithc();
    }

    private void Respawn(OfferElement e)
    {
        var blockObj = Instantiate(e._prefab);
        var blockScript = blockObj.GetComponent<BlockScript>();

        blockScript.Init(false, e._lifeTime, e._color);
        blockScript.SetPosition(e._shopMarker.position);
        e._instance = blockObj;

        ui.HandleNewShopOffer(Cash, GetRerollPrice());
    }

    public void CleanOffer()
    {
        if (offer != null)
        {
            foreach (var item in offer)
            {
                item.DestroyInstance();
            }
        }

        offer = new List<OfferElement>();
    }

    public List<BlockScript> GetOfferBlockScripts()
    {
        return offer.Select(e => e._instance.GetComponent<BlockScript>()).ToList();
    }

    public bool CanBeBought(BlockScript block)
    {
        if(block.GetPrice() <= _cash)
        {
            return true;
        }
        return false;
    }

    private bool IsBought(BlockScript blockScript)
    {
        return blockScript.IsBought();
    }

    private void UpdateDebetTimer()
    {
        if (Cash < 0)
        {
            if (_debetCountdownStarted)
            {
                var remainingTIme = debetDuration - (Time.time - _debetCountdownStartTime);

                if(remainingTIme > 0)
                {
                    ui.SetDebetCountdownValue(remainingTIme);
                }
                else
                {
                    GlobalGameManager.Instance.GameOver();
                    SoundManager.Instance.PlayTickTock(false);
                }               
            }
            else
            {
                _debetCountdownStartTime = Time.time;
                ui.SetDebetCountdonwVisible(true);
                SoundManager.Instance.PlayTickTock(true);
                _debetCountdownStarted = true;
            }

        }
        else
        {
            if (_debetCountdownStarted)
            {
                ui.SetDebetCountdonwVisible(false);
                _debetCountdownStarted = false;
                SoundManager.Instance.PlayTickTock(false);
            }
        }
    }

    private void HandleBlockBought(BlockScript block)
    {
        foreach (var e in offer)
        {
            if (e._instance == block.gameObject)
            {
                Respawn(e);
            }
        }
    }

    public void Buy(BlockScript block)
    {
        Cash = Cash - block.GetPrice();
        HandleBlockBought(block);
        ui.HandleItemBought(block);
        block.ProcessPurchase();
    }
}
