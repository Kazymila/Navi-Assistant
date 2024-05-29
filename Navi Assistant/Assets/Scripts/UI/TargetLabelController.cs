using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapDataModel;

public class TargetLabelController : MonoBehaviour
{
    [Header("Target Label Settings")]
    public TranslatedText _targetLabelName;

    [Header("External References")]
    [SerializeField] private NavigationManager _navigationManager;

    private GameObject _floatingLabel;
    private Animator _labelAnimator;
    private bool _isLabelVisible = false;

    void Awake()
    {
        _floatingLabel = this.transform.GetChild(0).gameObject;
        _labelAnimator = this.GetComponent<Animator>();
    }

    void Update()
    {   // Show the target label when the user is looking at the path
        if (_isLabelVisible) _floatingLabel.SetActive(true);
        else _floatingLabel.SetActive(false);
    }

    private void HideLabel() => _isLabelVisible = false;

    private void OnTriggerStay(Collider other)
    {   // Show the target when the user is looking at the path
        if (other.CompareTag("Player"))
        {
            string _currentRoom = _navigationManager.GetCurrentRoom();
            float _distance = Vector3.Distance(this.transform.position, other.transform.position);

            if (_currentRoom == _targetLabelName.key)
            {   // Hide the label if the user is in the target room
                _isLabelVisible = false;
            }
            else if (_distance < 0.5f)
            {   // Show the label if the user is close to the target
                _labelAnimator.Play("Pop", 0);
                Invoke("HideLabel", 0.10f);
            }
            else
            {
                _isLabelVisible = true;
                if (this.transform.localScale == Vector3.zero)
                    _labelAnimator.Play("Show", 0); // Show the label if it is hidden
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
