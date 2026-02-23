using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private Transform _trayContainer;
    [SerializeField] private Transform _slotContainer;

    private List<TrayItem> _totalTrays;
    private List<FoodSlot> _totalSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _totalTrays = Utils.GetListInChild<TrayItem>(_trayContainer);
        _totalSlot = Utils.GetListInChild<FoodSlot>(_slotContainer);
    }

    public void OnInitGrill(int totalTray, List<Sprite> listFood)
    {
        // xu ly set gia tri cho bep trc
        int foodCount = Random.Range(1, _totalSlot.Count + 1);
        List<Sprite> list = listFood;
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(list, foodCount);

        for(int i = 0; i < list.Count; i++)
        {
            FoodSlot slot = this.RandomSlot();
            slot.OnSetSlot(list[i]);
        }
    }

    private FoodSlot RandomSlot()
    {
    reRand: int n = Random.Range(0, _totalSlot.Count);
        if (_totalSlot[n].HasFood) goto reRand;

        return _totalSlot[n];
    }
}
