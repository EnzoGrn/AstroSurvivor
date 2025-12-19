using AstroSurvivor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadUI : MonoBehaviour {

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text waveText;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestartClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    public void Show(int currentWave)
    {
        root.SetActive(true);

        waveText.text = $"Wave reached: {currentWave}";
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();

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
