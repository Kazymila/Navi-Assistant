using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMenuManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private PathVisualization _pathVisualizer;
    [SerializeField] private NavArrowController _navArrowController;

    [Header("UI Elements")]
    [SerializeField] private Slider _pathHeightSlider;
    [SerializeField] private Slider _arrowHeightSlider;
    [SerializeField] private Slider _showPathToggle;
    [SerializeField] private Slider _showArrowToggle;
    private TextMeshProUGUI _pathHeightText;
    private TextMeshProUGUI _arrowHeightText;
    private GameObject _debugMenu;

    void Start()
    {   // Get references to UI elements
        _pathHeightText = _pathHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _arrowHeightText = _arrowHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _debugMenu = this.transform.GetChild(1).gameObject;
        _debugMenu.SetActive(false);

        _pathHeightSlider.value = _pathVisualizer.pathYOffset;
        _arrowHeightSlider.value = _navArrowController.ArrowYOffset;
        _showPathToggle.value = _pathVisualizer.showPath ? 1 : 0;
        _showArrowToggle.value = _navArrowController.showPathArrow ? 1 : 0;
    }

    public void ShowDebugMenu()
    {   // toggle debug menu visibility on button click
        _debugMenu.SetActive(!_debugMenu.activeSelf);
    }

    public void SetPathHeight()
    {   // Set path height to slider value
        _pathVisualizer.pathYOffset = _pathHeightSlider.value;
        _pathHeightText.text = _pathHeightSlider.value.ToString("F2");
    }

    public void TogglePathLine()
    {   // Toggle path visibility
        bool _showPathLine = _showPathToggle.value == 1;
        _pathVisualizer.showPath = _showPathLine;
    }

    public void SetArrowHeight()
    {   // Set arrow height to slider value
        _navArrowController.ArrowYOffset = _arrowHeightSlider.value;
        _arrowHeightText.text = _arrowHeightSlider.value.ToString("F2");
    }

    public void TogglePathArrow()
    {   // Toggle path arrow visibility
        bool _showPathArrow = _showArrowToggle.value == 1;
        _navArrowController.showPathArrow = _showPathArrow;
        _navArrowController.EnableNavArrow(_showPathArrow);
    }
}
