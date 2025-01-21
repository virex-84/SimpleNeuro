using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Neuro.Layers;
using Neuro.Tensors;

namespace Neuro.Models
{
    public class Sequential : ModelBase
    {
        public override ModelBase Clone()
        {
            Sequential clone = new Sequential();
            foreach (var layer in Layers)
                clone.Layers.Add(layer.Clone());
            return clone;
        }

        public override void FeedForward(Tensor[] inputs)
        {
            if (inputs.Length > 1) throw new Exception("Only single input is allowed for sequential model.");

            for (int l = 0; l < Layers.Count; l++)
                Layers[l].FeedForward(l == 0 ? inputs : new [] {Layers[l - 1].Output});
        }

        public override void BackProp(Tensor[] deltas)
        {
            if (deltas.Length > 1) throw new Exception("Only single delta is allowed for sequential model.");

            Tensor delta = deltas[0];
            for (int l = Layers.Count - 1; l >= 0; --l)
                delta = Layers[l].BackProp(delta)[0];
        }

        public override Tensor[] GetOutputs()
        {
            return new [] { LastLayer.Output };
        }

        public override IEnumerable<LayerBase> GetOutputLayers()
        {
            return new[] {Layers.Last()};
        }

        public override int GetOutputLayersCount()
        {
            return 1;
        }

        public override IEnumerable<LayerBase> GetLayers()
        {
            return Layers;
        }

        public override string Summary()
        {
            int totalParams = 0;
            string output = "_________________________________________________________________\n";
            output += "Layer                        Output Shape              Param #\n";
            output += "=================================================================\n";

            foreach (var layer in Layers)
            {
                totalParams += layer.GetParamsNum();
                output += $"{(layer.Name + " (" + layer.GetType().Name + ")").PadRight(29)}" + $"({layer.OutputShape.Width}, {layer.OutputShape.Height}, {layer.OutputShape.Depth})".PadRight(26) + $"{layer.GetParamsNum()}\n";
                output += "_________________________________________________________________\n";
            }

            output += $"Total params: {totalParams}";

            return output;
        }

        public override XmlDocument SaveStateXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement modelElem = doc.CreateElement("Sequential");

            for (int l = 0; l < Layers.Count; l++)
            {
                XmlElement layerElem = doc.CreateElement(Layers[l].GetType().Name);
                Layers[l].SerializeParameters(layerElem);
                modelElem.AppendChild(layerElem);
            }

            doc.AppendChild(modelElem);
            //doc.Save(filename);

            return doc;
        }

        private LayerBase getLayer(string name, int inputShapes, int outputShapes, ActivationFunc activation, LayerBase lastLayer)
        {
            switch (name)
            {
                case "Flatten":
                    return new Flatten(new Shape(inputShapes));
                case "Dense":
                    return new Dense(lastLayer, outputShapes, activation);

                default:
                    return null;
            }
        }

        //полная загрузка сети
        public override void LoadStateXml2(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement modelElem = doc.FirstChild as XmlElement;

            Layers.Clear();

            for (int l = 0; l < modelElem.ChildNodes.Count; l++)
            {
                XmlElement layerElem = modelElem.ChildNodes.Item(l) as XmlElement;
                int inputShapes = 0;
                int outputShapes = 0;
                Int32.TryParse(layerElem.GetAttribute("inputShapes"), out inputShapes);
                Int32.TryParse(layerElem.GetAttribute("outputShapes"), out outputShapes);

                ActivationFunc activation = Activation.getByName(layerElem.GetAttribute("ActivationFunc"));

                var layer = getLayer(layerElem.Name, inputShapes, outputShapes, activation, this.LastLayer);
                if (layer != null)
                {
                    layer.Init();

                    layer.DeserializeParameters(layerElem);

                    Layers.Add(layer);
                }
            }
        }

        //загрузка только состояния уже существующей сети
        public override void LoadStateXml(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement modelElem = doc.FirstChild as XmlElement;

            for (int l = 0; l < Layers.Count; l++)
            {
                XmlElement layerElem = modelElem.ChildNodes.Item(l) as XmlElement;
                Layers[l].Init();
                Layers[l].DeserializeParameters(layerElem);
            }
        }

        public LayerBase Layer(int i)
        {
            return Layers[i];
        }

        public LayerBase LastLayer
        {
            get { return Layers.Count == 0 ? null : Layers.Last(); }
        }

        public int LayersCount
        {
            get { return Layers.Count; }
        }

        public void AddLayer(LayerBase layer)
        {
            Layers.Add(layer);
        }

        private List<LayerBase> Layers = new List<LayerBase>();
    }
}
