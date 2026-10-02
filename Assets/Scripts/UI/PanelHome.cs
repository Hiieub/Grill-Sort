using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class PanelHome : MonoBehaviour
{
    [SerializeField] private PanelHomeSetting panelSetting;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnPlay;

    [SerializeField] private TextMeshProUGUI txtPlayLevel;

    [SerializeField] private float scaleMultiplier = 1.05f;
    [SerializeField] private float duration = 0.6f;

    private Vector3 originalScale;
    private UnityAction onSettingClick;
    private UnityAction onPlayClick;

    private void Awake()
    {
        originalScale = btnPlay.transform.localScale;

        onSettingClick = () => panelSetting.Open();
        if (btnSetting != null) btnSetting.onClick.AddListener(onSettingClick);

        onPlayClick = OnBtnPlayClicked;
        if (btnPlay != null) btnPlay.onClick.AddListener(onPlayClick);
    }

    private void OnDestroy()
    {
        if (btnSetting) btnSetting.onClick.RemoveListener(onSettingClick);
        if (btnPlay) btnPlay.onClick.RemoveListener(onPlayClick);
    }

    private void OnBtnPlayClicked()
    {
        Debug.Log("[PanelHome] Play clicked");

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.GoToMain();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    private void OnEnable()
    {
        RefreshLevelText();

        btnPlay.transform.localScale = originalScale;

        Vector3 targetScale = originalScale * scaleMultiplier;

        btnPlay.transform.DOScale(targetScale, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        btnPlay.transform.DOKill();
        btnPlay.transform.localScale = Vector3.one;
    }

    private void RefreshLevelText()
    {
        if (txtPlayLevel == null) return;

        int level = PlayerProgress.Instance != null
            ? PlayerProgress.Instance.CurrentLevel
            : 1;

        txtPlayLevel.text = "Level " + level;
    }
}

