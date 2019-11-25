using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public List<WheelCollider> WheelColliders;
    public float torque = 200;
    public float maxSteerAngle = 30;
    public GameObject[] Wheels;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        Go(a, s);
    }

    void Go(float accel, float steer) 
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        float thrustTorque = accel * torque;
        for (int i = 0; i < 4; i++) 
        {
            WheelColliders[i].motorTorque = thrustTorque;
            if (i == 0 || i == 3)
                WheelColliders[i].steerAngle = steer;

            Quaternion quaternion;
            Vector3 position;

            WheelColliders[i].GetWorldPose(out position, out quaternion);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quaternion;
        }
    }
}
