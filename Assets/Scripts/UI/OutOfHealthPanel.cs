using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutOfHealthPanel : MonoBehaviour
{
    [SerializeField] private Button btnClose;

    private UnityAction HidePoup = () => HealthManager.Instance.HideOutOfHealthPopup();

    private void Awake()
    {
        if (btnClose != null)
        {
            btnClose.onClick.AddListener(HidePoup);
        }
    }

    private void OnDestroy()
    {
        if (btnClose != null)
        {
            btnClose.onClick.RemoveListener(HidePoup);
        }
    }
}
