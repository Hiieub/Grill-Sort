using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnHome;

    private void Awake()
    {
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
        if (btnClose != null)
        {
            btnClose.onClick.AddListener(OnBtnCloseClicked);
        }

        if (btnContinue != null)
        {
            btnContinue.onClick.AddListener(OnBtnContinueClicked);
        }

        if (btnRestart != null)
        {
            btnRestart.onClick.AddListener(OnBtnRestartClicked);
        }

        if (btnHome != null)
        {
            btnHome.onClick.AddListener(OnBtnHomeClicked);
        }
    }

    private void OnDestroy()
    {
        if (btnClose != null)
        {
            btnClose.onClick.RemoveListener(OnBtnCloseClicked);
        }

        if (btnContinue != null)
        {
            btnContinue.onClick.RemoveListener(OnBtnContinueClicked);
        }

        if (btnRestart != null)
        {
            btnRestart.onClick.RemoveListener(OnBtnRestartClicked);
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
                ShowPanel();
                break;

            case GameState.Playing:
                HidePanel();
                break;

            case GameState.Victory:
                HidePanel();
                break;

            case GameState.GameOver:
                HidePanel();
                break;

            case GameState.Loading:
                HidePanel();
                break;
        }
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

    private void OnBtnCloseClicked()
    {
        ResumeGame();
    }

    private void OnBtnContinueClicked()
    {
        ResumeGame();
    }

    private void ResumeGame()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.ResumeGame();
    }

    private void OnBtnRestartClicked()
    {
        Debug.Log("[PausePanel] Restart clicked");

        if (GameManager.Instance == null)
            return;

        GameManager.Instance.RestartLevel();
    }

    private void OnBtnHomeClicked()
    {
        Debug.Log("[PausePanel] Home clicked");

        // TODO:
        // Gọi LevelLoader để về Home Scene.
    }
}