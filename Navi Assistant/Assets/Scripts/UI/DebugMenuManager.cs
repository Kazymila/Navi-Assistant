using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMenuManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private PathArrowVisualization _pathArrowVisualizer;
    [SerializeField] private PathLineVisualization _pathLineVisualizer;
    [SerializeField] private NavArrowController _navArrowController;

    [Header("UI Elements")]
    [SerializeField] private Slider _pathArrowsCountSlider;
    [SerializeField] private Slider _pathArrowsHeightSlider;
    [SerializeField] private Slider _showPathArrowToggle;
    [SerializeField] private Slider _pathLineHeightSlider;
    [SerializeField] private Slider _showPathLineToggle;
    [SerializeField] private Slider _arrowHeightSlider;
    [SerializeField] private Slider _showArrowToggle;
    private TextMeshProUGUI _pathArrowsCountText;
    private TextMeshProUGUI _pathArrowHeightText;
    private TextMeshProUGUI _pathLineHeightText;
    private TextMeshProUGUI _arrowHeightText;
    private GameObject _debugMenu;

    void Start()
    {   // Get references to UI elements
        _pathArrowsCountText = _pathArrowsCountSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _pathArrowHeightText = _pathArrowsHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _pathLineHeightText = _pathLineHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _arrowHeightText = _arrowHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _debugMenu = this.transform.GetChild(1).gameObject;
        _debugMenu.SetActive(false);

        // Set initial values to UI elements
        _pathArrowsCountSlider.value = _pathArrowVisualizer.maxArrowCount;
        _pathArrowsHeightSlider.value = _pathArrowVisualizer.pathYOffset;
        _pathLineHeightSlider.value = _pathLineVisualizer.pathYOffset;
        _arrowHeightSlider.value = _navArrowController.arrowYOffset;

        _showPathLineToggle.value = _pathLineVisualizer.showPathLine ? 1 : 0;
        _showPathArrowToggle.value = _pathArrowVisualizer.showPath ? 1 : 0;
        // _showArrowToggle.value = _navArrowController.showPathArrow ? 1 : 0;
    }

    public void ShowDebugMenu()
    {   // toggle debug menu visibility on button click
        _debugMenu.SetActive(!_debugMenu.activeSelf);
    }

    public void SetPathArrowsCount()
    {   // Set path arrows count to slider value
        _pathArrowVisualizer.maxArrowCount = (int)_pathArrowsCountSlider.value;
        _pathArrowsCountText.text = _pathArrowsCountSlider.value.ToString();
    }

    public void SetPathArrowHeight()
    {   // Set path height to slider value
        _pathArrowVisualizer.pathYOffset = _pathArrowsHeightSlider.value;
        _pathArrowHeightText.text = _pathArrowsHeightSlider.value.ToString("F2");
    }

    public void TogglePathArrows()
    {   // Toggle path visibility
        bool _showPath = _showPathArrowToggle.value == 1;
        _pathArrowVisualizer.showPath = _showPath;
    }

    public void SetPathLineHeight()
    {   // Set path line height to slider value
        _pathLineVisualizer.pathYOffset = _pathLineHeightSlider.value;
        _pathLineHeightText.text = _pathLineHeightSlider.value.ToString("F2");
    }

    public void TogglePathLine()
    {   // Toggle path line visibility
        bool _showPathLine = _showPathLineToggle.value == 1;
        _pathLineVisualizer.showPathLine = _showPathLine;
    }

    public void SetArrowHeight()
    {   // Set arrow height to slider value
        _navArrowController.arrowYOffset = _arrowHeightSlider.value;
        _arrowHeightText.text = _arrowHeightSlider.value.ToString("F2");
    }

    public void ToggleIndicatorArrow()
    {   // Toggle path arrow visibility
        bool _showPathArrow = _showArrowToggle.value == 1;
        _navArrowController.showPathArrow = _showPathArrow;
        _navArrowController.EnableNavArrow(_showPathArrow);
    }
}
