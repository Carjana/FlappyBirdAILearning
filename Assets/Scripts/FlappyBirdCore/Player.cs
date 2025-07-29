using System;
using JohaToolkit.UnityEngine.Extensions;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlappyBirdCore
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravityScale = 1;
        private Rigidbody2D _rb;

        [SerializeField] private InputActionReference action;
        [SerializeField] private GameEvent deathEvent;
        [SerializeField] private GameEvent resetEvent;

        private bool _isDead;
    
        private void Awake()
        {
            _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
            action.action.performed += (_) => Jump();
        }

        private void Start()
        {
            resetEvent.Subscribe((_) => Reset());
        }

        public void Jump()
        {
            if (_isDead)
                return;
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Death"))
                return;
            HandleDeath();
        }

        private void HandleDeath()
        {
            _isDead = true;
            deathEvent.RaiseEvent(this);
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0;
        }

        [Button]
        public void Reset()
        {
            transform.position = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            _isDead = false;
            _rb.gravityScale = gravityScale;
        }
    }
}