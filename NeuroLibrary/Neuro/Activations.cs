﻿using System;
using Neuro.Tensors;

namespace Neuro
{
    public abstract class ActivationFunc
    {
        public abstract void Compute(Tensor input, Tensor result);
        public abstract void Derivative(Tensor output, Tensor outputGradient, Tensor result);
    }

    public static class Activation
    {
        public static ActivationFunc Linear = new Linear();
        public static ActivationFunc Sigmoid = new Sigmoid();
        public static ActivationFunc Tanh = new Tanh();
        public static ActivationFunc ReLU = new ReLU();
        public static ActivationFunc ELU = new ELU();
        public static ActivationFunc Softmax = new Softmax();

        public static ActivationFunc getByName(string name)
        {
            switch (name)
            {
                case "Neuro.Linear":
                    return Activation.Linear;
                case "Neuro.Sigmoid":
                    return Activation.Sigmoid;
                case "Neuro.Tanh":
                    return Activation.Tanh;
                case "Neuro.ReLU":
                    return Activation.ReLU;
                case "Neuro.ELU":
                    return Activation.ELU;
                case "Neuro.Softmax":
                    return Activation.Softmax;
            }
            return null;
        }
    }

    public class Linear : ActivationFunc
    {
        public override void Compute(Tensor input, Tensor result)
        {
            input.CopyTo(result);
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            outputGradient.CopyTo(result);
        }
    }

    public class Sigmoid : ActivationFunc
    {
        public override void Compute(Tensor input, Tensor result)
        {
            input.Map(x => 1 / (1 + (float)Math.Exp(-x)), result);
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            output.Map((x, x2) => x * (1 - x) * x2, outputGradient, result);
        }
    }

    public class Tanh : ActivationFunc
    {
        public override void Compute(Tensor input, Tensor result)
        {
            input.Map(x => 2 / (1 + (float)Math.Exp(-2 * x)) - 1, result);
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            output.Map((x, x2) => (1 - x * x) * x2, outputGradient, result);
        }
    }

    public class ReLU : ActivationFunc
    {
        public override void Compute(Tensor input, Tensor result)
        {
            input.Map(x => Math.Max(0, x), result);
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            output.Map((x, x2) => x > 0 ? x2 : 0, outputGradient, result);
        }
    }

    public class ELU : ActivationFunc
    {
        private readonly float ALPHA = 1;

        public override void Compute(Tensor input, Tensor result)
        {
            input.Elu(ALPHA, result);
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            Tensor.EluGradient(output, outputGradient, ALPHA, result);
        }
    }

    public class Softmax : ActivationFunc
    {
        public override void Compute(Tensor input, Tensor result)
        {
            Tensor shifted = input.Sub(input.Max());
            Tensor exps = shifted.Map(x => (float)Math.Exp(x));

            for (int n = 0; n < input.BatchSize; ++n)
            {
                float sum = exps.Sum(n);

                for (int d = 0; d < input.Depth; ++d)
                for (int h = 0; h < input.Height; ++h)
                for (int w = 0; w < input.Width; ++w)
                    result[w, h, d, n] = exps[w, h, d, n] / sum;
            }
        }

        public override void Derivative(Tensor output, Tensor outputGradient, Tensor result)
        {
            var outputReshaped = output.Reshaped(new Shape(1, Shape.Auto, 1, output.BatchSize));
            Tensor jacob = outputReshaped.DiagFlat().Sub(outputReshaped.Mul(outputReshaped.Transposed()));
            jacob.Mul(outputGradient, result);
        }
    }
}
