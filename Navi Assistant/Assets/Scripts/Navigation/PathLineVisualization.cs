using UnityEngine;
using UnityEngine.AI;

public class PathLineVisualization : MonoBehaviour
{
    [Header("Path Line Settings")]
    public bool showPathLine = true;
    public float pathYOffset = 0.0f;
    private LineRenderer _lineRenderer;

    void Awake()
    {   // Get the LineRenderer component
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawPathLine(NavMeshPath _navPath)
    {   // Draw the line path from agent to target
        if (!showPathLine || _navPath.corners.Length < 2)
        {   // Hide the line if path is not reachable
            _lineRenderer.enabled = false;
            return;
        }
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
