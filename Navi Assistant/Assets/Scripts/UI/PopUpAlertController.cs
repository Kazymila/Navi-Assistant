using UnityEngine;
using TMPro;

public class PopUpAlertController : MonoBehaviour
{
    private TextMeshProUGUI _alertText;

    void Awake()
    {
        _alertText = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(false);
    }

    public void ShowAlert(string message)
    {   // Show the alert message until the user closes it
        _alertText.text = message;
        this.gameObject.SetActive(true);
    }

    public void ShowTimingAlert(string message, float duration)
    {   // Show the alert message for a specific duration
        _alertText.text = message;
        this.gameObject.SetActive(true);
        Invoke("HideAlert", duration);
    }

    public void HideAlert()
    {   // Hide the alert message
        _alertText.text = "";
        this.gameObject.SetActive(false);
    }
}
