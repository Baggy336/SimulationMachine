using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public CharacterController controller;
    private Scavenger scavenger;
    float rotationSpeed = 10f;

    private void Awake()
    {
        scavenger = GetComponent<Scavenger>();
        controller = GetComponent<CharacterController>();
    }

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
