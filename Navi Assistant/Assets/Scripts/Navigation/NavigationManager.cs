using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private GameObject _MiniMapCanvas;
    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private Camera _ARCamera;
    [SerializeField] private GameObject _navTarget;
    [SerializeField] private GameObject _errorPanel;

    [Header("Path Visualization")]
    [SerializeField] private PathArrowVisualization _pathArrowVisualizer;
    [SerializeField] private NavArrowController _navArrowController;
    [SerializeField] private PathLineVisualization _pathLineVisualizer;
    [SerializeField] private PathLineVisualization _miniMapLineVisualizer;

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
            _errorPanel.SetActive(false);
            _pathArrowVisualizer.DrawPath(_navPath);
            _pathLineVisualizer.DrawPathLine(_navPath);
            _miniMapLineVisualizer.DrawPathLine(_navPath);

            if (_navArrowController.showPathArrow)
            {   // Show path arrow if enabled
                _navArrowController.UpdateNavArrow(_navPath);
            }
            else _navArrowController.EnableNavArrow(false);
        }
        else
        {   // Clear path if not reachable
            _pathArrowVisualizer.ClearPath();
            _pathLineVisualizer.ClearPathLine();
            _miniMapLineVisualizer.ClearPathLine();
            _navArrowController.EnableNavArrow(false);
            Debug.Log("Path is not reachable");

            // Show an alert message to the user
            _errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No es posible llegar al destino";
            _errorPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                "Asegurese de encontrarse dentro del camino, puede verificar su posición en el minimapa de su izquierda.\n\n" +
                " Si el problema persiste, intente reiniciar su ubicación leyendo un código QR cercano.";
            _errorPanel.SetActive(true);
        }
    }
}
