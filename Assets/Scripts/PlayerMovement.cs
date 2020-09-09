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

    void Start()
    {
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
}
