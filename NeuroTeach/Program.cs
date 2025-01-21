using Neuro.Layers;
using Neuro.Models;
using Neuro.Optimizers;
using Neuro.Tensors;
using Neuro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuroTeach
{
    internal class Program
    {
        static NeuralNetwork TestOR()
        {
            //обучающие данные
            List<Data> trainingData = new List<Data>()
            {
            new Data(new Tensor(new float[] { 0, 0 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1))),
            new Data(new Tensor(new float[] { 0, 1 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 0 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 1 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1)))
            };

            var net = new NeuralNetwork("simple_net_or_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 4, Activation.ReLU));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 1, 150, null, 0, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join(" ", trainingData[i].Input.GetValues());
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2})", inp, Math.Round(predict), outp);
            }

            return net;
        }

        static NeuralNetwork TestAND()
        {
            //обучающие данные
            List<Data> trainingData = new List<Data>()
            {
            new Data(new Tensor(new float[] { 0, 0 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1))),
            new Data(new Tensor(new float[] { 0, 1 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 0 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 1 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1)))
            };

            var net = new NeuralNetwork("simple_net_and_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 4, Activation.ReLU));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 1, 150, null, 0, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join(" ", trainingData[i].Input.GetValues());
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2})", inp, Math.Round(predict), outp);
            }

            return net;
        }

        static NeuralNetwork TestXOR()
        {
            //обучающие данные
            List<Data> trainingData = new List<Data>()
            {
            new Data(new Tensor(new float[] { 0, 0 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1))),
            new Data(new Tensor(new float[] { 0, 1 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 0 }, new Shape(2)), new Tensor(new float[] {1 }, new Shape(1))),
            new Data(new Tensor(new float[] { 1, 1 }, new Shape(2)), new Tensor(new float[] {0 }, new Shape(1)))
            };

            var net = new NeuralNetwork("simple_net_xor_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 4, Activation.ReLU));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 1, 150, null, 0, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join(" ", trainingData[i].Input.GetValues());
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2})", inp, Math.Round(predict), outp);
            }

            return net;
        }

        static NeuralNetwork TestSUM()
        {
            List<Data> trainingData = new List<Data>();

            //обучающие данные
            for (int a = 0; a <= 9; ++a)
            {
                for (int b = 0; b <= 9; ++b)
                {
                    trainingData.Add(new Data(new Tensor(new float[] { a, b }, new Shape(2)), new Tensor(new float[] { a + b }, new Shape(1))));
                }
            }

            var net = new NeuralNetwork("simple_net_sum_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 4, Activation.ReLU));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 1, 150, null, 0, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join("+", trainingData[i].Input.GetValues());
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2})", inp, Math.Round(predict), outp);
            }

            return net;
        }

        static NeuralNetwork TestSUB()
        {
            List<Data> trainingData = new List<Data>();

            //обучающие данные
            for (int a = 0; a <= 9; ++a)
            {
                for (int b = 0; b <= 9; ++b)
                {
                    trainingData.Add(new Data(new Tensor(new float[] { a, b }, new Shape(2)), new Tensor(new float[] { a - b }, new Shape(1))));
                }
            }

            var net = new NeuralNetwork("simple_net_sub_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 4, Activation.ReLU));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 1, 150, null, 0, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join("-", trainingData[i].Input.GetValues());
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2})", inp, Math.Round(predict), outp);
            }

            return net;
        }

        static NeuralNetwork TestMUL()
        {
            List<Data> trainingData = new List<Data>();

            //обучающие данные
            for (int a = 0; a <= 9; ++a)
            {
                for (int b = 0; b <= 9; ++b)
                {
                    trainingData.Add(new Data(new Tensor(new float[] { a, b }, new Shape(2)), new Tensor(new float[] { a * b }, new Shape(1))));
                }
            }

            var net = new NeuralNetwork("simple_net_mul_test");
            var model = new Sequential();
            model.AddLayer(new Flatten(new Shape(2)));
            model.AddLayer(new Dense(model.LastLayer, 120, Activation.Sigmoid));
            model.AddLayer(new Dense(model.LastLayer, 64, Activation.Sigmoid));
            model.AddLayer(new Dense(model.LastLayer, 1, Activation.Linear));
            net.Model = model;

            net.Optimize(new SGD(), Loss.MeanSquareError);

            net.Fit(trainingData, 4, 200, null, 1, Track.Nothing);

            for (int i = 0; i < trainingData.Count; ++i)
            {
                var inp = string.Join("*", trainingData[i].Input.GetValues());
                var need = trainingData[i].Output.GetValues().First();
                var predict = net.Predict(trainingData[i].Input).First().GetValues().First();
                var outp = string.Join(" ", predict);
                Console.WriteLine("{0} = {1} ({2}) {3}", inp, Math.Round(predict), outp, need);
            }

            return net;
        }

        static string DisplayMenu()
        {
            Console.WriteLine("Choose neuro test:");
            Console.WriteLine();
            Console.WriteLine("1 - OR");
            Console.WriteLine("2 - AND");
            Console.WriteLine("3 - XOR");
            Console.WriteLine("4 - SUM");
            Console.WriteLine("5 - SUB");
            Console.WriteLine("6 - MUL (long time!)");
            Console.WriteLine();
            return Console.ReadLine();
        }

        static void Main(string[] args)
        {
            string userInput = "";
            do
            {
                Console.Clear();
                userInput = DisplayMenu();

                NeuralNetwork net = null;

                switch (userInput)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("OR");
                        net = TestOR();
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("AND");
                        net = TestAND();
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("XOR");
                        net = TestXOR();
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("SUM");
                        net = TestSUM();
                        break;
                    case "5":
                        Console.Clear();
                        Console.WriteLine("SUB");
                        net = TestSUB();
                        break;
                    case "6":
                        Console.Clear();
                        Console.WriteLine("MUL");
                        net = TestMUL();
                        break;
                    case "":
                        return;
                }
                Console.WriteLine();
                Console.WriteLine("Save net state?");
                Console.WriteLine("1 - YES");
                Console.WriteLine("2 - NO");

                switch (Console.ReadLine())
                {
                    case "1":
                        switch (userInput)
                        {
                            case "1":
                                net.SaveStateXml("or.xml");
                                break;
                            case "2":
                                net.SaveStateXml("and.xml");
                                break;
                            case "3":
                                net.SaveStateXml("xor.xml");
                                break;
                            case "4":
                                net.SaveStateXml("sum.xml");
                                break;
                            case "5":
                                net.SaveStateXml("sub.xml");
                                break;
                            case "6":
                                net.SaveStateXml("mul.xml");
                                break;
                        }
                        break;
                }

            } while (userInput != "");
        }
    }
}

