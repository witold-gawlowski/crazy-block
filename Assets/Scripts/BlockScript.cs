using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public interface IWeighted
{
    public float GetWeight();
}

public class BlockScript : MonoBehaviour, IWeighted
{
    [SerializeField] private float targetColorV = 0.45f;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float destroyDelay = 3f;
    [SerializeField] private int price = 10;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float prePurchaseAlphssa = 0.6f;
    [SerializeField] private Color placedCOlor;

    private GameObject _pivot;

    private List<SpriteRenderer> srs;

    private int _size;

    private float colorH;
    private float initColorV;
    private float colorS;
    private float timeProgress;

    private bool isAlive;
    private bool isBought;
    private bool isPlaced;


    private void Awake()
    {

    }

    private void Start()
    {
        timeProgress = 0;
        isAlive = true;
        isBought = false;
        isPlaced = false;
        UpdateToShopColor();
    }

    private void Update()
    {
        if (isBought && !isPlaced)
        {
            timeProgress += Time.deltaTime;

            if (timeProgress < lifeTime)
            {
                UpdateColor();
            }
        }

        if(timeProgress > lifeTime && isAlive)
        {
            isAlive = false;
            StartCoroutine(BlinkAndDestroyCoroutine());
        }

    }

    public void Init()
    {
        srs = GetComponentsInChildren<SpriteRenderer>().ToList<SpriteRenderer>();
        _size = srs.Count;

        CenterPivot();

        var baseRGB = srs[0].color;
        Color.RGBToHSV(baseRGB, out colorH, out colorS, out initColorV);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos - _pivot.transform.localPosition;
    }

    private void CenterPivot()
    {
        foreach (Transform t in transform)
        {
            if (t.tag == "Pivot")
            {
                _pivot = t.gameObject;
                break;
            }
        }

        Vector3 middle = Vector3.zero;
        foreach (SpriteRenderer r in srs)
        {
            middle += r.transform.position;
        }
        _pivot.transform.position = middle / _size;
    }

    public int GetSize()
    {
        return _size;
    }

    public Transform GetPivot()
    {
        return _pivot.transform;
    }

    public bool IsOverShop()
    {
        var mask = LayerMask.GetMask(new string[] { "Respawn" });
        if (Physics2D.OverlapPoint(transform.position, mask))
        {
            return true;
        }
        return false;
    }

    public Vector2 GetMiddleOffset()
    {
        return _pivot.transform.position - transform.position;
    }

    public int GetPrice()
    {
        return price;
    }

    public float GetWeight()
    {
        return frequency;
    }

    public bool IsBought()
    {
        return isBought;
    }

    public void ProcessPurchase()
    {
        UpdateCOlorToPurchased();
        isBought = true;
        MoveToBack();
    }

    public void ProcessPlacement()
    {
        isPlaced = true;
        UpdateColorToPlaced();
    }

    private IEnumerator BlinkAndDestroyCoroutine()
    {
        // Blinking effect
        float startTime = Time.time;
        while (Time.time - startTime < blinkDuration)
        {
            foreach (SpriteRenderer sr in srs)
            {
                sr.enabled = !sr.enabled;
            }

            yield return new WaitForSeconds(0.2f); // Adjust blink speed if needed
        }

        // Ensure SpriteRenderers are enabled before destroying the object
        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = true;
        }

        // Delay before destroying the object
        yield return new WaitForSeconds(destroyDelay);

        // Destroy the entire object
        if (MapManager.GetInstance().IsPlaced(this.gameObject))
        {
            MapManager.GetInstance().Remove(this.gameObject);
        }

        Destroy(gameObject);
    }
    
    private void UpdateCOlorToPurchased()
    {
        Color newColor = srs[0].color;
        newColor.a = 1;
        foreach (var item in srs)
        {
            item.color = newColor;
        }
    }

    private void UpdateColorToPlaced()
    {
        foreach (var item in srs)
        {
            item.color = placedCOlor;
        }
    }

    private void UpdateToShopColor()
    {
        Color newColor = srs[0].color;
        newColor.a = prePurchaseAlphssa;
        foreach (var item in srs)
        {
            item.color = newColor;
        }
    }

    private void UpdateColor()
    {
        float t = timeProgress / lifeTime;
        float newColorV = Mathf.Lerp(initColorV, targetColorV, t);
        Color newCOlor = Color.HSVToRGB(colorH, colorS, newColorV);

        foreach (var item in srs)
        {
            item.color = newCOlor;
        }
    }

    private void MoveToBack()
    {
        foreach (var item in srs)
        {
            item.sortingOrder = 1;
        }
    }
}
