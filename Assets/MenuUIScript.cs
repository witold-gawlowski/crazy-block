using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIScript : MonoBehaviour
{
    public void HandleStartClick()
    {
        Debug.Log("omg");
        SceneManager.LoadScene("MainScene");
    }
}
