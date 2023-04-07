using UnityEngine;

public class smoothRotation: PlayerLook
{
    [Header("Player Rotation Smooth Properties")] 
    [SerializeField] private float _smoothTime;
    [SerializeField] private Transform _horizontalRotationHelper;

    private float _oldVertical;
    private float _vertAngularVelocity;
    private float _horiAngularVelocity;

    private void Start()
    {
        _horizontalRotationHelper.localRotation = transform.rotation;
    }

    protected override void Rotate()
    {
        _oldVertical = _xRotation;
        base.Rotate();
    }

    protected override void RotateHorizontal()
    {
        _horizontalRotationHelper.Rotate(Vector3.up * GetHorizontalValue(),Space.Self);
        transform.localRotation = Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.localEulerAngles.y, _horizontalRotationHelper.localEulerAngles.y, ref _horiAngularVelocity, _smoothTime), 0f);
    }

    protected override void RotateVertical()
    {
        _xRotation = Mathf.SmoothDampAngle(_oldVertical, _xRotation, ref _vertAngularVelocity, _smoothTime);
        base.RotateVertical();
    }
}
