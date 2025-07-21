using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public float torqueGain = 50f;      // Proportional gain
    public float damping = 10f;         // Derivative gain (damping)
    public float maxTorque = 100f;      // Clamp torque

    private float lastError = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // STEP 1: Direction to target
        Vector3 toTarget = target.position - transform.position;
        Vector3 localDir = transform.InverseTransformDirection(toTarget.normalized);

        // STEP 2: Desired pitch angle (in degrees)
        float targetPitch = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

        // STEP 3: Current pitch angle
        float currentPitch = NormalizeAngle(transform.localEulerAngles.x);

        // STEP 4: PID error
        float error = targetPitch - currentPitch;
        float derivative = (error - lastError) / Time.fixedDeltaTime;
        lastError = error;

        // STEP 5: PID output = torque
        float torqueValue = (error * torqueGain) - (derivative * damping);
        torqueValue = Mathf.Clamp(torqueValue, -maxTorque, maxTorque);

        // STEP 6: Apply torque on local X-axis
        Vector3 torque = transform.right * torqueValue;
        rb.AddTorque(torque);
    }

    // Utility to get angle in -180 to 180 range
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
