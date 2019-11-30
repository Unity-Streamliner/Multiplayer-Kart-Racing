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
    public AudioSource skidSound;
    public Transform SkidTrailPrefab;
    public ParticleSystem smokePrefab;

    Transform[] skidTrails = new Transform[4];
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }
        skidTrails[i].parent = WheelColliders[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WheelColliders[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null) return;
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");
        Go(a, s, b);

        CheckForSkid();
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

    void CheckForSkid ()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++) 
        {
            WheelHit wheelHit;
            WheelColliders[i].GetGroundHit(out wheelHit);
            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if (!skidSound.isPlaying) 
                {
                    skidSound.Play();
                }
                skidSmoke[i].transform.position = WheelColliders[i].transform.position - WheelColliders[i].transform.up * WheelColliders[i].radius;
                skidSmoke[i].Emit(1);
                //StartSkidTrail(i);
            } else {
                skidSmoke[i].Stop();
                //EndSkidTrail(i); 
            }
        }
        if (numSkidding == 0 && skidSound.isPlaying) 
        {
            skidSound.Stop();
        }
    }
}
