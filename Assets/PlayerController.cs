using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive drive;

    // Start is called before the first frame update
    void Start()
    {
        drive = GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");
        drive.Go(a, s, b);

        drive.CheckForSkid();

        drive.CalculateEngineSound();
    }
}
