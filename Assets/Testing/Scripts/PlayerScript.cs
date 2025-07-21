using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    [Header("Input Variables")]
    public bool useJoystick = false;
    [SerializeField] private float pitchTorque = 3;
    [SerializeField] private float rollTorque = 9;
    [SerializeField] private float engineMaxPower = 400;
    [SerializeField] private float engineTimeToMax = 4f;
    [SerializeField] private float engineStartPower = 10f;

    [Header("Object References")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject ping;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameHandler gameHandler;


    [Header("Effects")]
    [SerializeField] private float FOVDistortionMagnitude = 20f;
    [SerializeField] private float FOVDistortionRate = 0.1f;

    JoystickControls controls;
    Vector2 joystickInput;
    [HideInInspector]
    public bool joystickThrust = false;
    [HideInInspector]
    public Vector2 joystickRot = Vector2.zero;

    float enginePower = 0;
    Rigidbody rb;

    float FOVStart;
    [SerializeField] public float thrustRatio = 1;

    private void Awake()
    {
        controls = new JoystickControls();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameHandler = FindObjectOfType<GameHandler>(); 
        FOVStart = cam.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        
        //Movement
        rb.AddForce(transform.forward * enginePower * Time.deltaTime * 100);

        thrustRatio = enginePower / (engineMaxPower - engineStartPower);


        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOVStart + thrustRatio * FOVDistortionMagnitude, FOVDistortionRate);

        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Space) || joystickThrust)
        {
            if (gameHandler != null)
            {
                if (!gameHandler.started)
                {
                    gameHandler.started = true;
                    gameHandler.colorOverlay.enabled = false;
                    gameHandler.promptText.gameObject.SetActive(false);
                }
            }

            if (enginePower <= 0)
                enginePower = engineStartPower;
            else if (enginePower < engineMaxPower)
                enginePower += Time.deltaTime * (engineMaxPower / engineTimeToMax);


        }
        else if(enginePower > 0)
        {
            enginePower -= Time.deltaTime * (engineMaxPower / engineTimeToMax) * 5;
        }
        if (enginePower < 0)
            enginePower = 0;

        // Input for Pitch/Roll

        if(useJoystick)
        {
            //Thrust
            controls.Gameplay.Thrust.performed += ctx => joystickThrust = true;
            controls.Gameplay.Thrust.canceled += ctx => joystickThrust = false;

            //Pitch and Roll
            controls.Gameplay.PitchRoll.performed += ctx => joystickInput = ctx.ReadValue<Vector2>();
            controls.Gameplay.PitchRoll.canceled += ctx => joystickInput = Vector2.zero;



            //Roll remap
            if(joystickInput.x > 0 && joystickInput.x < Mathf.Sqrt(2)/2)
            {
                joystickRot.x = Mathf.Pow((Mathf.Sqrt(2)/2 - joystickInput.x) * 2 / Mathf.Sqrt(2), 2);
            }else if(joystickInput.x < 0 && joystickInput.x > -Mathf.Sqrt(2) / 2)
            {
                joystickRot.x = -Mathf.Pow((-Mathf.Sqrt(2) / 2 - joystickInput.x) * 2 / Mathf.Sqrt(2), 2);
            }


            //Pitch Remap
            if (joystickInput.y > 0 && joystickInput.y < Mathf.Sqrt(2) / 2)
            {
                joystickRot.y = Mathf.Pow((Mathf.Sqrt(2) / 2 - joystickInput.y) * 2 / Mathf.Sqrt(2), 2);
            }
            else if (joystickInput.y < 0 && joystickInput.y > -Mathf.Sqrt(2) / 2)
            {
                joystickRot.y = -Mathf.Pow((-Mathf.Sqrt(2) / 2 - joystickInput.y) * 2 / Mathf.Sqrt(2), 2);
            }

            if (gameHandler == null || gameHandler.started)
            {
                rb.AddTorque(-joystickRot.y * transform.right * pitchTorque * Time.deltaTime * 100);

                rb.AddTorque(joystickRot.x * transform.forward * rollTorque * Time.deltaTime * 100);
                rb.AddTorque(-joystickRot.x * transform.up * rollTorque * Time.deltaTime * 20);
            }

            //Debug.Log(joystickRot.x + "  " + joystickRot.y);
        }
        else if(gameHandler == null || gameHandler.started)
        {

            if (Input.GetKey("w"))
            {
                rb.AddTorque(transform.right * pitchTorque * Time.deltaTime * 100);

            }
            if (Input.GetKey("s"))
            {
                rb.AddTorque(-transform.right * pitchTorque * Time.deltaTime * 100);
            }

            if (Input.GetKey("a"))
            {
                rb.AddTorque(transform.forward * rollTorque * Time.deltaTime * 100);
                rb.AddTorque(-transform.up * rollTorque * Time.deltaTime * 20);
            }
            if (Input.GetKey("d"))
            {
                rb.AddTorque(-transform.forward * rollTorque * Time.deltaTime * 100);
                rb.AddTorque(transform.up * rollTorque * Time.deltaTime * 20);

            }

        }

        rb.velocity = transform.forward * rb.velocity.magnitude;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Instantiate(ping, transform.position, Quaternion.identity);
    //    }

    //}


    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "PathMaker")
        {
            other.GetComponent<PathScript>().moveForward();
            other.GetComponent<PathScript>().moveForward();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Asteroid")
        {

            if (Vector3.Angle(collision.transform.position - transform.position, rb.velocity) > 60)
            {
                int rand = UnityEngine.Random.Range(0, 2);
                
                if (rand == 0)
                {
                    audioManager.Play("Scrape1");
                    //Debug.Log("Scrape! 1 " + rand);

                }
                else
                {
                    audioManager.Play("Scrape2");
                    //Debug.Log("Scrape! 2 " + rand);

                }


            }
            else
            {
                //Debug.Log("Crash!");
                audioManager.Play("Crash1");
            }
        }
    }
}
