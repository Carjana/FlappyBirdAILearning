using System.Text;
using FlappyBirdCore;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace AI
{
    public class BirdAgent : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Bird possessedBird;
        [SerializeField] private GameEvent agentStartEvent;
        [SerializeField] private GameEvent agentResetEvent;
        [SerializeField] private GameEvent agentTickEvent;
        [SerializeField] private GameEvent postAgentTickEvent;
        
        [Title("Settings")]
        [SerializeField] private float gridSizePos = 0.1f;
        [SerializeField] private bool shouldLog;
        [SerializeReference] private ActionChoosingStrategy actionChoosingStrategy;

        private (GameState state, bool action) _selectedAction;
        
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
            
            GameState currentState = GetCurrentState();
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
            GameState currentState = GetCurrentState();
            if (currentState.NearestObstacle.Equals(_selectedAction.state.NearestObstacle))
            {
                if(shouldLog)
                    Debug.LogError("Current state is the same as previous state, skipping Q-value update.");
                return;
            }
            BirdAgentManager.Instance.QLearningManager.UpdateQValue(_selectedAction.state, _selectedAction.action, currentState, reward);
        }

        private GameState GetCurrentState()
        {
            Vector2 nearestObstacle = ObstacleObserver.Instance.GetNearestObstacleLocal(possessedBird.Position);
            return new GameState
            {
                NearestObstacle = PosOnGrid(nearestObstacle)
            };
        }

        private Vector2 PosOnGrid(Vector2 pos)
        {
            return new Vector2(
                Mathf.RoundToInt(pos.x / gridSizePos),
                Mathf.RoundToInt(pos.y / gridSizePos)
                );
        }
        
        private bool ChooseAction(GameState currentState)
        {
            (bool action, float qValue)[] actions = BirdAgentManager.Instance.QLearningManager.GetActions(currentState);
            if (actions.Length == 0)
            {
                if(shouldLog)
                    Debug.Log("Choosing uninformed action");
                return actionChoosingStrategy.ChooseUnInformedAction();
            }

            if(shouldLog)
            {
                StringBuilder builder = new();
                actions.ForEach(a => builder.Append($"Action: {a.action} | QValue: {a.qValue}"));
                Debug.Log($"Choosing informed action {builder}");
            }
            return actionChoosingStrategy.ChooseInformedAction(actions);
        }

        private float GetReward()
        {
            float distToCenter = Mathf.Abs(ObstacleObserver.Instance.GetNearestObstacleGlobal(possessedBird.Position).y - possessedBird.Position.y);
            return possessedBird.IsDead ? -1000 : 15;
        }
    }
}
