using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public interface IWeighted
{
    public float GetWeight();
}

public class BlockScript : MonoBehaviour, IWeighted
{
    [SerializeField] private float targetColorV = 0.45f;
    [SerializeField] private float destroyDelay = 3f;
    [FormerlySerializedAs("price")] [SerializeField] private int initialPrice = 10;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private Color placedCOlor;
    [SerializeField] private Color deadColor;
    [SerializeField] private Color initialColor;
    [SerializeField] private float maxLifeTime = 90;
    [SerializeField] private float averageLifeTime = 45;
    [SerializeField] private float lifeTimeVariance = 10;


    private GameObject _pivot;

    private List<SpriteRenderer> srs;

    private int _size;

    private float lifeTime;
    private float timeElapsed;

    private bool isAlive;
    private bool isBought;
    private bool isPlaced;

    private void Start()
    {
        timeElapsed = 0;
        isAlive = true;
        isBought = false;
        isPlaced = false;
    }

    private void Update()
    {
        if (isBought && !isPlaced)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < lifeTime)
            {
                AddTint();
            }
        }

        if(timeElapsed > lifeTime && isAlive)
        {
            isAlive = false;
            StartCoroutine(Die());
            DragManager.Instance.HandleBlockDead(this);
        }

    }

    public void Init()
    {
        srs = GetComponentsInChildren<SpriteRenderer>().ToList<SpriteRenderer>();
        _size = srs.Count;

        initialColor = srs[0].color;

        CenterPivot();

        RandomizeLifeTimne();

        AddTint();
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
        return ShopManager.Instance.GetPrice(lifeTime, initialPrice);
    }

    public float GetWeight()
    {
        return frequency;
    }

    public bool IsBought()
    {
        return isBought;
    }

    public bool IsDead()
    {
        return !isAlive;
    }

    public void ProcessPurchase()
    {
        isBought = true;
        MoveToBack();
    }

    public void ProcessPlacement()
    {
        isPlaced = true;
        UpdateColorToPlaced();
    }

    private IEnumerator Die()
    {
        foreach (var r in srs)
        {
            r.color = deadColor;
        }

        yield return new WaitForSeconds(3.0f);

        float fadeOutDuration = 3f;
        float startTime = Time.time;
        var fadeoutCOlor = deadColor;
        while (Time.time - startTime < fadeOutDuration)
        {
            float fadeoutProgress = Mathf.Clamp01((Time.time - startTime) / fadeOutDuration);
            fadeoutCOlor.a = 1 - fadeoutProgress;
            foreach (var r in srs)
            {
                r.color = fadeoutCOlor; 
            }
            yield return new WaitForSeconds(0.1f);
        }

        // Destroy the entire object
        if (MapManager.GetInstance().IsPlaced(this.gameObject))
        {
            MapManager.GetInstance().Remove(this.gameObject);
        }

        Destroy(gameObject);
    }

    private void RandomizeLifeTimne()
    {
        lifeTime = Mathf.Clamp(GlobalGameManager.Instance.GetBlockLifeLength() + Random.Range(-lifeTimeVariance, lifeTimeVariance), 10, 180);
    }

    private void UpdateColorToPlaced()
    {
        foreach (var item in srs)
        {
            item.color = placedCOlor;
        }
    }

    private void AddTint()
    {
        float colorH, initColorV, colorS;
        var untintedColor = GetUntintedColor();
        Color.RGBToHSV(untintedColor, out colorH, out colorS, out initColorV);

        float timeLeft = lifeTime - timeElapsed;
        float decayParam = 1 - timeLeft / maxLifeTime;
        float newColorV = Mathf.Lerp(initColorV, targetColorV, decayParam);
        Color newCOlor = Color.HSVToRGB(colorH, colorS, newColorV);

        foreach (var item in srs)
        {
            item.color = newCOlor;
        }
    }

    private Color GetUntintedColor()
    {        
        return initialColor;
    }

    private void MoveToBack()
    {
        foreach (var item in srs)
        {
            item.sortingOrder = 1;
        }
    }
}
