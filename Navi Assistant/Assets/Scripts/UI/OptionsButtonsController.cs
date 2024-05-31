using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using MapDataModel;
using TMPro;

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
        _optionsTranslatedTexts.Add(_optionText);

        Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();
        _onClick.AddListener(_optionAction);              // Add the action to the option button
        _onClick.AddListener(() => HideOptionsButtons()); // Hide the options buttons after click
        _optionButton.GetComponent<Button>().onClick = _onClick;
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
        // Clean null values from the array
        _optionsShowGroups = _optionsShowGroups.Where(_group => _group.Count > 0).ToArray();

        // Show the first group of options buttons
        foreach (GameObject _optionButton in _optionsShowGroups[0])
            _optionButton.SetActive(true);

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
                if (i + 1 == _optionsShowGroups.Length - 1)
                {   // Only show the back button if is the last group
                    _backOptionsButton.SetActive(true);
                    _moreOptionsButton.SetActive(false);
                }
                else
                {   // Show the more options button if is not the last group
                    _backOptionsButton.SetActive(true);
                    _moreOptionsButton.SetActive(true);
                }
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
                if (i - 1 == 0)
                {   // Only show the more options button if is the first group
                    _backOptionsButton.SetActive(false);
                    _moreOptionsButton.SetActive(true);
                }
                else
                {   // Show the more options button if is not the first group
                    _backOptionsButton.SetActive(true);
                    _moreOptionsButton.SetActive(true);
                }
                break;
            }
        }
    }
}
