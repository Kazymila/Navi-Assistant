using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MapDataModel;
using UnityEngine.Localization.Settings;

public class OptionsButtonsController : MonoBehaviour
{
    [SerializeField] private int _maxOptions = 2;
    private List<TranslatedText> _optionsTranslatedTexts;
    private List<GameObject>[] _optionsShowGroups;
    private GameObject _optionButtonTemplate;
    private GameObject _moreOptionsButton;
    private GameObject _backOptionsButton;

    void Awake()
    {
        _optionsTranslatedTexts = new List<TranslatedText>();
        _optionButtonTemplate = transform.GetChild(0).gameObject;
        _moreOptionsButton = transform.GetChild(1).gameObject;
        _backOptionsButton = transform.GetChild(2).gameObject;
        _moreOptionsButton.GetComponent<Button>().onClick.AddListener(GoNextOptions);
        _backOptionsButton.GetComponent<Button>().onClick.AddListener(GoBackOptions);
    }

    public void AddOptionButton(TranslatedText _optionText, UnityAction _optionAction)
    {   // Add a new option button to the list
        GameObject _optionButton = Instantiate(_optionButtonTemplate, transform);
        _optionButton.name = "Option " + (_optionButton.transform.GetSiblingIndex() - 3) + ": " + _optionText.key;
        _optionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _optionText.key;
        _optionButton.GetComponent<Button>().onClick.AddListener(_optionAction);
        _optionsTranslatedTexts.Add(_optionText);
    }

    public void ShowOptionsButtons()
    {   // Show the options buttons in groups
        int _optionsToShow = transform.childCount - 3 / _maxOptions;
        _optionsShowGroups = new List<GameObject>[_optionsToShow];

        for (int i = 0; i < _optionsToShow; i++)
        {   // Add the options buttons to the groups
            _optionsShowGroups[i] = new List<GameObject>();
            for (int j = 0; j < _maxOptions; j++)
            {
                int _optionIndex = i * _maxOptions + j;
                if (_optionIndex < transform.childCount - 3)
                {
                    GameObject _optionButton = transform.GetChild(_optionIndex + 3).gameObject;
                    _optionsShowGroups[i].Add(_optionButton);
                    UpdateLanguageText(_optionButton);
                }
            }
        }
        // Show the first group of options buttons
        foreach (GameObject _optionButton in _optionsShowGroups[0])
            _optionButton.SetActive(true);

        print(_optionsShowGroups.Length);

        _moreOptionsButton.transform.SetAsLastSibling();
        _moreOptionsButton.SetActive(_optionsShowGroups.Length > 1);
        _backOptionsButton.transform.SetAsLastSibling();
        _backOptionsButton.SetActive(false);
    }

    public void HideOptionsButtons()
    {   // Hide the options buttons
        foreach (Transform _child in transform)
            _child.gameObject.SetActive(false);

        _backOptionsButton.transform.SetAsFirstSibling();
        _moreOptionsButton.transform.SetAsFirstSibling();
        _optionButtonTemplate.transform.SetAsFirstSibling();
        _optionsShowGroups = null;
    }

    private void UpdateLanguageText(GameObject _optionButton)
    {   // Update the text of the option button to the current language
        string _optionKey = _optionButton.name.Split(':')[1].Trim();
        TranslatedText _optionText = _optionsTranslatedTexts.Find(_option => _option.key == _optionKey);

        if (_optionText != null)
        {   // Get the translation of the option text by the current language
            string _languageCode = LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0];
            _optionButton.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                _optionText.GetTranslationByCode(_languageCode);
        }
    }

    private void GoNextOptions()
    {   // Show the next group of options buttons
        for (int i = 0; i < _optionsShowGroups.Length; i++)
        {
            if (_optionsShowGroups[i][0].activeSelf)
            {   // Hide the current group and show the next group
                foreach (GameObject _optionButton in _optionsShowGroups[i])
                    _optionButton.SetActive(false);

                if (i + 1 < _optionsShowGroups.Length)
                {    // Show the next group of options buttons
                    foreach (GameObject _optionButton in _optionsShowGroups[i + 1])
                        _optionButton.SetActive(true);
                }
                _backOptionsButton.SetActive(true);
                break;
            }
        }
    }

    private void GoBackOptions()
    {   // Show the previous group of options buttons
        for (int i = 0; i < _optionsShowGroups.Length; i++)
        {
            if (_optionsShowGroups[i][0].activeSelf)
            {   // Hide the current group and show the previous group
                foreach (GameObject _optionButton in _optionsShowGroups[i])
                    _optionButton.SetActive(false);

                if (i - 1 >= 0)
                {   // Show the previous group of options buttons
                    foreach (GameObject _optionButton in _optionsShowGroups[i - 1])
                        _optionButton.SetActive(true);
                    _backOptionsButton.SetActive(i - 1 > 0);
                }
                break;
            }
        }
    }
}
