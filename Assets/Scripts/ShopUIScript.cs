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
    [SerializeField] private TMP_Text levelNuberText;

    [SerializeField] private Color availableColor;
    [SerializeField] private Color unavailableColor;
    [SerializeField] private float targetIncreasedCashSize = 85;
    [SerializeField] private float targetDecreasedCashSize = 50;

    private float regularCashTextSize;

    public Action RerollPressedEvent;

    private void Awake()
    {
        shopUIItems = new List<ShopUIItem>();
        foreach(var text in priceTMPs)
        {
            var newUIItem = new ShopUIItem() { priceText = text};
            shopUIItems.Add(newUIItem);
        }

        regularCashTextSize = totalCashText.fontSize;
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

    private IEnumerator AnimateCashCoroutine(int current, int target)
    {
        int diff = current < target ? 1 : -1;
        float baseInterval = 0.02f;

        int i = current;

        float newCashTExtSize = regularCashTextSize;
        do
        {
            newCashTExtSize += (diff > 0 ? 0.5f : -0.5f);
            SetCashTextSize(Mathf.Clamp(newCashTExtSize, targetDecreasedCashSize, targetIncreasedCashSize));
            i += diff;
            SetCashText(i);
            yield return new WaitForSeconds(baseInterval);
        } while (i != target);

        yield return new WaitForSeconds(0.5f);

        float cashTExtSize = totalCashText.fontSize;
        for(float j=0; j<1; j += 0.1f)
        {
            float newCashTextSize = Mathf.Lerp(cashTExtSize, regularCashTextSize, j);
            SetCashTextSize(newCashTextSize);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    private void HandleCashChanged(int value, int oldValue, bool canAffordReroll)
    {
        UpdateBlockTextColors(value);
        StartCoroutine(AnimateCashCoroutine(oldValue, value));
        UpdateRerollButton(canAffordReroll);
    }

    public void HandleLevelCompleted(int cash)
    {
        mapCompletedPanel.SetActive(true);
        rewardText.text = "+$" + cash;
      
    }

    public void HandleNewLevel(int level)
    {
        mapCompletedPanel.SetActive(false);
        levelNuberText.text = "Level " + level;
    }
    

    public void HandleRerollPress()
    {
        RerollPressedEvent();
    }

    public void SetCashText(int value)
    {
        totalCashText.text = "$" + value.ToString();
    }

    public void SetCashTextSize(float value)
    {
        Debug.Log(value);
        totalCashText.fontSize = value;
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
            rerollText.color = availableColor;
        }
        else
        {
            rerollButton.interactable = false;
            rerollText.color = unavailableColor;
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
                    item.priceText.color = availableColor;
                }
                else
                {
                    item.priceText.color = unavailableColor;
                }
            }
        }
    }
}
