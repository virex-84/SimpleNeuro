﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Neuro.Tensors;

namespace Neuro
{
    public static class Tools
    {
        public static readonly float _EPSILON = 10e-7f;

        public static Random Rng = new Random();

        public static int AccNone(Tensor target, Tensor output)
        {
            return 0;
        }

        public static int AccBinaryClassificationEquality(Tensor target, Tensor output)
        {
            int hits = 0;
            for (int n = 0; n < output.BatchSize; ++n)
                hits += target[0, 0, 0, n].Equals(Math.Round(output[0, 0, 0, n])) ? 1 : 0;
            return hits;
        }

        public static int AccCategoricalClassificationEquality(Tensor target, Tensor output)
        {
            int hits = 0;
            for (int n = 0; n < output.BatchSize; ++n)
                hits += target.ArgMax(n).Equals(output.ArgMax(n)) ? 1 : 0;
            return hits;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n-- > 1)
            {
                int k = Tools.Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float Clip(float value, float min, float max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static int Sign(float value)
        {
            return value < 0 ? -1 : (value > 0 ? 1 : 0);
        }

        public static List<float> LinSpace(float start, float stop, int num = 50, bool endPoint = true)
        {
            List<float> result = new List<float>();
            float interval = (stop - start) / num;
            for (int i = 0; i < num; ++i)
            {
                result.Add(start);
                start += interval;
            }

            if (endPoint)
                result.Add(stop);

            return result;
        }

        public static string GetProgressString(int iteration, int maxIterations, string extraStr = "", int barLength = 30)
        {
            int maxIterLen = maxIterations.ToString().Length;
            float step = maxIterations / (float)barLength;
            int currStep = (int)Math.Min(Math.Ceiling(iteration / step), barLength);
            return $"{iteration.ToString().PadLeft(maxIterLen)}/{maxIterations} [{new string('=', currStep - 1)}" + (iteration == maxIterations ? "=" : ">") + $"{new string('.', barLength - currStep)}]" + extraStr;
        }

        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static void WriteBigInt32(this BinaryWriter bw, Int32 v)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)v;
            bytes[1] = (byte)(((uint)v >> 8) & 0xFF);
            bytes[2] = (byte)(((uint)v >> 16) & 0xFF);
            bytes[3] = (byte)(((uint)v >> 24) & 0xFF);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            bw.Write(BitConverter.ToInt32(bytes, 0));
        }

        public static List<Data> ReadMnistData(string imagesFile, string labelsFile, bool generateBmp = false, int maxImages = -1)
        {
            List<Data> dataSet = new List<Data>();

            using (FileStream fsLabels = new FileStream(labelsFile, FileMode.Open))
            using (FileStream fsImages = new FileStream(imagesFile, FileMode.Open))
            using (BinaryReader brLabels = new BinaryReader(fsLabels))
            using (BinaryReader brImages = new BinaryReader(fsImages))
            {
                int magic1 = brImages.ReadBigInt32(); // discard
                int numImages = brImages.ReadBigInt32();
                int imgWidth = brImages.ReadBigInt32();
                int imgHeight = brImages.ReadBigInt32();

                int magic2 = brLabels.ReadBigInt32(); // 2039 + number of outputs
                int numLabels = brLabels.ReadBigInt32();

                maxImages = maxImages < 0 ? numImages : Math.Min(maxImages, numImages);

                int outputsNum = magic2 - 2039;
            }

            return dataSet;
        }

        public static void WriteMnistData(List<Data> data, string imagesFile, string labelsFile)
        {
            if (data.Count == 0)
                return;

            using (FileStream fsLabels = new FileStream(labelsFile, FileMode.Create))
            using (FileStream fsImages = new FileStream(imagesFile, FileMode.Create))
            using (BinaryWriter bwLabels = new BinaryWriter(fsLabels))
            using (BinaryWriter bwImages = new BinaryWriter(fsImages))
            {
                int imgHeight = data[0].Inputs[0].Height;
                int imgWidth = data[0].Inputs[0].Width;
                int outputsNum = data[0].Outputs[0].Length;

                bwImages.WriteBigInt32(1337); // discard
                bwImages.WriteBigInt32(data.Count);
                bwImages.WriteBigInt32(imgHeight);
                bwImages.WriteBigInt32(imgWidth);

                bwLabels.WriteBigInt32(2039 + outputsNum);
                bwLabels.WriteBigInt32(data.Count);

                for (int i = 0; i < data.Count; ++i)
                {
                    for (int h = 0; h < imgHeight; ++h)
                    for (int x = 0; x < imgWidth; ++x)
                        bwImages.Write((byte)(data[i].Inputs[0][h, x] * 255));

                    for (int j = 0; j < outputsNum; ++j)
                    {
                        if (data[i].Outputs[0][j] == 1)
                        {
                            bwLabels.Write((byte)j);
                        }
                    }
                }
            }
        }

        public static List<Data> LoadCSVData(string filename, int outputs, bool outputsOneHotEncoded = false)
        {
            List<Data> dataSet = new List<Data>();

            using (var f = new StreamReader(filename))
            {
                string line;
                while ((line = f.ReadLine()) != null)
                {
                    string[] tmp = line.Split(',');

                    Tensor input = new Tensor(new Shape(1, tmp.Length - (outputsOneHotEncoded ? 1 : outputs)));
                    Tensor output = new Tensor(new Shape(1, outputs));

                    for (int i = 0; i < input.Length; ++i)
                        input[0, i] = float.Parse(tmp[i]);

                    for (int i = 0; i < (outputsOneHotEncoded ? 1 : outputs); ++i)
                    {
                        float v = float.Parse(tmp[input.Length + i]);
                        if (outputsOneHotEncoded)
                            output[0, (int)v] = 1;
                        else
                            output[0, i] = v;
                    }

                    dataSet.Add(new Data(input, output));
                }
            }

            return dataSet;
        }

        public static List<Data> MergeData(List<Data> dataList, int batchSize = -1)
        {
            if (batchSize < 0)
                batchSize = dataList.Count;

            List<Data> mergedData = new List<Data>();

            int batchesNum = dataList.Count / batchSize;
            int numberOfInputs = dataList[0].Inputs.Length;
            int numberOfOutputs = dataList[0].Outputs.Length;

            for (int b = 0; b < batchesNum; ++b)
            {
                var inputs = new Tensor[numberOfInputs];
                for (int i = 0; i < numberOfInputs; ++i)
                    inputs[i] = Tensor.MergeIntoBatch(dataList.GetRange(b * batchSize, batchSize).Select(x => x.Inputs[i]).ToList());

                var outputs = new Tensor[numberOfOutputs];
                for (int i = 0; i < numberOfOutputs; ++i)
                    outputs[i] = Tensor.MergeIntoBatch(dataList.GetRange(b * batchSize, batchSize).Select(x => x.Outputs[i]).ToList());

                mergedData.Add(new Data(inputs, outputs));
            }

            // add support for reminder of training data
            Debug.Assert(dataList.Count % batchSize == 0);

            return mergedData;
        }
    }
}
