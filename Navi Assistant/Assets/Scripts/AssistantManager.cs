using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;
using MapDataModel;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class AssistantManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private NavigationManager _navManager;
    [SerializeField] private QRCodeLocalization _qrLocalization;
    [SerializeField] private DestinationManager _destinationsManager;

    [Header("UI References")]
    [SerializeField] private GameObject _assistantUI;
    [SerializeField] private GameObject _navigationUI;
    [SerializeField] private DialogController _dialogPanel;
    [SerializeField] private OptionsButtonsController _assistantOptionsButtons;
    [SerializeField] private OptionsButtonsController _changeLanguageButtons;
    [SerializeField] private OptionsButtonsController _continueOptionsButtons;
    [SerializeField] private SearchableDropdownController _destinationDropdown;

    [Header("Assistant Settings")]
    [SerializeField] private float _assistantDistance = 1.0f;
    [SerializeField] private TranslatedText[] _assistantOptions;
    [SerializeField] private TranslatedText[] _languageOptions;
    [SerializeField] private TranslatedText[] _continueOptions;

    [Header("General Dialogues")]
    [SerializeField] private TranslatedText[] _introDialog;
    [SerializeField] private TranslatedText[] _assistantOptionsDialog;
    [SerializeField] private TranslatedText[] _destinationReachedDialog;
    [Header(" Localization Dialogues")]
    [SerializeField] private TranslatedText[] _goQRScannerDialog;
    [SerializeField] private TranslatedText[] _scannerBackDialog;
    [SerializeField] private TranslatedText[] _localizedDialog;

    [Header("Destination Selection Dialogues")]
    [SerializeField] private TranslatedText[] _selectDestinationDialog;
    [SerializeField] private TranslatedText[] _selectFromDropdownDialog;
    [SerializeField] private TranslatedText[] _goToDestinationDialog;

    private GameObject _assistantModel;
    private Animator _assistantAnimator;

    void Awake()
    {
        _assistantModel = this.transform.GetChild(0).gameObject;
        _assistantAnimator = _assistantModel.GetComponent<Animator>();
        _navigationUI.SetActive(false);
        InitializeSystemLanguage();
    }

    void Start()
    {   // Start the assistant when the scene starts
        _destinationDropdown.gameObject.SetActive(false);
        _assistantModel.SetActive(true);
        SetAssitantOptionsButtons();
        SetLanguageOptonsButtons();
        SetContinueOptionsButtons();

        WelcomeAssistant();
    }

    private void Update()
    {   // Update the assistant position to face the camera
        this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * _assistantDistance;
    }

    #region --- Initialization Methods ---
    private void InitializeSystemLanguage()
    {   // Initialize the system language
        if (Application.systemLanguage == SystemLanguage.Spanish)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("es");
        else if (Application.systemLanguage == SystemLanguage.English)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");

        Debug.Log("This system is in " + Application.systemLanguage);
    }

    private void SetAssitantOptionsButtons()
    {   // Set the assistant options buttons
        _assistantOptionsButtons.AddOptionButton(_assistantOptions[0], StartNavigation);
        _assistantOptionsButtons.AddOptionButton(_assistantOptions[1], StartTour);
        _assistantOptionsButtons.AddOptionButton(_assistantOptions[2], _changeLanguageButtons.ShowOptionsButtons);
    }

    private void SetLanguageOptonsButtons()
    {   // Set the language options buttons
        foreach (TranslatedText _language in _languageOptions)
            _changeLanguageButtons.AddOptionButton(_language, () => ChangeLanguage(_language.key));
    }

    private void SetContinueOptionsButtons()
    {   // Set the continue options buttons
        _continueOptionsButtons.AddOptionButton(_continueOptions[0], ShowAssistantOptions);
        _continueOptionsButtons.AddOptionButton(_continueOptions[1], ExitAplication);
    }
    #endregion

    #region --- Assistance Events ---
    private void WelcomeAssistant()
    {   // Welcome the assistant when the scene starts
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(ShowAssistantOptions);
        _dialogPanel.SetDialogueToDisplay(_introDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Hello", 0);
    }

    private void ExitAplication()
    {   // Exit the application
        Debug.Log("Exiting the application");

        // TODO: Exit the application
    }

    private void ShowAssistantOptions()
    {   // Show the assistant options
        _dialogPanel.SetDialogueToDisplay(_assistantOptionsDialog, null, true);
        _dialogPanel.PlayDialogue();

        _assistantOptionsButtons.ShowOptionsButtons();
    }

    public void StartNavigation()
    {   // Start the navigation process
        _assistantOptionsButtons.HideOptionsButtons();
        _dialogPanel.SetDialogueToDisplay(_goQRScannerDialog, GoLocalizationScanner());
        _dialogPanel.PlayDialogue();
    }

    public void StartTour()
    {   // Start the building tour
        Debug.Log("Starting the tour");
        // TODO: Start the tour
    }

    public void ChangeLanguage(string _languageCode)
    {   // Change the language of the assistant
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(_languageCode);
        _changeLanguageButtons.HideOptionsButtons();
        ShowAssistantOptions();
    }

    public void DestinationReached()
    {   // When the user reaches the destination
        _destinationDropdown.gameObject.SetActive(false);
        _navigationUI.SetActive(false);
        _navManager.EndNavigation();

        _assistantModel.SetActive(true);

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() =>
        {
            _continueOptionsButtons.ShowOptionsButtons();
        });
        _dialogPanel.SetDialogueToDisplay(_destinationReachedDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();

        // TODO: Celebrate the user reaching the destination
        //_assistantAnimator.Play("Happy", 0);
    }
    #endregion

    #region --- Localization Events ---
    private UnityEvent GoLocalizationScanner()
    {   // Go to the QR code localization scanner
        UnityEvent _event = new UnityEvent();
        Button.ButtonClickedEvent _onBack = new Button.ButtonClickedEvent();

        _onBack.AddListener(() =>
        {   // When the user goes back from the QR code scanner
            _assistantUI.SetActive(true);
            _assistantModel.SetActive(true);
            _qrLocalization.gameObject.SetActive(false);

            _dialogPanel.SetDialogueToDisplay(_scannerBackDialog, GoLocalizationScanner());
            _dialogPanel.PlayDialogue();
        });

        _event.AddListener(() =>
        {   // When the user goes to the QR code scanner
            _assistantUI.SetActive(false);
            _assistantModel.SetActive(false);
            _qrLocalization.gameObject.SetActive(true);
            _qrLocalization.ChangeLocalizedAction(OnLocalizedEvent());
            _qrLocalization.ChangeBackButtonAction(_onBack);
        });
        return _event;
    }

    private UnityEvent OnLocalizedEvent()
    {   // When the user is localized, show the destination options
        UnityEvent _event = new UnityEvent();
        _event.AddListener(() =>
        {
            _assistantUI.SetActive(true);
            _assistantModel.SetActive(true);
            SelectDestinationInteraction();
            _qrLocalization.gameObject.SetActive(false);
        });
        return _event;
    }
    #endregion

    #region --- Choose Destination Events ---
    public void GoToDestination(string _destinationName)
    {   // Go to the selected destination
        _assistantModel.SetActive(true);
        _destinationDropdown.gameObject.SetActive(true);
        _destinationsManager.SetAllDestinationsOnDropdown();
        _destinationDropdown.ChangeSelectedItem(_destinationName);
        _navigationUI.SetActive(true);

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() =>
        {
            _assistantModel.SetActive(false);
        });
        _dialogPanel.SetDialogueToDisplay(_goToDestinationDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();
    }

    public void SelectDestinationFromDropdown()
    {   // Select the destination alternative
        _dialogPanel.SetDialogueToDisplay(_selectFromDropdownDialog);
        _dialogPanel.PlayDialogue();

        _destinationDropdown.gameObject.SetActive(true);
        _destinationDropdown.ShowDropdown();
    }

    private void SelectDestinationInteraction()
    {   // Show interaction to select a destination
        UnityEvent _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() =>
        {   // When the dialogue ends, show the destination options
            _destinationsManager.ShowDestinationOptionsButtons();
        });
        _dialogPanel.SetDialogueToDisplay(_selectDestinationDialog, _onDialogueEnd, true);
        _dialogPanel.PlayDialogue();
    }
    #endregion

    #region --- Trigger Events ---
    private void OnTriggerStay(Collider _collision)
    {   // Disable the wall when the arrow is inside the collision
        if (_collision.CompareTag("Wall"))
        {   // Change the layer to only view in minimap, to avoid collision with the arrow
            _collision.gameObject.layer = LayerMask.NameToLayer("MiniMapIndicators");
        }
    }

    private void OnTriggerExit(Collider _collision)
    {   // Enable the wall when the arrow exits the collision
        if (_collision.CompareTag("Wall"))
        {   // Change the layer to wall again to render it in camera
            _collision.gameObject.layer = LayerMask.NameToLayer("Wall");
        }
    }
    #endregion
}
