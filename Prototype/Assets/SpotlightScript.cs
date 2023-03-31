using TMPro;
using UnityEngine;

public class SpotlightScript : MonoBehaviour
{
    private Transform _transform;
    private Light _light;
    private float _outsideAngle;
    private Vector3 _angleAxisUp;
    private float _angleToObject;
    private bool _inCone;
    private void Awake()
    {
        _transform = transform;
        _light = GetComponent<Light>();
    }

    void Start()
    {
        _outsideAngle = _light.spotAngle;
    }

    void Update()
    {
        _outsideAngle = _light.spotAngle / 2;
        _angleAxisUp = new Vector3(0, _transform.rotation.y, 0);
        DrawRays();
    }

    private void DrawRays()
    {
        var Ray = new Ray();
        Ray.origin = _transform.position;
        Ray.direction = (Quaternion.AngleAxis(_outsideAngle, _transform.up) * transform.forward) * _light.range;
        Debug.DrawLine(Ray.origin, Ray.origin + Ray.direction * _light.range, Color.green);
        Ray.direction = (Quaternion.AngleAxis(-_outsideAngle, _transform.up) * transform.forward) * _light.range;
        Debug.DrawLine(Ray.origin, Ray.origin + Ray.direction * _light.range, Color.green);
    }

    private bool CalculateDot(Transform oTransform)
    {
        var cosAngle = Vector3.Dot(
            (oTransform.position - _transform.position).normalized, _transform.forward);
        var angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        Debug.Log(angle < _outsideAngle);
        return angle < _outsideAngle;
    }

    private void OnTriggerEnter(Collider oCollider)
    {
        CalculateDot(oCollider.transform);
    }

    private void OnTriggerStay(Collider oCollider)
    {
        CalculateDot(oCollider.transform);
    }

    private void OnTriggerExit()
    {

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
}
 