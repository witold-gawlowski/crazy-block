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
    [SerializeField] private float lifeTimeVariance = 10;
    [SerializeField] private float clockAppearenceInterval = 15;
    [SerializeField] private GameObject clockPrefab;
    [SerializeField] private BlockColorList blockColorList;

    static int order = 5;

    private GameObject _pivot;
    private BlockClock _clock;
    

    private List<SpriteRenderer> srs;
    private List<Collider2D> colliders;

    private int _size;

    private float _lifeTime;
    private float timeElapsed;

    private bool isAlive;
    private bool isBought;
    private bool isPlaced;

    private void Update()
    {
        if (isBought && !isPlaced)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < _lifeTime)
            {
                AddTint();
            }

            if(_clock == null && _lifeTime - timeElapsed < clockAppearenceInterval)
            {
                var canvas = GlobalGameManager.Instance.GetShopUIObj().transform;
                var clockObj = Instantiate(clockPrefab, canvas);
                _clock = clockObj.GetComponent<BlockClock>();
            }
        }

        if (_clock)
        {
            _clock.UpdateImageFill((_lifeTime - timeElapsed) / clockAppearenceInterval);
            _clock.UpdatePoistion(transform);
        }

        if (timeElapsed > _lifeTime && isAlive)
        {
            isAlive = false;
            StartCoroutine(Die());
            DragManager.Instance.HandleBlockDead(this);
        }

    }

    public void OnDestroy()
    {
        if (_clock)
        {
            Destroy(_clock.gameObject);
        }
    }   


    public void Init(bool alreadyBought = false, float lifeTime = -1, object color = null)
    {
        srs = GetComponentsInChildren<SpriteRenderer>().ToList<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>().ToList<Collider2D>();
        SetSrLayerOrder();
        _size = srs.Count;

        initialColor = srs[0].color;

        CenterPivot();

        if (lifeTime < 0)
        {
            RandomizeLifeTimne();
        }
        else
        {
            _lifeTime = lifeTime;
        }

        if (color == null)
        {
            SetInitialColor();
        }
        else
        {
            initialColor = (Color)color;   
        }

        AddTint();

        isBought = alreadyBought;
        timeElapsed = 0;
        isAlive = true;
        isPlaced = false;
    }

    private void SetSrLayerOrder()
    {
        foreach(var rs in srs)
        {
            rs.sortingOrder = order;
        }

        order++;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos - _pivot.transform.localPosition;
    }

    public Color GetInitialColor()
    {
        return initialColor;
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

    public float GetLifeTime()
    {
        return _lifeTime;
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
        return ShopManager.Instance.GetPrice(_lifeTime, initialPrice);
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

    public void HandleBlockPlaced()
    {
        isPlaced = true;
        UpdateColorToPlaced();

        if (_clock)
        {
            Destroy(_clock.gameObject);
            _clock = null;
        }

        foreach(var rs in srs)
        {
            rs.sortingOrder = 1;
        }

        foreach(var col in colliders)
        {
            col.enabled = false;
        }

        SoundManager.Instance.PlayLock();
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
        _lifeTime = Mathf.Clamp(GlobalGameManager.Instance.GetBlockLifeLength() + Random.Range(-lifeTimeVariance, lifeTimeVariance), 10, 180);
    }

    private void SetInitialColor()
    {
        float hue = Random.value;
        initialColor = Helpers.GetRandomElement<Color>(blockColorList.colorList);
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

        float timeLeft = _lifeTime - timeElapsed;
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
