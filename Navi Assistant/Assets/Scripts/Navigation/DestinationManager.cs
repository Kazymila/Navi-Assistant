using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using MapDataModel;
using TMPro;

public class DestinationManager : MonoBehaviour
{
    [Header("Test Destination")]
    [SerializeField] private string _roomDestination;
    [SerializeField] private string _floorDestination;

    [Header("Destination Settings")]
    [SerializeField] private GameObject _destinationPrefab;
    [SerializeField] private GameObject _navTarget;

    [Header("External References")]
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private NavigationManager _navManager;

    [Header("UI Settings")]
    [SerializeField] private TMP_InputField _locationsInputField;
    [SerializeField] private TMP_Dropdown _locationsDropdown;
    private List<TMP_Dropdown.OptionData> _dropdownOptions;

    public void StartDestinationManager()
    {   // Start destination manager
        GenerateDestinationPoints();
        SetDropdownOptions();
    }

    #region --- Dropdown Options ---
    private void SetDropdownOptions()
    {   // Set dropdown options from destination points
        _dropdownOptions = new List<TMP_Dropdown.OptionData>();

        foreach (Transform _floor in this.transform)
        {   // Add floor name to dropdown options
            foreach (Transform _room in _floor)
            {   // Add room name to dropdown options
                TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData(_room.name);
                _dropdownOptions.Add(_option);
            }
        }
        _locationsDropdown.options = _dropdownOptions;
    }

    public void FilterDropdown(string _input)
    {   // Filter dropdown options based on input text
        string _inputText = _input.ToLower();
        _locationsDropdown.Hide();

        if (_inputText == "")
        {   // Show all options if input is empty
            _locationsDropdown.options = _dropdownOptions;
        }
        else
        {   // Show filtered options based on input text
            _locationsDropdown.options = _dropdownOptions.FindAll(
                option => option.text.ToLower().IndexOf(_inputText) >= 0);
        }
        _locationsDropdown.RefreshShownValue();
        _locationsDropdown.Show();

        _locationsInputField.ActivateInputField();
    }

    public void SetDestinationFromDropdown()
    {   // Set destination point from dropdown selection
        string _roomName = _locationsDropdown.options[_locationsDropdown.value].text;
        int _floorLevel = 0; // Set floor level to 0 for now (single floor map)

        _locationsInputField.text = _roomName;
        SetDestinationPoint(_roomName, _floorLevel);
        _locationsDropdown.Hide();
    }
    #endregion

    #region --- Set Navigation Destination ---
    public void SetDestinationPoint(string _roomName, int _floorLevel)
    {   // Set destination point to navigation manager
        Vector3 _startPos = _navManager.transform.position;
        Transform _destination = GetEntrancePoint(_roomName, _floorLevel, _startPos);
        _navManager.destinationPoint = _destination;
        _navTarget.transform.position = _destination.position;
    }

    private Transform GetEntrancePoint(string _roomName, int _floorLevel, Vector3 _startPos)
    {   // Get entrance point for a room, to set as destination point
        Transform _room = this.transform.GetChild(_floorLevel).Find(_roomName);

        if (_room.childCount == 0) return null;
        if (_room.childCount == 1) return _room.GetChild(0);

        // Get the closest entrance point to the start position
        float[] _pathDistances = new float[_room.childCount];

        for (int i = 0; i < _room.childCount; i++)
        {   // Calculate path distance from start position to each entrance
            Transform _entrance = _room.GetChild(i);
            NavMeshPath _navPath = new NavMeshPath();
            NavMesh.CalculatePath(_startPos, _entrance.position, NavMesh.AllAreas, _navPath);
            _pathDistances[i] = GetPathDistance(_navPath);
        }
        // Return the entrance with the shortest path distance
        int _minIndex = 0;
        for (int i = 1; i < _pathDistances.Length; i++)
            if (_pathDistances[i] < _pathDistances[_minIndex]) _minIndex = i;

        return _room.GetChild(_minIndex);
    }

    private float GetPathDistance(NavMeshPath _navPath)
    {   // Get distance of the path
        float _distance = 0;
        for (int i = 1; i < _navPath.corners.Length; i++)
        {   // Calculate distance between each corner of the path
            _distance += Vector3.Distance(_navPath.corners[i - 1], _navPath.corners[i]);
        }
        return _distance;
    }
    #endregion

    #region --- Load Destination Data ---
    public void GenerateDestinationPoints()
    {   // Generate destination points from map data
        foreach (FloorData _floor in _mapLoader.mapData.floors)
        {   // Generate a object for each floor
            GameObject _floorObj = new GameObject(_floor.floorName);
            _floorObj.transform.SetParent(this.transform);

            foreach (RoomData _room in _floor.rooms)
            {   // Generate a object for each room in the floor
                GameObject _roomObj = new GameObject(_room.roomName);
                _roomObj.transform.SetParent(_floorObj.transform);

                CreateEntrancesFromRoom(_room, _roomObj);
            }
        }
    }

    private void CreateEntrancesFromRoom(RoomData _roomData, GameObject _roomObj)
    {   // Get entrances from room data and create destination points
        foreach (int _wallID in _roomData.walls)
        {   // Get wall data of the room to get the entrances
            WallData _wallData = _mapLoader.mapData.floors[0].walls[_wallID];

            // Check if wall belongs to the room 
            if (!WallBelongsToRoom(_wallData, _roomData)) continue;

            foreach (EntranceData _entrance in _wallData.entrances)
            {   // Create destination point for each entrance
                Vector3 _startPos = _entrance.startNodePosition.GetVector3;
                Vector3 _endPos = _entrance.endNodePosition.GetVector3;
                Vector3 _position = Quaternion.Euler(90, 0, 0) * (_startPos + _endPos) / 2;
                _position.y = 0.0f;

                GameObject _obj = Instantiate(_destinationPrefab, _position, Quaternion.identity, _roomObj.transform);
                _obj.name = "Entrance_" + _obj.transform.GetSiblingIndex().ToString();
            }
        }
    }

    private bool WallBelongsToRoom(WallData wallData, RoomData roomData)
    {   // Check if wall belongs to the room
        int nodeCount = 0;
        foreach (int node in roomData.nodes)
        {   // Check if wall has start and end node in the room
            if (node == wallData.startNode) nodeCount++;
            if (node == wallData.endNode) nodeCount++;
        }
        // Wall does not belong to the room, if both nodes are not in the room
        if (nodeCount < 2) return false;
        return true;
    }
    #endregion
}
