using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationScript : MonoBehaviour
{
    private float _angleX;
    private float _angleY;

    private void Start()
    {
        _angleX = 45;
        _angleY = 0;
    }
    void Update()
    {
        _angleY = AngleClamp(_angleY);
        transform.eulerAngles = new Vector3(_angleX, _angleY, 0);
    }

    private static float AngleClamp(float angle)
    {
        angle = angle == 180 ? -180 : angle += 20f * Time.deltaTime;
        angle = Mathf.Clamp(angle, -180, 180);
        return angle;
    }
}
