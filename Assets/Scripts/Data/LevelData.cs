using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int difficult; // Độ khó: 0 = Easy, 1 = Medium, 2 = Hard

    public int levelSeconds;

    public BoardData boardData;
    public SpawnWareData spawnWareData;
}

[Serializable]
public class BoardData
{
    public string id; // "freeform_board" hoặc "grid_board"

    // Chỉ dùng cho "grid_board" — số cột và hàng
    public int width;
    public int height;

    public List<SerializableVector2> listTrayPos;
    public List<TrayConfig> listTrayData;
}

[Serializable]
public class SerializableVector2
{
    public float x;
    public float y;

    public Vector2 ToVector2() => new Vector2(x, y);
}

[Serializable]
public class TrayConfig
{
    // "normal_tray"  = bếp thường
    // "target_tray"  = bếp mục tiêu (cần xếp đúng layer)
    // "bonus_tray"   = bếp thưởng điểm (implement sau)
    public string id;

    // Số FoodSlot của bếp này
    public int size;

    // Chỉ dùng cho "target_tray":
    // specificLayer = phải lấy đồ từ layer nào
    // requiredMatch = cần ghép đúng bao nhiêu cái
    public int specificLayer;
    public int requiredMatch;
}

[Serializable]
public class SpawnWareData
{
    // Tổng số bộ cần xếp xong để thắng
    // (mỗi bộ = 3 món cùng loại merge thành công)
    public int totalWare;

    // Số loại đồ ăn xuất hiện trong màn
    public int totalWarePattern;

    // Tên thư mục Sprite trong Resources/
    public List<string> listWareSet;

    public List<LayerData> listLayerData;
}

[Serializable]
public class LayerData
{
    // Số slot trống trên vỉ khi bắt đầu layer này
    public int numberEmptySlot;

    // Tỉ lệ % đĩa trong layer này có sẵn bộ 3 cùng loại
    [Range(0f, 100f)]
    public float match3Ratio;

    // Tỉ lệ % đĩa có sẵn cặp 2 cùng loại
    [Range(0f, 100f)]
    public float match2Ratio;

    // Tỉ lệ % đĩa hoàn toàn ngẫu nhiên
    [Range(0f, 100f)]
    public float match1Ratio;
}
