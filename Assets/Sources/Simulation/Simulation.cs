using System;
using System.Collections.Generic;
using AStar;
using AStar.Options;
using Sources.Saves;
using Random = System.Random;

namespace Sources
{
    public class Simulation
    {
        public IReadOnlyCollection<Animal> Animals => _animals;
        
        private readonly int _maxFoodSpawnDistance;
        private readonly Animal[] _animals;
        private readonly WorldGrid _animalGrid;
        private readonly WorldGrid _foodGrid;
        private readonly PathFinder _pathFinder;
        private readonly SimulationConfig _config;
        private readonly Random _random;
        
        private const int MaxAnimalMoveTime = 5;

        public Simulation(SimulationConfig config)
        {
            var pathfinderOptions = new PathFinderOptions
            {
                PunishChangeDirection = false,
                UseDiagonals = true
            };

            _config = config;
            _random = new Random();
            _maxFoodSpawnDistance = _config.AnimalSpeed * MaxAnimalMoveTime;
            _animalGrid = new WorldGrid(_config.GridSize, _config.GridSize);
            
            FillGridWith(_animalGrid, 1);
            
            _foodGrid = new WorldGrid(_config.GridSize, _config.GridSize);
            _pathFinder = new PathFinder(_animalGrid, pathfinderOptions);
            _animals = CreateAnimals(_config.AnimalsCount);
        }
        public Simulation(Snapshot snapshot)
        {
            var pathfinderOptions = new PathFinderOptions
            {
                PunishChangeDirection = false,
                UseDiagonals = true
            };

            _config = snapshot.GetConfig();
            _random = new Random();
            _maxFoodSpawnDistance = _config.AnimalSpeed * MaxAnimalMoveTime;
            _animals = snapshot.GetAnimals();
            _animalGrid = CreateAnimalsGrid(_config.GridSize, _animals);
            _foodGrid = CreateFoodGrid(_config.GridSize, _animals);
            _pathFinder = new PathFinder(_animalGrid, pathfinderOptions);
        }
        
        public void Simulate()
        {
            foreach (var animal in _animals)
            {
                MoveAnimal(animal);
            }
        }
        public Snapshot GetSnapshot()
        {
            return new Snapshot(_animals, _config);
        }

        private void MoveAnimal(Animal animal)
        {
            var path = _pathFinder.FindPath(animal.Position, animal.Target);
            var stepsCount = Math.Clamp(_config.AnimalSpeed, 0, path.Length);
            if (stepsCount == 0) return;
            
            var resultPosition = path[stepsCount == path.Length ? stepsCount - 1 : stepsCount];
            
            ChangeAnimalPositionTo(animal, resultPosition);
            
            if (animal.Position != animal.Target) return;
            
            TakeFoodAt(animal.Target);
            animal.Target = CreateAnimalTarget(animal.Position);
        }
        private void ChangeAnimalPositionTo(Animal animal, Position newPosition)
        {
            _animalGrid[animal.Position] = 1;
            _animalGrid[newPosition] = 0;
            animal.Position = newPosition;
        }
        private void TakeFoodAt(Position position)
        {
            _foodGrid[position] = 0;
        }
        
        private void FillGridWith(WorldGrid grid, short value)
        {
            for (var i = 0; i < grid.Height; i++)
            {
                for (var j = 0; j < grid.Width; j++)
                {
                    grid[i, j] = value;
                }
            }
        }
        private Animal[] CreateAnimals(int animalsCount)
        {
            var spawnedAnimals = new Animal[animalsCount];

            for (var i = 0; i < spawnedAnimals.Length; i++)
            {
                spawnedAnimals[i] = CreateAnimal();
            }

            return spawnedAnimals;
        }
        private Animal CreateAnimal()
        {
            var spawnPosition = GetRandomUnoccupiedPosition();
            var foodPosition = CreateAnimalTarget(spawnPosition);

            _animalGrid[spawnPosition] = 0;

            return new Animal(spawnPosition, foodPosition);
        }

        private WorldGrid CreateAnimalsGrid(int gridSize, Animal[] animals)
        {
            var grid = new WorldGrid(gridSize, gridSize);
            
            FillGridWith(grid, 1);

            foreach (var animal in animals)
            {
                grid[animal.Position] = 0;
            }

            return grid;
        }
        private WorldGrid CreateFoodGrid(int gridSize, Animal[] animals)
        {
            var grid = new WorldGrid(gridSize, gridSize);

            foreach (var animal in animals)
            {
                grid[animal.Target] = 1;
            }
            return grid;
        }

        private Position CreateAnimalTarget(Position animalPosition)
        {
            Position position;
            
            do
            {
                var xMin = Math.Max(animalPosition.Column - _maxFoodSpawnDistance, 0);
                var xMax = Math.Min(animalPosition.Column + _maxFoodSpawnDistance, _config.GridSize);
                var yMin = Math.Max(animalPosition.Row - _maxFoodSpawnDistance, 0);
                var yMax = Math.Min(animalPosition.Row + _maxFoodSpawnDistance, _config.GridSize);
                
                var x = _random.Next(xMin, xMax);
                var y = _random.Next(yMin, yMax);

                position = new Position(y, x);
            }
            while (position == animalPosition || _foodGrid[position] == 1);

            _foodGrid[position] = 1;
            
            return position;
        }
        private Position GetRandomUnoccupiedPosition()
        {
            Position position;

            do
            {
                var x = _random.Next(0, _config.GridSize);
                var y = _random.Next(0, _config.GridSize);
                position = new Position(y, x);
            }
            while (_animalGrid[position] == 0);

            return position;
        }
    }
}