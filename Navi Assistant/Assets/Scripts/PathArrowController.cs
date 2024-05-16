using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathArrowController : MonoBehaviour
{
    [Header("Path Arrow Settings")]
    public bool showPathArrow = false;
    public float ArrowYOffset = -0.27f;
    [SerializeField] private float _moveOnDistance;

    private GameObject _arrow;
    private Vector3 _nextPoint = Vector3.zero;

    void Awake()
    {
        _arrow = this.transform.GetChild(0).gameObject;
    }

    public void EnablePathArrow(bool _enable)
    {   // Enable or disable the path arrow
        _arrow.SetActive(_enable);
    }

    public void UpdatePathArrow(NavMeshPath _navPath)
    {   // Update the path arrow position and rotation
        Vector3[] _pathPoints = AddOffsetToPath(_navPath.corners);
        _nextPoint = SelectNextNavigationPoint(_pathPoints);
        AddOffsetToArrow();

        _arrow.transform.LookAt(_nextPoint);
    }

    private Vector3[] AddOffsetToPath(Vector3[] _points)
    {   // Add height offset to path points
        Vector3[] _newPoints = new Vector3[_points.Length];

        for (int i = 0; i < _points.Length; i++)
            _newPoints[i] = new Vector3(_points[i].x, this.transform.position.y, _points[i].z);
        return _newPoints;
    }

    private void AddOffsetToArrow()
    {   // Add height offset to arrow
        if (ArrowYOffset != 0)
            _arrow.transform.position = new Vector3(
                _arrow.transform.position.x, ArrowYOffset, _arrow.transform.position.z);
    }

    private Vector3 SelectNextNavigationPoint(Vector3[] _points)
    {   // Select the next navigation point within a distance
        for (int i = 0; i < _points.Length; i++)
        {
            float _currentDistance = Vector3.Distance(this.transform.position, _points[i]);
            if (_currentDistance > _moveOnDistance)
                return _points[i];
        }
        return _points[_points.Length - 1];
    }
}
