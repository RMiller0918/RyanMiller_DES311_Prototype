using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMarker : MonoBehaviour
{
    [SerializeField] private Image _markerImage;
    [SerializeField] private Vector3 _startPosition;
    [field: SerializeField] public bool IsValid { get; set; }
    [field: SerializeField] public bool IsActive { get; private set; }

    [SerializeField] private LightManager _lightManager;

    private void Awake()
    {
        _markerImage = GetComponentInChildren<Image>();
        _markerImage.gameObject.SetActive(false);
        _lightManager = FindObjectOfType<LightManager>();

    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;
        IsValid = _lightManager.CheckValidSpot(this.gameObject, IsValid);
    }

    public void SetColor(Color newColor) //Use to set color 
    {
        _markerImage.color = newColor;
    }

    public void SetPosition(Vector3 newPosition) //Use to move the canvas to a target position, activates the image
    {
        IsActive = true;
        transform.position = newPosition;
        if(!_markerImage.gameObject.activeSelf)
            _markerImage.gameObject.SetActive(true);
    }

    public void ResetPosition() //resets position and deactivates the image
    {
        IsActive = false;
        transform.position = _startPosition;
        if (_markerImage.gameObject.activeSelf)
            _markerImage.gameObject.SetActive(false);
    }
}
