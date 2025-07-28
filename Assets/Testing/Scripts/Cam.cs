using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [Header("Variables")]
    public float followSpeed = 1.0f;
    public float rotSpeed = 1.0f;


    [SerializeField] Transform target;
    [SerializeField] Transform player;



    void FixedUpdate()
    {

        /*

        if(transform.parent.GetComponent<PlayerScript>().useJoystick)
        {
            Vector2 joystickRot = transform.parent.GetComponent<PlayerScript>().joystickRot;

            targRot += new Vector3(joystickRot.y * lagMagnitude * rotMultiplier, 0, joystickRot.x * lagMagnitude * 15);
            targPos += new Vector3(0, joystickRot.y * lagMagnitude * 1.5f, 0);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
                targPos += new Vector3(0, 0, -lagMagnitude * 2);

            if (Input.GetKey(KeyCode.A))
                targRot += new Vector3(0, 0, lagMagnitude * rotMultiplier * 2);
            if (Input.GetKey(KeyCode.D))
                targRot += new Vector3(0, 0, -lagMagnitude * rotMultiplier * 2);


            if (Input.GetKey(KeyCode.W))
            {
                targRot += new Vector3(-lagMagnitude * rotMultiplier, 0, 0);
                targPos += new Vector3(0, -lagMagnitude * 1.5f, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                targRot += new Vector3(lagMagnitude * rotMultiplier, 0, 0);
                targPos += new Vector3(0, lagMagnitude * 1.5f, 0);

            }
        }
        */

        Vector3 targetPos = target.position + transform.up * player.GetComponent<Rigidbody>().velocity.x * 10f;
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed);

        Quaternion targetRot = Quaternion.LookRotation((player.position + player.forward * 10000f) - transform.position, player.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotSpeed);
    }
}
