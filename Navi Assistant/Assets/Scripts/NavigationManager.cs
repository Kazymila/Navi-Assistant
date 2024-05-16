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
    [SerializeField] private PathLineController _pathLineController;
    [SerializeField] private PathArrowController _pathArrowController;

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
            if (_pathLineController.showPathLine)
            {   // Draw path line if enabled
                _MiniMapCanvas.SetActive(true);
                _pathLineController.DrawPathLine(_navPath);
            }
            else _pathLineController.EnablePathLine(false);

            if (_pathArrowController.showPathArrow)
            {   // Show path arrow if enabled
                //_MiniMapCanvas.SetActive(false);
                _pathArrowController.UpdatePathArrow(_navPath);
            }
            else _pathArrowController.EnablePathArrow(false);
        }
        else Debug.Log("Path not reachable!");
    }
}
