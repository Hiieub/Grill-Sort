using UnityEngine;
using UnityEngine.UI;

public class BtnSetting : MonoBehaviour
{
    [SerializeField] private PausePanel pausePanel;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();


        if (button != null)
        {
            button.onClick.AddListener(OnSettingClicked);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnSettingClicked);
        }
    }

    private void OnSettingClicked()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("[BtnSetting] PausePanel chưa được assign.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[BtnSetting] GameManager.Instance không tồn tại.");
            return;
        }

        pausePanel.Open();

        GameManager.Instance.PauseGame();
    }
}
