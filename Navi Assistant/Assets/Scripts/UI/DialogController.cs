using System.Collections;
using UnityEngine.Localization;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class DialogController : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private float _typeTime;
    [SerializeField] private string _splitChar = "\n";
    private string[] _sentencesToDisplay;
    private int _sentenceIndex = -1;
    private int _charIndex = 0;
    private bool _displayingText;
    private UnityEvent _onDialogueEnd;
    private GameObject _dialoguePanel;

    private TextMeshProUGUI _dialogueTextDisplay;
    private GameObject _dialogArrow;
    private PlayerInput _input;

    void Awake()
    {
        _input = this.GetComponent<PlayerInput>();
        _dialoguePanel = this.transform.GetChild(0).gameObject;
        _dialogArrow = this.transform.GetChild(1).gameObject;
        _dialogArrow.SetActive(false);
        HideDialogPanel();

        _dialogueTextDisplay = _dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_displayingText && _sentencesToDisplay.Length > 0)
        {   // Handle the dialogue display
            if (_sentenceIndex >= 0 && _charIndex == _sentencesToDisplay[_sentenceIndex].Length)
                _dialogArrow.SetActive(true); // Show the arrow to continue

            if (_input.actions["Click"].triggered)
            {   // Check for input to skip dialogue or show next sentence
                if (_charIndex < _sentencesToDisplay[_sentenceIndex].Length)
                {   // Skip the current sentence
                    StopAllCoroutines();
                    _dialogueTextDisplay.text = _sentencesToDisplay[_sentenceIndex];
                    _charIndex = _sentencesToDisplay[_sentenceIndex].Length;
                }
                else if (_charIndex == _sentencesToDisplay[_sentenceIndex].Length)
                    DisplayNextSentence(_sentencesToDisplay); // Display the next sentence
            }
        }
    }

    public void PlayDialogue()
    {   // Play the dialogue set to display
        _dialoguePanel.SetActive(true);
        _dialogArrow.SetActive(false);
        _displayingText = true;
        StartDialogue();
    }

    public void SetDialogueToDisplay(LocalizedString _dialog, UnityEvent _onEndEvent = null, bool _keepLastSentence = false)
    {   // Set the dialogue to display
        _sentencesToDisplay = _dialog.GetLocalizedString().Split(_splitChar);

        // Set the event to invoke when the dialogue ends
        if (_onEndEvent != null) _onDialogueEnd = _onEndEvent;
        else _onDialogueEnd = new UnityEvent();

        if (_keepLastSentence)
        {   // Keep the last sentence displayed
            _onDialogueEnd.AddListener(() =>
            {
                _dialogueTextDisplay.text = _sentencesToDisplay[_sentencesToDisplay.Length - 1];
                _dialoguePanel.SetActive(true);
            });
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

    public void EndDialogDisplay()
    {   // Clean the text display and ends the dialogue
        _charIndex = 0;
        _sentenceIndex = -1;
        _displayingText = false;
        _dialogueTextDisplay.SetText("");
        StopAllCoroutines();
        HideDialogPanel();

        // Invoke the event when the dialogue ends
        if (_onDialogueEnd != null) _onDialogueEnd.Invoke();
    }

    public void HideDialogPanel()
    {   // Hide the dialog panel and arrow
        _dialoguePanel.SetActive(false);
        _dialogArrow.SetActive(false);
    }

    private IEnumerator TypeText(string sentence)
    {   // Display the text on screen like typing it
        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueTextDisplay.text += letter;
            _charIndex += 1;
            yield return new WaitForSeconds(_typeTime);
        }
    }
}
