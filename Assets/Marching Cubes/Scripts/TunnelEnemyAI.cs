using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TunnelEnemyAI : MonoBehaviour
{
    [SerializeField] private float rollTorque = 30f;
    [SerializeField] private float pitchTorque = 30f;
    [SerializeField] private float thrust = 30f;
    [SerializeField] private float sightDistance = 30f;
    [SerializeField] private float correctionDistance = 10f;
    [SerializeField] private float correctionAngle = 10f;

    Vector3 targetPos;
    LayerMask mask;
    Rigidbody rb;

    List<RaycastHit> hits = new List<RaycastHit>();
    private void Start()
    {
        mask = LayerMask.GetMask("Cave");
        rb = GetComponent<Rigidbody>();
        targetPos = transform.position + transform.forward * 2f;
    }
    private void FixedUpdate()
    {
        // Raycasts //

        Physics.SphereCast(transform.position, 2f, rb.velocity.normalized, out RaycastHit forwardHit, sightDistance, mask);
        if (forwardHit.distance == 0f)
            forwardHit.distance = sightDistance;
        

        int ringCount = 12;
        int raysPerRing = 12;
        float verticalAngleRange = 45f;
        float longestDistance = correctionDistance;

        float velocityCorrectionDistance = correctionDistance * rb.velocity.magnitude / 20f + correctionDistance / 4f;

        bool avoidObstacle = false;
        bool cutEngine = false;

        Vector3 averagePos = Vector3.zero;
        int numObstacles = 0;

        for (int i = 0; i < ringCount; i++)
        {
            // Vertical angle (phi): from 0 (top) to verticalAngleRange
            float phi = Mathf.Lerp(0, verticalAngleRange, (float)i / (ringCount - 1)) * Mathf.Deg2Rad;

            float ringThetaOffset = (i % 2) * (360f / raysPerRing / 2f) * Mathf.Deg2Rad;

            for (int j = 0; j < raysPerRing; j++)
            {
                // Horizontal angle (theta): full 360° sweep
                float theta = ((float)j / raysPerRing) * 360f * Mathf.Deg2Rad + ringThetaOffset;

                // Spherical to Cartesian conversion
                Vector3 direction = new Vector3(
                    Mathf.Sin(phi) * Mathf.Cos(theta),
                    Mathf.Sin(phi) * Mathf.Sin(theta),
                    Mathf.Cos(phi)  // forward component
                );

                // Rotate direction to match transform orientation
                direction = transform.TransformDirection(direction);

                Ray ray = new Ray(transform.position, direction);
                Physics.Raycast(ray, out RaycastHit hit, sightDistance, mask);
                
                if(hit.distance == 0)
                    hit.distance = sightDistance;


                // update longest distance and target direction
                if (hit.distance > Vector3.Distance(transform.position, targetPos))
                {
                    longestDistance = hit.distance;
                    targetPos = transform.position + ray.direction * hit.distance;
                }



                // locate obstacles (closest distances) and set to avoid
                if ((i != 0 && j != 0) && Vector3.Angle(rb.velocity.normalized, hit.point - transform.position) < correctionAngle && hit.distance < velocityCorrectionDistance)
                {
                    averagePos += hit.point;
                    numObstacles++;

                }


                Debug.DrawRay(transform.position, direction * sightDistance, Color.cyan);
            }
        }


        if (numObstacles > 0)
        {
            averagePos /= (float)numObstacles;
            targetPos = averagePos;

            avoidObstacle = true;

            cutEngine = true;
        }

        Vector3 dirToTarget = (targetPos - transform.position).normalized;

        if(!avoidObstacle)
            Debug.DrawLine(transform.position, targetPos, Color.green);
        else
            Debug.DrawLine(transform.position, targetPos, Color.red);



        //------ Movement Logic -------//

        // Roll //

        float currentPitch = Vector3.Dot(transform.up, dirToTarget);
        float currentRoll = Vector3.Dot(transform.right, dirToTarget);

        bool topRight = (currentPitch > 0 && currentRoll > 0);
        bool topLeft = (currentPitch > 0 && currentRoll < 0);
        bool bottomRight = (currentPitch < 0 && currentRoll > 0);
        bool bottomLeft = (currentPitch < 0 && currentRoll < 0);


        float roll = 0;
        float pitch = 0;

        if (bottomLeft || topRight)
        {
            roll = -1;
        }
        if (topLeft || bottomRight)
        {
            roll = 1;
        }

        
        if (currentPitch < 0)
        {
            pitch = 1;
        }
        else if (currentPitch > 0)
        {
            pitch = -1;
        }

        // obstacle correction //
        if(avoidObstacle)
        {
            pitch *= -1;

        }

        if(avoidObstacle && Vector3.Angle(dirToTarget, rb.velocity.normalized) < 3f) // MAYDAY
        {
            roll = (Mathf.PerlinNoise(Time.time / 2f, 0f) - 0.5f) * 4;
            pitch = 1;

            if(rb.velocity.magnitude < 0.1f)
                cutEngine = true;
        }



        rb.AddTorque(pitch * pitchTorque * transform.right * Time.deltaTime);
        rb.AddTorque(roll * rollTorque * transform.forward * Time.deltaTime);

        if (!cutEngine)
            rb.AddForce(transform.forward * thrust * Time.deltaTime);


        float velMag = rb.velocity.magnitude;
        rb.velocity = transform.forward * velMag;
    }
}
