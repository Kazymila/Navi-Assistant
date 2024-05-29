using System.Collections;
using System.Collections.Generic;
using MapDataModel;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using System;

public class AssistantManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private NavigationManager _navManager;
    [SerializeField] private DestinationManager _destinationsManager;

    [Header("UI References")]
    [SerializeField] private GameObject _navigationUI;
    [SerializeField] private SearchableDropdownController _destinationDropdown;
    [SerializeField] private GameObject _dialoguePanel;
    private TextMeshProUGUI _dialogueTextDisplay;
    private GameObject _dialogArrow;

    [Header("Assistant Settings")]
    [SerializeField] private float typeTime;

    [Header("General Dialogues")]
    [SerializeField] private TranslatedText[] _introDialog;
    [SerializeField] private TranslatedText[] _goodbyeDialog;

    [Header("Destination Selection Dialogues")]
    [SerializeField] private TranslatedText[] _selectDestinationDialog;
    [SerializeField] private TranslatedText[] _selectFromDropdownDialog;
    [SerializeField] private TranslatedText[] _goToDestinationDialog;

    private GameObject _assistantModel;
    private string[] _sentencesToDisplay;
    private int _sentenceIndex = -1;
    private int _charIndex = 0;
    private bool _displayingText;
    private UnityEvent _onDialogueEnd;

    private PlayerInput _input;

    void Awake()
    {
        _input = this.GetComponent<PlayerInput>();
        _dialogArrow = _dialoguePanel.transform.GetChild(1).gameObject;
        _dialogueTextDisplay = _dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        _assistantModel = this.transform.GetChild(0).gameObject;

        _assistantModel.SetActive(true);
        _dialogArrow.SetActive(false);
        _navigationUI.SetActive(false);
        _destinationDropdown.gameObject.SetActive(false);

        SelectDestinationInteraction(); // Test the destination selection
    }

    private void Update()
    {
        if (_displayingText && _sentencesToDisplay.Length > 0)
        {
            if (_sentenceIndex >= 0 && _charIndex == _sentencesToDisplay[_sentenceIndex].Length)
                _dialogArrow.SetActive(true); // Show the arrow to continue

            if (_sentenceIndex == -1) StartDialogue();
            if (_input.actions["Click"].triggered) // TODO: CHECK INPUT HANDLER
            {
                /*if (_charIndex < _sentencesToDisplay[_sentenceIndex].Length)
                {   // Skip the current sentence
                    _dialogueTextDisplay.text = _sentencesToDisplay[_sentenceIndex];
                    _charIndex = _sentencesToDisplay[_sentenceIndex].Length;
                }
                else*/
                if (_charIndex == _sentencesToDisplay[_sentenceIndex].Length)
                    DisplayNextSentence(_sentencesToDisplay); // Display the next sentence
            }
        }
    }

    public void GoToDestination(string _destinationName)
    {   // Go to the selected destination
        SetDialogueToDisplay(_goToDestinationDialog);
        _displayingText = true;

        _assistantModel.SetActive(true);
        _destinationsManager.SetAllDestinationsOnDropdown();
        _destinationDropdown.ChangeSelectedItem(_destinationName);
        _destinationDropdown.gameObject.SetActive(true);
        _navigationUI.SetActive(true);

        _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() =>
        {
            _assistantModel.SetActive(false);
        });
    }

    public void SelectDestinationFromDropdown()
    {   // Select the destination alternative
        SetDialogueToDisplay(_selectFromDropdownDialog);
        _displayingText = true;

        _destinationDropdown.gameObject.SetActive(true);
    }

    private void SelectDestinationInteraction()
    {   // Show interaction to select a destination
        SetDialogueToDisplay(_selectDestinationDialog);
        _displayingText = true;

        _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() =>
        {   // When the dialogue ends, show the destination options
            _dialoguePanel.SetActive(true);
            _dialogueTextDisplay.text = _sentencesToDisplay[_selectDestinationDialog.Length - 1];
            _destinationsManager.ShowDestinationOptionsButtons();
        });
    }

    #region --- Dialogue Managment ---
    private void SetDialogueToDisplay(TranslatedText[] _dialog)
    {   // Set the dialogue to display
        string _languageCode = LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0];
        _sentencesToDisplay = new string[_dialog.Length];

        for (int i = 0; i < _dialog.Length; i++)
        {   // Get the sentences to display in the current language
            _sentencesToDisplay[i] = _dialog[i].GetTranslationByCode(_languageCode);
        }
    }

    private void StartDialogue()
    {   // Play the given dialogue in the current language
        _dialogueTextDisplay.text = "";
        _dialoguePanel.SetActive(true);
        _sentenceIndex = 0;
        _charIndex = 0;

        StartCoroutine(TypeText(_sentencesToDisplay[0]));
    }

    private void DisplayNextSentence(string[] _sentences)
    {   // Display the next sentence and check when dialogue ends
        if (_sentenceIndex == _sentences.Length - 1) EndDialogDisplay();
        else
        {   // Display the next sentence
            _charIndex = 0;
            _sentenceIndex++;
            _dialogueTextDisplay.SetText("");
            _dialogArrow.SetActive(false);

            StopAllCoroutines();
            string sentence = _sentences[_sentenceIndex];
            StartCoroutine(TypeText(sentence));
        }
    }

    private void EndDialogDisplay()
    {   // Clean the text display and ends the dialogue
        _charIndex = 0;
        _sentenceIndex = -1;
        _displayingText = false;
        _dialoguePanel.SetActive(false);
        _dialogueTextDisplay.SetText("");
        StopAllCoroutines();

        _onDialogueEnd.Invoke();
        _onDialogueEnd.RemoveAllListeners();
    }

    private IEnumerator TypeText(string sentence)
    {   // Display the text on screen like typing it
        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueTextDisplay.text += letter;
            _charIndex += 1;
            yield return new WaitForSeconds(typeTime);
        }
    }
    #endregion
}
