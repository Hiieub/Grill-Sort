using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;

[RequireComponent(typeof(CanvasGroup))]
public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Button btnContinue;
    [SerializeField] private TextMeshProUGUI txtLevelComplete;
    [SerializeField] private TextMeshProUGUI txtSubMessage;

    [Header("Animation - Popup")]
    [SerializeField] private RectTransform popupBox;

    [Header("Animation - Spine")]
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private string animationName = "win";
    [SerializeField] private bool loopAnimation  = true;

    [Header("Level Config")]
    [SerializeField] private int totalLevels = 5;

    private const string MSG_NORMAL   = "Level {0} Complete!";
    private const string MSG_ALL_DONE = "All Levels Complete";
    private const string SUB_ALL_DONE = "More levels coming soon!\nStarting from Level 1";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetVisible(false, instant: true);

        if (btnContinue != null)
            btnContinue.onClick.AddListener(OnBtnContinueClicked);

        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnStateChanged -= HandleStateChanged;

        if (btnContinue != null)
            btnContinue.onClick.RemoveListener(OnBtnContinueClicked);
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Victory)
            ShowPanel();
    }

    public void ShowPanel()
    {
        int currentLevel = GameManager.Instance != null ? GameManager.Instance.CurrentLevelIndex : 1;
        bool isLastLevel  = (currentLevel >= totalLevels);

        if (txtLevelComplete != null)
            txtLevelComplete.text = isLastLevel
                ? MSG_ALL_DONE
                : string.Format(MSG_NORMAL, currentLevel);

        if (txtSubMessage != null)
            txtSubMessage.text = isLastLevel ? SUB_ALL_DONE : string.Empty;

        SetVisible(true, instant: true);

        if (popupBox != null)
        {
            popupBox.DOKill();
            popupBox.localScale = Vector3.zero;
            popupBox.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        PlaySpine();
    }

    private void PlaySpine()
    {
        if (skeletonGraphic == null || string.IsNullOrEmpty(animationName)) return;

        skeletonGraphic.AnimationState.SetAnimation(0, animationName, loopAnimation);
    }

    private void SetVisible(bool visible, bool instant = false)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha          = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable   = visible;
    }

    private void OnBtnContinueClicked()
    {
        Debug.Log("[VictoryPanel] Continue clicked");

        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.AdvanceToNextLevel(totalLevels);

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.GoToHome();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }
}
