using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : MonoBehaviour
{
    // Reference to the moving script, and neural network
    public Network net;
    public Mover move;

    // The object to replicate at reproduction
    public GameObject scavenger;

    // Starting energy, and energy gained when a resource is gathered
    public float energy = 10;
    public float energyGained = 15;

    // Starting reproduction energy, energy gained per resource, and the amount of energy needed to reproduce
    public float reproEnergy = 0;
    public float reproEnergyGained = 1;
    public float reproEnergyThreshold = 100;

    // Modifiers to pass into the mutation function of the network
    public float mutateAmount = .2f;
    public float mutateChance = .2f;

    // Timer to count down lifespan
    public float elapsedTime = 0;

    // Size of the object to instantiate
    public float size = 1f;

    // Forward backward, and left to right movement
    public float FB = 0;
    public float LR = 0;

    // How far the raycasts go for detection
    public float FOV = 10;

    // Unused currently, amount of spawned children for this parent object
    public int numChildren = 0;

    // Activated after the timer has expired
    public bool isDead = false;

    // Mutate the network once at the start of the FixedUpdate
    public bool doMutate = true;
    public bool isMutated = false;

    void OnAwake()
    {
        net = gameObject.GetComponent<Network>();
        move = gameObject.GetComponent<Mover>();

        this.name = "scavenger";
    }

    /// <summary>
    /// This function interacts with the resource objects on the ground
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // Check to make sure the object is a resource
        if(other.gameObject.tag == "resource")
        {
            energy += energyGained;
            reproEnergy += reproEnergyGained;

            Destroy(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Mutate once at the first frame
        if (!isMutated)
        {
            MutateScavenger();
            this.transform.localScale = new Vector3(size, size, size);
            isMutated = true;
            energy = 20;
        }

        ManageEnergy();
        
        // Set the inputs to the neural network based on the raycast results
        float[] inputsToNet = ManageSense(8, 360 / 8);

        // Get the FrontBack and LeftRight values from the neural network based on the raycast results
        float[] outputsFromNet = net.GetNetworkOutputs(inputsToNet);
        // Set the collected values from the network's 2 outputs
        FB = outputsFromNet[0];
        LR = outputsFromNet[1];

        // Use the outputs to perform actions
        move.Move(FB, LR);
    }

    /// <summary>
    /// The numRays input of this function is the input layer int for the neural network
    /// Calculates distances to resources based on raycasts
    /// </summary>
    /// <param name="numRays"></param>
    /// <param name="angleBetween"> 360 / numRays gives every direction </param>
    /// <returns></returns>
    float[] ManageSense(int numRays, float angleBetween)
    {
        //Distances ultimately gets passed to the Network
        float[] distances = new float[numRays];

        RaycastHit hit;
        for(int i = 0; i < numRays; i++)
        {
            // Calculation for the ray angles, rotation, direction
            float angle = ((2 * i + 1 - numRays) * angleBetween / 2);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * transform.forward * -1;
            Vector3 rayStart = transform.position + Vector3.up * 0.1f;

            // Cast a ray from the results, ending at the scavenger's field of view
            if(Physics.Raycast(rayStart, dir, out hit, FOV))
            {
                Debug.DrawRay(rayStart, dir * hit.distance, Color.green);
                if(hit.transform.gameObject.tag == "resource")
                {
                    distances[i] = hit.distance;
                }
                else
                {
                    distances[i] = FOV;
                }
            }
            else
            {
                Debug.DrawRay(rayStart, dir * FOV, Color.red);

                distances[i] = 1;
            }
        }
        // Distances array is passed as the function result to the network
        return (distances);
    }

    /// <summary>
    /// Handles the amount of time this scavenger has to survive, and reproduction
    /// </summary>
    void ManageEnergy()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= 1f)
        {
            elapsedTime = elapsedTime % 1f;

            energy -= 1f;
        }

        float agentY = this.transform.position.y;
        if(energy <= 0 || agentY < -10)
        {
            this.transform.Rotate(0, 0, 180);
            Destroy(gameObject, 3);
            GetComponent<Mover>().enabled = false;
        }

        if(reproEnergy >= reproEnergyThreshold)
        {
            reproEnergy = 0;
            Repro();
        }
    }

    /// <summary>
    /// Handles the mutation values to be passed to the network, set by the public variables mutateAmount and Chance
    /// </summary>
    void MutateScavenger()
    {
        if (doMutate)
        {
            mutateAmount += Random.Range(-1.0f, 1.0f) / 100;
            mutateChance += Random.Range(-1.0f, 1.0f) / 100;
        }

        mutateAmount = Mathf.Max(mutateAmount, 0);
        mutateChance = Mathf.Max(mutateChance, 0);

        // Initialize the mutation
        net.MutateNet(mutateAmount, mutateChance);
    }

    /// <summary>
    /// Instantiates a child of the scavenger, with a copy of the parent's network
    /// </summary>
    void Repro()
    { 
        for(int i = 0; i <= numChildren; i++)
        {
            GameObject child = Instantiate(scavenger, new Vector3((float)this.transform.position.x + Random.Range(-10, 11), 0.75f, (float)this.transform.position.z + Random.Range(-10, 11)), Quaternion.identity);

            child.GetComponent<Network>().layers = GetComponent<Network>().CastNet();
        }
        reproEnergy = 0;
    }

}
