using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
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
        Cash = initialCash;
    }

    public int GetPrice(float lifeTime, int initialPrice)
    {
        return Mathf.RoundToInt(lifetimeToPrice.Evaluate(lifeTime) * initialPrice);
    }

    public void HandleRerollPressedEvent()
    {
        Reroll();
        Cash = Cash - rerollPrice;
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
            var prefab = Helpers.GetRandomWeightedElement<BlockScript>(blockPrefabs);
            var block = Instantiate(prefab);
            var script = block.GetComponent<BlockScript>();
            script.Init();
            script.SetPosition(t.transform.position);


            offer.Add(script);
        }

        ui.HandleNewShopOffer(offer, Cash);
    }

    public void HandleBlockRelease(BlockScript draggedScript)
    {
        if (!IsBought(draggedScript))
        {
            if(!draggedScript.IsOverShop())
            {
                Buy(draggedScript);
            }
        }
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
        if(offer.Count == 0)
        {
            Reroll();
        }
    }
}
