using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image imgFoodDrag;
    [SerializeField] private float timeCheckHint = 5f;

    private FoodSlot currentFood, cacheFood;
    private bool isDragging;
    private Vector3 offset;
    private float idleTimer; // Đếm thời gian không tương tác

    void Update()
    {
        bool canInteract = GameManager.Instance == null || GameManager.Instance.IsInteractable;

        if (canInteract)
        {
            HandleHintTimer();
            HandleDragInput();
        }
    }

    private void HandleHintTimer()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= timeCheckHint)
        {
            idleTimer = 0f;
            GameManager.Instance?.TryShowHint();
        }
    }

    private void ResetIdleTimer()
    {
        idleTimer = 0f;
    }

    private void HandleDragInput()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            OnBeginDrag();

        if (isDragging)
            OnDragging();

        if (Pointer.current != null && Pointer.current.press.wasReleasedThisFrame && isDragging)
            OnEndDrag();
    }

    private void OnBeginDrag()
    {
        Vector2 pointerPos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        FoodSlot hitSlot = Utils.GetRayCastUI<FoodSlot>(pointerPos);

        if (hitSlot == null || !hitSlot.HasFood) return;

        ResetIdleTimer();
        isDragging = true;
        currentFood = hitSlot;
        cacheFood = currentFood;

        imgFoodDrag.gameObject.SetActive(true);
        imgFoodDrag.sprite = currentFood.GetSpriteFood;
        imgFoodDrag.SetNativeSize();
        imgFoodDrag.transform.position = currentFood.transform.position;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(pointerPos);
        offset = mouseWorldPos - currentFood.transform.position;

        currentFood.OnActiveFood(false);
    }

    private void OnDragging()
    {
        if (imgFoodDrag == null || Camera.main == null) return;

        Vector2 pointerPos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;

        // Di chuyển dummy image theo chuột
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Vector3 foodPos = mouseWorldPos + offset;
        foodPos.z = 0f;
        imgFoodDrag.transform.position = foodPos;

        // Raycast để tìm slot đang hover
        FoodSlot hoveredSlot = Utils.GetRayCastUI<FoodSlot>(pointerPos);
        UpdatePreviewSlot(hoveredSlot);
    }

    private void UpdatePreviewSlot(FoodSlot hoveredSlot)
    {
        if (hoveredSlot == null)
        {
            ClearCacheSlot();
            return;
        }

        FoodSlot targetSlot;

        if (!hoveredSlot.HasFood)
        {
            // Đang hover vào ô trống -> đó là target
            targetSlot = hoveredSlot;
        }
        else
        {
            // Đang hover vào ô có đồ ăn -> tìm ô trống trên cùng bếp
            targetSlot = hoveredSlot.GetSlotNull;
        }

        if (targetSlot == null)
        {
            ClearCacheSlot();
            return;
        }

        // Chỉ cập nhật preview nếu target slot thay đổi
        if (cacheFood == null || cacheFood.GetInstanceID() != targetSlot.GetInstanceID())
        {
            cacheFood?.OnHideFood();
            cacheFood = targetSlot;
            cacheFood.OnFadeFood();
            cacheFood.OnSetSlot(currentFood.GetSpriteFood);
        }
    }

    private void OnEndDrag()
    {
        isDragging = false;

        if (cacheFood != null)
        {
            // Có slot target hợp lệ -> bay đến slot đó
            FoodSlot destination = cacheFood;
            FoodSlot source = currentFood;

            imgFoodDrag.transform.DOMove(destination.transform.position, 0.15f)
                .OnComplete(() =>
                {
                    imgFoodDrag.gameObject.SetActive(false);
                    destination.OnSetSlot(source.GetSpriteFood);
                    destination.OnActiveFood(true);
                    destination.OnCheckMerge();
                    source.OnCheckPrepareTray();
                    cacheFood = null;
                    currentFood = null;
                });
        }
        else
        {
            // Không có slot hợp lệ -> bay về chỗ cũ
            FoodSlot source = currentFood;

            imgFoodDrag.transform.DOMove(source.transform.position, 0.3f)
                .OnComplete(() =>
                {
                    imgFoodDrag.gameObject.SetActive(false);
                    source.OnActiveFood(true);
                });
        }
    }

    private void ClearCacheSlot()
    {
        if (cacheFood != null && cacheFood.GetInstanceID() != currentFood?.GetInstanceID())
        {
            cacheFood.OnHideFood();
            cacheFood = null;
        }
    }
}
