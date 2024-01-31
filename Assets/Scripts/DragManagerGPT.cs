using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;

public class DragManagerGPT : MonoBehaviour
{
    private GameObject _draggedObject;
    private BlockScript _draggedScript;
    private GameObject DraggedBlock
    {
        get { return _draggedObject; }
        set
        {
            _draggedObject = value;
            _draggedScript = value?.GetComponent<BlockScript>();
        }
    }
    private Vector2 _offset;
    private Vector3 _mousePosition;
    private bool _isDraggedBlockSnapped;
    private float _dragDuration;
    private Vector3 _mouseTouchStartPosition;
    private bool _dragStarted;

    public static DragManagerGPT Instance { get; private set; }

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

    private void Start()
    {
        _isDraggedBlockSnapped = false;
        _dragStarted = false;
    }

    void Update()
    {
#if UNITY_ANDROID
        // Android touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Consider the first touch, you may adjust if needed
            _mousePosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                // Check if a touch has just begun
                var mask = LayerMask.GetMask(new string[] { "Blocks" });
                Collider2D collider = Physics2D.OverlapPoint(_mousePosition, mask);

                if (collider)
                {
                    // If a block is touched, initiate dragging
                    DraggedBlock = collider.transform.parent.gameObject;
                    _offset = DraggedBlock.transform.position - _mousePosition;
                    _dragDuration = 0;
                    _mouseTouchStartPosition = _mousePosition;
                }
            }

            if (DraggedBlock)
            {
                // Check if the block is being dragged
                if (!IsTap() && !_dragStarted && IsInteractible(_draggedScript))
                {
                    // If dragging has just started, perform necessary actions
                    OnStartDrag();
                    _dragStarted = true;
                }

                if (_dragStarted)
                {
                    // If already dragging, update the drag position
                    OnDrag();
                }
            }
        }
#else
    // Mouse input in Unity editor
    if (Input.GetMouseButton(0))
    {
         
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is pressed down
            var mask = LayerMask.GetMask(new string[] { "Blocks" });
            Collider2D collider = Physics2D.OverlapPoint(_mousePosition, mask);

            if (collider)
            {
                // If a block is clicked, initiate dragging
                DraggedBlock = collider.transform.parent.gameObject;
                _offset = DraggedBlock.transform.position - _mousePosition;
                _dragDuration = 0;
                _mouseTouchStartPosition = _mousePosition;
            }
        }

        if (DraggedBlock)
        {
            // Check if the block is being dragged
            if (!IsTap() && !_dragStarted && IsInteractible(_draggedScript))
            {
                // If dragging has just started, perform necessary actions
                OnStartDrag();
                _dragStarted = true;
            }

            if (_dragStarted)
            {
                // If already dragging, update the drag position
                OnDrag();
            }
        }
    }
#endif

        if (DraggedBlock)
        {
#if UNITY_ANDROID
            // Android touch input
            if (Input.touchCount == 0)
#else
        // Mouse input in Unity editor
        if (!Input.GetMouseButton(0))
#endif
            {
                // Check if there are no touches (touch released or mouse button released)
                if (IsTap() && IsInteractible(_draggedScript))
                {
                    // If it was a tap, handle tap action
                    HandleTap();
                }

                if (_dragStarted)
                {
                    // If dragging was ongoing, finish the drag
                    FinishDrag();
                    _dragStarted = false;
                }

                // Reset the dragged block reference
                DraggedBlock = null;
            }
        }
    }


    private bool IsTap()
    {
        var mag = (_mousePosition - _mouseTouchStartPosition).magnitude;
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
        var newPosition = (Vector2)_mousePosition + _offset;
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
        {
            MapManager.GetInstance().Remove(DraggedBlock);
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
