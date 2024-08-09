using System;
using System.Linq;
using UnityEngine;

namespace Sources
{
    public class SimulationView : MonoBehaviour
    {
        [SerializeField] private float _animalsAnimationSpeed = 5;
        [Header("Prefabs")]
        [SerializeField] private GameObject _animalPrefab;
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private ParticleSystem _particle;
        
        private bool _isInited;
        private int _animalsSpeed;
        private int _simulationSpeed;
        private Simulation _simulation;
        private Transform[] _animalsView;
        private Transform[] _foodsView;
        
        private void Update()
        {
            if (!_isInited) return;
            if (_simulationSpeed == 0) return;

            var animals = _simulation.Animals;
            
            for (var i = 0; i < _animalsView.Length; i++)
            {
                var animal = animals.ElementAt(i);
                var animalView = _animalsView[i];
                var foodView = _foodsView[i];

                var newAnimalPosition = new Vector3(animal.Position.Column, 0, animal.Position.Row);
                var newFoodPosition = new Vector3(animal.Target.Column, 0, animal.Target.Row);

                if (foodView.position != newFoodPosition)
                {
                    SpawnParticleAt(foodView.position);
                }
                
                foodView.position = newFoodPosition;
                animalView.position = Vector3.Lerp(animalView.position, newAnimalPosition,
                    _simulationSpeed * _animalsSpeed * _animalsAnimationSpeed * Time.deltaTime);
                animalView.LookAt(foodView);
            }
        }

        public void Init(Simulation simulation, int animalsSpeed)
        {
            if (_isInited) throw new InvalidOperationException();
            
            _isInited = true;
            _simulation = simulation;
            _animalsSpeed = animalsSpeed;
            _animalsView = new Transform[_simulation.Animals.Count()];
            _foodsView = new Transform[_simulation.Animals.Count()];
            
            SpawnAnimals();
            SpawnFoods();
        }
        public void SetSimulationSpeed(int simulationSpeed)
        {
            _simulationSpeed = simulationSpeed;
        }

        private void SpawnParticleAt(Vector3 position)
        {
            Instantiate(_particle, position, Quaternion.identity, transform);
        }
        private void SpawnAnimals()
        {
            var colorStep = 1 / (float)_foodsView.Length;
            
            for (var i = 0; i < _animalsView.Length; i++)
            {
                var animalPosition = _simulation.Animals.ElementAt(i).Position;
                var spawnPosition = new Vector3(animalPosition.Column, 0, animalPosition.Row);

                _animalsView[i] = Instantiate(_animalPrefab, spawnPosition, Quaternion.identity, transform).transform;
                _animalsView[i].GetComponent<MeshRenderer>().material.color =
                    new Color(i * colorStep, i * colorStep, i * colorStep, 1);
            }
        }
        private void SpawnFoods()
        {
            var colorStep = 1 / (float)_foodsView.Length;
            
            for (var i = 0; i < _foodsView.Length; i++)
            {
                var foodPosition = _simulation.Animals.ElementAt(i).Target;
                var spawnPosition = new Vector3(foodPosition.Column, 0, foodPosition.Row);

                _foodsView[i] = Instantiate(_foodPrefab, spawnPosition, Quaternion.identity, transform).transform;
                _foodsView[i].GetComponent<MeshRenderer>().material.color =
                    new Color(i * colorStep, i * colorStep, i * colorStep, 1);
            }
        }
    }
}