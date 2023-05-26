using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationScript : MonoBehaviour
{
    
    [SerializeField] [Range(0,90)] private int _angleX;
    private float _angleY;
    [SerializeField] [Range(0, 20)] private int _speed;

    private void Start()
    {
        _angleY = 0;
    }

    //rotates an object around the Y axis. X-axis rotation is determined through the Angle X value. was used on spotlights.
    void Update()
    {
        _angleY = AngleClamp(_angleY, _speed);
        transform.rotation = Quaternion.Euler(_angleX, _angleY, 0);
    }

    private static float AngleClamp(float angle, int speed) //returns a clamped rotation angle.
    {
        angle = angle == 180 ? -180 : angle += speed * Time.deltaTime;
        angle = Mathf.Clamp(angle, -180, 180);
        return angle;
    }
}
