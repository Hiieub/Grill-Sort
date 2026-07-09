using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelData LoadLevel(int levelIndex)
    {
        string path = "Levels/level_" + levelIndex;

        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError("ko thay file: " + path);
            return null;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(jsonFile.text);

        Debug.Log($"Load level {levelIndex} thanh cong Grills: {data.totalGrill}");

        return data;
    }
}
