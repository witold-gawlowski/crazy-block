using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockClock : MonoBehaviour
{
    private Image image;
    private RectTransform rt;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
        rt  = gameObject.GetComponent<RectTransform>();
    }

    public void UpdateImageFill(float value)
    {
        image.fillAmount = value;
    }

    public void UpdatePoistion(Transform blockT)
    {
        rt.position = blockT.position;
    }
}
