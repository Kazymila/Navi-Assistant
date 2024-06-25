using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using MapDataModel;
using UnityEngine.Localization.Settings;
using TMPro;
using System;
using Unity.XR.CoreUtils;

public class DestinationManager : MonoBehaviour
{
    [Header("Test Destination")]
    [SerializeField] private string _roomDestination;
    [SerializeField] private string _floorDestination;

    [Header("Teleport Settings (Extra points)")]
    [SerializeField] private GameObject _teleportPoints;
    [SerializeField] private OptionsButtonsController _teleportOptionsButtons;

    [Header("Destination Settings")]
    [SerializeField] private GameObject _destinationPrefab;
    [SerializeField] private TargetIndicatorController _navTarget;

    [Header("External References")]
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private NavigationManager _navManager;
    [SerializeField] private AssistantManager _assistantManager;

    [Header("UI References")]
    [SerializeField] private SearchableDropdownController _dropdownController;
    [SerializeField] private OptionsButtonsController _destinationOptionsButtons;
    [SerializeField] private GameObject _floatingLabelTemplate;

    [Header("Destination Classes")]
    [SerializeField] private List<TranslatedText> _destinationFilterOptions;
    [SerializeField] private List<Transform>[] _destinationRoomsByType;

    private List<FloatingLabelController> _floatingLabels;

    public void StartDestinationManager()
    {   // Start destination manager
        _floatingLabels = new List<FloatingLabelController>();
        _destinationFilterOptions = new List<TranslatedText>();
        _destinationRoomsByType = new List<Transform>[_mapLoader.mapData.roomTypes.Length];

        GetDestinationClasses();
        LoadDestinationPoints();
        SetDestinationOptionsButtons();
        SetTeleportOptionsButtons();
    }

    #region --- UI Managment ---
    public void SetAllDestinationsOnDropdown()
    {   // Set dropdown options from destination points
        List<string> _dropdownOptions = new List<string>();

        foreach (Transform _floor in this.transform)
            foreach (Transform _room in _floor)
            {   // Add room name to dropdown options
                _dropdownOptions.Add(_room.name);
            }
        SetDestinationsOnDropdown(_dropdownOptions);
    }

    private void SetDestinationsOnDropdown(List<string> _destionations)
    {   // Set destination options on dropdown from a list of room names
        List<TranslatedText> _dropdownOptions = new List<TranslatedText>();

        foreach (FloorData _floor in _mapLoader.mapData.floors)
            foreach (RoomData _room in _floor.rooms)
            {   // Add translated room name to dropdown options
                if (_destionations.Contains(_room.roomName.key))
                    _dropdownOptions.Add(_room.roomName);
            }
        _dropdownController.SetDropdownOptions(_dropdownOptions);
    }

    public void SelectDestinationFromDropdown()
    {   // Set destination point from dropdown selection
        string _roomName = _dropdownController.GetSelectedOption();
        int _floorLevel = 0; // Set floor level to 0 for now

        _assistantManager.GoToDestination(_roomName);
        SetDestinationPoint(_roomName, _floorLevel);
    }

    public void SetDestinationOptionsButtons()
    {   // Set destination types buttons from destination classes
        foreach (TranslatedText _option in _destinationFilterOptions)
        {   // Create a button for each destination class
            _destinationOptionsButtons.AddOptionButton(_option, () => SetNavigationOption(_option.key));
        }
        // Add teleport option to destination options
        _destinationOptionsButtons.AddOptionButton(new TranslatedText()
        {
            key = "Teleport",
            englishTranslation = "Another place",
            spanishTranslation = "Otro lugar"
        }, () => _assistantManager.SelectAnotherDestination());
    }

    public void ShowDestinationOptionsButtons()
    {   // Show destination options buttons
        _destinationOptionsButtons.ShowOptionsButtons();
    }

    public void ShowTeleportOptionsButtons()
    {   // Show teleport options buttons
        _teleportOptionsButtons.ShowOptionsButtons();
    }

    public void SetTeleportOptionsButtons()
    {   // Set teleport options from teleport points
        List<Transform> _upFloorPoints = new List<Transform>();
        List<Transform> _downFloorPoints = new List<Transform>();

        foreach (Transform _point in _teleportPoints.transform.GetChild(0))
        {
            _upFloorPoints.Add(_point);
            _downFloorPoints.Add(_point);
        }
        foreach (Transform _point in _teleportPoints.transform.GetChild(1)) _upFloorPoints.Add(_point);
        foreach (Transform _point in _teleportPoints.transform.GetChild(2)) _downFloorPoints.Add(_point);

        _teleportOptionsButtons.AddOptionButton(
            new TranslatedText()
            {
                key = "GoToSecondFloor",
                englishTranslation = "Up to another floor",
                spanishTranslation = "Subir a otro piso"
            },
            () => SetExtraPointAsDestination(_upFloorPoints)
        );
        _teleportOptionsButtons.AddOptionButton(
            new TranslatedText()
            {
                key = "GoToBasement",
                englishTranslation = "Go to basement",
                spanishTranslation = "Al subterraneo"
            },
            () => SetExtraPointAsDestination(_downFloorPoints)
        );
        _teleportOptionsButtons.AddOptionButton(
            new TranslatedText()
            {
                key = "GoOutside",
                englishTranslation = "Go outside",
                spanishTranslation = "Hacia fuera"
            },
            () => SetExtraPointAsDestination(new List<Transform>() {
                    _teleportPoints.transform.GetChild(3).GetChild(0)})
        );
        _teleportOptionsButtons.AddOptionButton(
            new TranslatedText()
            {
                key = "Back",
                englishTranslation = "Go back",
                spanishTranslation = "Volver"
            },
            () =>
            {   // Hide teleport options and select destination interaction
                _teleportOptionsButtons.HideOptionsButtons();
                _assistantManager.SelectDestinationInteraction();
            }
        );
    }
    #endregion

