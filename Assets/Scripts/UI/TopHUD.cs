using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopHUD : MonoBehaviour
{
    [SerializeField] private Image timeProgressFill;
    [SerializeField] private TextMeshProUGUI txtTimer;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private Image levelProgressFill;
    [SerializeField] private TextMeshProUGUI txtStarCount;

    private float maxTime;
    private int maxWare;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
        GameManager.OnMergeSuccess += HandleMergeSuccess;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
        GameManager.OnMergeSuccess -= HandleMergeSuccess;
    }

    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
        {
            SetupHUD();
        }
    }

    private void SetupHUD()
    {
        maxTime = GameManager.Instance.MaxTime;
        maxWare = GameManager.Instance.RemainingWare;

        txtLevel.text = "Level " + GameManager.Instance.CurrentLevelIndex;
        txtStarCount.text = maxWare.ToString();

        timeProgressFill.fillAmount = 1f;
        levelProgressFill.fillAmount = 0f;
    }

    private void HandleMergeSuccess(int remainingWare)
    {
        txtStarCount.text = remainingWare.ToString();

        if (maxWare > 0)
        {
            int matchedWare = maxWare - remainingWare;
            levelProgressFill.fillAmount = (float)matchedWare / maxWare;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        float currentTime = GameManager.Instance.RemainingTime;

        txtTimer.text = currentTime.ToString("F0");

        if (maxTime > 0)
        {
            timeProgressFill.fillAmount = currentTime / maxTime;
        }
    }
}
