using System;
using JohaToolkit.UnityEngine.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace FlappyBirdCore
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float maxX;
        [SerializeField] private Vector2 yOffset;
        private Transform _startTransform;
        private ObjectPool<Obstacle> _pool;

        private bool _shouldMove;
        
        public void Init(Transform startTransform, ObjectPool<Obstacle> obstaclePool)
        {
            _startTransform = startTransform;
            _pool = obstaclePool;
        }
        
        public void StartMoving()
        {
            _shouldMove = true;
            transform.position = new Vector3(_startTransform.position.x, _startTransform.position.y + yOffset.RandomRange(), 0);
        }

        private void Update()
        {
            if (!_shouldMove)
                return;
            transform.position -= Vector3.right * (speed * Time.deltaTime);
            if(transform.position.x < maxX)
                _pool.Release(this);
        }

        public void ResetObstacle()
        {
            _shouldMove = false;
        }
    }
}