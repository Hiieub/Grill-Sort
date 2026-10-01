using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class BottomBarController : MonoBehaviour
{
    [SerializeField] private GameObject panelHome;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject panelLock;

    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnLock;

    [SerializeField] private GameObject homeLabel;
    [SerializeField] private GameObject shopLabel;
    [SerializeField] private GameObject lockLabel;

    [SerializeField] private RectTransform slideBackground;
    [SerializeField] private RectTransform iconHome;
    [SerializeField] private RectTransform iconShop;
    [SerializeField] private RectTransform iconLock;

    [SerializeField] private float iconMoveUpAmount = 55f;
    private float iconOriginalY;

    private UnityAction onHomeClick;
    private UnityAction onShopClick;
    private UnityAction onLockClick;

    private void Awake()
    {
        onHomeClick = () => ShowTab(Tab.Home, btnHome.GetComponent<RectTransform>());
        onShopClick = () => ShowTab(Tab.Shop, btnShop.GetComponent<RectTransform>());
        onLockClick = () => ShowTab(Tab.Lock, btnLock.GetComponent<RectTransform>());

        if (btnHome) btnHome.onClick.AddListener(onHomeClick);
        if (btnShop) btnShop.onClick.AddListener(onShopClick);
        if (btnLock) btnLock.onClick.AddListener(onLockClick);

        if (iconShop != null) iconOriginalY = iconShop.anchoredPosition.y;
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();

        ShowTab(Tab.Home, btnHome.GetComponent<RectTransform>(), true);
    }

    private void OnDestroy()
    {
        if (btnHome) btnHome.onClick.RemoveListener(onHomeClick);
        if (btnShop) btnShop.onClick.RemoveListener(onShopClick);
        if (btnLock) btnLock.onClick.RemoveListener(onLockClick);
    }

    private enum Tab { Home, Shop, Lock }

    private void ShowTab(Tab tab, RectTransform targetBtn, bool isInstant = false)
    {
        float duration = isInstant ? 0f : 0.3f;

        panelHome.SetActive(tab == Tab.Home);
        panelShop.SetActive(tab == Tab.Shop);
        panelLock.SetActive(tab == Tab.Lock);
        //btnHome.image.enabled = (tab == Tab.Home);
        //btnShop.image.enabled = (tab == Tab.Shop);
        //btnLock.image.enabled = (tab == Tab.Lock);

        if (slideBackground && targetBtn)
        {
            slideBackground.DOKill();
            slideBackground.DOMoveX(targetBtn.position.x, duration).SetEase(Ease.OutBack);
        }

        AnimateIcon(iconHome, tab == Tab.Home, duration);
        AnimateIcon(iconShop, tab == Tab.Shop, duration);
        AnimateIcon(iconLock, tab == Tab.Lock, duration);

        if (homeLabel) homeLabel.SetActive(tab == Tab.Home);
        if (shopLabel) shopLabel.SetActive(tab == Tab.Shop);
        if (lockLabel) lockLabel.SetActive(tab == Tab.Lock);
    }

    private void AnimateIcon(RectTransform icon, bool isSelected, float duration)
    {
        if (icon == null) return;

        icon.DOKill();
        float targetY = isSelected ? iconOriginalY + iconMoveUpAmount : iconOriginalY;
        icon.DOAnchorPosY(targetY, duration).SetEase(Ease.OutBack);
    }
}
