using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<int, int> OnMergeSuccess; // Tham số: còn bao nhiêu bộ nữa + số star
    public static event Action<GameState> OnStateChanged; // Tham số: State mới

    [Header("References")]
    [SerializeField] private LevelLoader levelLoader;

    [SerializeField] private Transform gridGrill;

    [Header("Level Settings")]
    [SerializeField] private int currentLevelIndex = 1;

    private GameState currentState = GameState.Loading;
    private LevelData currentLevelData;

    private int remainingWare;
    private float remainingTime;
    private bool hasTimeLimit;

    private int starCount; // xu nhận được (hình dạng ngôi sao)
    private int currentStar = 0; // reset mỗi level

    private List<GrillStation> allGrills = new List<GrillStation>();
    private List<GrillStation> activeGrills = new List<GrillStation>();

    // Cache sprite đã load
    private List<Sprite> loadedSprites = new List<Sprite>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return; 
        }
        Instance = this;

        if (gridGrill != null)
            allGrills = Utils.GetListInChild<GrillStation>(gridGrill);

        starCount = PlayerPrefs.GetInt("TotalStar", 0);
    }

    private void Start()
    {
        // Ưu tiên đọc level từ PlayerProgress (lưu qua scene)
        if (PlayerProgress.Instance != null)
            currentLevelIndex = PlayerProgress.Instance.CurrentLevel;

        LoadAndStartLevel(currentLevelIndex);
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Playing:
                HandlePlayingUpdate();
                break;
        }
    }


    private void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"[GameManager] State: {newState}");

        OnStateChanged?.Invoke(newState);

        OnEnterState(newState);
    }

    private void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1.0f;
                break;

            case GameState.Pausing:
                Time.timeScale = 0f;
                break;

            case GameState.Victory:
                Debug.Log("[GameManager] VICTORY!");
                HealthManager.Instance.RefundHealth();

                starCount += currentStar;
                PlayerPrefs.SetInt("TotalStar", starCount);
                PlayerPrefs.Save();
                break;

            case GameState.GameOver:
                Debug.Log("[GameManager] GAME OVER");
                break;
        }
    }

    // LEVEL LOADING
    private void LoadAndStartLevel(int levelIndex)
    {
        ChangeState(GameState.Loading);

        currentLevelData = levelLoader.LoadLevel(levelIndex);

        if (currentLevelData == null)
        {
            Debug.LogError($"[GameManager] Không load được level {levelIndex}!");
            return;
        }

        InitLevelFromData(currentLevelData);
    }

    private void InitLevelFromData(LevelData data)
    {
        hasTimeLimit = data.levelSeconds > 0;
        remainingTime = data.levelSeconds;

        // Số bộ đồ ăn cần xếp
        remainingWare = data.spawnWareData.totalWare;

        // Load Sprites từ listWareSet
        loadedSprites.Clear();
        foreach (string setName in data.spawnWareData.listWareSet)
        {
            Sprite[] loaded = Resources.LoadAll<Sprite>(setName);
            loadedSprites.AddRange(loaded);
        }

        if (loadedSprites.Count == 0)
        {
            Debug.LogError("[GameManager] Không tìm thấy Sprite nào!");
            return;
        }

        // Reset tất cả bếp trước khi init mới
        foreach (var grill in allGrills)
            grill.ResetGrill();

        // Bật/tắt bếp theo level data
        int totalGrill = data.boardData.listTrayData?.Count ?? allGrills.Count;
        activeGrills.Clear();
        for (int i = 0; i < allGrills.Count; i++)
        {
            bool isActive = i < totalGrill;
            allGrills[i].gameObject.SetActive(isActive);
            if (isActive) activeGrills.Add(allGrills[i]);
        }

        // Rải đồ ăn vào các bếp đang active
        SpawnFoodToGrills(data.spawnWareData);

        ChangeState(GameState.Playing);
    }

    private void SpawnFoodToGrills(SpawnWareData spawnData)
    {
        int numGrills = Mathf.Min(activeGrills.Count, spawnData.totalWare);

        // Chọn loại đồ ăn
        List<Sprite> pickedTypes = loadedSprites
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(spawnData.totalWarePattern)
            .ToList();

        // Tạo flat list: mỗi ware = 3 items cùng loại, đảm bảo tổng chia hết cho 3
        List<Sprite> allItems = new List<Sprite>();
        for (int i = 0; i < spawnData.totalWare; i++)
        {
            Sprite t = pickedTypes[i % pickedTypes.Count];
            allItems.Add(t);
            allItems.Add(t);
            allItems.Add(t);
        }

        // Shuffle TỪNG ITEM — phá vỡ grouping "3 cùng loại nằm cạnh nhau"
        for (int i = allItems.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (allItems[i], allItems[j]) = (allItems[j], allItems[i]);
        }

        // Đọc config từ listLayerData để tính số slot để trống trên vỉ
        // Layer 0 = trạng thái ban đầu khi level bắt đầu
        LayerData layer0 = spawnData.listLayerData?.Count > 0
            ? spawnData.listLayerData[0] : null;

        // numberEmptySlot = tổng số slot trống trên TẤT CẢ bếp
        // Chia đều: mỗi bếp để trống emptyPerGrill slot
        int totalEmpty = layer0?.numberEmptySlot ?? numGrills; // default: 1 trống/bếp
        int emptyPerGrill = Mathf.Max(0, Mathf.RoundToInt((float)totalEmpty / numGrills));
        // số slot hiển thị đồ ăn trên vỉ khi bắt đầu
        int slotsToFill = Mathf.Clamp(3 - emptyPerGrill, 1, 3);

        // Phân bổ items (không phải sets) đều vào các bếp
        List<int> itemsPerGrill = DistributeEvenly(numGrills, allItems.Count);

        int idx = 0;
        for (int g = 0; g < numGrills; g++)
        {
            int count = itemsPerGrill[g];
            List<Sprite> grillItems = allItems.GetRange(idx, count);
            idx += count;
            activeGrills[g].OnInitGrillByItems(grillItems, slotsToFill);
        }
    }

    private List<int> DistributeEvenly(int count, int total)
    {
        List<int> result = new List<int>();
        int low = total / count;
        int high = low + 1;
        int highCount = total - low * count;

        for (int i = 0; i < count - highCount; i++) result.Add(low);
        for (int i = 0; i < highCount; i++) result.Add(high);

        // Xáo ngẫu nhiên để không bếp nào luôn nhiều nhất
        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result;
    }


    // PLAYING STATE LOGIC
    private void HandlePlayingUpdate()
    {
        if (!hasTimeLimit) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            ChangeState(GameState.GameOver);
        }
    }


    // GrillStation gọi hàm này khi xếp xong 1 bộ đồ ăn
    public void ReportMergeSuccess()
    {
        if (currentState != GameState.Playing) return;

        remainingWare--;
        currentStar++;
        //starCount += currentStar;
        //PlayerPrefs.SetInt("TotalStar", starCount);
        //PlayerPrefs.Save();
        Debug.Log($"[GameManager] Merge thành công! Còn lại: {remainingWare} bộ và có {currentStar} star");

        OnMergeSuccess?.Invoke(remainingWare, currentStar);

        if (remainingWare <= 0)
        {
            ChangeState(GameState.Victory);
        }
    }

    public bool IsInteractable => currentState == GameState.Playing;

    public void TryShowHint()
    {
        if (currentState != GameState.Playing) return;

        // Gom tất cả FoodSlot từ tất cả bếp đang hoạt động
        var groups = new Dictionary<string, List<FoodSlot>>();

        foreach (var grill in activeGrills)
        {
            if (!grill.gameObject.activeInHierarchy) continue;

            foreach (var slot in grill.TotalSlots)
            {
                if (!slot.HasFood) continue;

                string spriteName = slot.GetSpriteFood.name;
                if (!groups.ContainsKey(spriteName))
                    groups[spriteName] = new List<FoodSlot>();

                groups[spriteName].Add(slot);
            }
        }

        // Tìm loại đồ ăn nào có >= 3 cái thì rung
        foreach (var kvp in groups)
        {
            if (kvp.Value.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                    kvp.Value[i].DoShake();

                return;
            }
        }
    }

    public void RegisterGrill(GrillStation grill)
    {
        if (!allGrills.Contains(grill))
            allGrills.Add(grill);
        if (!activeGrills.Contains(grill))
            activeGrills.Add(grill);
    }


    public void PauseGame()
    {
        if (currentState != GameState.Playing)
            return;

        ChangeState(GameState.Pausing);
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Pausing)
            return;

        ChangeState(GameState.Playing);
    }

    public void RestartLevel()
    {
        currentStar = 0;

        Time.timeScale = 1f;
        LoadAndStartLevel(currentLevelIndex);
    }


    public float RemainingTime => remainingTime;
    public int RemainingWare => remainingWare;
    public int CurrentStar => currentStar;
    public GameState CurrentState => currentState;
    public int CurrentLevelIndex => currentLevelIndex;
    public float MaxTime => currentLevelData != null ? currentLevelData.levelSeconds : 0f;
}
