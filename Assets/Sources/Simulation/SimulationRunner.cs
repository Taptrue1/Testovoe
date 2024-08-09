using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sources
{
    public class SimulationRunner
    {
        public int SimulationSpeed { get; set; }
        public bool IsRunning { get; private set; }

        private readonly Simulation _simulation;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public SimulationRunner(Simulation simulation)
        {
            _simulation = simulation;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (IsRunning) throw new InvalidOperationException();
            
            var thread = new Thread(Tick)
            {
                IsBackground = true
            };

            thread.Start();
        }
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void Tick()
        {
            IsRunning = true;
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if (SimulationSpeed == 0)
                {
                    Task.Yield();
                    continue;
                }
                _simulation.Simulate();

                var resultSimulationSpeed = SimulationSpeed == 0 ? 1 : SimulationSpeed;
                Thread.Sleep(1000 / resultSimulationSpeed);
            }
            IsRunning = false;
        }
    }
}