using System;
using JohaToolkit.UnityEngine.Extensions;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using JohaToolkit.UnityEngine.Timer;
using UnityEngine;

namespace FlappyBirdCore
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private GameEvent resetEvent;

        private CountdownTimer _timer;
        private bool _shouldReset;

        private void Start()
        {
            resetEvent.Subscribe(_ => Reset());
            _timer = new CountdownTimer();
            _timer.TimerFinished += OnTimerFinished;
            _timer.TimerStarted += OnTimerStarted;
        }

        private void OnTimerStarted()
        {
            SpawnObstacle();
        }

        private void OnTimerFinished()
        {
            if (_shouldReset)
                return;
            _timer.Start(spawnInterval.Seconds());
        }
        
        private void StartSpawning()
        {
            _shouldReset = false;
            _timer.Start(spawnInterval.Seconds());
        }
        
        private void Reset()
        {
            _shouldReset = true;
            _timer.Stop();
            DespawnAllObstacles();
        }

        private void DespawnAllObstacles()
        {
            
        }
        
        private void SpawnObstacle()
        {
            
        }
    }
}
