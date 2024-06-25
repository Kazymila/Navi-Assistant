using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.AI;
using UnityEngine;
using System;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint;

    [Header("External References")]
    [SerializeField] private AnalyticsDataManager _analyticsManager;
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private Camera _ARCamera;

    [Header("UI References")]
    [SerializeField] private GameObject _navigationUI;
    [SerializeField] private GameObject _miniMapCanvas;
    [SerializeField] private GameObject _floatingLabels;
    [SerializeField] private GameObject _teleportLabels;
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
    private bool _isNavigating = false;
    private bool _pathCalculated = false;
    private DateTime _startPathTime;

    void Start()
    {
        _navPath = new NavMeshPath();
        _navigationUI.SetActive(false);
        _floatingLabels.SetActive(false);
        _teleportLabels.SetActive(false);
    }

    void Update()
    {
        if (destinationPoint == null) return;

        if (_isNavigating) GenerateNavigationPath();
        else HideNavigation();
    }

    public void SetDestinationPoint(Transform destination)
    {   // Set the destination point for navigation
        destinationPoint = destination;
        _pathCalculated = false;
    }

    public void StartNavigation()
    {   // Start navigation to the destination point
        _isNavigating = true;
        _navigationUI.SetActive(true);
    }
    public void StopNavigation() => _isNavigating = false;

    public void EndNavigation()
    {   // End navigation and clear path
        destinationPoint = null;
        _pathCalculated = false;
        HideNavigation();

        // Update analytics data with time taken to complete the path
        _analyticsManager.analyticsData.timeTakenToCompletePath =
            (System.DateTime.Now - _startPathTime).TotalMilliseconds.ToString().Replace(".", ",");
    }

    private void GenerateNavigationPath()
    {   // Calculate path from agent to target and visualize it
        if (!_pathCalculated)
        {   // Calculate time to generate path
            _pathCalculated = true;
            System.DateTime startTime = System.DateTime.Now;

            NavMesh.CalculatePath(transform.position, destinationPoint.position, NavMesh.AllAreas, _navPath);

            System.TimeSpan timeToCalculatePath = System.DateTime.Now - startTime;
            Debug.Log("Nav path to " + destinationPoint.parent.name + " generated in " + timeToCalculatePath.TotalMilliseconds + "ms");

            // Update analytics data
            _analyticsManager.analyticsData.timeToCalculatePath = timeToCalculatePath.TotalMilliseconds.ToString().Replace(".", ",");
            _analyticsManager.analyticsData.pathDistance = GetPathLength(_navPath).ToString().Replace(".", ",");
            _analyticsManager.analyticsData.startPosition = this.transform.position.ToString();
            _analyticsManager.analyticsData.destinationPoint = destinationPoint.parent.name;
            _startPathTime = System.DateTime.Now;
        }
        else NavMesh.CalculatePath(transform.position, destinationPoint.position, NavMesh.AllAreas, _navPath);

        if (_navPath.status == NavMeshPathStatus.PathComplete)
        {   // Show the path and navigation arrow if reachable
            _errorPanel.gameObject.SetActive(false);
            _pathArrowVisualizer.DrawPath(_navPath);
            _pathLineVisualizer.DrawPathLine(_navPath);
            _miniMapLineVisualizer.DrawPathLine(_navPath);
            _navArrowController.UpdateNavArrow(_navPath);
            _floatingLabels.SetActive(true);
            _teleportLabels.SetActive(true);
        }
        else
        {   // Clear path if not reachable
            if (!_errorPanel.gameObject.activeSelf)
                _analyticsManager.analyticsData.cannotCalculatePathErrorCount++;

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

    private void HideNavigation()
    {   // Clear the path and hide the navigation arrow
        _pathArrowVisualizer.ClearPath();
        _pathLineVisualizer.ClearPathLine();
        _miniMapLineVisualizer.ClearPathLine();
        _navArrowController.EnableNavArrow(false);
        _floatingLabels.SetActive(false);
        _teleportLabels.SetActive(false);
        _navigationUI.SetActive(false);
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

    public static float GetPathLength(NavMeshPath path)
    {   // Calculate the length of the path
        float _pathLenght = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {   // Calculate the distance between each corner of the path
            for (int i = 1; i < path.corners.Length; ++i)
                _pathLenght += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return _pathLenght;
    }
}
