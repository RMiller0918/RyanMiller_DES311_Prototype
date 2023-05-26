using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookATCam : MonoBehaviour
{
    private Transform _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = this.transform.position;
        var camPos = _camera.position;

        pos = LockXZaxis(pos);
        camPos = LockXZaxis(camPos);

        this.transform.rotation = Quaternion.LookRotation(camPos - pos);
    }
    //Rotates an object to look at the camera.
    private Vector3 LockXZaxis(Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }
}
