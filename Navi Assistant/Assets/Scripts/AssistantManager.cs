using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;
using MapDataModel;

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
    [SerializeField] private SearchableDropdownController _destinationDropdown;

    [Header("Assistant Settings")]
    [SerializeField] private float _assistantDistance = 1.0f;

    [Header("General Dialogues")]
    [SerializeField] private TranslatedText[] _introDialog;
    [SerializeField] private TranslatedText[] _goodbyeDialog;

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
        _assistantModel.SetActive(true);
    }

    void Start()
    {   // Start the assistant when the scene starts
        _destinationDropdown.gameObject.SetActive(false);

        //SelectDestinationInteraction(); // Test the destination selection
        WelcomeAssistant();
    }

    private void Update()
    {   // Update the assistant position to face the camera
        this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * _assistantDistance;
    }

    private void WelcomeAssistant()
    {   // Welcome the assistant when the scene starts
        UnityEvent _onDialogueEnd = new UnityEvent();
        _onDialogueEnd.AddListener(() =>
        {
            _assistantUI.SetActive(false);
            _assistantModel.SetActive(false);
            _qrLocalization.gameObject.SetActive(true);
        });
        _dialogPanel.SetDialogueToDisplay(_introDialog, _onDialogueEnd);
        _dialogPanel.PlayDialogue();

        _assistantAnimator.Play("Hello", 0);
    }

    public void UserLocalized()
    {   // When the user is localized, show the destination options
        _navigationUI.SetActive(false);
        _assistantModel.SetActive(true);
        _assistantUI.SetActive(true);

        SelectDestinationInteraction();
    }

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

        _assistantAnimator.Play("Hello", 0);
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
