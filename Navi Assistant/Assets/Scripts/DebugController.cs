using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
public class DebugController : MonoBehaviour
{
    [SerializeField] private float _movingSpeed = 1.0f;
    [SerializeField] private float _rotationSpeed = 1.0f;
    private PlayerInput _input;

    private void OnEnable()
    {
        _input = this.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 move = _input.actions["Move"].ReadValue<Vector2>();
        Vector2 look = _input.actions["Rotate"].ReadValue<Vector2>();
        this.transform.position += new Vector3(move.x, 0, move.y) * _movingSpeed * Time.deltaTime;
        this.transform.Rotate(new Vector3(look.y, look.x, 0) * _rotationSpeed * Time.deltaTime);
    }
}
#endif