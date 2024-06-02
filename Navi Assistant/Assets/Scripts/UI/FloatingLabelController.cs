using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine;
using MapDataModel;
using TMPro;

public class FloatingLabelController : MonoBehaviour
{
    [Header("Target Label Settings")]
    [SerializeField] private TranslatedText _labelText;

    [Header("External References")]
    [SerializeField] private NavigationManager _navigationManager;

    private GameObject _floatingLabel;
    private TextMeshProUGUI _labelTextComponent;
    private Animator _labelAnimator;
    private bool _isLabelVisible = false;

    void Awake()
    {
        _floatingLabel = this.transform.GetChild(0).gameObject;
        _labelTextComponent = _floatingLabel.GetComponentInChildren<TextMeshProUGUI>();
        _labelAnimator = this.GetComponent<Animator>();
    }

    void Update()
    {   // Show the label when the user is looking at the path
        if (_isLabelVisible) _floatingLabel.SetActive(true);
        else _floatingLabel.SetActive(false);
    }

    public void SetLabelText(TranslatedText _text)
    {   // Set the label text based on the selected language
        string _languageCode = LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0];

        if (_labelTextComponent == null)
        {   // Initialize the label text component
            _floatingLabel = this.transform.GetChild(0).gameObject;
            _labelTextComponent = _floatingLabel.GetComponentInChildren<TextMeshProUGUI>();
        }
        _labelTextComponent.text = _text.GetTranslationByCode(_languageCode);
        _labelText = _text;
    }

    private void HideLabel() => _isLabelVisible = false;

    private void OnTriggerStay(Collider other)
    {   // Show the label when the user is looking at the path
        if (other.CompareTag("Player"))
        {
            string _currentRoom = _navigationManager.GetCurrentRoom();
            float _distance = Vector3.Distance(this.transform.position, other.transform.position);

            // Hide the label if the user is in the target room
            if (_currentRoom == _labelText.key) _isLabelVisible = false;

            else if (_distance < 0.7f)
            {   // Show the label if the user is close to the target
                _labelAnimator.Play("Pop", 0);
                Invoke("HideLabel", 0.10f);
            }
            else
            {   // Show the label if it is hidden
                _isLabelVisible = true;
                if (this.transform.localScale == Vector3.zero)
                    _labelAnimator.Play("Show", 0);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {   // Hide the target when the user is not looking at the path
        if (other.CompareTag("Player"))
        {
            _isLabelVisible = false;
        }
    }
}
