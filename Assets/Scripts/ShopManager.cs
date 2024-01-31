using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopUIScript ui;
    [SerializeField] private List<GameObject> blockPrefabs;
    [SerializeField] private List<Transform> positionMarkers;

    [SerializeField] private int initialCash = 100;
    [SerializeField] private int rerollPrice = 10;

    public System.Action<int, bool> CashChanged;

    public int Cash { get { return _cash; } private set { _cash = value; CashChanged(_cash, CanAffordReroll()); } }

    private int _cash;
    private List<BlockScript> offer;
    private BlockScript draggedScript;

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
            var prefab = Helpers.GetRandomElement<GameObject>(blockPrefabs);
            var block = Instantiate(prefab, t.transform.position, Quaternion.identity);
            var script = block.GetComponent<BlockScript>();
            offer.Add(script);
        }

        ui.HandleNewShopOffer(offer, Cash);
    }

    public void HandleBlockDrag(GameObject block)
    {
        if(draggedScript == null || draggedScript.gameObject != block)
        {
            draggedScript = block.GetComponent<BlockScript>();
        }

        if(!IsBought(draggedScript))
        {
            var mask = LayerMask.GetMask(new string[] { "Respawn" });
            if (!Physics2D.OverlapPoint(block.transform.position, mask))
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
        return !offer.Contains(draggedScript);
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
