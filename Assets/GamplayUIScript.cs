using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamplayUIScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text levelReachedMessage;
    public void HandleGameOver()
    {
        levelReachedMessage.text = "You reached level " + GlobalGameManager.Instance.GetLevel() + "!";
        gameOverPanel.SetActive(true);
    }

    public void HandleMenuButtonPressed()
    {
        GlobalGameManager.Instance.OpenMenu();
    }

    public void HandleRestartButtonPressed()
    {
        GlobalGameManager.Instance.Restart();
    }

    public void HandleRestart()
    {
        gameOverPanel.SetActive(false);
    }

    public void HandleCheat()
    {
        GlobalGameManager.Instance.FinalizeLevel();
    }

}
