using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private GameObject navTarget;

    private NavMeshPath _navPath;
    private LineRenderer _lineRenderer;

    private void Start()
    {
        _navPath = new NavMeshPath();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        NavMesh.CalculatePath(transform.position, navTarget.transform.position, NavMesh.AllAreas, _navPath);
        _lineRenderer.positionCount = _navPath.corners.Length;
        _lineRenderer.SetPositions(_navPath.corners);
        _lineRenderer.enabled = true;
    }
}
