using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float bulletShakeDuration = 0.3f;
    [SerializeField] private float bulletShakeMagnitude = 0.2f;
    [SerializeField] private float bulletSize = 1f;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform Cam;
    [SerializeField] private GameHandler gameHandler;

    float time = 0;

    AudioManager audioManager;


    JoystickControls controls;
    bool joystickShoot = false;

    private void Awake()
    {
        controls = new JoystickControls();
    }
    void Start()
    {
        time = cooldown;
        gameHandler = FindObjectOfType<GameHandler>();
        audioManager = FindObjectOfType<AudioManager>();
        Cam = Camera.main.transform;
    }

    void Update()
    {
        controls.Gameplay.Shoot.performed += ctx => joystickShoot = true;
        controls.Gameplay.Shoot.canceled += ctx => joystickShoot = false;

        if ((Input.GetMouseButton(0) || joystickShoot) && time <= 0 && (gameHandler == null || gameHandler.started))
        {
            Shoot();
            time = cooldown;
            audioManager.Play("Shoot");

            StartCoroutine(Cam.gameObject.GetComponent<CameraShake>().Shake(bulletShakeDuration, bulletShakeMagnitude));
        }

        if(time > 0)
        {
            time -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(Bullet, transform.position, transform.rotation);
        newBullet.transform.localScale = Vector3.one * bulletSize;
        newBullet.GetComponent<BulletScript>().audioManager = audioManager;
        float currentSpeed = Player.GetComponent<Rigidbody>().velocity.magnitude;
        newBullet.GetComponent<BulletScript>().speed = bulletSpeed + currentSpeed;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
