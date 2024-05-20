using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathVisualization : MonoBehaviour
{
    [Header("Path Settings")]
    public bool showPath = false;
    public float pathYOffset = -0.5f;
    [SerializeField] private int _maxArrowCount = 10;
    [SerializeField] private float _pathArrowSpacing = 0.5f;
    [SerializeField] private GameObject _pathArrowPrefab;

    private List<GameObject> _pathArrows = new List<GameObject>();

    public void DrawPath(NavMeshPath _navPath)
    {   // Draw arrows along the path to visualize it
        if (_pathArrows.Count > 0) ClearPath();
        if (!showPath || _navPath.corners.Length < 2) return;

        Vector3 _start = _navPath.corners[0];
        Vector3 _end = _navPath.corners[1];
        Vector3 _dir = _end - _start;

        float _dist = Vector3.Distance(_start, _end);
        int _arrowCount = Mathf.FloorToInt(_dist / _pathArrowSpacing);

        if (_arrowCount == 0)
        {   // Draw arrows in corners

            // TODO: Draw the arrows in the corner

            GameObject _pathArrow = Instantiate(
                _pathArrowPrefab,
                _start + Vector3.up * pathYOffset,
                Quaternion.LookRotation(_dir),
                this.transform
                );
            _pathArrows.Add(_pathArrow);
            return;
        }
        else
        {
            for (int i = 0; i < _arrowCount; i++)
            {   // Draw arrows between path corners
                Vector3 _pos = _start + _dir.normalized * _pathArrowSpacing * i;
                GameObject _pathArrow = Instantiate(
                    _pathArrowPrefab,
                    _pos + Vector3.up * pathYOffset,
                    Quaternion.LookRotation(_dir),
                    this.transform
                    );
                _pathArrows.Add(_pathArrow);

                if (i == _arrowCount - 1)
                {   // Draw the last arrow to the end of the path corner
                    Vector3 _cornerDir = _navPath.corners[2];
                    _pathArrow.transform.LookAt(_cornerDir);

                    // TODO: fix the last arrow rotation

                    // Add a little offset to the arrow
                    Vector3 _perpendicular = Vector3.Cross(_dir, Vector3.up);
                    _pathArrow.transform.position += _perpendicular * 0.02f;
                }
            }
        }
    }

    public void DrawAllPath(NavMeshPath _navPath)
    {   // Draw arrows along the path to visualize it
        if (_pathArrows.Count > 0) ClearPath();
        if (!showPath || _navPath.corners.Length < 2) return;

        for (int i = 0; i < _navPath.corners.Length - 1; i++)
        {   // Draw arrows between path corners
            Vector3 _start = _navPath.corners[i];
            Vector3 _end = _navPath.corners[i + 1];
            Vector3 _dir = _end - _start;
            float _dist = Vector3.Distance(_start, _end);
            int _arrowCount = Mathf.FloorToInt(_dist / _pathArrowSpacing);

            for (int j = 0; j < _arrowCount; j++)
            {
                Vector3 _pos = _start + _dir.normalized * _pathArrowSpacing * j;
                GameObject _pathArrow = Instantiate(
                    _pathArrowPrefab,
                    _pos + Vector3.up * pathYOffset,
                    Quaternion.LookRotation(_dir),
                    this.transform
                    );
                _pathArrows.Add(_pathArrow);
            }
        }
    }

    private void ClearPath()
    {   // Clear the path visualization
        foreach (GameObject _pathArrow in _pathArrows)
            Destroy(_pathArrow);
        _pathArrows.Clear();
    }
}
