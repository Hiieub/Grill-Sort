using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button btnTryAgain;
    [SerializeField] private Button btnHome;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();

        RegisterButtonEvents();
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= OnGameStateChanged;
    }

    private void RegisterButtonEvents()
    {
        if (btnTryAgain != null)
        {
            btnTryAgain.onClick.AddListener(OnBtnTryAgainClicked);
        }

        if (btnHome != null)
        {
            btnHome.onClick.AddListener(OnBtnHomeClicked);
        }
    }

    private void OnDestroy()
    {
        if (btnTryAgain != null)
        {
            btnTryAgain.onClick.RemoveListener(OnBtnTryAgainClicked);
        }

        if (btnHome != null)
        {
            btnHome.onClick.RemoveListener(OnBtnHomeClicked);
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Pausing:
                HidePanel();
                break;

            case GameState.Playing:
                HidePanel();
                break;

            case GameState.Victory:
                HidePanel();
                break;

            case GameState.GameOver:
                ShowPanel();
                break;

            case GameState.Loading:
                HidePanel();
                break;
        }
    }

    private void ShowPanel()
    {
        img.raycastTarget = true;

        Color startColor = img.color;
        startColor.a = 0f;
        img.color = startColor;

        img.DOKill();
        img.DOFade(150f / 255f, 0.3f).SetUpdate(true);

        GameObject content = transform.GetChild(0).gameObject;

        if (!content.activeSelf)
        {
            content.SetActive(true);
        }
        content.transform.DOKill();
        content.transform.localScale = Vector3.zero;
        content.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void HidePanel()
    {
        img.raycastTarget = false;

        img.DOKill();
        img.DOFade(0f, 0.2f).SetUpdate(true);

        GameObject content = transform.GetChild(0).gameObject;
        if (content.activeSelf)
        {
            content.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                content.SetActive(false);
            });
        }
    }

    private void OnBtnTryAgainClicked()
    {
        Debug.Log("[GameOverPanle] TryAgain clicked");

        if (HealthManager.Instance.CanPlay())
        {
            HealthManager.Instance.DeductHealth();

            if (GameManager.Instance == null)
                return;

            GameManager.Instance.RestartLevel();
        }
        else
        {
            Debug.Log("Hết Tym");
            // TODO: hết lượt, hiện bảng thông báo
            HealthManager.Instance.ShowOutOfHealthPopup();
        }
    }

    private void OnBtnHomeClicked()
    {
        Debug.Log("[GameOverPanle] Home clicked");

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.GoToHome();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }
}
