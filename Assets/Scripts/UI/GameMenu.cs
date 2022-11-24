using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _pauseExitButton;
    [SerializeField] private Button _gameOverExitButton;
    public GameObject pauseMenuUI;

    private void Start()
    {
        _pauseButton.onClick.AddListener(Pause);
        _resumeButton.onClick.AddListener(Resume);
        _pauseExitButton.onClick.AddListener(Quit);
        _gameOverExitButton.onClick.AddListener(Quit);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseOrResume();
        }   
    }

    public void PauseOrResume()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Quit()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
