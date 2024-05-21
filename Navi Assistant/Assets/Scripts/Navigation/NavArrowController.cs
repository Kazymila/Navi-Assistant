using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavArrowController : MonoBehaviour
{
    [Header("Path Arrow Settings")]
    public bool showPathArrow = false;
    public float arrowYOffset = -0.3f;
    [SerializeField] private float _moveOnDistance;

    private GameObject _arrow;
    private Vector3 _nextPoint = Vector3.zero;

    void Awake()
    {
        _arrow = this.transform.GetChild(0).gameObject;

#if UNITY_EDITOR
        // Set a offset for testing in the editor
        arrowYOffset = -0.15f;
#endif
    }

    public void EnableNavArrow(bool _enable)
    {   // Enable or disable the path arrow
        _arrow.SetActive(_enable);
    }

    public void UpdateNavArrow(NavMeshPath _navPath)
    {   // Update the path arrow position and rotation
        RaycastHit _hit;

        if (Physics.Raycast(_arrow.transform.position, Vector3.down, out _hit, Mathf.Infinity,
            LayerMask.GetMask("Navigation")) && _hit.collider.CompareTag("PathArrow"))
        {   // If the arrow is over the navigation path, disable the arrow
            EnableNavArrow(false);
            return;
        }
        // Draw inidicator arrow if the user is not looking the path
        Vector3[] _pathPoints = AddOffsetToPath(_navPath.corners);
        _nextPoint = SelectNextNavigationPoint(_pathPoints);
        AddOffsetToArrow();

        _arrow.transform.LookAt(_nextPoint);
        _arrow.transform.rotation = Quaternion.Euler(0,
            _arrow.transform.rotation.eulerAngles.y, 0);
        EnableNavArrow(true);
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
        if (arrowYOffset != 0)
            _arrow.transform.position = new Vector3(
                _arrow.transform.position.x, arrowYOffset, _arrow.transform.position.z);
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

    private void OnTriggerEnter(Collider _collision)
    {   // Disable the wall when the arrow collides with it
        if (_collision.CompareTag("Wall"))
        {
            _collision.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider _collision)
    {   // Enable the wall when the arrow exits the collision
        if (_collision.CompareTag("Wall"))
        {
            _collision.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
