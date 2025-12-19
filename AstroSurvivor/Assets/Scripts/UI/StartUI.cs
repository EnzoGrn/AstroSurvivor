using AstroSurvivor;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour {

    [SerializeField] private GameObject root;

    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveButton;

    public Action OnGameStarted;

    public void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    private void OnStartClicked()
    {
        OnGameStarted?.Invoke();

        Hide();
    }

    private void OnLeaveClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
