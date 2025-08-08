using System.Collections.Generic;
using System.Linq;
using JohaToolkit.UnityEngine.DataStructures;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using JohaToolkit.UnityEngine.ScriptableObjects.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI
{
    public class BirdAgentManager : MonoBehaviourSingleton<BirdAgentManager>
    {
        [Title("Bird Agents Settings")]
        [SerializeField] private float learningRate = 0.1f;
        [SerializeField] private float discountFactor = 0.9f;
        
        [SerializeField] private float updateRate = 0.2f;

        [Title("Events")]
        [SerializeField] private GameEvent agentPreStartEvent; 
        [SerializeField] private GameEvent startEvent;
        [SerializeField] private GameEvent resetEvent;
        [SerializeField] private GameEvent preAgentTickEvent;
        [SerializeField] private GameEvent agentTickEvent;
        [SerializeField] private GameEvent postAgentTickEvent;

        [SerializeField] private IntVariable currentGeneration;
        [SerializeField] private IntVariable aliveAgentsCount;
        
        [Title("Bird Agents")]
        [SerializeField, ReadOnly] private List<BirdAgent> birdAgents;

        public QLearningManager QLearningManager { get; private set; }

        private float _updateTimer;
        private bool _hasUpdated;
        private bool _isRunning;
        
        protected override void Awake()
        {
            base.Awake();
            QLearningManager = new QLearningManager(learningRate, discountFactor);
            birdAgents = new List<BirdAgent>();
            
            currentGeneration.Value = 0;
            // Birds register in Start Method!!!!
        }
        
        public void RegisterAgent(BirdAgent agent)
        {
            birdAgents ??= new List<BirdAgent>();

            if (!birdAgents.Contains(agent))
            {
                birdAgents.Add(agent);
            }
        }

        [Button]
        public void StartTraining()
        {
            if (_isRunning)
                return;
            
            StartGeneration();
        }

        [Button]
        public void ResetCurrentGeneration()
        {
            if (!_isRunning)
                return;
            ResetGeneration();
        }
        
        public void Update()
        {
            if (!_isRunning)
                return;
            
            _updateTimer += Time.deltaTime;

            if (_updateTimer >= updateRate && !_hasUpdated)
            {
                _hasUpdated = true;
                preAgentTickEvent?.RaiseEvent(this);
                agentTickEvent?.RaiseEvent(this);
            }
            else if(_updateTimer >= updateRate + updateRate/2 && _hasUpdated)
            {
                _updateTimer = 0;
                _hasUpdated = false;
                postAgentTickEvent?.RaiseEvent(this);
                
                int aliveAgents = birdAgents.Count(a => !a.IsDead);
            
                aliveAgentsCount.Value = aliveAgents;

                if (aliveAgents > 0) 
                    return;
            
                ResetGeneration();
                StartGeneration();
            }

            aliveAgentsCount.Value = birdAgents.Count(a => !a.IsDead);
        }
        
        public void StartGeneration()
        {
            if (_isRunning)
                return;
            
            currentGeneration.Value++;
            aliveAgentsCount.Value = birdAgents.Count;
            
            agentPreStartEvent?.RaiseEvent(this);
            startEvent?.RaiseEvent(this);
            _isRunning = true;
            _hasUpdated = false;
        }
        
        public void ResetGeneration()
        {
            if (!_isRunning)
                return;
            
            _isRunning = false;
            resetEvent?.RaiseEvent(this);
            _updateTimer = 0;
        }
    }
}