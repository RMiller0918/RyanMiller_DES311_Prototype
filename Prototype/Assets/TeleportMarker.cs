using UnityEngine;
using UnityEngine.UI;

public class TeleportMarker : MonoBehaviour
{
    [SerializeField] private GameObject _markerImage;
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Transform _groundCanvas;
    [SerializeField] private Image _groundMarkerImage;
    [field: SerializeField] public bool IsValid { get; set; }
    [field: SerializeField] public bool IsActive { get; private set; }

    [SerializeField] private LightManager _lightManager;

    private void Awake()
    {
        _lightManager = FindObjectOfType<LightManager>();
    }

    private void Start()
    {
        _markerImage.gameObject.SetActive(false);
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;
        IsValid = _lightManager.CheckValidSpot(this.gameObject, IsValid);
    }

    public void SetColor(Color newColor) //Use to set color 
    {
        //_markerImage.color = newColor;
        _groundMarkerImage.color = newColor;
    }

    public void SetPosition(Vector3 newPosition) //Use to move the canvas to a target position, activates the image
    {
        IsActive = true;
        transform.LookAt(transform.parent);
        var mask = LayerMask.GetMask("Player") | LayerMask.GetMask("UI") | LayerMask.GetMask("Ignore Raycast");
        Physics.Raycast(transform.position, Vector3.down, out var hit,  Mathf.Infinity, ~mask);
        Debug.Log(hit.transform);
        if (!_markerImage.gameObject.activeSelf)
        {
            _markerImage.gameObject.SetActive(true);
        }

        if (hit.transform != null) //moves the target marker to the hit point if the player is aiming at an object into the air.
        {
            _groundCanvas.position = hit.point;
            _groundMarkerImage.gameObject.SetActive(true);
            var difference = 1.7f - Vector3.Distance(newPosition, hit.point);
            transform.position = Vector3.Distance(newPosition, hit.point) < 1.7f
                ? newPosition + new Vector3(0, difference, 0) : newPosition; //moves the marker slightly higher if the player is aiming at the ground.
        }
        else //if the raycast returns no hit, don't show the ground marker and move the Standard teleport marker. 
        {
            _groundMarkerImage.gameObject.SetActive(false);
            transform.position = newPosition;
        }
    }

    public void ResetPosition() //resets position and deactivates the image
    {
        IsActive = false;
        transform.position = _startPosition;
        _groundCanvas.position = _startPosition;
        if (_markerImage.gameObject.activeSelf)
        {
            _markerImage.gameObject.SetActive(false);
            _groundMarkerImage.gameObject.SetActive(false);
        }
    }
}
