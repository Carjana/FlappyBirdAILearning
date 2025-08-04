using System;
using System.Collections.Generic;
using JohaToolkit.UnityEngine.Extensions;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using JohaToolkit.UnityEngine.Timer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace FlappyBirdCore
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private Obstacle obstaclePrefab;
        
        [SerializeField] private GameEvent resetEvent;
        [SerializeField] private GameEvent preAgentStartEvent;
        
        private CountdownTimer _timer;
        private bool _shouldReset;
        
        private ObjectPool<Obstacle> _obstaclePool;
        public List<Obstacle> ActiveObstacles { get; } = new();

        private void Start()
        {
            resetEvent.Subscribe(_ => ResetSpawner());
            preAgentStartEvent.Subscribe(_ => StartSpawning());
            
            _timer = new CountdownTimer();
            _timer.TimerFinished += OnTimerFinished;
            _timer.TimerStarted += OnTimerStarted;
            SetUpObstaclePool();
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
        }

        private void SetUpObstaclePool()
        {
            _obstaclePool = new ObjectPool<Obstacle>(
                OnCreateObstacle,
                OnGetObstacle,
                OnReleaseObstacle,
                OnDestroyObstacle
                );
        }

        private void OnDestroyObstacle(Obstacle obstacle)
        {
            Destroy(obstacle);
        }

        private void OnReleaseObstacle(Obstacle obstacle)
        {
            obstacle.ResetObstacle();
            obstacle.gameObject.SetActive(false);
            ActiveObstacles.Remove(obstacle);
        }

        private void OnGetObstacle(Obstacle obstacle)
        {
            obstacle.StartMoving();
            obstacle.gameObject.SetActive(true);
            ActiveObstacles.Add(obstacle);
        }

        private Obstacle OnCreateObstacle()
        {
            Obstacle newObj = Instantiate(obstaclePrefab, transform.position, Quaternion.identity, transform);
            newObj.gameObject.SetActive(false);
            newObj.Init(transform, _obstaclePool);
            return newObj;
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
        
        [Button]
        public void StartSpawning()
        {
            _shouldReset = false;
            _timer.Start(spawnInterval.Seconds());
        }
        
        private void ResetSpawner()
        {
            _shouldReset = true;
            _timer.Stop();
            DespawnAllObstacles();
        }

        private void DespawnAllObstacles()
        {
            for (int i = ActiveObstacles.Count - 1; i >= 0; i--)
            {
                _obstaclePool.Release(ActiveObstacles[i]);
            }
        }
        
        private void SpawnObstacle()
        {
            _obstaclePool.Get();
        }
    }
}
