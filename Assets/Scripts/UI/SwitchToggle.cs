using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Toggle))]
public class SwitchToggle : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform handleRect;
    [SerializeField] private Image backgroundImage;

    [Header("Settings")]
    [SerializeField] private float onPositionX = 25f;
    [SerializeField] private float offPositionX = -25f;

    [SerializeField] private Color onColor = new Color(0f, 0.8f, 0f); // Xanh lá
    [SerializeField] private Color offColor = new Color(0.6f, 0.6f, 0.6f); // Xám

    [SerializeField] private float animationDuration = 0.2f;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        
        toggle.onValueChanged.AddListener(OnSwitch);

        OnSwitch(toggle.isOn, instant: true);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

    private void OnSwitch(bool isOn)
    {
        OnSwitch(isOn, instant: false);
    }

    private void OnSwitch(bool isOn, bool instant)
    {
        float targetX = isOn ? onPositionX : offPositionX;
        Color targetColor = isOn ? onColor : offColor;

        if (handleRect != null)
        {
            handleRect.DOKill();
            if (instant)
            {
                handleRect.anchoredPosition = new Vector2(targetX, handleRect.anchoredPosition.y);
            }
            else
            {
                handleRect.DOAnchorPosX(targetX, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }

        if (backgroundImage != null)
        {
            backgroundImage.DOKill();
            if (instant)
            {
                backgroundImage.color = targetColor;
            }
            else
            {
                backgroundImage.DOColor(targetColor, animationDuration).SetUpdate(true);
            }
        }
    }
}
