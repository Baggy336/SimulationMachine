using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Matrix
{
    public float[][] matrix;
    string activation;

    public Matrix(int inputDimension, int outputDimension, string _activation = "tanh")
    {
        activation = _activation;

        matrix = new float[outputDimension][];

        for(int i = 0; i < outputDimension; i++)
        {
            matrix[i] = new float[inputDimension + 1];

            for(int j = 0; j < inputDimension + 1; j++)
            {
                matrix[i][j] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    public Vector MatrixOutput(Vector input)
    {
        Vector output = new Vector(matrix.Length);
        for(int i = 0; i < matrix.Length; i++)
        {
            if (matrix[i].Length - 1 == input.Length)
            {
                for(int j = 0; j < matrix[i].Length - 1; j++)
                {
                    float valueWeight = input.values[j] * matrix[i][j];
                    if(valueWeight != valueWeight)
                    {
                        valueWeight = 0;
                    }
                    output.values[i] += valueWeight;
                }
                output.values[i] += 1f * matrix[i][matrix[i].Length - 1];
                output.values[i] = Activation.TanH(output.values[i]);
            }
            else
            {
                throw new System.Exception($" matrix column length ({matrix.Length - 1}) is not the same as input vector length ({input.Length})");
            }
        }
        return output;
    }
}
