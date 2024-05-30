using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathArrowVisualization : MonoBehaviour
{
    [Header("Path Settings")]
    public bool showPath = false;
    public int maxArrowCount = 10;
    public float pathYOffset = -0.5f;
    [SerializeField] private float _pathArrowSpacing = 0.5f;
    [SerializeField] private GameObject _pathArrowPrefab;

    private List<GameObject> _pathArrows = new List<GameObject>();

    private void Start()
    {
#if UNITY_EDITOR
        // Set variables for testing in the editor
        pathYOffset = 0.2f;
        maxArrowCount = 7;
#endif
    }

    public void DrawPath(NavMeshPath _navPath)
    {   // Draw arrows along the path to visualize it
        if (_pathArrows.Count > 0) ClearPath();
        if (!showPath || _navPath.corners.Length < 2) return;

        int _countArrowsDrawn = 0;

        for (int i = 0; i < _navPath.corners.Length - 1; i++)
        {   // Draw arrows between path corners
            if (_countArrowsDrawn >= maxArrowCount) return;
            Vector3 _start = _navPath.corners[i];
            Vector3 _end = _navPath.corners[i + 1];
            Vector3 _dir = _end - _start;
            float _dist = Vector3.Distance(_start, _end);
            int _arrowCount = Mathf.FloorToInt(_dist / _pathArrowSpacing);

            if (_arrowCount == 0 || _dist < _pathArrowSpacing)
            {   // Draw arrows in a corner
                Vector3 _pos = _start + _dir.normalized * _pathArrowSpacing;
                InstantiateArrow(_pos, _dir);
                _countArrowsDrawn++;
            }
            else
            {
                for (int j = 1; j < _arrowCount + 1; j++)
                {   // Draw arrows between path corners
                    Vector3 _pos = _start + _dir.normalized * _pathArrowSpacing * j;
                    InstantiateArrow(_pos, _dir);
                    _countArrowsDrawn++;
                    if (_countArrowsDrawn >= maxArrowCount) return;
                }
            }
        }
    }

    private GameObject InstantiateArrow(Vector3 _pos, Vector3 _dir)
    {   // Instantiate a path arrow
        GameObject _pathArrow = Instantiate(
            _pathArrowPrefab,
            _pos + Vector3.up * pathYOffset,
            Quaternion.LookRotation(_dir),
            this.transform
            );
        _pathArrows.Add(_pathArrow);
        return _pathArrow;
    }

    public void ClearPath()
    {   // Clear the path visualization
        foreach (GameObject _pathArrow in _pathArrows)
            Destroy(_pathArrow);
        _pathArrows.Clear();
    }
}