    #region --- Set Navigation Destination ---
    public void SetExtraPointAsDestination(List<Transform> _points)
    {   // Set an extra point as destination point
        Vector3 _startPos = _navManager.transform.position;
        Transform _selectedPoint = _points[0];

        float _minDistance = Vector3.Distance(_startPos, _selectedPoint.position);

        foreach (Transform _point in _points)
        {   // Get the most nearest point to the user
            if (Vector3.Distance(_startPos, _point.position) < _minDistance)
            {
                _selectedPoint = _point;
                _minDistance = Vector3.Distance(_startPos, _point.position);
            }
        }
        _navTarget.SetTargetPosition(_selectedPoint.position);
        _navManager.SetDestinationPoint(_selectedPoint);
        _assistantManager.GoToAnotherPlace();
    }

    public void SetNavigationOption(string _optionName)
    {   // Set navigation destination from option name
        _destinationOptionsButtons.HideOptionsButtons();

        foreach (RoomTypeData _roomType in _mapLoader.mapData.roomTypes)
        {   // Check for room type selected to set destination
            if (_roomType.typeName.key == _optionName)
            {
                if (_roomType.searchNearestMode)
                {   // Search nearest room of the type selected to set as destination
                    List<Transform> _rooms = _destinationRoomsByType[_roomType.typeID];
                    string _roomName = SetNearestRoomAsDestination(_rooms);
                    _assistantManager.GoToDestination(_roomName);
                    return;
                }
                else
                {   // Set dropdown options from destination rooms of the type selected
                    List<string> _options = _destinationRoomsByType[_roomType.typeID].ConvertAll(_room => _room.name);
                    SetDestinationsOnDropdown(_options);
                    _assistantManager.SelectDestinationFromDropdown();
                    return;
                }
            }
        }
        // If is a unique room, set directly the destination
        SetDestinationPoint(_optionName, 0);
        _assistantManager.GoToDestination(_optionName);
        return;
    }

    public void SetDestinationPoint(string _roomName, int _floorLevel)
    {   // Set destination point to navigation manager
        Vector3 _startPos = _navManager.transform.position;
        Transform _destination = GetNeareastEntrancePoint(_roomName, _floorLevel, _startPos);
        _navManager.SetDestinationPoint(_destination);
        _navTarget.SetTargetPosition(_destination.position);
    }

    private string SetNearestRoomAsDestination(List<Transform> _rooms)
    {   // Get the nearest room from a list of rooms
        Vector3 _startPos = _navManager.transform.position;
        float[] _pathDistances = new float[_rooms.Count];

        for (int i = 0; i < _rooms.Count; i++)
        {   // Calculate path distance from start position to each room
            Transform _room = _rooms[i];
            Transform _entrance = _room.GetChild(0);
            NavMeshPath _navPath = new NavMeshPath();
            NavMesh.CalculatePath(_startPos, _entrance.position, NavMesh.AllAreas, _navPath);
            _pathDistances[i] = GetPathDistance(_navPath);
        }
        // Return the room with the shortest path distance
        int _minIndex = 0;
        for (int i = 1; i < _pathDistances.Length; i++)
            if (_pathDistances[i] < _pathDistances[_minIndex]) _minIndex = i;

        SetDestinationPoint(_rooms[_minIndex].name, 0);
        return _rooms[_minIndex].name;
    }

    private Transform GetNeareastEntrancePoint(string _roomName, int _floorLevel, Vector3 _startPos)
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
    public void LoadDestinationPoints()
    {   // Generate destination points from map data
        foreach (FloorData _floor in _mapLoader.mapData.floors)
        {   // Generate a object for each floor
            GameObject _floorObj = new GameObject(_floor.floorName);
            _floorObj.transform.SetParent(this.transform);

            foreach (RoomData _room in _floor.rooms)
            {   // Generate a object for each room in the floor
                if (_room.roomType == 1) continue;
                GameObject _roomObj = new GameObject(_room.roomName.key);
                _roomObj.transform.SetParent(_floorObj.transform);
                CreateEntrancesFromRoom(_room, _roomObj);

                // Discard room if it has no entrances
                if (_roomObj.transform.childCount == 0) Destroy(_roomObj);
                else
                {   // If room type is unique, add to destination filter options
                    if (_room.roomType == 0) _destinationFilterOptions.Add(_room.roomName);
                    else _destinationRoomsByType[_room.roomType].Add(_roomObj.transform);
                }
            }
        }
    }

    private void GetDestinationClasses()
    {   // Get destination classes from map data
        foreach (RoomTypeData _roomType in _mapLoader.mapData.roomTypes)
        {   // Add room type name to destination classes
            if (_roomType.typeName.key != "Unique" && _roomType.typeName.key != "Space")
            {
                _destinationRoomsByType[_roomType.typeID] = new List<Transform>();
                _destinationFilterOptions.Add(_roomType.typeName);
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

                CreateTargetFloatingLabel(_roomData.roomName, _position);
            }
        }
    }

    private void CreateTargetFloatingLabel(TranslatedText _roomName, Vector3 _position)
    {   // Create a floating label for the destination point
        GameObject _label = Instantiate(_floatingLabelTemplate, _position, Quaternion.identity);
        _label.transform.SetParent(_floatingLabelTemplate.transform.parent);
        _label.name = "TargetLabel " + _label.transform.GetSiblingIndex() + ": " + _roomName.key;

        // Set the room name in the label in the current language
        _label.GetComponent<FloatingLabelController>().SetLabelText(_roomName);
        _floatingLabels.Add(_label.GetComponent<FloatingLabelController>());
        _label.SetActive(true);
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
