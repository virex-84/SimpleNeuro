﻿using Neuro.Tensors;

namespace Neuro.Initializers
{
    public class Uniform : InitializerBase
    {
        public Uniform(float min = -0.05f, float max = 0.05f)
        {
            Min = min;
            Max = max;
        }

        public static float NextSingle(float min, float max)
        {
            return min + (float)Tools.Rng.NextDouble() * (max - min);
        }

        public override void Init(Tensor t, int fanIn, int fanOut)
        {
            t.Map(x => NextSingle(Min, Max), t);
        }

        private readonly float Min;
        private readonly float Max;
    }
}
