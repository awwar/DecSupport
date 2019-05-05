using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    [Serializable]
    class Project
    {
        public string Name { get; set; } = null;
        public string Path { get; set; } = null;
        public string Hash { get; set; } = null;
        public double Lat { get; set; } = 0;
        public double Lon { get; set; } = 0;
        public Layer[] Layers = new Layer[] { };

        public void AddLayer(Layer layer)
        {
            foreach (Layer item in Layers)
            {
                if (item.Hash == layer.Hash)
                {
                    throw new Exception($"Layer {layer.Name} alrady exist!");
                }
            }
            layer.Id = Layers.Length;
            List<Layer> newLay = Layers.ToList();
            newLay.Add(layer);
            Layers = newLay.ToArray();
        }

        public void DeleteLayer(Layer layer)
        {
            bool isFinde = false;

            for (int i = 0; i < Layers.Length; i++)
            {
                if (Layers[i].Hash == layer.Hash)
                {
                    isFinde = true;
                }
                if (isFinde)
                {
                    Layers[i] = Layers[i + 1];
                }
            }
            if (isFinde)
            {
                Array.Resize(ref Layers, Layers.Length - 1);  
            }
        }
    }
}
