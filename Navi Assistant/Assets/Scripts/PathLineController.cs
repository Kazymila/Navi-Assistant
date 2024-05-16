using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathLineController : MonoBehaviour
{
    [Header("Path Line Settings")]
    public bool showPathLine = false;
    public float pathYOffset = -0.5f;

    private LineRenderer _lineRenderer;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void EnablePathLine(bool _enable)
    {   // Enable or disable the path line
        _lineRenderer.enabled = _enable;
    }

    public void DrawPathLine(NavMeshPath _navPath)
    {   // Draw the line path from agent to target
        Vector3[] _pathPoints = AddHeightOffset(_navPath.corners);
        _lineRenderer.positionCount = _pathPoints.Length;
        _lineRenderer.SetPositions(_pathPoints);
        _lineRenderer.enabled = true;
    }

    private Vector3[] AddHeightOffset(Vector3[] _points)
    {   // Add height offset to path points
        Vector3[] _newPoints = new Vector3[_points.Length];

        for (int i = 0; i < _points.Length; i++)
            _newPoints[i] = new Vector3(_points[i].x, pathYOffset, _points[i].z);
        return _newPoints;
    }
}
