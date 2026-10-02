using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    private const string KEY_CURRENT_LEVEL = "CurrentLevel";
    private const int MIN_LEVEL = 1;

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentLevel = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, MIN_LEVEL);
    }

    public bool AdvanceToNextLevel(int totalLevels)
    {
        bool wasLastLevel = (CurrentLevel >= totalLevels);

        if (wasLastLevel)
            CurrentLevel = MIN_LEVEL; // loop về đầu
        else
            CurrentLevel++;

        Save();
        return wasLastLevel;
    }

    public void SetLevel(int level)
    {
        CurrentLevel = Mathf.Max(MIN_LEVEL, level);
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, CurrentLevel);
        PlayerPrefs.Save();
    }
}
