using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private Transform trayContainer;
    [SerializeField] private Transform slotContainer;

    private List<TrayItem> totalTrays;
    private List<FoodSlot> totalSlot;
    private Stack<TrayItem> stackTrays = new Stack<TrayItem>();

    public List<FoodSlot> TotalSlots => totalSlot;

    private void Awake()
    {
        totalTrays = Utils.GetListInChild<TrayItem>(trayContainer);
        totalSlot = Utils.GetListInChild<FoodSlot>(slotContainer);

        GameManager.Instance?.RegisterGrill(this);
    }

    public void ResetGrill()
    {
        stackTrays.Clear();

        foreach (var slot in totalSlot)
            slot.ResetSlot();

        foreach (var tray in totalTrays)
        {
            tray.ResetTray();
            tray.gameObject.SetActive(false);
        }
    }

    public void OnInitGrill(int totalTray, List<Sprite> listFood)
    {
        int maxSlotFood = Mathf.Max(1, listFood.Count - totalTray); // giữ lại ít nhất 1 món/đĩa
        int foodCount = Random.Range(1, Mathf.Min(totalSlot.Count, maxSlotFood) + 1);
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(listFood, foodCount);

        for (int i = 0; i < listSlot.Count; i++)
        {
            FoodSlot slot = GetRandomEmptySlot();
            slot?.OnSetSlot(listSlot[i]);
        }

        // Phân bổ đồ ăn còn lại vào các đĩa
        List<List<Sprite>> remainFood = new List<List<Sprite>>();

        int trayToCreate = Mathf.Min(totalTray - 1, listFood.Count);
        for (int i = 0; i < trayToCreate; i++)
        {
            remainFood.Add(new List<Sprite>());
            int n = Random.Range(0, listFood.Count);
            remainFood[i].Add(listFood[n]);
            listFood.RemoveAt(n);
        }

        // Nếu không tạo được đĩa nào nhưng vẫn còn đồ ăn → tạo 1 đĩa mặc định
        if (remainFood.Count == 0 && listFood.Count > 0)
            remainFood.Add(new List<Sprite>());

        // Phân bổ phần còn lại vào các đĩa đã tạo
        while (listFood.Count > 0 && remainFood.Count > 0)
        {
            int rand = Random.Range(0, remainFood.Count);
            if (remainFood[rand].Count < 4)
            {
                int n = Random.Range(0, listFood.Count);
                remainFood[rand].Add(listFood[n]);
                listFood.RemoveAt(n);
            }
        }

        for (int i = 0; i < totalTrays.Count; i++)
        {
            bool active = i < remainFood.Count;
            totalTrays[i].gameObject.SetActive(active);

            if (active)
            {
                totalTrays[i].OnSetFood(remainFood[i]);
                stackTrays.Push(totalTrays[i]);
            }
        }
    }

    // Khởi tạo bếp bằng SETS bộ 3 nguyên vẹn — đảm bảo đường thắng.
    public void OnInitGrillBySets(List<List<Sprite>> sets, int slotsToFill)
    {
        stackTrays.Clear();
        if (sets == null || sets.Count == 0) return;

        List<Sprite> surfaceItems = new List<Sprite>();
        for (int s = 0; s < sets.Count && surfaceItems.Count < slotsToFill; s++)
            surfaceItems.Add(sets[s][0]);

        for (int i = surfaceItems.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (surfaceItems[i], surfaceItems[j]) = (surfaceItems[j], surfaceItems[i]);
        }

        for (int i = 0; i < surfaceItems.Count && i < totalSlot.Count; i++)
            totalSlot[i].OnSetSlot(surfaceItems[i]);

        // Xây dựng stack đĩa từ các sets
        List<List<Sprite>> trayContents = new List<List<Sprite>>();

        for (int s = 0; s < sets.Count; s++)
        {
            if (trayContents.Count >= totalTrays.Count) break;

            List<Sprite> trayItems = new List<Sprite>(sets[s]);

            if (s < slotsToFill)
            {
                // Set này đã có 1 item lên vỉ → đĩa chỉ còn (3 - 1) = 2 items
                // Bỏ 1 item đã dùng cho vỉ
                trayItems.RemoveAt(0);
            }

            if (trayItems.Count > 0)
                trayContents.Add(trayItems);
        }

        for (int i = 0; i < trayContents.Count; i++)
        {
            totalTrays[i].gameObject.SetActive(true);
            totalTrays[i].OnSetFood(trayContents[i]);
        }

        for (int i = 0; i < trayContents.Count; i++)
            stackTrays.Push(totalTrays[i]);

        for (int i = trayContents.Count; i < totalTrays.Count; i++)
            totalTrays[i].gameObject.SetActive(false);
    }

    // Khởi tạo bếp bằng flat list items đã shuffle — mỗi tray sẽ chứa mix nhiều loại.
    // GameManager truyền vào items đã shuffle toàn cục (không còn 3 cùng loại nằm liền nhau).
    public void OnInitGrillByItems(List<Sprite> items, int slotsToFill)
    {
        stackTrays.Clear();
        if (items == null || items.Count == 0) return;

        // Đặt slotsToFill items đầu tiên lên vỉ nướng
        int surfaceCount = Mathf.Min(slotsToFill, Mathf.Min(totalSlot.Count, items.Count));
        for (int i = 0; i < surfaceCount; i++)
            totalSlot[i].OnSetSlot(items[i]);

        // Phần còn lại → nhét vào đĩa, mỗi đĩa chứa tối đa traySize items
        const int traySize = 3;
        int trayIdx = 0;

        for (int i = surfaceCount; i < items.Count && trayIdx < totalTrays.Count; i += traySize)
        {
            int batchEnd = Mathf.Min(i + traySize, items.Count);
            List<Sprite> trayItems = items.GetRange(i, batchEnd - i);

            totalTrays[trayIdx].gameObject.SetActive(true);
            totalTrays[trayIdx].OnSetFood(trayItems);
            trayIdx++;
        }

        // Push vào stack: tray đầu (index 0) nằm đáy stack → pop ra cuối
        // tray cuối (index trayIdx-1) nằm đỉnh stack → pop ra trước
        for (int i = 0; i < trayIdx; i++)
            stackTrays.Push(totalTrays[i]);

        // Ẩn các tray không dùng
        for (int i = trayIdx; i < totalTrays.Count; i++)
            totalTrays[i].gameObject.SetActive(false);
    }

    // Lấy một slot trống ngẫu nhiên
    private FoodSlot GetRandomEmptySlot()
    {
        List<FoodSlot> emptySlots = totalSlot.FindAll(s => !s.HasFood);
        if (emptySlots.Count == 0) return null;

        return emptySlots[Random.Range(0, emptySlots.Count)];
    }

    public FoodSlot GetSlotNull()
    {
        return totalSlot.Find(s => !s.HasFood);
    }

    private bool IsGrillEmpty()
    {
        return totalSlot.TrueForAll(s => !s.HasFood);
    }

    private bool IsGrillFull()
    {
        return GetSlotNull() == null;
    }

    // Kiểm tra điều kiện merge
    public void OnCheckMerge()
    {
        if (!IsGrillFull()) return;
        if (!CanMerge()) return;

        Debug.Log($"[GrillStation] {name} merge thành công!");

        totalSlot.ForEach(s => s.OnActiveFood(false));

        // Đẩy đĩa tiếp theo lên nếu có
        PrepareNextTray();

        GameManager.Instance?.ReportMergeSuccess();
    }

    // Gọi sau khi kéo đồ ăn đi khỏi bếp, kiểm tra nếu bếp trống thì đẩy đĩa lên
    public void OnCheckPrepareTray()
    {
        if (IsGrillEmpty())
            PrepareNextTray();
    }

    private void PrepareNextTray()
    {
        if (stackTrays.Count == 0) return;

        TrayItem item = stackTrays.Pop();

        int slotIdx = 0;
        for (int i = 0; i < item.FoodList.Count && slotIdx < totalSlot.Count; i++)
        {
            Image img = item.FoodList[i];
            if (!img.gameObject.activeInHierarchy) continue;

            totalSlot[slotIdx].OnPrepareItem(img);
            img.gameObject.SetActive(false);
            slotIdx++;
        }

        item.gameObject.SetActive(false);

        // Check merge, sau khi đĩa đẩy lên
        OnCheckMerge();
    }

    private bool CanMerge()
    {
        if (totalSlot.Count == 0) return false;

        string firstName = totalSlot[0].GetSpriteFood?.name;
        if (string.IsNullOrEmpty(firstName)) return false;

        return totalSlot.TrueForAll(s => s.GetSpriteFood?.name == firstName);
    }
}
