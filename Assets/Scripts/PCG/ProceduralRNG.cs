using System;
using System.Diagnostics;

//Procedural RNG Class that can be used by other scripts to initialize RNG.
//Bool returns a bool value based on probability. Range(int), Range(float) returns value based on min and max range.
//Next() / NextDouble() returns random value.
//if rng is not initalized, fallback to 12345.
namespace PCG
{
    public static class ProceduralRNG
    {
        private static Random rng;  //rng variable of class Random

        public static void Initialize(int seed)
        {
            rng = new Random(seed);
        }

        public static bool Bool(float probability = 0.5f)   //param could be overloaded
        {
            if (rng == null) rngBoolError();
            return rng.NextDouble() < probability;
        }

        public static int Range(int min, int max)
        {
            if (rng == null) rngBoolError();
            return rng.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            if (rng == null) rngBoolError();
            return (float)rng.NextDouble() * (max - min) + min;
        }

        public static void rngBoolError()
        {
            UnityEngine.Debug.Log("RNG not Initialized. RNG Initialized to 12345");
            if (rng == null) rngBoolError();
        }
    }
}