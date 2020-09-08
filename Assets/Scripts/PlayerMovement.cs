using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 7.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    float jumpVelocity;

    Vector3 velocity;
    bool isGrounded;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Player.Y"))
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("Player.X"),
                PlayerPrefs.GetFloat("Player.Y"),
                PlayerPrefs.GetFloat("Player.Z")
            );

            transform.rotation = Quaternion.Euler(
                PlayerPrefs.GetFloat("Player.RX"),
                PlayerPrefs.GetFloat("Player.RY"),
                PlayerPrefs.GetFloat("Player.RZ")
            );
        }
        else
        {
            // Set starting height
            RaycastHit raycastHit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out raycastHit, 1001f))
            {
                transform.position = raycastHit.point + Vector3.up * 2;
            }
        }

        jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        if (controller.isGrounded && velocity.y < 0)
        {
            //if (velocity.y < -22)
            //{
            //    combat.CmdDamage((int)velocity.y + 22);
            //}
            velocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpVelocity;
        }

        //if (Input.GetButton("Jump"))
        //if (player.playerAttributes.moonJump && MPNetworkManager.inputManager.GetButton("Jump"))
        //{
        //    if (!isMoonJumping)
        //    {
        //        isMoonJumping = true;
        //        gravity /= 3;
        //    }
        //}
        //else
        //{
        //    if (isMoonJumping)
        //    {
        //        isMoonJumping = false;
        //        gravity *= 3;
        //    }
        //}

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat("Player.X", transform.position.x);
        PlayerPrefs.SetFloat("Player.Y", transform.position.y);
        PlayerPrefs.SetFloat("Player.Z", transform.position.z);

        PlayerPrefs.SetFloat("Player.RX", transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("Player.RY", transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("Player.RZ", transform.rotation.eulerAngles.z);
    }
}
