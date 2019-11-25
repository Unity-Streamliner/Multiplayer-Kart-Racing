using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider WheelCollider;
    public float torque = 200;
    // Start is called before the first frame update
    void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        Go(a);
    }

    void Go(float accel) 
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * torque;
        WheelCollider.motorTorque = thrustTorque;

        Quaternion quaternion;
        Vector3 position;
        WheelCollider.GetWorldPose(out position, out quaternion);
        transform.position = position;
        transform.rotation = quaternion;
    }
}
