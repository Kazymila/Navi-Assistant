using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using MapDataModel;
using UnityEngine.Localization.Settings;

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
    [SerializeField] private AssistantManager _assistantManager;

    [Header("UI References")]
    [SerializeField] private SearchableDropdownController _dropdownController;
    [SerializeField] private OptionsButtonsController _destinationOptionsButtons;

    [Header("Destination Classes")]
    [SerializeField] private List<TranslatedText> _destinationFilterOptions;
    [SerializeField] private List<Transform>[] _destinationRoomsByType;

    public void StartDestinationManager()
    {   // Start destination manager
        _destinationFilterOptions = new List<TranslatedText>();
        _destinationRoomsByType = new List<Transform>[_mapLoader.mapData.roomTypes.Length];

        GetDestinationClasses();
        LoadDestinationPoints();
        SetDestinationOptionsButtons();
    }

    #region --- UI Managment ---
    public void SetAllDestinationsOnDropdown()
    {   // Set dropdown options from destination points
        List<TranslatedText> _dropdownOptions = new List<TranslatedText>();

        foreach (FloorData _floor in _mapLoader.mapData.floors)
            foreach (RoomData _room in _floor.rooms)
            {   // Add room name to dropdown options
                _dropdownOptions.Add(_room.roomName);
            }
        _dropdownController.SetDropdownOptions(_dropdownOptions);
    }

    private void SetDestinationsOnDropdown(List<string> _destionations)
    {   // Set destination options on dropdown from a list of room names
        string _languageCode = LocalizationSettings.SelectedLocale.name.Split("(")[1].Split(")")[0];
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
    }

    public void ShowDestinationOptionsButtons()
    {   // Show destination options buttons
        _destinationOptionsButtons.ShowOptionsButtons();
    }
    #endregion

    #region --- Set Navigation Destination ---
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
        _navManager.destinationPoint = _destination;
        _navTarget.transform.position = _destination.position;
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

    #region --- Destination Data ---
    public void LoadDestinationPoints()
    {   // Generate destination points from map data
        foreach (FloorData _floor in _mapLoader.mapData.floors)
        {   // Generate a object for each floor
            GameObject _floorObj = new GameObject(_floor.floorName);
            _floorObj.transform.SetParent(this.transform);

            foreach (RoomData _room in _floor.rooms)
            {   // Generate a object for each room in the floor
                GameObject _roomObj = new GameObject(_room.roomName.key);
                _roomObj.transform.SetParent(_floorObj.transform);
                CreateEntrancesFromRoom(_room, _roomObj);

                // If room type is unique, add to destination filter options
                if (_room.roomType == 0) _destinationFilterOptions.Add(_room.roomName);
                else _destinationRoomsByType[_room.roomType].Add(_roomObj.transform);
            }
        }
    }

    private void GetDestinationClasses()
    {   // Get destination classes from map data
        foreach (RoomTypeData _roomType in _mapLoader.mapData.roomTypes)
        {   // Add room type name to destination classes
            if (_roomType.typeName.key != "Unique")
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
