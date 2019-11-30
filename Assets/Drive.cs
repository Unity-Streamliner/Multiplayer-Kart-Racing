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
    public AudioSource highAccel;
    public Transform SkidTrailPrefab;
    public ParticleSystem smokePrefab;

    public GameObject brakeLight;
    public Rigidbody rb;
    public float gearLength = 3;
    public float currentSpeed {
        get { return rb.velocity.magnitude * gearLength; }
    }
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPercentage;
    public float maxSpeed = 200;

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
        brakeLight.SetActive(false);
    }

    void CalculateEngineSound()
    {
        float gearPercentage = 1 / (float) numGears;
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed/maxSpeed));
        currentGearPercentage = Mathf.Lerp(currentGearPercentage, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float) numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPercentage);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float) numGears) * (currentGear + 1);
        float downGearMax = (1 / (float) numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
        {
            currentGear--;
        } 
        if (speedPercentage > upperGearMax && currentGear < numGears - 1)
        {
            currentGear++;
        }
        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");
        Go(a, s, b);

        CheckForSkid();

        CalculateEngineSound();
    }

    void Go(float accel, float steer, float brake) 
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, -1, 1) * maxBrakeTorque;
        if (brake != 0) brakeLight.SetActive(true);
        else brakeLight.SetActive(false);
        float thrustTorque = 0;
        if (thrustTorque < maxSpeed)
        {
            thrustTorque = accel * torque;
        } 
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
