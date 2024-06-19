using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using MapDataModel;
using Firebase.Firestore;
using Firebase.Extensions;

public class MapLoader : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField] private string mapFileName = "ExampleMap";
    [SerializeField] private DestinationManager _destinationManager;

    [Header("Map Render")]
    [SerializeField] private GameObject wallRenderPrefab;
    [SerializeField] private GameObject roomRenderPrefab;
    [SerializeField] private GameObject shapeRenderPrefab;
    private NavMeshSurface _navMeshSurface;
    public MapData mapData;

    private void Awake()
    {   // Load map data and generate floor map render
        _navMeshSurface = this.GetComponent<NavMeshSurface>();
        LoadMapData();
    }

    public void LoadLocalMapData()
    {   // Load map data from local file
        string _path = Application.streamingAssetsPath + "/" + mapFileName + ".json";
        string jsonData = System.IO.File.ReadAllText(_path);
        mapData = JsonUtility.FromJson<MapData>(jsonData);
    }

    public void LoadMapData()
    {   // Load map data from Firestore database
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("MapData").Document(mapFileName);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {   // Load map data and generate map render
                string jsonData = snapshot.ToDictionary()["MapData"].ToString();
                mapData = JsonUtility.FromJson<MapData>(jsonData);

                print("Map Data: " + mapData.mapName + " loaded!");

                GenerateMapRender();
                _destinationManager.StartDestinationManager();
            }
            else Debug.LogError("Document does not exist!");
        });
    }

    #region --- Map Render Generation ---
    public void GenerateNavMesh() => _navMeshSurface.BuildNavMesh();
    private void GenerateMapRender()
    {   // Generate Map Render from data
        FloorData _floor = mapData.floors[0];
        foreach (WallData _wall in _floor.walls) GenerateWallRender(_wall);
        foreach (RoomData _room in _floor.rooms) GenerateRoomRender(_room);
        foreach (ShapeData _shape in _floor.shapes) GenerateShapeRender(_shape);
        GenerateNavMesh();
    }

    private void GenerateWallRender(WallData _wallData)
    {   // Generate wall render from data
        GameObject _wallRender = Instantiate(wallRenderPrefab, this.transform.GetChild(0));
        Vector3[] vertices = SerializableVector3.GetVector3Array(_wallData.renderData.vertices);
        int[] triangles = _wallData.renderData.triangles;

        _wallRender.GetComponent<MeshFilter>().mesh.vertices = vertices;
        _wallRender.GetComponent<MeshFilter>().mesh.triangles = triangles;
        _wallRender.GetComponent<MeshCollider>().sharedMesh = _wallRender.GetComponent<MeshFilter>().mesh;
        _wallRender.name = "Wall_" + _wallData.wallID.ToString();
    }

    private void GenerateRoomRender(RoomData _roomData)
    {   // Generate rooms floor render from data
        GameObject _roomRender = Instantiate(roomRenderPrefab, this.transform.GetChild(1));
        Vector3[] vertices = SerializableVector3.GetVector3Array(_roomData.renderData.vertices);
        int[] triangles = _roomData.renderData.triangles;

        _roomRender.GetComponent<MeshFilter>().mesh.vertices = vertices;
        _roomRender.GetComponent<MeshFilter>().mesh.triangles = triangles;
        _roomRender.GetComponent<MeshRenderer>().material.SetColor(
            "_Color1", _roomData.polygonData.materialColor.GetColor);
        _roomRender.name = _roomData.roomName.key;

        _roomRender.GetComponent<MeshCollider>().sharedMesh = _roomRender.GetComponent<MeshFilter>().mesh;
    }

    private void GenerateShapeRender(ShapeData _shapeData)
    {   // Generate shape render from data
        GameObject _shapeRender = Instantiate(shapeRenderPrefab, this.transform.GetChild(2));
        _shapeRender.transform.position = new Vector3(_shapeData.shapePosition.x, 0, _shapeData.shapePosition.y);
        Vector3[] vertices = SerializableVector3.GetVector3Array(_shapeData.renderData.vertices);
        int[] triangles = _shapeData.renderData.triangles;

        _shapeRender.GetComponent<MeshFilter>().mesh.vertices = vertices;
        _shapeRender.GetComponent<MeshFilter>().mesh.triangles = triangles;
        _shapeRender.GetComponent<MeshRenderer>().material.SetColor(
            "_Color1", _shapeData.polygonData.materialColor.GetColor);
        _shapeRender.name = "Shape_" + _shapeData.shapeID.ToString();
    }
    #endregion
}
