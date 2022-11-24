using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveHightScoreManager : MonoBehaviour
{
    public InputField userName;
    public Text score;
    public Button saveButton;
    private HightScoreManager hightScoreManager;

    private void Start()
    {
        hightScoreManager = new HightScoreManager();
        saveButton.onClick.AddListener(SaveResult);
    }

    private void SaveResult()
    {
        hightScoreManager.SaveHightScoreEntry(int.Parse(score.text),userName.text);
        SceneManager.LoadScene("Main Menu");
    }
}
