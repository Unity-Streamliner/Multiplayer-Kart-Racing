using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public List<WheelCollider> WheelColliders;
    public float torque = 200;
    public float maxSteerAngle = 30;
    public float maxBrakeTorque = 500;
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
        float b = Input.GetAxis("Jump");
        Go(a, s, b);
    }

    void Go(float accel, float steer, float brake) 
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, -1, 1) * maxBrakeTorque;
        float thrustTorque = accel * torque;
        for (int i = 0; i < 4; i++) 
        {
            WheelColliders[i].motorTorque = thrustTorque;
            if (i == 0 || i == 3)
                WheelColliders[i].steerAngle = steer;
            else 
                WheelColliders[i].brakeTorque = brake;
            Quaternion quaternion;
            Vector3 position;

            WheelColliders[i].GetWorldPose(out position, out quaternion);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quaternion;
        }
    }
}
