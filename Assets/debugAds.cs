using TMPro;
using UnityEngine;

public class DebugAds : MonoBehaviour
{
    private string debugMessage = "";

    [SerializeField] private TMP_Text debugText;

    public void PritText(string text)
    {
        debugMessage += "\n";
        debugMessage += text;
        debugText.text = debugMessage;
    }
}