using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using MapDataModel;

public class AssistantManager : MonoBehaviour
{
    #region --- External References ---
    [Header("External References")]
    [SerializeField] private string _surveyURL = "https://forms.gle/6nVvfgeqdTaYr894A";
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private NavigationManager _navManager;
    [SerializeField] private QRCodeLocalization _qrLocalization;
    [SerializeField] private DestinationManager _destinationsManager;

    [Header("UI References")]
    [SerializeField] private GameObject _assistantUI;
    [SerializeField] private Button _GoToSurveyButton;
    private DialogController _dialogPanel;
    private SearchableDropdownController _destinationDropdown;
    private OptionsButtonsController _assistantOptionsButtons;
    private OptionsButtonsController _onNavigationOptions;
    private OptionsButtonsController _problemSolvingButtons;
    private OptionsButtonsController _changeLanguageButtons;
    private OptionsButtonsController _continueOptionsButtons;
    #endregion

    #region --- Assistant Settings ---
    [Header("Assistant Settings")]
    [SerializeField] private float _assistantDistance = 1.0f;

    [Header("Assistant Options")]
    [SerializeField] private TranslatedText[] _assistantOptions;
    [SerializeField] private TranslatedText[] _navigationOptions;
    [SerializeField] private TranslatedText[] _problemSolvingOptions;
    [SerializeField] private TranslatedText[] _languageOptions;
    [SerializeField] private TranslatedText[] _continueOptions;

    [Header("General Dialogues")]
    [SerializeField] private LocalizedString _welcomeDialog;
    [SerializeField] private LocalizedString _goodbyeDialog;
    [SerializeField] private LocalizedString _displayOptionsDialog;
    [SerializeField] private LocalizedString _noAvailableDialog;
    [SerializeField] private LocalizedString _congratsForContinueDialog;
    [SerializeField] private LocalizedString _assistantContinueDialog;
    [SerializeField] private LocalizedString _goToSurveyDialog;
    [SerializeField] private LocalizedString _waitForSurveyDialog;

    [Header("Navigation Dialogues")]
    [SerializeField] private LocalizedString _destinationReachedDialog;
    [SerializeField] private LocalizedString _isAlreadyInDestinationDialog;

    [Header(" Localization Dialogues")]
    [SerializeField] private LocalizedString _goLocalizationScannerDialog;
    [SerializeField] private LocalizedString _localizationScannerBackDialog;

    [Header("Destination Selection Dialogues")]
    [SerializeField] private LocalizedString _selectDestinationDialog;
    [SerializeField] private LocalizedString _selectFromDropdownDialog;
    [SerializeField] private LocalizedString _goToDestinationDialog;
    [SerializeField] private LocalizedString _changeDestinationDialog;
    [SerializeField] private LocalizedString _anotherDestinationDialog;
    [SerializeField] private LocalizedString _goToAnotherPlaceDialog;
    [SerializeField] private LocalizedString _unknownOutsideDialog;
    #endregion

    private GameObject _assistantModel;
    private Animator _assistantAnimator;
    private bool _isOnNavigation = false;

    void Awake()
    {
        _assistantModel = this.transform.GetChild(0).gameObject;
        _assistantAnimator = _assistantModel.GetComponent<Animator>();
        _dialogPanel = _assistantUI.GetComponentInChildren<DialogController>();
        _destinationDropdown = _assistantUI.GetComponentInChildren<SearchableDropdownController>();

        _assistantOptionsButtons = _assistantUI.transform.GetChild(2).GetComponent<OptionsButtonsController>();
        _onNavigationOptions = _assistantUI.transform.GetChild(3).GetComponent<OptionsButtonsController>();
        _problemSolvingButtons = _assistantUI.transform.GetChild(4).GetComponent<OptionsButtonsController>();
        _changeLanguageButtons = _assistantUI.transform.GetChild(5).GetComponent<OptionsButtonsController>();
        _continueOptionsButtons = _assistantUI.transform.GetChild(6).GetComponent<OptionsButtonsController>();

        InitializeSystemLanguage();
    }

    void Start()
    {   // Start the assistant when the scene starts
        _destinationDropdown.gameObject.SetActive(false);
        _assistantModel.SetActive(true);

        SetAssitantOptionsButtons();
        SetLanguageOptonsButtons();
        SetContinueOptionsButtons();
        SetProblemSolvingOptions();
        SetNavigationAssistantOptions();

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
        //_assistantOptionsButtons.AddOptionButton(_assistantOptions[1], StartTour);
        _assistantOptionsButtons.AddOptionButton(_assistantOptions[2], _changeLanguageButtons.ShowOptionsButtons);
    }

    private void SetNavigationAssistantOptions()
    {   // Set the assistant options for navigation
        _onNavigationOptions.AddOptionButton(_navigationOptions[0], SelectDestinationInteraction);
        _onNavigationOptions.AddOptionButton(_navigationOptions[1], ShowProblemSolvingOptions);
        _onNavigationOptions.AddOptionButton(_navigationOptions[2], _changeLanguageButtons.ShowOptionsButtons);
        _onNavigationOptions.AddOptionButton(_navigationOptions[3], AssistantGoAway);
    }

