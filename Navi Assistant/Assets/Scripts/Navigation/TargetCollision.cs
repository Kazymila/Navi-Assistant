using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {   // Show the target when the user is looking at the path
        if (other.CompareTag("Player"))
        {
            _meshRenderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {   // Hide the target when the user is not looking at the path
        if (other.CompareTag("Player"))
        {
            _meshRenderer.enabled = false;
        }
    }

}
