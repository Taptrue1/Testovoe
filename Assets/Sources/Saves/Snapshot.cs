using System;
using System.Linq;
using AStar;

namespace Sources.Saves
{
    [Serializable]
    public class Snapshot
    {
        public int GridSize;
        public int AnimalSpeed;
        public int AnimalsCount;
        public int[] AnimalsPositionColumn;
        public int[] AnimalsPositionRow;
        public int[] AnimalsTargetColumn;
        public int[] AnimalsTargetRow;

        public Snapshot(Animal[] animals, SimulationConfig config)
        {
            GridSize = config.GridSize;
            AnimalSpeed = config.AnimalSpeed;
            AnimalsCount = config.AnimalsCount;

            AnimalsPositionRow = animals.Select(v => v.Position.Row).ToArray();
            AnimalsPositionColumn = animals.Select(v => v.Position.Column).ToArray();
            AnimalsTargetRow = animals.Select(v => v.Target.Row).ToArray();
            AnimalsTargetColumn = animals.Select(v => v.Target.Column).ToArray();
        }

        public Animal[] GetAnimals()
        {
            var animals = new Animal[AnimalsPositionRow.Length];

            for (var i = 0; i < AnimalsPositionRow.Length; i++)
            {
                var position = new Position(AnimalsPositionRow[i], AnimalsPositionColumn[i]);
                var target = new Position(AnimalsTargetRow[i], AnimalsTargetColumn[i]);
                
                animals[i] = new Animal(position, target);
            }
            
            return animals;
        }

        public SimulationConfig GetConfig()
        {
            return new SimulationConfig {AnimalsCount = AnimalsCount, AnimalSpeed = AnimalSpeed, GridSize = GridSize};
        }
    }
}