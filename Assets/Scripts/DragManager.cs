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
    private Vector2 _offset;
    private Vector3 _pointerPositionWorld;
    private bool _isDraggedBlockSnapped;
    private float _dragDuration;
    private Vector3 _mouseTouchStartPosition;
    private bool _dragStarted;

    [SerializeField] private float fingerOffsetBuildupDuration;

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

    //private void Update()
    //{
    //    var mousePos = Input.mousePosition;
    //    if (Input.GetMouseButton(0))
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            HandlePointerDown(mousePos);
    //        }
    //        HandlePointerHeld(mousePos);
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        HandlePointerUp();
    //    }
    //}

    public void HandlePointerDown(Vector3 pointerPosition)
    {
        _pointerPositionWorld = Camera.main.ScreenToWorldPoint(pointerPosition);
        var mask = LayerMask.GetMask(new string[] { "Blocks" });
        Collider2D collider = Physics2D.OverlapPoint(_pointerPositionWorld, mask);
        if (collider)
        {
            DraggedBlock = collider.transform.parent.gameObject;
            _offset = DraggedBlock.transform.position - _pointerPositionWorld;
            _dragDuration = 0;
            _mouseTouchStartPosition = _pointerPositionWorld;
        }
    }

    public void HandlePointerHeld(Vector3 pointerPosition)
    {
        _pointerPositionWorld = Camera.main.ScreenToWorldPoint(pointerPosition);
        if (DraggedBlock)
        {
            if (!IsTap() && !_dragStarted && IsInteractible(_draggedScript))
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
                FinishDrag();
                _dragStarted = false;
            }

            DraggedBlock = null;
        }
    }

    private bool IsTap()
    {
        var mag = (_pointerPositionWorld - _mouseTouchStartPosition).magnitude;
        return mag < 0.15f && _dragDuration < 0.15f;
    }

    private void OnStartDrag()
    {
        if (MapManager.GetInstance().IsPlaced(DraggedBlock))
        {
            MapManager.GetInstance().Remove(DraggedBlock);
        }
        else
        {

        }
    }

    private bool IsInteractible(BlockScript block)
    {
        return block.IsBought() || ShopManager.Instance.CanBeBought(block);
    }

    private void OnDrag()
    {
        UpdatePosition();
        ShopManager.Instance.HandleBlockDrag(DraggedBlock);

        _dragDuration += Time.deltaTime;
        return;
    }

    void UpdatePosition()
    {
        var fingerOffset = Vector2.up * Mathf.Clamp01(_dragDuration / fingerOffsetBuildupDuration) * 2.0f;
        var newPosition = (Vector2)_pointerPositionWorld + _offset + fingerOffset;
        var newPositionRounded = Helpers.RoundPosition(newPosition);
        if (MapManager.GetInstance().CanBePlaced(DraggedBlock, newPositionRounded))
        {
            DraggedBlock.transform.position = new Vector3(newPositionRounded.x, newPositionRounded.y);
            _isDraggedBlockSnapped = true;
            return;
        }
        DraggedBlock.transform.position = newPosition;
        _isDraggedBlockSnapped = false;
    }

    private void FinishDrag()
    {
        if (_isDraggedBlockSnapped)
        {
            var coords = Helpers.RoundPosition(DraggedBlock.transform.position);
            MapManager.GetInstance().Place(DraggedBlock, coords);
        }
        else
        {

        }
    }

    private void HandleTap()
    {
        if (MapManager.GetInstance().IsPlaced(DraggedBlock))
        {            MapManager.GetInstance().Remove(DraggedBlock);
        }
        DraggedBlock.transform.Rotate(Vector3.forward, 90);
        TryPlace(DraggedBlock, DraggedBlock.transform.position);
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

