using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Configuration")]
    public float movementThreshold = 10;

    [Header("Other Assets")]
    public Transform waterPostProcess;

    Vector3 lastPosition;
    float movementThresholdSquare;

    void Awake()
    {
        lastPosition = transform.position;
        movementThresholdSquare = movementThreshold * movementThreshold;
    }

    void Start()
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
    }

    void Update()
    {
        if ((transform.position - lastPosition).sqrMagnitude >= movementThresholdSquare)
        {
            lastPosition = transform.position;

            // Control the water post processing effect
            if (transform.position.y < 0)
            {
                waterPostProcess.position = transform.position;
            }
            else
            {
                waterPostProcess.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
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
