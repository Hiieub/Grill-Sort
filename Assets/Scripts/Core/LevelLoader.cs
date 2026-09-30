using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private const string LEVEL_PATH_PREFIX = "Levels/level_";

    public LevelData LoadLevel(int levelIndex)
    {
        string path = LEVEL_PATH_PREFIX + levelIndex;
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"[LevelLoader] Không tìm thấy file: Resources/{path}.json");
            return null;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(jsonFile.text);

        if (!ValidateLevelData(data, levelIndex))
            return null;

        Debug.Log($"[LevelLoader] Load level {levelIndex} thành công. " +
                  $"Bếp: {data.boardData.listTrayData.Count}, " +
                  $"Bộ ăn: {data.spawnWareData.totalWare}, " +
                  $"Thời gian: {data.levelSeconds}s");

        return data;
    }

    private bool ValidateLevelData(LevelData data, int levelIndex)
    {
        if (data.boardData == null)
        {
            Debug.LogError($"[LevelLoader] Level {levelIndex}: boardData bị null!");
            return false;
        }

        if (data.boardData.listTrayPos == null || data.boardData.listTrayPos.Count == 0)
        {
            Debug.LogError($"[LevelLoader] Level {levelIndex}: listTrayPos trống!");
            return false;
        }

        if (data.boardData.listTrayData == null ||
            data.boardData.listTrayPos.Count != data.boardData.listTrayData.Count)
        {
            Debug.LogError($"[LevelLoader] Level {levelIndex}: listTrayPos và listTrayData phải có cùng số phần tử!");
            return false;
        }

        if (data.spawnWareData == null || data.spawnWareData.totalWare <= 0)
        {
            Debug.LogError($"[LevelLoader] Level {levelIndex}: spawnWareData không hợp lệ!");
            return false;
        }

        return true;
    }
}
