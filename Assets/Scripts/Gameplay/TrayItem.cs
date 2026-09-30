using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrayItem : MonoBehaviour
{
    private List<Image> _foodList;
    public List<Image> FoodList => _foodList;

    private void Awake()
    {
        _foodList = Utils.GetListInChild<Image>(this.transform);

        for (int i = 0; i < _foodList.Count; i++)
            _foodList[i].gameObject.SetActive(false);
    }

    public void OnSetFood(List<Sprite> items)
    {
        if (items.Count > _foodList.Count)
        {
            Debug.LogWarning($"[TrayItem] items ({items.Count}) > slots ({_foodList.Count}), sẽ bỏ phần thừa.");
        }

        int count = Mathf.Min(items.Count, _foodList.Count);
        for (int i = 0; i < count; i++)
        {
            Image slot = RandomSlot();
            if (slot == null) break;

            slot.gameObject.SetActive(true);
            slot.sprite = items[i];
            slot.SetNativeSize();
        }
    }

    private Image RandomSlot()
    {
        List<Image> emptySlots = _foodList.FindAll(img => !img.gameObject.activeInHierarchy);

        if (emptySlots.Count == 0)
        {
            Debug.LogWarning("[TrayItem] Không còn slot trống!");
            return null;
        }

        return emptySlots[Random.Range(0, emptySlots.Count)];
    }
}
