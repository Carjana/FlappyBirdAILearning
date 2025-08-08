using System;
using System.Linq;
using FlappyBirdCore;
using JohaToolkit.UnityEngine.DataStructures;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using UnityEngine;

namespace AI
{
    public class ObstacleObserver : MonoBehaviourSingleton<ObstacleObserver>
    {
        [SerializeField] private GameEvent agentPreTickEvent;
        [SerializeField] private ObstacleSpawner obstacleSpawner;

        private Vector2 _nearestObstacle;

        private bool _isDirty;
        
        protected override void Awake()
        {
            base.Awake();
            agentPreTickEvent.Subscribe((_) => OnAgentPreTick());
        }

        private void OnAgentPreTick()
        {
            _isDirty = true;
        }

        public Vector2 GetNearestObstacleLocal(Vector2 position)
        {
            return GetNearestObstacleGlobal(position) - position;
            if(_isDirty)
                _nearestObstacle = GetNearestObstacle(position);
            return new Vector2(_nearestObstacle.x, _nearestObstacle.y) - position;
        }

        public Vector2 GetNearestObstacleGlobal(Vector2 position)
        {
            if(_isDirty)
                _nearestObstacle = GetNearestObstacle(position);
            return new Vector2(_nearestObstacle.x, _nearestObstacle.y);
        }

        private Vector2 GetNearestObstacle(Vector2 position)
        {
            Obstacle o = obstacleSpawner.ActiveObstacles
                .Where(o => o.transform.position.x > position.x)
                .OrderBy(o => o.transform.position.x)
                .FirstOrDefault();
            if (o != null) 
                return o.transform.position;
            
            throw new ArgumentNullException("_nearestObstacle", "No obstacle found after position: " + position);
        }

        private void OnDrawGizmos()
        {
            if (_nearestObstacle == null) 
                return;
            
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, _nearestObstacle);
        }
    }
}