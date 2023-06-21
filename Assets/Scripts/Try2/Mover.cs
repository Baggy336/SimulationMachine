using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script interacts with the charactercontroller to move the object
/// </summary>
public class Mover : MonoBehaviour
{
    public CharacterController controller;

    // Currently only references the scavenger, needs to be modular for every type of creature
    private Scavenger scavenger;
    float rotationSpeed = 10f;

    /// <summary>
    /// Initialize the connection to necessary scripts
    /// </summary>
    private void Awake()
    {
        scavenger = GetComponent<Scavenger>();
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Called from the creature's main script. Handles movement
    /// </summary>
    /// <param name="FB"></param>
    /// <param name="LR"></param>
    public void Move(float FB, float LR)
    {
        LR = Mathf.Clamp(LR, -1, 1);
        FB = Mathf.Clamp(FB, 0, 1);

        if (!scavenger.isDead)
        {
            transform.Rotate(0, LR * rotationSpeed, 0);

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            controller.SimpleMove(forward * rotationSpeed * FB * -1);
        }
    }
}
