using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShip : MonoBehaviour
{

    [Header("Input Variables")]
    [SerializeField] private float pitchTorque = 3;
    [SerializeField] private float rollTorque = 9;
    [SerializeField] private float engineMaxPowerChasing = 400;
    [SerializeField] private float engineMaxPowerFighting = 600;
    [SerializeField] private float engineTimeToMax = 4f;
    [SerializeField] private float engineStartPower = 10f;

    [Header("Object References")]
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] private GameObject enemyMarker;
    [SerializeField] private GameObject[] explosions;
    [HideInInspector]
    public EnemyHandler enemyHandler;

    [Header("AI")]
    public Transform player;

    [SerializeField] private float enemySightDist = 1000f;
    [SerializeField] private float slowDist = 100f;
    [SerializeField] private float shootCutoffAngle = 5f;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private float loopDistance = 500f;
    [SerializeField] private float retreatDistance = 100f;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float maxAngularVelocity = 20f;
    [SerializeField]


    AudioManager audioManager;

    int enemySightMask = 1 << 7; //Asteroid layer
    float enginePower = 0;
    Rigidbody rb;
    Rigidbody playerRB;
    float FOVStart;
    public float thrustRatio = 1;

    float cooldownState = 0f;

    //AI variable Placeholders
    float thrust = 0;
    float pitch = 0; // -1 down, +1 up
    float roll = 0; // -1 left, +1 right

    bool striking = true;

    GameObject marker;

    float engineMaxPower;

    PIDController PIDController;

    Vector3 targetPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerRB = player.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        PIDController = GetComponent<PIDController>();
        audioManager = FindObjectOfType<AudioManager>();

        marker = Instantiate(enemyMarker);

        marker.GetComponent<EnemyMarker>().target = transform;

        engineMaxPower = engineMaxPowerChasing;

    }

    void FixedUpdate()
    {

//------ Decisions -------//



        thrust = 1;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        //Target Logic
        if (distToPlayer > loopDistance)
            striking = true;

        if (distToPlayer < retreatDistance)
            striking = false;


        if (striking)
            targetPos = player.transform.position + (playerRB.velocity * distToPlayer * Time.deltaTime) / bulletSpeed;
        else
            targetPos = (transform.position - player.transform.position).normalized * loopDistance;

        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        Debug.DrawLine(transform.position, targetPos, Color.green);
        Debug.DrawLine(transform.position, player.position, Color.green);

//------ Movement Logic -------//



        Debug.DrawLine(transform.position, transform.position + transform.forward * 10f, Color.yellow);

        // Roll //

        float currentPitch = Vector3.Dot(transform.up, dirToTarget);
        float currentRoll = Vector3.Dot(transform.right, dirToTarget);

        bool topRight = (currentPitch > 0 && currentRoll > 0);
        bool topLeft = (currentPitch > 0 && currentRoll < 0);
        bool bottomRight = (currentPitch < 0 && currentRoll > 0);
        bool bottomLeft = (currentPitch < 0 && currentRoll < 0);


        roll = 0;
        pitch = 0;

        if (bottomLeft || topRight)
        {
            roll = -1;
        }
        if(topLeft || bottomRight)
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

        //roll = PIDController.Calculate(Time.fixedDeltaTime, transform.eulerAngles.z, targetRoll, rb.angularVelocity.z, roll_P, roll_D);


        // Thrust //


        if (distToPlayer > loopDistance)
        {
            engineMaxPower = engineMaxPowerChasing;
        }
        else
        {
            engineMaxPower = engineMaxPowerFighting;
        }
        RaycastHit hitForward;

        Physics.SphereCast(transform.position, 8f, transform.forward, out hitForward, enemySightDist, enemySightMask);
        float distForward = hitForward.distance;

        if(distForward == 0)
            distForward = enemySightDist;

        Debug.DrawLine(transform.position, transform.position + transform.forward * slowDist * rb.velocity.magnitude, Color.red);

        float facingFactor = Vector3.Dot(transform.forward, dirToTarget);

        if (distForward < slowDist * rb.velocity.magnitude || (facingFactor < -0.8f && striking))
        {
            thrust = 0f;
        }


        // Obstacle Correction //


        float narrowness = 0.2f + rb.velocity.magnitude / 10f;

        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;
        RaycastHit hit5;

        Physics.Raycast(transform.position, rb.velocity.normalized, out hit1, enemySightDist, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * narrowness + transform.up, out hit2, enemySightDist + rb.velocity.magnitude, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * narrowness + transform.right, out hit3, enemySightDist + rb.velocity.magnitude, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * narrowness - transform.up, out hit4, enemySightDist + rb.velocity.magnitude, enemySightMask);
        Physics.Raycast(transform.position, rb.velocity.normalized * narrowness - transform.right, out hit5, enemySightDist + rb.velocity.magnitude, enemySightMask);

        Debug.DrawRay(transform.position, rb.velocity.normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * narrowness + transform.up).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * narrowness + transform.right).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * narrowness - transform.up).normalized * enemySightDist, Color.white);
        Debug.DrawRay(transform.position, (rb.velocity.normalized * narrowness - transform.right).normalized * enemySightDist, Color.white);

        float veldistForward = hit1.distance;
        float veldistUp = hit2.distance;
        float veldistRight = hit3.distance;
        float veldistDown = hit4.distance;
        float veldistLeft = hit5.distance;

        if (veldistForward == 0)
            veldistForward = enemySightDist;
        if (veldistUp == 0)
            veldistUp = enemySightDist;
        if (veldistRight == 0)
            veldistRight = enemySightDist;
        if (veldistDown == 0)
            veldistDown = enemySightDist;
        if (veldistLeft == 0)
            veldistLeft = enemySightDist;

        Debug.DrawLine(transform.position, transform.position + rb.velocity.normalized * dodgeDistance * rb.velocity.magnitude, Color.magenta);

        RaycastHit sphereForward;
        Physics.SphereCast(transform.position, 8f, rb.velocity.normalized, out sphereForward, enemySightDist, enemySightMask);
        float sphereForwardDist = sphereForward.distance;
        if(sphereForwardDist == 0)
            sphereForwardDist = enemySightDist;

        if(sphereForwardDist < dodgeDistance * (rb.velocity.magnitude + 5f))
        {
            // if obstacle is top right or bottom left
            if ((veldistRight < veldistLeft && veldistUp < veldistDown) || (veldistLeft < veldistRight && veldistDown < veldistUp))
            {
                roll = -1;
            }
            // if obstacle is top left or bottom right
            if ((veldistLeft < veldistRight && veldistUp < veldistDown) || (veldistRight < veldistLeft && veldistDown < veldistUp))
            {
                roll = 1;
            }

            if (veldistUp < veldistDown)
            {
                pitch = 1;
            }
            if (veldistDown < veldistUp)
            {
                pitch = -1;
            }

            if(veldistDown == enemySightDist && veldistUp == enemySightDist)
            {
                if (veldistRight < veldistDown)
                    roll = -1;
                else
                    roll = 1;
            }

            //If object is out of sight, roll to find it
            if(veldistDown == enemySightDist && veldistUp == enemySightDist && veldistRight == enemySightDist && veldistLeft == enemySightDist)
            {
                roll = 1;
            }



        }

        //------ Shooting -------//

        if ((Vector3.Angle(dirToTarget, transform.forward) < shootCutoffAngle && facingFactor > 0f) && striking)
        {
            Shoot();
        }
        cooldownState -= Time.deltaTime;





        // Force / Torque Appication //
        rb.AddForce(transform.forward * enginePower * Time.deltaTime * 100);

        thrustRatio = enginePower / (engineMaxPower - engineStartPower);


        if (thrust == 1f)
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

        if(thrust == -1f)
        {
            enginePower = -engineStartPower * 1000f;
        }


        rb.AddTorque(pitch * pitchTorque * transform.right * Time.deltaTime * 100);
        rb.AddTorque(roll * rollTorque * transform.forward * Time.deltaTime * 100);


        rb.velocity *= 0.97f;
    }



    void Shoot()
    {
        if(cooldownState < 0)
        {
            GameObject newBullet = Instantiate(enemyBullet, transform.position + transform.forward * 3f, transform.rotation);
            newBullet.GetComponent<EnemyBullet>().audioManager = audioManager;
            float currentSpeed = rb.velocity.magnitude;
            newBullet.GetComponent<EnemyBullet>().speed = bulletSpeed + currentSpeed;

            cooldownState = cooldown;

            audioManager.PlayAtPos("EnemyShoot", transform.position, 0.01f, 1f);
        }
 
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bullet")
        {

            int index = UnityEngine.Random.Range(0, explosions.Length);
            GameObject newExplosion = Instantiate(explosions[index], transform.position, transform.rotation);


            Camera.main.GetComponent<CameraShake>().PrimeShake(0.5f, 0.7f);

            Destroy(marker);
            Destroy(collision.gameObject);

            gameObject.SetActive(false);

            enemyHandler.AddScore(15, Color.yellow);

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        rb.AddForce(-transform.forward * 1000 * Time.deltaTime * 100);
    }

}
