namespace PCG
{
    public static class ProceduralRNG
    {
        private static System.Random rng;

        public static void Initialize(int seed)
        {
            rng = new System.Random(seed);
        }

        public static bool Bool(float probability = 0.5f)  
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
            if (UnityEngine.Debug.isDebugBuild) UnityEngine.Debug.Log("RNG not Initialized. RNG Initialized to 12345");
            rng = new System.Random(12345);
        }
    }
}