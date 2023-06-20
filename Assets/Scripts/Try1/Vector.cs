using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Vector : IEquatable<Vector>
{
    public float[] values;
    public Vector(int length, bool randomize = false)
    {
        values = new float[length];
        if (randomize)
        {
            for(int i = 0; i < values.Length; i++)
            {
                values[i] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    public Vector(float[] _values)
    {
        values = _values;
    }

    public void JoinVector(Vector other)
    {
        float[] newValues = new float[values.Length + other.values.Length];

        for(int i = 0; i < values.Length; i++)
        {
            newValues[i] = values[i];
        }
        for (int i = 0; i < other.values.Length; i++)
        {
            newValues[values.Length + i] = other.values[i];
        }
    }


    public bool Equals(Vector other)
    {
        if(values.Length != other.values.Length)
        {
            return false;
        }
        for(int i = 0; i < values.Length; i++)
        {
            if (values[i] != other.values[i])
            {
                return false;
            }
        }
        return true;
    }

    public int Length
    {
        get
        {
            return values.Length;
        }
    }
}
