using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelHomeSetting : MonoBehaviour
{
    [SerializeField] private Button btnClose;

    private void Awake()
    {
        if (btnClose) btnClose.onClick.AddListener(HidePanel);
    }

    private void OnDestroy()
    {
        if (btnClose) btnClose.onClick.RemoveListener(HidePanel);
    }

    private void Start()
    {
        HidePanel();
    }

    public void Open()
    {
        ShowPanel();
    }

    private void ShowPanel()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        transform.GetChild(0).DOKill();
        transform.GetChild(0).localScale = Vector3.zero;
        transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void HidePanel()
    {
        if (gameObject.activeSelf)
        {
            transform.GetChild(0).DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
