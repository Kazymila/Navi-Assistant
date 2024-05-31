using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorMessagePanelController : MonoBehaviour
{
    [SerializeField] private Sprite[] _errorIcons;
    private TextMeshProUGUI _errorTitle;
    private TextMeshProUGUI _errorMessage;
    private Image _errorIcon;

    void Awake()
    {
        _errorTitle = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _errorMessage = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _errorIcon = this.transform.GetChild(2).GetComponent<Image>();
        this.gameObject.SetActive(false);
    }

    public void SetErrorMessage(string title, string message, int iconIndex)
    {   // Set the error message to be displayed
        _errorTitle.text = title;
        _errorMessage.text = message;
        _errorIcon.sprite = _errorIcons[iconIndex];
    }
}
