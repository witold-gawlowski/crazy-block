using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIScript : MonoBehaviour
{
    [System.Serializable]
    private class ShopUIItem
    {
        public BlockScript blockScript;
        public TMP_Text priceText;
    }

    [SerializeField] private List<TMP_Text> priceTMPs;
    [SerializeField] private List<ShopUIItem> shopUIItems;
    [SerializeField] private TMP_Text totalCashText;
    [SerializeField] private TMP_Text rerollText;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject mapCompletedPanel;


    public Action RerollPressedEvent;

    private void Awake()
    {
        shopUIItems = new List<ShopUIItem>();
        foreach(var text in priceTMPs)
        {
            var newUIItem = new ShopUIItem() { priceText = text};
            shopUIItems.Add(newUIItem);
        }
    }


    void Update()
    {
        foreach (ShopUIItem item in shopUIItems)
        {
            if (item.priceText.gameObject.activeSelf)
            {
                var test = item.blockScript.transform.position;
                item.priceText.rectTransform.position = test;
            }
        }
    }

    private void OnEnable()
    {
        ShopManager.Instance.CashChanged += HandleCashChanged;
    }

    private void OnDisable()
    {
        ShopManager.Instance.CashChanged -= HandleCashChanged;
    }

    private void HandleCashChanged(int value, bool canAffordReroll)
    {
        UpdateBlockTextColors(value);
        SetCashText(value);
        UpdateRerollButton(canAffordReroll);
    }

    public void HandleLevelCompleted(int cash)
    {
        mapCompletedPanel.SetActive(true);
        rewardText.text = "+$" + cash;  
    }

    public void HandleNewLevel()
    {
        mapCompletedPanel.SetActive(false);
    }
    

    public void HandleRerollPress()
    {
        RerollPressedEvent();
    }

    public void SetCashText(int value)
    {
        totalCashText.text = "$" + value.ToString();
    }

    public void HandleItemBought(BlockScript block)
    {
        foreach(var item in shopUIItems)
        {
            if(item.blockScript == block)
            {
                item.blockScript = null;
                item.priceText.gameObject.SetActive(false);
            }
        }
    }

    public void HandleNewShopOffer(List<BlockScript> newOffer, int cash)
    {
        foreach (var (block, item) in newOffer.Zip(shopUIItems, (a, b) => (a, b)))
        {
            item.blockScript = block;
            item.priceText.text = "$"+block.GetPrice();
            item.priceText.gameObject.SetActive(true);
        }

        UpdateBlockTextColors(cash);
    }

    private void UpdateRerollButton(bool canAffordReroll)
    {
        if(canAffordReroll)
        {
            rerollButton.interactable = true;
            rerollText.color = Color.white;
        }
        else
        {
            rerollButton.interactable = false;
            rerollText.color = Color.red;
        }
    }

    private void UpdateBlockTextColors(int currentCash)
    {
        foreach(ShopUIItem item in shopUIItems)
        {
            if (item.blockScript != null)
            {
                var price = item.blockScript.GetPrice();
                if (price <= currentCash)
                {
                    item.priceText.color = Color.white;
                }
                else
                {
                    item.priceText.color = Color.red;
                }
            }
        }
    }
}
