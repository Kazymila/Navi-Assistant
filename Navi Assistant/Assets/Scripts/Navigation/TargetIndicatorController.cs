using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicatorController : MonoBehaviour
{
    [SerializeField] private AssistantManager _assistantManager;
    private GameObject _targetMarker;
    void Start()
    {
        _targetMarker = this.transform.GetChild(0).gameObject;
        _targetMarker.SetActive(false);
    }

    public void SetTargetPosition(Vector3 _position)
    {   // Set the target position
        Vector3 _newPosition = new Vector3(_position.x, -0.25f, _position.z);
        this.transform.position = _newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {   // Show the target when the user is looking at the path
        if (other.CompareTag("Player"))
        {
            _targetMarker.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {   // Show the target when the user is looking at the path
        if (other.CompareTag("Player"))
        {
            float _distance = Vector3.Distance(this.transform.position, other.transform.position);
            if (_distance < 0.5f && _targetMarker.activeSelf)
            {   // Notify the assistant manager when the destination is reached
                _assistantManager.DestinationReached();
                _targetMarker.SetActive(false);

                Debug.Log("Destination Reached");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {   // Hide the target when the user is not looking at the path
        if (other.CompareTag("Player"))
        {
            _targetMarker.SetActive(false);
        }
    }

}
