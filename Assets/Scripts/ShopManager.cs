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

    private List<BlockScript> offer;
    private List<OfferElement> offer2;
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
        Reroll();
        ResetCash();
    }

    public void HandleNewLevel()
    {
        spammer.HandleNewLevel();
    }

    public void HandleRerollPressedEvent()
    {
        Reroll();
        Cash = Cash - rerollPrice;
    }

    public void HandleBlockRelease(BlockScript draggedScript)
    {
        if (!IsBought(draggedScript))
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

    public void Reroll()
    {
        if (offer != null)
        {
            foreach (var item in offer)
            {
                Destroy(item.gameObject);
            }
        }

        offer = new List<BlockScript>();

        foreach (Transform t in positionMarkers)
        {
            var prefabScript = Helpers.GetRandomWeightedElement<BlockScript>(blockPrefabs);
            var block = Instantiate(prefabScript);
            var script = block.GetComponent<BlockScript>();
            script.Init();
            script.SetPosition(t.transform.position);

            offer.Add(script);
        }

        ui.HandleNewShopOffer(offer, Cash);
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
                _price = blockScript.GetPrice()
            };

            offer2.Add(newOfferElem);
        }

        var offerObjects = offer2.Select(e => e._instance.GetComponent<BlockScript>()).ToList();
        ui.HandleNewShopOffer(offerObjects, Cash);
    }

    public void CleanOffer2()
    {
        if (offer2 != null)
        {
            foreach (var item in offer2)
            {
                item.DestroyInstance();
            }
        }

        offer2 = new List<OfferElement>();
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
        return !offer.Contains(blockScript);
    }

    public void Buy(BlockScript block)
    {
        Cash = Cash - block.GetPrice();
        offer.Remove(block);
        ui.HandleItemBought(block);
        block.ProcessPurchase();

        if (offer.Count == 0)
        {
            Reroll();
        }
    }
}
