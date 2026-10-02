using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    public static event Action OnAnyButtonClicked;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BtnClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(BtnClicked);
    }

    private void BtnClicked()
    {
        OnAnyButtonClicked?.Invoke();
    }
}