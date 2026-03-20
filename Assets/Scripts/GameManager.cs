using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private int _allFood;
    [SerializeField] private int _totalFood; // tong so loai thuc an
    [SerializeField] private int _totalGrill; // tong so bep
    [SerializeField] private Transform _gridGrill;

    private List<GrillStation> _listGrills;
    private float _avgTray; // gia tri trung binh thuc an cho 1 dia
    private List<Sprite> _totalSPriteFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("Items");
        _totalSPriteFood = loadedSprite.ToList();
        _instance = this;
    }

    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeFood = _totalSPriteFood.OrderBy(x => Random.value).Take(_totalFood).ToList();
        List<Sprite> useFood = new List<Sprite>();

        for(int i = 0; i < _allFood; i++)
        {
            int n = i % takeFood.Count;
            for(int j = 0; j < 3; j++)
                useFood.Add(takeFood[n]);
        }

        // random vi tri cac item
        for(int i = 0; i < useFood.Count; i++)
        {
            int rand = Random.Range(i, useFood.Count);
            (useFood[i], useFood[rand]) = (useFood[rand],  useFood[i]); // doi vi tri
        }

        _avgTray = Random.Range(1.5f, 2f);
        int totalTray = Mathf.RoundToInt(useFood.Count / _avgTray); // tinh tong so dia

        List<int> trayPerGrill = this.DistributeEvelyn(_totalGrill, totalTray);
        List<int> foodPerGrill = this.DistributeEvelyn(_totalGrill, useFood.Count);

        for(int i = 0; i < _listGrills.Count; i++)
        {
            bool activeGrill = i < _totalGrill;
            _listGrills[i].gameObject.SetActive(activeGrill);

            if (activeGrill)
            {
                List<Sprite> listFood = Utils.TakeAndRemoveRandom<Sprite>(useFood, foodPerGrill[i]);
                _listGrills[i].OnInitGrill(trayPerGrill[i], listFood);
            }
        }
    }

    private List<int> DistributeEvelyn(int grillCount, int totalTrays)
    {
        List<int> result = new List<int>();

        // tinh trung binh slg dia
        float avg = (float)totalTrays / grillCount; // 3.5
        int low = Mathf.FloorToInt(avg); // 3
        int high = Mathf.CeilToInt(avg); // 4

        int hightCount = totalTrays - low * grillCount;
        int lowCount = grillCount - hightCount;

        for (int i = 0; i < lowCount; i++)
            result.Add(low);

        for (int i = 0; i < hightCount; i++)
            result.Add(high);

        // dao vi tri
        for(int i = 0; i < result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }

        return result;
    }

    public void OnMinusFood()
    {
        --_allFood;
        if(_allFood <= 0)
        {
            Debug.Log("Game Complete");
        }
    }

    public void OnCheckAndShake()
    {
        Dictionary<string, List<FoodSlot>> groups = new Dictionary<string, List<FoodSlot>>();

        foreach (var grill in _listGrills)
        {
            if (grill.gameObject.activeInHierarchy)
            {
                for(int i = 0; i < grill.TotalSlots.Count; i++)
                {
                    FoodSlot slot = grill.TotalSlots[i];
                    if (slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;
                        if (!groups.ContainsKey(name))
                        {
                            groups.Add(name, new List<FoodSlot>());
                        }

                        groups[name].Add(slot);
                    }
                }
            }
        }
        
        foreach(var kvp in groups)
        {
            if(kvp.Value.Count >= 3)
            {
                for(int i = 0; i < 3; i++)
                {
                    kvp.Value[i].DoShake();
                }

                return;
            }
        }
    }
}
