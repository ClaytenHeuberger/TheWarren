using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EnemyShipML : Agent
{

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject enemyBullet;

    [Header("Input Variables")]
    [SerializeField] private float pitchTorque = 3;
    [SerializeField] private float rollTorque = 9;
    [SerializeField] private float engineMaxPower = 100f;
    [SerializeField] private float engineTimeToMax = 4f;
    [SerializeField] private float engineStartPower = 10f;

    [Header("AI")]
    [SerializeField] private Transform player;

    [SerializeField] private float enemySightDist = 1000f;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float cooldown = 2f;
    //[SerializeField] private float maxAngularVelocity = 20f;
    [SerializeField] private float shootCutoffAngle = 0.2f;

    int enemySightMask = 1 << 7; //Asteroid layer

    Rigidbody rb;
    Rigidbody targetRB;

    float cooldownState = 0f;

    float enginePower = 0;

    float thrust = 0;
    float pitch = 0; // -1 down, +1 up
    float roll = 0; // -1 left, +1 right

    Vector3 startPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRB = target.GetComponent<Rigidbody>();
        startPos = transform.position;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        
        sensor.AddObservation(transform.position); //3
        sensor.AddObservation(transform.eulerAngles); //3
        sensor.AddObservation(rb.velocity); //3
        sensor.AddObservation(rb.angularVelocity); //3

        // Sight

        #region raycasting

        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;
        RaycastHit hit5;

        RaycastHit hit6;
        RaycastHit hit7;
        RaycastHit hit8;
        RaycastHit hit9;

        RaycastHit hit10;
        RaycastHit hit11;
        RaycastHit hit12;
        RaycastHit hit13;

        Physics.Raycast(transform.position, rb.velocity.normalized, out hit1, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*2 + transform.up, out hit2, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*2 + transform.right, out hit3, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*2 - transform.up, out hit4, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*2 - transform.right, out hit5, enemySightDist, enemySightMask);

        Physics.Raycast(transform.position, rb.velocity.normalized * 2 + transform.up + transform.right, out hit6, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * 2 + transform.up - transform.right, out hit7, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * 2 - transform.up + transform.right, out hit8, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * 2 - transform.up - transform.right, out hit9, enemySightDist, enemySightMask);

        Physics.Raycast(transform.position, rb.velocity.normalized*4 + transform.up, out hit10, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*4 + transform.right, out hit11, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*4 - transform.up, out hit12, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized*4 - transform.right, out hit13, enemySightDist, enemySightMask);


        Debug.DrawRay(transform.position, rb.velocity.normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized + transform.up).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized + transform.right).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized - transform.up).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized - transform.right).normalized * enemySightDist, Color.white);

        Debug.DrawRay(transform.position, (rb.velocity.normalized * 2 + transform.up + transform.right) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 2 + transform.up - transform.right) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 2 - transform.up + transform.right) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 2 - transform.up - transform.right) * enemySightDist, Color.white);

        Debug.DrawRay(transform.position, (rb.velocity.normalized * 4 + transform.up) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 4 + transform.right) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 4 - transform.up) * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * 4 - transform.right) * enemySightDist, Color.white);

        #endregion

        sensor.AddObservation(hit1.distance);
        sensor.AddObservation(hit2.distance);
        sensor.AddObservation(hit3.distance);
        sensor.AddObservation(hit4.distance);
        sensor.AddObservation(hit5.distance);
        sensor.AddObservation(hit6.distance);
        sensor.AddObservation(hit7.distance);
        sensor.AddObservation(hit8.distance);
        sensor.AddObservation(hit9.distance);
        sensor.AddObservation(hit10.distance);
        sensor.AddObservation(hit11.distance);
        sensor.AddObservation(hit12.distance);
        sensor.AddObservation(hit13.distance); //13 total

        sensor.AddObservation(target.transform.position); //3
        sensor.AddObservation(target.transform.eulerAngles); //3
        sensor.AddObservation(targetRB.velocity); //3
        sensor.AddObservation(targetRB.angularVelocity); //3
        
        //37 Total Observations

    }

    public override void OnEpisodeBegin()
    {
        transform.position = target.transform.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
    }
    public override void OnActionReceived(ActionBuffers actions)
    {

        // Thrust //

        thrust = actions.DiscreteActions[0];
        rb.AddForce(transform.forward * enginePower * Time.deltaTime * 100);


        if (thrust == 1)
        {
            if (enginePower <= 0)
                enginePower = engineStartPower;
            else if (enginePower < engineMaxPower)
                enginePower += Time.deltaTime * (engineMaxPower / engineTimeToMax);


        }
        else if (enginePower > 0)
        {
            enginePower -= Time.deltaTime * (engineMaxPower / engineTimeToMax) * 5;
        }
        if (enginePower < 0)
            enginePower = 0;

        // Pitch and Roll //
        pitch = actions.ContinuousActions[0] * 2f - 1f;
        roll = actions.ContinuousActions[1] * 2f - 1f;

        rb.AddTorque(pitch * pitchTorque * transform.right * Time.deltaTime * 100);
        rb.AddTorque(roll * rollTorque * transform.forward * Time.deltaTime * 100);


        // Shooting //
        if (actions.DiscreteActions[1] == 1)
        {
            Shoot();

            float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);

            if (angle < shootCutoffAngle)
            {
                AddReward(5 / (1 + angle));
            }
        }
        Debug.Log(actions.ContinuousActions[0] + "  " + actions.ContinuousActions[1]);

        // Other Rewards //

        //Reward for running when "player" is facing you (and vice versa)
        bool targetMovingTowardsThis = Vector3.Distance(target.transform.position + targetRB.velocity, transform.position) < Vector3.Distance(target.transform.position, transform.position);
        bool movingTowardsTarget = Vector3.Distance(transform.position + rb.velocity, target.transform.position) < Vector3.Distance(transform.position, target.transform.position);
        if (!targetMovingTowardsThis && movingTowardsTarget)
        {
            AddReward(0.1f);
        }
        if (targetMovingTowardsThis && !movingTowardsTarget)
        {
            AddReward(0.1f);
        }


        // End Episode

        if (Vector3.Distance(transform.position, target.transform.position) > 500)
        {
            EndEpisode();   
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        AddReward(-0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemyBullet")
        {
            AddReward(-1f);
        }
    }
    void Shoot()
    {
        if (cooldownState < 0)
        {
            GameObject newBullet = Instantiate(enemyBullet, transform.position + transform.forward * 3f, transform.rotation);
            float currentSpeed = rb.velocity.magnitude;
            newBullet.GetComponent<EnemyBullet>().speed = bulletSpeed + currentSpeed;

            cooldownState = cooldown;

        }

    }
}
