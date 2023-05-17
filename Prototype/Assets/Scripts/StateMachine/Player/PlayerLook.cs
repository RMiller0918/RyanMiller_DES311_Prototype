using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _camera;
    private PlayerControls _controls;
    
    [Header("Settings")]
    [Tooltip("Used to limit the downward angle for the camera. Adjust to liking.")]
    [Range(25,70)][SerializeField] private float _minViewDistance = 25f;

    [Tooltip("Up and down rotation speed.")]
    [Range(4,10)][SerializeField] protected float _xSensitivity = 4f;
    [Tooltip("Left and right rotation speed.")]
    [Range(0.8f,20)][SerializeField] protected float _ySensitivity = 4f;

    [Header("Values")] 
    [SerializeField] protected float _xRotation;

    [SerializeField] protected Vector2 _mouseInput;

    [Header("Player Rotation Smooth Properties")]
    [SerializeField] private float _smoothTime;
    [SerializeField] private Transform _horizontalRotationHelper;

    private float _oldVertical;
    private float _vertAngularVelocity;
    private float _horiAngularVelocity;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>().transform;
        _controls = new PlayerControls();

        //_controls.MainControls.MouseX.performed += ctx => _mouseInput.x = ctx.ReadValue<float>();
        //_controls.MainControls.MouseY.performed += ctx => _mouseInput.y = ctx.ReadValue<float>();
    }

    private void LateUpdate()
    {
        _mouseInput.x = Mouse.current.delta.x.ReadValue() * Time.smoothDeltaTime;
        _mouseInput.y = Mouse.current.delta.y.ReadValue() * Time.smoothDeltaTime;
        Rotate();
    }

    protected virtual void Rotate()
    {
        _oldVertical = _xRotation;
        _xRotation -= GetVerticalValue();
        _xRotation = _xRotation <= -90f ? -90f : _xRotation >= _minViewDistance ? _minViewDistance : _xRotation;
        //TurnPlayer(_mouseInput.x);
        //TurnCamera(_mouseInput.y);
        RotateVertical();
        RotateHorizontal();
    }

    protected float GetVerticalValue() => _mouseInput.y * _ySensitivity;
    protected float GetHorizontalValue() => _mouseInput.x * _xSensitivity;
    protected virtual void RotateVertical()
    {
        _xRotation = Mathf.SmoothDampAngle(_oldVertical, _xRotation, ref _vertAngularVelocity, _smoothTime);
        _camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    protected virtual void RotateHorizontal()
    {
        _horizontalRotationHelper.Rotate(Vector3.up * GetHorizontalValue() , Space.Self);
        transform.localRotation = Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.localEulerAngles.y, _horizontalRotationHelper.localEulerAngles.y, ref _horiAngularVelocity, _smoothTime), 0f);
        //transform.Rotate(Vector3.up * GetHorizontalValue());
    }


    protected virtual void TurnPlayer(float deltaX) // for turning player left or right, Takes X input and rotates on Y axis.
    {
        deltaX = deltaX * _xSensitivity * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, deltaX, 0);
        transform.Rotate(Vector3.up * deltaX);
    }

    protected virtual void TurnCamera(float deltaY) //for turning camera up or down, Takes Y input and rotates on the X axis.
    {
        _xRotation -= deltaY * _xSensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -90f, _minViewDistance);
        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f,0f);
    }

    private void OnEnable()
    {
        _controls.MainControls.Enable();
    }

    private void OnDisable()
    {
        _controls.MainControls.Disable();
    }
}
