using System;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [SerializeField] private GameObject panelOutOfHealth;

    [SerializeField] private TextMeshProUGUI txtHealthCount;
    [SerializeField] private TextMeshProUGUI txtRestoreTime;

    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float timeToRestoreOneHealth = 5f * 60f;

    private int currentHealth;
    private DateTime nextRestoreTime;

    private const string PREF_HEALTH = "CurrentHealth";
    private const string PREF_NEXT_RESTORE_TIME = "NextRestoreTime";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.txtHealthCount = this.txtHealthCount;
            Instance.txtRestoreTime = this.txtRestoreTime;

            Instance.UpdateUI();

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadHealthData();
    }

    private void Update()
    {
        if (currentHealth < maxHealth)
        {
            TimeSpan timeRemaining = nextRestoreTime - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                currentHealth++;

                if (currentHealth < maxHealth)
                {
                    nextRestoreTime = nextRestoreTime.AddSeconds(timeToRestoreOneHealth);
                }

                SaveHealthData();
                UpdateUI();
            }
            else
            {
                UpdateTimerUI(timeRemaining);
            }
        }
    }

    private void LoadHealthData()
    {
        currentHealth = PlayerPrefs.GetInt(PREF_HEALTH, maxHealth);
        string timeString = PlayerPrefs.GetString(PREF_NEXT_RESTORE_TIME, string.Empty);

        if (currentHealth < maxHealth && !string.IsNullOrEmpty(timeString))
        {
            if (DateTime.TryParse(timeString, out DateTime savedTime))
            {
                nextRestoreTime = savedTime;
                CalculateOfflineProgress();
            }
        }
        UpdateUI();
    }

    private void CalculateOfflineProgress()
    {
        while (currentHealth < maxHealth && DateTime.Now >= nextRestoreTime)
        {
            currentHealth++;
            nextRestoreTime = nextRestoreTime.AddSeconds(timeToRestoreOneHealth);
        }

        if (currentHealth == maxHealth)
        {
            nextRestoreTime = DateTime.MinValue;
        }

        SaveHealthData();
    }

    private void SaveHealthData()
    {
        PlayerPrefs.SetInt(PREF_HEALTH, currentHealth);
        if (currentHealth < maxHealth)
        {
            PlayerPrefs.SetString(PREF_NEXT_RESTORE_TIME, nextRestoreTime.ToString());
        }
        else
        {
            PlayerPrefs.DeleteKey(PREF_NEXT_RESTORE_TIME);
        }

        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (txtHealthCount != null)
        {
            txtHealthCount.text = currentHealth.ToString();
        }
    }

    private void UpdateTimerUI(TimeSpan timeRemaining)
    {
        // 4:59
        if (txtRestoreTime != null)
        {
            txtRestoreTime.text = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);
        }

        if (currentHealth >= maxHealth)
        {
            txtRestoreTime.text = "Full";
        }
    }

    public bool CanPlay()
    {
        return currentHealth > 0;
    }

    // trừ lượt chơi
    public void DeductHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
        }

        if (currentHealth == maxHealth - 1)
        {
            nextRestoreTime = DateTime.Now.AddSeconds(timeToRestoreOneHealth);
        }

        SaveHealthData();
        UpdateUI();
    }

    // hoàn trả lượt chơi nếu thắng
    public void RefundHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;

            if (currentHealth == maxHealth)
            {
                nextRestoreTime = DateTime.MinValue;
            }

            SaveHealthData();
            UpdateUI();
        }
    }

    public void ShowOutOfHealthPopup()
    {
        if (panelOutOfHealth != null)
        {
            panelOutOfHealth.SetActive(true);
        }
    }

    public void HideOutOfHealthPopup()
    {
        if (panelOutOfHealth != null)
        {
            panelOutOfHealth.SetActive(false);
        }
    }
}