    private void SetProblemSolvingOptions()
    {   // Set the problem solving options
        _problemSolvingButtons.AddOptionButton(_problemSolvingOptions[0], () => Debug.Log("Option clicked"));
        //_problemSolvingButtons.AddOptionButton(_problemSolvingOptions[1], null);
        //_problemSolvingButtons.AddOptionButton(_problemSolvingOptions[2], null);
    }

    private void SetLanguageOptonsButtons()
    {   // Set the language options buttons
        foreach (TranslatedText _language in _languageOptions)
            _changeLanguageButtons.AddOptionButton(_language, () => ChangeLanguage(_language.key));
    }

    private void SetContinueOptionsButtons()
    {   // Set the continue options buttons
        _continueOptionsButtons.AddOptionButton(_continueOptions[0], ShowInitialAssistantOptions);
        _continueOptionsButtons.AddOptionButton(_continueOptions[1], ExitAplication);
    }
    #endregion

    #region --- Assistance Events ---
    private void WelcomeAssistant()
    {   // Welcome the assistant when the scene starts
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(ShowInitialAssistantOptions);
        _dialogPanel.SetDialogueToDisplay(_welcomeDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Hello", 0);
    }
    public void CallAssistant()
    {   // Call the assistant when the user presses the assistant button
        _destinationDropdown.gameObject.SetActive(false);
        _assistantModel.SetActive(true);
        _navManager.StopNavigation();

        ShowNavigationAssistantOptions();
    }

    private void AssistantGoAway()
    {   // Hide the assistant
        _assistantModel.SetActive(false);
        _dialogPanel.EndDialogDisplay(false);
        _destinationDropdown.gameObject.SetActive(true);
        _onNavigationOptions.HideOptionsButtons();
        _navManager.StartNavigation();
    }

    public void DestinationReached()
    {   // When the user reaches the destination
        _destinationDropdown.gameObject.SetActive(false);
        _navManager.EndNavigation();
        _isOnNavigation = false;
        _assistantModel.SetActive(true);

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(GoToSurveyInteraction);
        _dialogPanel.SetDialogueToDisplay(_destinationReachedDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Happy", 0);
    }

    private void GoToSurveyInteraction()
    {   // Show the button to go to the survey
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() => _GoToSurveyButton.gameObject.SetActive(true));
        _dialogPanel.SetDialogueToDisplay(_goToSurveyDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();
    }
    #endregion

    #region --- Assistant Options Methods ---
    public void GoToSurvey()
    {   // Open the survey link in the browser
        _GoToSurveyButton.gameObject.SetActive(false);

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() =>
        {   // When the survey is completed, show the continue options
            _assistantAnimator.Play("Happy", 0);
            Invoke("ShowContinueOptions", 2.0f);
        });
        _dialogPanel.SetDialogueToDisplay(_waitForSurveyDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();

        Application.OpenURL(_surveyURL);
    }

    private void ShowInitialAssistantOptions()
    {   // Show the assistant options
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() => _assistantOptionsButtons.ShowOptionsButtons());
        _dialogPanel.SetDialogueToDisplay(_displayOptionsDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();
    }

    private void ShowNavigationAssistantOptions()
    {   // Show the assistant options for navigation
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() => _onNavigationOptions.ShowOptionsButtons());
        _dialogPanel.SetDialogueToDisplay(_displayOptionsDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();
    }

    private void ShowContinueOptions()
    {   // Show the continue options after reaching the destination
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() => _continueOptionsButtons.ShowOptionsButtons());
        _dialogPanel.SetDialogueToDisplay(_assistantContinueDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();
    }

    private void ShowProblemSolvingOptions()
    {   // Show the problem solving options
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() => _problemSolvingButtons.ShowOptionsButtons());
        _dialogPanel.SetDialogueToDisplay(_displayOptionsDialog, _onDialogEnd, true);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Thinking", 0);
    }

    public void StartNavigation()
    {   // Start the navigation process
        _assistantOptionsButtons.HideOptionsButtons();
        _dialogPanel.SetDialogueToDisplay(_goLocalizationScannerDialog, GoLocalizationScanner());
        _dialogPanel.PlayDialogue();
    }

    public void StartTour()
    {   // Start the building tour
        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(ShowInitialAssistantOptions);
        _dialogPanel.SetDialogueToDisplay(_noAvailableDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Nope", 0);
    }

    public void ChangeLanguage(string _languageCode)
    {   // Change the language of the assistant
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(_languageCode);
        _changeLanguageButtons.HideOptionsButtons();

        if (_isOnNavigation) ShowNavigationAssistantOptions();
        else ShowInitialAssistantOptions();
    }

    private void ExitAplication()
    {   // Exit the application
        _assistantAnimator.Play("Hello", 0);

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(() =>
        {
            Application.Quit();
        });
        _dialogPanel.SetDialogueToDisplay(_goodbyeDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();
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

            _dialogPanel.SetDialogueToDisplay(_localizationScannerBackDialog, GoLocalizationScanner());
            _dialogPanel.PlayDialogue();

            _assistantAnimator.Play("No", 0);
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
        UnityEvent _selectDestinationEvent = new UnityEvent();
        _selectDestinationEvent.AddListener(SelectDestinationInteraction);

        UnityEvent _event = new UnityEvent();
        _event.AddListener(() =>
        {
            _assistantUI.SetActive(true);
            _assistantModel.SetActive(true);
            _qrLocalization.gameObject.SetActive(false);

            _dialogPanel.SetDialogueToDisplay(_congratsForContinueDialog, _selectDestinationEvent);
            _dialogPanel.PlayDialogue();

            _assistantAnimator.Play("Affirm", 0);
        });
        return _event;
    }

    #endregion

    #region --- Choose Destination Events ---
    public void SelectAnotherDestination()
    {   // Select a teleport point (stairs, elevators or exits)
        UnityEvent _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() =>
        {   // When the dialogue ends, show the destination options
            _destinationsManager.ShowTeleportOptionsButtons();
        });
        _dialogPanel.SetDialogueToDisplay(_anotherDestinationDialog, _onDialogueEnd, true);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Thinking", 0);
    }

    public void GoToAnotherPlace()
    {   // Go to another place in the building (by stairs, elevators or exits)
        _assistantModel.SetActive(true);
        _destinationDropdown.gameObject.SetActive(true);
        _qrLocalization.ResetScannerButtonsActions();
        _destinationsManager.SetAllDestinationsOnDropdown();
        _destinationDropdown.ClearInputText();
        _navManager.StartNavigation();
        _isOnNavigation = true;

        UnityEvent _onDialogEnd = new UnityEvent();
        _onDialogEnd.AddListener(UnknownOutside);
        _dialogPanel.SetDialogueToDisplay(_goToAnotherPlaceDialog, _onDialogEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Yeah", 0);
    }

    private void UnknownOutside()
    {   // Show interaction when the user is outside the building
        UnityEvent _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() => _assistantModel.SetActive(false));
        _dialogPanel.SetDialogueToDisplay(_unknownOutsideDialog, _onDialogueEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Nope", 0);
    }

    public void SelectDestinationFromDropdown()
    {   // Select the destination alternative
        _dialogPanel.SetDialogueToDisplay(_selectFromDropdownDialog, null, true);
        _dialogPanel.PlayDialogue();

        _destinationDropdown.gameObject.SetActive(true);
        _destinationDropdown.ShowAllDropdownItems();

        _assistantAnimator.Play("Thinking", 0);
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

    public void GoToDestination(string _destinationName)
    {   // Go to the selected destination
        if (_destinationName == _navManager.GetCurrentRoom())
        {   // If the destination is the current room, show a message
            _assistantModel.SetActive(true);
            UnityEvent _onDialogEnd = new UnityEvent();
            _onDialogEnd.AddListener(() =>
            {   // When the dialogue ends, show the destination options
                _destinationsManager.ShowDestinationOptionsButtons();
            });
            _dialogPanel.SetDialogueToDisplay(_isAlreadyInDestinationDialog, _onDialogEnd, true);
            _dialogPanel.PlayDialogue();

            _assistantAnimator.Play("Thinking", 0);
        }
        else if (_isOnNavigation)
        {   // If the assistant is already navigating, change the destination
            _assistantModel.SetActive(true);
            _destinationDropdown.gameObject.SetActive(true);
            _destinationsManager.SetAllDestinationsOnDropdown();
            _destinationDropdown.ChangeSelectedItem(_destinationName);
            _navManager.StartNavigation();
            _isOnNavigation = true;

            UnityEvent _onDialogEnd = new UnityEvent();
            _onDialogEnd.AddListener(() => _assistantModel.SetActive(false));
            _dialogPanel.SetDialogueToDisplay(_changeDestinationDialog, _onDialogEnd);
            _dialogPanel.PlayDialogue();

            _assistantAnimator.Play("Yeah", 0);
        }
        else
        {   // Go to the selected destination
            _assistantModel.SetActive(true);
            _destinationDropdown.gameObject.SetActive(true);
            _destinationsManager.SetAllDestinationsOnDropdown();
            _destinationDropdown.ChangeSelectedItem(_destinationName);
            _qrLocalization.ResetScannerButtonsActions();
            _navManager.StartNavigation();
            _isOnNavigation = true;

            UnityEvent _onDialogEnd = new UnityEvent();
            _onDialogEnd.AddListener(() => _assistantModel.SetActive(false));
            _dialogPanel.SetDialogueToDisplay(_goToDestinationDialog, _onDialogEnd);
            _dialogPanel.PlayDialogue();

            _assistantAnimator.Play("Yeah", 0);
        }
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
