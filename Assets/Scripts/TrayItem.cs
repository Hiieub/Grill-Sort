using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrayItem : MonoBehaviour
{
    private List<Image> _foodList;

    private void Awake()
    {
        _foodList = Utils.GetListInChild<Image>(this.transform);

        for (int i = 0; i < _foodList.Count; i++)
            _foodList[i].gameObject.SetActive(false);
    }

    public void OnSetFood(List<Sprite> items)
    {
        if (items.Count < _foodList.Count)
        {
            Image slot = this.RandomSlot();
            slot.gameObject.SetActive(false);
            slot.sprite = items[1];
            slot.SetNativeSize();
        }
    }

    private Image RandomSlot()
    {
    rerand:  int n = Random.Range(0, _foodList.Count);
        if (_foodList[n].gameObject.activeInHierarchy) goto rerand;

        return _foodList[n];
    }
}
