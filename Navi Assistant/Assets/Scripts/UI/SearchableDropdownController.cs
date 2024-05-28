using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using MapDataModel;
using UnityEngine.Localization.Settings;

public class SearchableDropdownController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _itemsDisplay;
    [SerializeField] private GameObject _itemsContainer;
    [SerializeField] private GameObject _itemTemplate;
    [SerializeField] private Animator _arrowAnimator;

    [Header("Dropdown Settings")]
    [SerializeField] private string _inputText;
    [SerializeField] private int _selectedOptionIndex = -1;
    [SerializeField] private List<TranslatedText> _dropdownOptions;
    [SerializeField] private List<string> _filteredOptions;
    [SerializeField] private UnityEvent _onOptionSelected;

    private void Awake()
    {
        _dropdownOptions = new List<TranslatedText>();
        _filteredOptions = new List<string>();
        _itemTemplate.SetActive(false);
        _itemsDisplay.SetActive(false);

        // Test dropdown options
        /*
        List<TranslatedText> _options = new List<TranslatedText> {
            new TranslatedText("Option 1", "Option 1", "Opción 1"),
            new TranslatedText("Option 2", "Option 2", "Opción 2"),};
        SetDropdownOptions(_options);*/
    }

    #region --- Dropdown visibility ---
    public void ShowDropdown()
    {   // Show dropdown items
        if (_filteredOptions.Count == 0) return;
        _arrowAnimator.Play("ShowDropdown", 0, 0);
        _itemsDisplay.SetActive(true);
    }
    public void HideDropdown()
    {   // Hide dropdown items
        if (_filteredOptions.Count == 0) return;
        _arrowAnimator.Play("HideDropdown", 0, 0);
        _itemsDisplay.SetActive(false);
    }

    public void ToogleDropdown()
    {   // Toggle dropdown visibility on click
        if (_itemsDisplay.activeSelf) HideDropdown();
        else ShowDropdown();
    }

    public void AdjustItemDisplaySize()
    {   // Adjust dropdown items display size
        RectTransform _itemsRect = _itemsDisplay.GetComponent<RectTransform>();

        if (_filteredOptions.Count > 3)
            _itemsRect.sizeDelta = new Vector2(_itemsRect.sizeDelta.x, 360);
        else
            _itemsRect.sizeDelta = new Vector2(_itemsRect.sizeDelta.x, _filteredOptions.Count * 120);
    }
    #endregion

    public void SetDropdownOptions(List<TranslatedText> _options)
    {   // Set dropdown options from a list
        _dropdownOptions.Clear();
        _filteredOptions.Clear();
        _dropdownOptions = _options;
        _filteredOptions = _dropdownOptions.ConvertAll(option => option.key);

        // Clear existing dropdown items
        if (_itemsContainer.transform.childCount > 1)
            foreach (Transform _child in _itemsContainer.transform)
                if (_child != _itemTemplate.transform)
                    Destroy(_child.gameObject);

        // Instantiate dropdown items based on selected language
        string _languageCode = LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0];

        foreach (TranslatedText _option in _options) InstantiateItem(_option);

        _selectedOptionIndex = -1;
        _inputField.text = "";
    }

    public string GetSelectedOption()
    {   // Get selected option from dropdown
        if (_selectedOptionIndex < 0) return "";
        return _dropdownOptions[_selectedOptionIndex].key;
    }

    public void FilterDropdown(string _input)
    {   // Filter dropdown options based on input text
        if (_input == "")
        {   // Show all options if input is empty
            _filteredOptions = _dropdownOptions.ConvertAll(option => option.key);
            UpdateDropdownOptions();
            ShowDropdown();
            return;
        }
        _itemsDisplay.SetActive(false);
        _inputText = _input.ToLower();
        _filteredOptions = _dropdownOptions.ConvertAll(option => option.key).FindAll(
            option => option.ToLower().Contains(_inputText)
        );
        UpdateDropdownOptions();
        ShowDropdown();
    }

    private void UpdateDropdownOptions()
    {   // Update dropdown options based on filtered options
        foreach (Transform _child in _itemsContainer.transform)
            _child.gameObject.SetActive(false); // Hide all items

        foreach (string _option in _filteredOptions)
        {   // Show filtered items in dropdown
            int _itemIndex = _dropdownOptions.FindIndex(_opt => _opt.key == _option) + 1;
            _itemsContainer.transform.GetChild(_itemIndex).gameObject.SetActive(true);
        }
        AdjustItemDisplaySize();
    }

    #region --- Dropdown Items ---
    public void ChangeSelectedItem(string _option)
    {   // Change selected option based on input text
        if (_filteredOptions.Count == 0) return;
        _inputField.text = _dropdownOptions.Find(_opt => _opt.key == _option).GetTranslationByCode(
            LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0]);
        _selectedOptionIndex = _filteredOptions.IndexOf(_option);
        _itemsDisplay.SetActive(false);
    }

    public void SelectOption(int _index)
    {   // Select an option from dropdown
        if (_index < 0 || _index > _dropdownOptions.Count) return;

        _selectedOptionIndex = _index;
        _inputField.text = _dropdownOptions[_selectedOptionIndex].GetTranslationByCode(
            LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0]);

        _onOptionSelected.Invoke();
        HideDropdown();
    }

    private void InstantiateItem(TranslatedText _itemName)
    {   // Instantiate dropdown item
        GameObject _item = Instantiate(_itemTemplate, _itemsContainer.transform);
        _item.name = "Item " + _dropdownOptions.IndexOf(_itemName) + ": " + _itemName.key;
        Toggle _itemToggle = _item.GetComponent<Toggle>();

        // Add listener to toggle component
        _item.GetComponent<Toggle>().onValueChanged.AddListener(
            (value) => SelectOption(_dropdownOptions.IndexOf(_itemName)));

        // Set item text and toggle state
        _item.GetComponentInChildren<TextMeshProUGUI>().text = _itemName.GetTranslationByCode(
            LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0]);
        _itemToggle.isOn = false;
        _item.SetActive(true);
    }
    #endregion
}
