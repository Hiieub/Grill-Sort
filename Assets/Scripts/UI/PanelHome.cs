using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PanelHome : MonoBehaviour
{
    [SerializeField] private PanelHomeSetting panelSetting;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnPlay;

    [SerializeField] private float scaleMultiplier = 1.05f;
    [SerializeField] private float duration = 0.6f;

    private Vector3 originalScale;
    private UnityAction onSettingClick;

    private void Awake()
    {
        originalScale = btnPlay.transform.localScale;

        onSettingClick = () => panelSetting.Open();
        if (btnSetting) btnSetting.onClick.AddListener(onSettingClick);
    }

    private void OnDestroy()
    {
        if (btnSetting) btnSetting.onClick.RemoveListener(onSettingClick);
    }

    private void OnEnable()
    {
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
}
