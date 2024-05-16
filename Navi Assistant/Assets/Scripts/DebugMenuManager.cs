using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMenuManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private PathLineController _pathLineController;
    [SerializeField] private PathArrowController _pathArrowController;

    [Header("UI Elements")]
    [SerializeField] private Slider _pathHeightSlider;
    [SerializeField] private Slider _arrowHeightSlider;
    [SerializeField] private Slider _showPathLineToggle;
    [SerializeField] private Slider _showPathArrowToggle;
    private TextMeshProUGUI _pathHeightText;
    private TextMeshProUGUI _arrowHeightText;
    private GameObject _debugMenu;

    void Start()
    {   // Get references to UI elements
        _pathHeightText = _pathHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _arrowHeightText = _arrowHeightSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _debugMenu = this.transform.GetChild(1).gameObject;
        _debugMenu.SetActive(false);

        _pathHeightSlider.value = _pathLineController.pathYOffset;
        _arrowHeightSlider.value = _pathArrowController.ArrowYOffset;
        _showPathLineToggle.value = _pathLineController.showPathLine ? 1 : 0;
        _showPathArrowToggle.value = _pathArrowController.showPathArrow ? 1 : 0;
    }

    public void ShowDebugMenu()
    {   // toggle debug menu visibility on button click
        _debugMenu.SetActive(!_debugMenu.activeSelf);
    }

    public void SetPathHeight()
    {   // Set path height to slider value
        _pathLineController.pathYOffset = _pathHeightSlider.value;
        _pathHeightText.text = _pathHeightSlider.value.ToString("F2");
    }

    public void TogglePathLine()
    {   // Toggle path line visibility
        bool _showPathLine = _showPathLineToggle.value == 1;
        _pathLineController.showPathLine = _showPathLine;
        _pathLineController.EnablePathLine(_showPathLine);
    }

    public void SetArrowHeight()
    {   // Set arrow height to slider value
        _pathArrowController.ArrowYOffset = _arrowHeightSlider.value;
        _arrowHeightText.text = _arrowHeightSlider.value.ToString("F2");
    }

    public void TogglePathArrow()
    {   // Toggle path arrow visibility
        bool _showPathArrow = _showPathArrowToggle.value == 1;
        _pathArrowController.showPathArrow = _showPathArrow;
        _pathArrowController.EnablePathArrow(_showPathArrow);
    }
}
