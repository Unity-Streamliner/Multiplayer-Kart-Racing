using UnityEngine;

class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive drive;
    public float steetingSensitivity = 0.01f;
    Vector3 target;
    int currentWaypoint = 0;

    private void Start()
    {
        drive = GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position ;
    }

    private void Update()
    {
        Vector3 localTarget = drive.rb.gameObject.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, drive.rb.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg; // Mathf.Rad2Deg convert to deg

        float steer = Mathf.Clamp(targetAngle * steetingSensitivity, -1, 1) * Mathf.Abs(drive.currentSpeed);
        float accel = 0.5f;
        float brake = 0;

        if (distanceToTarget < 5)
        {
            brake = 0.8f;
            accel = 0.1f; 
        }

        drive.Go(accel, steer, brake);

        if (distanceToTarget < 2) // treshold, make larger if car starts to circle waypoint  
        {
            currentWaypoint++;
            if (currentWaypoint >= circuit.waypoints.Length) currentWaypoint = 0;
            target = circuit.waypoints[currentWaypoint].transform.position;
        }

        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}