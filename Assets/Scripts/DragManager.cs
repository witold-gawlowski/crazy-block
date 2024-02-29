using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;

public class DragManager : MonoBehaviour
{
    private GameObject _draggedObject;
    private BlockScript _draggedScript;
    private GameObject DraggedBlock
    { 
        get { return _draggedObject; } 
        set { 
            _draggedObject = value;
            _draggedScript = value?.GetComponent<BlockScript>();
        } 
    }
    private Vector2 _middleOffset;
    private Vector3 _pointerPositionWorld;
    private bool _isDraggedBlockSnapped;
    private float _touchDuration;
    private float _timeFromDragStart;
    private Vector3 _mouseTouchStartPosition;
    private bool _dragStarted;

    [SerializeField] private float _fingerOffsetBuildupYDistance;
    [SerializeField] private float _fingerOffsetBuildupDuration = 0.25f;
    [SerializeField] private float tapThresholdDuration;

    public static DragManager Instance { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 120;
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

    private void Start()
    {
        _isDraggedBlockSnapped = false;
        _dragStarted = false;
    }

    public void HandlePointerDown(Vector3 pointerPosition)
    {
        _timeFromDragStart = 0;

        _pointerPositionWorld = Camera.main.ScreenToWorldPoint(pointerPosition);
        var mask = LayerMask.GetMask(new string[] { "Blocks" });
        Collider2D collider = Physics2D.OverlapPoint(_pointerPositionWorld, mask);
        if (collider)
        {
            DraggedBlock = collider.transform.parent.gameObject;
            _touchDuration = 0;
            _mouseTouchStartPosition = _pointerPositionWorld;
        }
    }

    public void HandlePointerHeld(Vector3 pointerPosition)
    {
        _pointerPositionWorld = Camera.main.ScreenToWorldPoint(pointerPosition);
        if (DraggedBlock)
        {
            _touchDuration += Time.deltaTime;

            if (IsDrag() && !_dragStarted && IsInteractible(_draggedScript)) 
            {
                OnStartDrag();
                _dragStarted = true;
            }

            if (_dragStarted)
            {
                OnDrag();
            }
        }
    }


    public void HandleBlockDead(BlockScript blockScript)
    {
        if (_draggedScript == blockScript)
        {
            HandlePointerUp();
        }
    }

    public void HandlePointerUp()
    {
        if (DraggedBlock)
        {
            if (IsTap() && IsInteractible(_draggedScript))
            {
                HandleTap();
            }

            if (_dragStarted)
            {
                HandleFinishDrag();
                _dragStarted = false;
            }

            DraggedBlock = null;
        }
    }

    public bool IsSnapped(BlockScript blockScript)
    {
        if(_draggedScript == blockScript && _isDraggedBlockSnapped)
        {
            return true;
        }
        return false;
    }

    private bool IsDrag()
    {
        var mag = (_pointerPositionWorld - _mouseTouchStartPosition).magnitude;
        return mag > 0.15f;
    }

    private bool IsTap()
    {
        var mag = (_pointerPositionWorld - _mouseTouchStartPosition).magnitude;
        return mag < 0.15f && _touchDuration < tapThresholdDuration;
    }

    private void OnStartDrag()
    {
        if (MapManager.GetInstance().IsPlaced(DraggedBlock))
        {
            MapManager.GetInstance().Remove(DraggedBlock);
        }
    }

    private bool IsInteractible(BlockScript block)
    {
        if(GlobalGameManager.Instance.IsPaused())
        {
            return false;
        }

        if (block.IsDead())
        {
            return false;
        }

        if (MapManager.GetInstance().IsPlaced(block.gameObject))
        {
            return false;
        }

        return true;
        //return block.IsBought() || ShopManager.Instance.CanBeBought(block);
    }

    private void OnDrag()
    {
        _timeFromDragStart += Time.deltaTime;

        UpdatePosition();
        return;
    }

    void UpdatePosition()
    {
        bool isOverShop = _draggedScript.IsOverShop();
        float yAboveShop = Mathf.Clamp(_pointerPositionWorld.y + 8.5f, 0, _fingerOffsetBuildupYDistance);
        bool isBought = _draggedScript.IsBought();
        float shopBlockOffsetBlend = isBought ? 1 : Mathf.Clamp01(yAboveShop / _fingerOffsetBuildupYDistance);
        float boughtBlockOffsetBlend = isBought ? Mathf.Clamp01(_timeFromDragStart / _fingerOffsetBuildupDuration) : 1;
        var fingerOffset = Vector2.up * shopBlockOffsetBlend * boughtBlockOffsetBlend * 3.0f;
        var newPosition = (Vector2)_pointerPositionWorld + fingerOffset - _draggedScript.GetMiddleOffset();
        var newPositionRounded = Helpers.RoundPosition(newPosition);
        if (MapManager.GetInstance().CanBePlaced(DraggedBlock, newPositionRounded))
        {
            DraggedBlock.transform.position = new Vector3(newPositionRounded.x, newPositionRounded.y);
            _isDraggedBlockSnapped = true;
            //_draggedScript.HandleSnap();
            return;
        }

        DraggedBlock.transform.position = newPosition;

        //if (_isDraggedBlockSnapped)
        //{
        //    _draggedScript.HandleUnsnap();
        //}
        _isDraggedBlockSnapped = false;
    }

    private void HandleFinishDrag()
    {
        ShopManager.Instance.HandleBlockRelease(_draggedScript);
        SoundManager.Instance.PlayClick();

        if (_isDraggedBlockSnapped && !_draggedScript.IsDead())
        {
            var coords = Helpers.RoundPosition(DraggedBlock.transform.position);
            MapManager.GetInstance().Place(DraggedBlock, coords);
            _draggedScript.HandleBlockPlaced();
        }
    }

    private void HandleTap()
    {
        if (MapManager.GetInstance().IsPlaced(DraggedBlock))
        {           
            MapManager.GetInstance().Remove(DraggedBlock);
        }
        DraggedBlock.transform.RotateAround(_draggedScript.GetPivot().position, Vector3.forward, 90);
        TryPlace(DraggedBlock, DraggedBlock.transform.position);
        SoundManager.Instance.PlayClick();
    }

    void TryPlace(GameObject block, Vector2 pos)
    {
        var newPositionRounded = Helpers.RoundPosition(pos);
        if (MapManager.GetInstance().CanBePlaced(block, newPositionRounded))
        {
            block.transform.position = new Vector3(newPositionRounded.x, newPositionRounded.y);
            MapManager.GetInstance().Place(DraggedBlock, newPositionRounded);
        }
    }
}

