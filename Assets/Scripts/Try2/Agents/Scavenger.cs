using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : MonoBehaviour
{
    public Network net;
    public Mover move;

    public GameObject scavenger;

    public float energy = 10;
    public float energyGained = 15;

    public float reproEnergy = 0;
    public float reproEnergyGained = 1;
    public float reproEnergyThreshold = 100;

    public float mutateAmount = .2f;
    public float mutateChance = .2f;

    public float elapsedTime = 0;

    public float size = 1f;

    public float FB = 0;
    public float LR = 0;

    public float FOV = 10;

    public int numChildren = 0;

    public bool isDead = false;
    public bool doMutate = false;
    public bool isMutated = false;

    void OnAwake()
    {
        net = gameObject.GetComponent<Network>();
        move = gameObject.GetComponent<Mover>();

        this.name = "scavenger";
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "resource")
        {
            energy += energyGained;
            reproEnergy += reproEnergyGained;

            Destroy(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!isMutated)
        {
            MutateScavenger();
            this.transform.localScale = new Vector3(size, size, size);
            isMutated = true;
            energy = 20;
        }

        ManageEnergy();
        
        float[] inputsToNet = ManageSense(8, 360 / 8);

        float[] outputsFromNet = net.GetNetworkOutputs(inputsToNet);

        FB = outputsFromNet[0];
        LR = outputsFromNet[1];

        move.Move(FB, LR);
    }

    float[] ManageSense(int numRays, float angleBetween)
    {
        float[] distances = new float[numRays];

        RaycastHit hit;
        for(int i = 0; i < numRays; i++)
        {
            float angle = ((2 * i + 1 - numRays) * angleBetween / 2);

            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * transform.forward * -1;

            Vector3 rayStart = transform.position + Vector3.up * 0.1f;

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
        return (distances);
    }

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

    void MutateScavenger()
    {
        if (doMutate)
        {
            mutateAmount += Random.Range(-1.0f, 1.0f) / 100;
            mutateChance += Random.Range(-1.0f, 1.0f) / 100;
        }

        mutateAmount = Mathf.Max(mutateAmount, 0);
        mutateChance = Mathf.Max(mutateChance, 0);

        net.MutateNet(mutateAmount, mutateChance);

    }

    void Repro()
    { 
        for(int i = 0; i < numChildren; i++)
        {
            GameObject child = Instantiate(scavenger, new Vector3((float)this.transform.position.x + Random.Range(-10, 11), 0.75f, (float)this.transform.position.z + Random.Range(-10, 11)), Quaternion.identity);

            child.GetComponent<Network>().layers = GetComponent<Network>().CastNet();
        }
        reproEnergy = 0;
    }

}
