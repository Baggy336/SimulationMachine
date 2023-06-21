using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Network : MonoBehaviour
{
    // Net array to hold all layers
    public Net[] layers;

    /// <summary>
    /// inputs (number of raycasts)
    /// any amount of hidden layers
    /// outputs
    /// </summary>
    public int[] networkShape = { 8, 12, 12, 2 };

    /// <summary>
    /// The net class, instantiates each layer
    /// </summary>
    public class Net
    {
        public float[,] weightsArray;
        public float[] biasArray;
        public float[] nodesArray;

        private int nodes;
        private int inputs;

        /// <summary>
        /// Net constructor, relies on the network shape for inputs and nodes
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="nodes"></param>
        public Net(int inputs, int nodes)
        {
            this.nodes = nodes;
            this.inputs = inputs; 

            weightsArray = new float[nodes, inputs];
            biasArray = new float[nodes];
            nodesArray = new float[nodes];
        }      

        // I = nodes, j = inputs
        /// <summary>
        /// This function commits weights and biases to the nodes array
        /// </summary>
        /// <param name="input"></param>
        public void Push(float[] input)
        {
            // crate a float array for the amount of nodes in this net
            nodesArray = new float[nodes];

            for(int i = 0; i < nodes; i++)
            {
                for(int j = 0; j < inputs; j++)
                {
                    // Add the sum of the weights * the inputs to the nodes array
                    nodesArray[i] += weightsArray[i, j] * input[j];
                }

                // Add bias to the node array
                nodesArray[i] += biasArray[i];
            }
        }

        /// <summary>
        /// 4 different types of activation, mess with which one to use
        /// </summary>
        public void Activation()
        {
            /*
            // Leaky Relu
            for(int i = 0; i < nodesArray.Length; i++)
            {
                 if(nodesArray[i] < 0)
                 {
                     nodesArray[i] = nodesArray[i]/10;
                 }
            }

            // Sigmoid activation
            for(int i = 0; i < nodesArray.Length; i++)
            {
                nodesArray[i] = 1/(1 + Mathf.Exp(-nodesArray[i]));
            }

            // TanH Activation
            for (int i = 0; i < nodesArray.Length; i++)
            {
                nodesArray[i] = (float)System.Math.Tanh(nodesArray[i]);
            }
            */

            // Relu Activation
            for(int i = 0; i < nodesArray.Length; i++)
            {
                if(nodesArray[i] < 0)
                {
                    nodesArray[i] = 0;
                }
            }
        }

        /// <summary>
        /// Mutation values are passed from MutateNet() function, mutates a single layer's values at a time
        /// </summary>
        /// <param name="mutateAmount"></param>
        /// <param name="mutateChance"></param>
        public void MutateLayer(float mutateAmount, float mutateChance)
        {
            // loop through each node in the layer
            for(int i = 0; i < nodes; i++)
            {
                // Loop through each input in the layer
                for(int j = 0; j < inputs; j++)
                {
                    // Decide whether to mutate the inputs or not
                    if(UnityEngine.Random.value < mutateChance)
                    {
                        // Mutate node weights by a small amount
                        weightsArray[i, j] += UnityEngine.Random.Range(-1f, 1f) * mutateAmount;
                    }
                }

                // Decide whether to mutate the nodes or not
                if (UnityEngine.Random.value < mutateChance)
                {
                    // Mutate each bias by a small amount
                    biasArray[i] += UnityEngine.Random.Range(-1f, 1f) * mutateAmount;
                }
            }
        }
    }

    /// <summary>
    /// Sets up the initial net shape, dictated by the network shape array
    /// </summary>
    public void Awake()
    {
        // The number of layers for this netowrk (exclusive)
        layers = new Net[networkShape.Length - 1];

        // Each layer's Net setup
        for(int i = 0; i < layers.Length; i++)
        {
            // Instantiate a new layer with network shape of the array element
            layers[i] = new Net(networkShape[i], networkShape[i + 1]);
        }
    }

    /// <summary>
    /// Takes inputs from the creature, currently outputs the FB and LR values to use for movement
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public float[] GetNetworkOutputs(float[] inputs)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            // If this is the input layer
            if(i == 0)
            {
                layers[i].Push(inputs);
                layers[i].Activation();
            }
            // If this is one of the middling layers
            else if(i == layers.Length - 1)
            {
                layers[i].Push(layers[i - 1].nodesArray);
            }
            // If this is the output layer
            else
            {
                layers[i].Push(layers[i - 1].nodesArray);
                layers[i].Activation();
            }
        }
        return (layers[layers.Length - 1].nodesArray);
    }

    /// <summary>
    /// This function creates a copy of the current network, and returns a copy of it
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Called once from the creature, to mutate the layers of this network
    /// </summary>
    /// <param name="mutateAmount"></param>
    /// <param name="mutateChance"></param>
    public void MutateNet(float mutateAmount, float mutateChance)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            // Mutate the current layer
            layers[i].MutateLayer(mutateAmount, mutateChance);
        }
    }

}
