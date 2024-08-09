using System;

namespace Sources
{
    [Serializable]
    public class SimulationConfig
    {
        public int GridSize { get; set; }
        public int AnimalsCount { get; set; }
        public int AnimalSpeed { get; set; }
    }
}