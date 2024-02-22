using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamplayUIScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    public void HandleGameOver()
    {
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

}
