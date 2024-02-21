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
            if(_instance != null )
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
    [SerializeField] private int rerollPrice = 10;
    [SerializeField] private AnimationCurve lifetimeToPrice;

    public System.Action<int, int, bool> CashChanged;

    public int Cash { get { return _cash; } private set { _oldCash = _cash; _cash = value; CashChanged(_cash, _oldCash, CanAffordReroll()); } }

    private int _cash;
    private int _oldCash;

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

    private void OnEnable()
    {
        ui.RerollPressedEvent += HandleRerollPressedEvent;
    }

    private void OnDisable()
    {
        ui.RerollPressedEvent -= HandleRerollPressedEvent;
    }

    private void Start()
    {
        Reroll2();
        ResetCash();
    }

    public void HandleNewLevel()
    {
        spammer.HandleNewLevel();
    }

    public void HandleRerollPressedEvent()
    {
        Reroll2();
        Cash = Cash - rerollPrice;
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
        return Mathf.RoundToInt(lifetimeToPrice.Evaluate(lifeTime) * initialPrice);
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
        return rerollPrice <= Cash;
    }

    public void Reroll2()
    {
        CleanOffer2();

        foreach (Transform t in positionMarkers)
        {
            var prefabScript = Helpers.GetRandomWeightedElement<BlockScript>(blockPrefabs);
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

        var offerObjects = offer.Select(e => e._instance.GetComponent<BlockScript>()).ToList();
        ui.HandleNewShopOffer(offerObjects, Cash);
    }

    private void Respawn(OfferElement e)
    {
        var blockObj = Instantiate(e._prefab);
        var blockScript = blockObj.GetComponent<BlockScript>();

        blockScript.Init(false, e._lifeTime, e._color);
        blockScript.SetPosition(e._shopMarker.position);
        e._instance = blockObj;

        var offerObjects = offer.Select(e => e._instance.GetComponent<BlockScript>()).ToList();
        ui.HandleNewShopOffer(offerObjects, Cash);
    }

    public void CleanOffer2()
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
