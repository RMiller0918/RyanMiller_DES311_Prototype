using System;
using UnityEngine;
using UnityEngine.InputSystem;


//Followed Unity3D Tutorial - Fixing First Person View Camera Stutter/Jitter Issue - Kamran Wali - https://youtu.be/hsoJLJ22GVA
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
    }

    private void Update()
    {
        _mouseInput.x = (Mouse.current.delta.x.ReadValue() * Time.smoothDeltaTime); //gets the current mouse delta for x/y axis.
        _mouseInput.y = (Mouse.current.delta.y.ReadValue() * Time.smoothDeltaTime);
        Rotate();
    }

    protected virtual void Rotate() //handles rotation
    {
        _oldVertical = _xRotation;
        _xRotation -= GetVerticalValue();
        _xRotation = _xRotation <= -90f ? -90f : _xRotation >= _minViewDistance ? _minViewDistance : _xRotation; //clamps the x-rotation values.
        RotateVertical();
        RotateHorizontal();
    }

    protected float GetVerticalValue() => _mouseInput.y * _ySensitivity / Time.timeScale;
    protected float GetHorizontalValue() => _mouseInput.x * _xSensitivity; //not scaled by time scale to prevent issue with oversensitivity in slow motion.
    protected virtual void RotateVertical() //rotates the camera around the x-axis
    {
        _xRotation = Mathf.SmoothDampAngle(_oldVertical, _xRotation, ref _vertAngularVelocity, _smoothTime);
        _camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    protected virtual void RotateHorizontal() //rotates the player around the y-axis.
    {
        _horizontalRotationHelper.Rotate(Vector3.up * GetHorizontalValue() , Space.Self);
        transform.localRotation = Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.localEulerAngles.y, _horizontalRotationHelper.localEulerAngles.y, ref _horiAngularVelocity, _smoothTime), 0f);
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
