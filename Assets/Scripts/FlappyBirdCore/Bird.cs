using JohaToolkit.UnityEngine.Extensions;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FlappyBirdCore
{
    public class Bird : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = 9.81f;
        private Rigidbody2D _rb;

        [SerializeField] private GameEvent startEvent;
        [SerializeField] private GameEvent resetEvent;

        public bool IsDead { get; private set; }
        public bool HasStarted { get; private set; }
        public float Velocity { get; private set; }
        
        public int Score { get; private set; }
        public Vector2 Position => _rb.position;
        

        private void Awake()
        {
            _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void Start()
        {
            resetEvent.Subscribe((_) => Reset());
            startEvent.Subscribe((_) => StartBird());
        }

        private void Update()
        {
            if (IsDead || !HasStarted)
                return;
            Velocity -= gravity * Time.deltaTime;
            transform.position += Vector3.up * (Velocity * Time.deltaTime);
        }

        [Button]
        public void StartBird()
        {
            HasStarted = true;
            Score = 0;
        }

        public void Jump()
        {
            if (IsDead || !HasStarted)
                return;
            Velocity = jumpForce;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Death"))
                HandleDeath();
            else if(other.CompareTag("Score"))
                Score++;
        }

        private void HandleDeath()
        {
            IsDead = true;
            _rb.linearVelocity = Vector2.zero;
        }

        [Button]
        public void Reset()
        {
            transform.position = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            IsDead = false;
            HasStarted = false;
            Velocity = 0;
        }
    }
}