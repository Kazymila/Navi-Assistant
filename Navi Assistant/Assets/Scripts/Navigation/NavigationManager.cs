using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MapDataModel;
using TMPro;
using UnityEngine.Localization;

public class NavigationManager : MonoBehaviour
{
    public Transform destinationPoint;

    [Header("External References")]
    [SerializeField] private GameObject _MiniMapCanvas;
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private Camera _ARCamera;
    [SerializeField] private GameObject _navTarget;
    [SerializeField] private GameObject _floatingLabels;
    [SerializeField] private ErrorMessagePanelController _errorPanel;

    [Header("Path Visualization")]
    [SerializeField] private PathArrowVisualization _pathArrowVisualizer;
    [SerializeField] private NavArrowController _navArrowController;
    [SerializeField] private PathLineVisualization _pathLineVisualizer;
    [SerializeField] private PathLineVisualization _miniMapLineVisualizer;

    [Header("Error Messages")]
    [SerializeField] private LocalizedString _destinationErrorTitle;
    [SerializeField] private LocalizedString _destinationErrorMessage;

    private NavMeshPath _navPath;
    void Start()
    {
        _navPath = new NavMeshPath();
    }

    void Update()
    {   // Calculate path from agent to target
        if (destinationPoint == null) return;
        NavMesh.CalculatePath(transform.position, destinationPoint.position, NavMesh.AllAreas, _navPath);

        if (_navPath.status == NavMeshPathStatus.PathComplete)
        {
            _floatingLabels.SetActive(true);
            _errorPanel.gameObject.SetActive(false);
            _pathArrowVisualizer.DrawPath(_navPath);
            _pathLineVisualizer.DrawPathLine(_navPath);
            _miniMapLineVisualizer.DrawPathLine(_navPath);
            _navArrowController.UpdateNavArrow(_navPath);
        }
        else
        {   // Clear path if not reachable
            _pathArrowVisualizer.ClearPath();
            _pathLineVisualizer.ClearPathLine();
            _miniMapLineVisualizer.ClearPathLine();
            _navArrowController.EnableNavArrow(false);
            Debug.Log("Path is not reachable");

            // Show an alert message to the user
            _errorPanel.SetErrorMessage(_destinationErrorTitle.GetLocalizedString(), _destinationErrorMessage.GetLocalizedString(), 0);
            _errorPanel.gameObject.SetActive(true);
        }
    }

    public void EndNavigation()
    {   // Clear the path and hide the navigation arrow
        destinationPoint = null;
        _pathArrowVisualizer.ClearPath();
        _pathLineVisualizer.ClearPathLine();
        _miniMapLineVisualizer.ClearPathLine();
        _navArrowController.EnableNavArrow(false);
        _floatingLabels.SetActive(false);
    }

    public string GetCurrentRoom()
    {   // Get the current room where the user is located
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, LayerMask.GetMask("Floor")))
        {   // Check if the user is over a room
            if (hit.collider.CompareTag("Room"))
            {   // Return the current room name
                print("Current Room: " + hit.collider.name);
                return hit.collider.name;
            }
        }
        return "";
    }
}
