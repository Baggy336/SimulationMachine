using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Network : MonoBehaviour
{
    public Net[] layers;
    public int[] networkShape = { 8, 32, 2};

    public class Net
    {
        public float[,] weightsArray;
        public float[] biasArray;
        public float[] nodesArray;

        private int nodes;
        private int inputs;

        public Net(int inputs, int nodes)
        {
            this.nodes = nodes;
            this.inputs = inputs; 

            weightsArray = new float[nodes, inputs];
            biasArray = new float[nodes];
            nodesArray = new float[nodes];
        }      

        // I = nodes, j = inputs
        public void Push(float[] input)
        {
            nodesArray = new float[nodes];

            for(int i = 0; i < nodes; i++)
            {
                for(int j = 0; j < inputs; j++)
                {
                    nodesArray[i] += weightsArray[i, j] * input[j];
                }

                // Add bias to the node array
                nodesArray[i] += biasArray[i];
            }
        }

        public void Activation()
        {
            for(int i = 0; i < nodes; i++)
            {
                if (nodesArray[i] < 0)
                {
                    nodesArray[i] = 0;
                }
            }
        }

        public void MutateLayer(float mutateAmount, float mutateChance)
        {
            for(int i = 0; i < nodes; i++)
            {
                for(int j = 0; j < inputs; j++)
                {
                    if(UnityEngine.Random.value < mutateChance)
                    {
                        weightsArray[i, j] += UnityEngine.Random.Range(-1f, 1f) * mutateAmount;
                    }
                }

                if (UnityEngine.Random.value < mutateChance)
                {
                    biasArray[i] += UnityEngine.Random.Range(-1f, 1f) * mutateAmount;
                }
            }
        }
    }

    public void Awake()
    {
        layers = new Net[networkShape.Length - 1];
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Net(networkShape[i], networkShape[i + 1]);
        }
    }

    public float[] GetNetworkOutputs(float[] inputs)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            if(i == 0)
            {
                layers[i].Push(inputs);
                layers[i].Activation();
            }
            else if(i == layers.Length - 1)
            {
                layers[i].Push(layers[i - 1].nodesArray);
            }
            else
            {
                layers[i].Push(layers[i - 1].nodesArray);
                layers[i].Activation();
            }
        }
        return (layers[layers.Length - 1].nodesArray);
    }

    public Net[] CastNet()
    {
        Net[] tempNet = new Net[networkShape.Length - 1];

        for(int i = 0; i < layers.Length; i++)
        {
            tempNet[i] = new Net(networkShape[i], networkShape[i + 1]);
            Array.Copy(layers[i].weightsArray, tempNet[i].weightsArray, layers[i].weightsArray.GetLength(0) * layers[i].weightsArray.GetLength(1));
            Array.Copy(layers[i].biasArray, tempNet[i].biasArray, layers[i].biasArray.GetLength(0));

        }
        return (tempNet);
    }

    public void MutateNet(float mutateAmount, float mutateChance)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i].MutateLayer(mutateAmount, mutateChance);
        }
    }

}
