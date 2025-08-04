using System;
using FlappyBirdCore;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class BirdAgent : MonoBehaviour
    {
        [SerializeField] private Bird possessedBird;
        [SerializeField] private GameEvent agentStartEvent;
        [SerializeField] private GameEvent agentResetEvent;
        [SerializeField] private GameEvent agentTickEvent;
        [SerializeField] private GameEvent postAgentTickEvent;

        private (Vector2 state, bool action) _selectedAction;
        
        public bool IsDead => possessedBird.IsDead;
        
        private void Awake()
        {
            agentTickEvent.Subscribe(_ => OnAgentTick());
            postAgentTickEvent.Subscribe(_ => OnPostAgentTick());
            agentStartEvent.Subscribe(_ => StartAgent());
            agentResetEvent.Subscribe(_ => ResetAgent());
        }

        private void Start()
        {
            BirdAgentManager.Instance.RegisterAgent(this);
        }

        public void SetBird(Bird bird) => possessedBird = bird;

        public void ResetAgent()
        {
            possessedBird.Reset();
        }

        public void StartAgent()
        {
            possessedBird.StartBird();
        }
        
        private void OnAgentTick()
        {
            if (possessedBird.IsDead || !possessedBird.HasStarted)
                return;
            
            Vector2 currentState = GetCurrentState();

            bool shouldJump = ChooseAction(currentState);
            
            if (shouldJump)
            {
                possessedBird.Jump();
            }
            _selectedAction = (currentState, shouldJump);
        }
        
        private void OnPostAgentTick()
        {
            float reward = GetReward();
            
            Vector2 currentState = GetCurrentState();
            BirdAgentManager.Instance.QLearningManager.UpdateQValue(_selectedAction.state, _selectedAction.action, currentState, reward);
        }

        private Vector2 GetCurrentState()
        {
            Vector2 rawState = ObstacleObserver.Instance.GetNearestObstacleLocal(possessedBird.Position);
            return new Vector2(
                Mathf.Round(rawState.x * 10f) / 10f, // Round to 1 decimal place
                Mathf.Round(rawState.y * 10f) / 10f  // Round to 1 decimal place
            );
        }
        
        private bool ChooseAction(Vector2 currentState)
        {
            (bool action, float qValue)[] actions = BirdAgentManager.Instance.QLearningManager.GetActions(currentState);
            if (actions.Length == 0)
            {
                float jumpProbability = 0.1f;
                return Random.Range(0f, 1f) < jumpProbability;
            }

            WeightedPicker<bool> picker = new();
            foreach ((bool action, float qValue) in actions)
            {
                picker.Add(action, qValue);
            }
                
            return picker.Pick();
        }

        private float GetReward()
        {
            // Maybe take time into account?
            return possessedBird.IsDead ? -1 : 1;
        }
    }
}
