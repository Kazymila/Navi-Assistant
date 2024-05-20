using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private GameObject _MiniMapCanvas;
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private Camera _ARCamera;
    [SerializeField] private GameObject _navTarget;

    [Header("Path Visualization")]
    [SerializeField] private PathVisualization _pathVisualizer;
    [SerializeField] private NavArrowController _navArrowController;
    [SerializeField] private MiniMapPathVisualization _miniMapLineVisualizer;

    private NavMeshPath _navPath;
    void Start()
    {
        _navPath = new NavMeshPath();
    }

    void Update()
    {   // Calculate path from agent to target
        NavMesh.CalculatePath(transform.position, _navTarget.transform.position, NavMesh.AllAreas, _navPath);

        if (_navPath.status == NavMeshPathStatus.PathComplete)
        {
            _pathVisualizer.DrawPath(_navPath);
            _miniMapLineVisualizer.DrawPathLine(_navPath);

            if (_navArrowController.showPathArrow)
            {   // Show path arrow if enabled
                _navArrowController.UpdateNavArrow(_navPath);
            }
            else _navArrowController.EnableNavArrow(false);
        }
        else Debug.Log("Path not reachable!");
    }
}
