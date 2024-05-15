using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private GameObject _navTarget;
    [SerializeField] private GameObject _pathPointPrefab;
    public bool showLinePath = true;
    public bool showPathPoints = false;
    public float pathHeight = 0.1f;

    private NavMeshPath _navPath;
    private LineRenderer _lineRenderer;
    private Transform[] _pathPoints = new Transform[0];

    private void Start()
    {
        _navPath = new NavMeshPath();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        NavMesh.CalculatePath(transform.position, _navTarget.transform.position, NavMesh.AllAreas, _navPath);
        if (showLinePath) DrawPathLine();
        else _lineRenderer.enabled = false;

        if (showPathPoints) DrawPathPoints();
        else DestroyPathPoints();
    }

    private void DrawPathLine()
    {   // Draw the line path from agent to target
        for (int i = 0; i < _navPath.corners.Length; i++)
            _navPath.corners[i] = new Vector3(_navPath.corners[i].x, pathHeight, _navPath.corners[i].z);

        _lineRenderer.positionCount = _navPath.corners.Length;
        _lineRenderer.SetPositions(_navPath.corners);
        _lineRenderer.enabled = true;
    }

    private void DrawPathPoints()
    {   // Draw the path points from agent to target
        for (int i = 0; i < _navPath.corners.Length; i++)
        {
            Vector3 point = new Vector3(_navPath.corners[i].x, pathHeight, _navPath.corners[i].z);
            if (_pathPoints.Length > 0 && _pathPoints[i].position == point)
                _pathPoints[i].position = point;
            else
            {
                GameObject _sphere = Instantiate(_pathPointPrefab, point, Quaternion.identity);
                _pathPoints[i] = _sphere.transform;
            }
        }
    }

    private void DestroyPathPoints()
    {   // Destroy all path points
        if (_pathPoints.Length < 1) return;
        else
        {   // Destroy all path points and clear the array
            for (int i = 0; i < _pathPoints.Length; i++)
                Destroy(_pathPoints[i].gameObject);
            _pathPoints = new Transform[0];
        }
    }
}
