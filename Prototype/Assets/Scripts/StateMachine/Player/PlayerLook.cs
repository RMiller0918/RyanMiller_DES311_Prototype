using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _camera;
    private PlayerControls _controls;
    
    [Header("Settings")]
    [Tooltip("Used to limit the downward angle for the camera. Adjust to liking.")]
    [Range(25,45)][SerializeField] private static float _minViewDistance = 25f;

    [Tooltip("Up and down rotation speed.")]
    [Range(4,10)][SerializeField] private float _xSensitivity = 4f;
    [Tooltip("Left and right rotation speed.")]
    [Range(0.8f,10)][SerializeField] private float _ySensitivity = 4f;

    [Header("Values")] 
    [SerializeField] private float _xRotation;

    [SerializeField] private Vector2 _mouseInput;
    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>().transform;
        _controls = new PlayerControls();

        _controls.MainControls.MouseX.performed += ctx => _mouseInput.x =  ctx.ReadValue<float>();
        _controls.MainControls.MouseY.performed += ctx => _mouseInput.y =  ctx.ReadValue<float>();
    }

    private void Update()
    {
        TurnPlayer(_mouseInput.x);
        TurnCamera(_mouseInput.y);
    }

    private void TurnPlayer(float deltaX) // for turning player left or right, Takes X input and rotates on Y axis.
    {
        deltaX = deltaX * _xSensitivity * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, deltaX, 0);
        transform.Rotate(Vector3.up * deltaX);
    }

    private void TurnCamera(float deltaY) //for turning camera up or down, Takes Y input and rotates on the X axis.
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
