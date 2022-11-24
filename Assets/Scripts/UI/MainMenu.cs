using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _recordsButton;
    [SerializeField] private Button _backButton;
    public GameObject recordsPanel;
    public GameObject mainMenuPanel;

    private void Start()
    {
        _playButton.onClick.AddListener(StartGame);
        _recordsButton.onClick.AddListener(ShowRecords);
        _exitButton.onClick.AddListener(Exit);
        _backButton.onClick.AddListener(HideRecords);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void ShowRecords()
    {
        mainMenuPanel.SetActive(false);
        recordsPanel.SetActive(true);
    }

    private void HideRecords()
    {
        recordsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
