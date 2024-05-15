using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMenuManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private AgentNavigation _agentNavigation;
    [SerializeField] private LineRenderer _pathLine;

    [Header("UI Elements")]
    [SerializeField] private Slider _pathHeightSlider;
    [SerializeField] private Slider _showPathLineToggle;
    [SerializeField] private Slider _showPathPointsToggle;
    [SerializeField] private Slider _showPathArrowToggle;
    private TextMeshProUGUI _pathHeightText;
    private GameObject _debugMenu;

    void Start()
    {   // Get references to UI elements
        _pathHeightText = _pathHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _debugMenu = this.transform.GetChild(1).gameObject;
        _debugMenu.SetActive(false);
    }

    public void ShowDebugMenu()
    {   // toggle debug menu visibility on button click
        _debugMenu.SetActive(!_debugMenu.activeSelf);
    }

    public void SetPathHeight()
    {   // Set path height to slider value
        _agentNavigation.pathHeight = _pathHeightSlider.value;
        _pathHeightText.text = _pathHeightSlider.value.ToString("F1");
    }

    public void TogglePathLine()
    {   // Toggle path line visibility
        _agentNavigation.showLinePath = _showPathLineToggle.value == 1;
        _pathLine.enabled = _agentNavigation.showLinePath;
    }

    public void TogglePathPoints()
    {   // Toggle path points visibility
        _agentNavigation.showPathPoints = _showPathPointsToggle.value == 1;
    }
}
