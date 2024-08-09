using System;
using Sources.Saves;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sources
{
    public class SimulationSetupWindow : MonoBehaviour
    {
        [SerializeField] private GameObject _setupPanel;
        [SerializeField] private GameObject _menuPanel;
        [Header("Texts")]
        [SerializeField] private TMP_Text _gridSizeTextObject;
        [SerializeField] private TMP_Text _animalsCountTextObject;
        [SerializeField] private TMP_Text _animalsSpeedTextObject;
        [Header("Buttons")]
        [SerializeField] private Button _setupNewGameButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _saveGameButton;
        [Header("Sliders")]
        [SerializeField] private Slider _gridSizeSlider;
        [SerializeField] private Slider _animalsCountSlider;
        [SerializeField] private Slider _animalsSpeedSlider;
        [SerializeField] private Slider _simulationSpeedSlider;
        [Header("Other")]
        [SerializeField] private SimulationView _simulationView;

        private Simulation _simulation;
        private SimulationRunner _simulationRunner;
        private SaveLoadSystem _saveLoadSystem;
        
        private void Awake()
        {
            _saveLoadSystem = new SaveLoadSystem();
            
            _gridSizeSlider.minValue = 2;
            _gridSizeSlider.maxValue = 1000;
            _animalsCountSlider.minValue = 1;
            _animalsCountSlider.maxValue = _gridSizeSlider.value * _gridSizeSlider.value / 2;
            _animalsSpeedSlider.minValue = 1;
            _animalsSpeedSlider.maxValue = 100;
            _simulationSpeedSlider.minValue = 0;
            _simulationSpeedSlider.maxValue = 1000;

            _gridSizeSlider.onValueChanged.AddListener(OnGridSizeChanged);
            _animalsCountSlider.onValueChanged.AddListener(OnAnimalsCountChanged);
            _animalsSpeedSlider.onValueChanged.AddListener(OnAnimalsSpeedChanged);
            _simulationSpeedSlider.onValueChanged.AddListener(OnSimulationSpeedChanged);
            
            _setupNewGameButton.onClick.AddListener(OnSetupNewGameButton);
            _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            _loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
            _saveGameButton.onClick.AddListener(OnSaveGameButtonClicked);
            
            OnGridSizeChanged(_gridSizeSlider.value);
            OnAnimalsCountChanged(_animalsCountSlider.value);
            OnAnimalsSpeedChanged(_animalsSpeedSlider.value);
        }
        private void OnDestroy()
        {
            _simulationRunner?.Stop();
        }

        private void OnSetupNewGameButton()
        {
            _menuPanel.SetActive(false);
            _setupPanel.SetActive(true);
        }
        private void OnNewGameButtonClicked()
        {
            var gridSize = Mathf.FloorToInt(_gridSizeSlider.value);
            var animalsCount = Mathf.FloorToInt(_animalsCountSlider.value);
            var animalsSpeed = Mathf.FloorToInt(_animalsSpeedSlider.value);
            var config = new SimulationConfig {GridSize = gridSize, AnimalsCount = animalsCount, AnimalSpeed = animalsSpeed};
            _simulation = new Simulation(config);
            
            _simulationView.SetSimulationSpeed(Mathf.FloorToInt(_simulationSpeedSlider.value));
            _simulationView.Init(_simulation, animalsSpeed);

            _simulationRunner = new SimulationRunner(_simulation)
            {
                SimulationSpeed = Mathf.FloorToInt(_simulationSpeedSlider.value)
            };
            _simulationRunner.Start();
            
            _setupPanel.SetActive(false);
        }
        private void OnLoadGameButtonClicked()
        {
            var snapshot = _saveLoadSystem.Load();
            if (snapshot == null) throw new Exception("Save does not exist!");
            
            _simulation = new Simulation(snapshot);
            
            _simulationView.SetSimulationSpeed(Mathf.FloorToInt(_simulationSpeedSlider.value));
            _simulationView.Init(_simulation, snapshot.GetConfig().AnimalSpeed);

            _simulationRunner = new SimulationRunner(_simulation)
            {
                SimulationSpeed = Mathf.FloorToInt(_simulationSpeedSlider.value)
            };
            _simulationRunner.Start();
            
            _menuPanel.SetActive(false);
        }
        private void OnSaveGameButtonClicked()
        {
            var snapshot = _simulation.GetSnapshot();
            
            _saveLoadSystem.Save(snapshot);
        }
        
        private void OnGridSizeChanged(float value)
        {
            _animalsCountSlider.maxValue = _gridSizeSlider.value * _gridSizeSlider.value / 2;
            _gridSizeTextObject.text = $"GridSize> {Mathf.FloorToInt(value)}";
        }
        private void OnAnimalsCountChanged(float value)
        {
            _animalsCountTextObject.text = $"AnimalsCount> {Mathf.FloorToInt(value)}";
        }
        private void OnAnimalsSpeedChanged(float value)
        {
            _animalsSpeedTextObject.text = $"AnimalsSpeed> {Mathf.FloorToInt(value)}";
        }
        private void OnSimulationSpeedChanged(float value)
        {
            var simulationSpeed = Mathf.FloorToInt(_simulationSpeedSlider.value);
            
            _simulationRunner.SimulationSpeed = simulationSpeed;
            _simulationView.SetSimulationSpeed(simulationSpeed);
        }
    }
}