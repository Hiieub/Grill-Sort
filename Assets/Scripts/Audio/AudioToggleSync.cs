using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggleSync : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            toggle.SetIsOnWithoutNotify(AudioManager.Instance.IsSoundEnabled());
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
    private void OnToggleValueChanged(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSoundEnabled(isOn);
        }
    }
}
