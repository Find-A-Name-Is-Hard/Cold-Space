using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //PlayerInput _playerInputActions;
    [SerializeField]
    
    public InputActionAsset moveAction;
    public float _speed = 5f;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        Vector2 _playerInput = moveAction.FindAction("Move").ReadValue<Vector2>();
        Debug.Log(moveAction.FindAction("Move").ReadValue<Vector2>());
        transform.Translate(_playerInput * Time.deltaTime * _speed);
    }

}
