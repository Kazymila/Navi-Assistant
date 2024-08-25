using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
public class DebugController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1.0f;
    private PlayerInput _input;

    private void OnEnable()
    {   // Get player input
        _input = this.GetComponent<PlayerInput>();
    }

    private void Update()
    {   // Rotate camera with keyboard input
        Vector2 look = _input.actions["Rotate"].ReadValue<Vector2>();
        this.transform.Rotate(new Vector3(look.y, look.x, 0) * _rotationSpeed * Time.deltaTime);
    }
}
#endif