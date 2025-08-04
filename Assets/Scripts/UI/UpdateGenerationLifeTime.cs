using System;
using JohaToolkit.UnityEngine.Extensions;
using JohaToolkit.UnityEngine.ScriptableObjects.Events;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UpdateGenerationLifeTime : MonoBehaviour
    {
        [SerializeField] private GameEvent agentStartEvent;
        [SerializeField] private GameEvent resetEvent;
        
        [SerializeField] private TextMeshProUGUI generationLifeTimeText;

        private bool _isRunning;

        private float _aliveTime;
        
        private void Awake()
        {
            agentStartEvent.Subscribe(_ => OnAgentStart());
            resetEvent.Subscribe(_ => OnReset());
        }

        private void OnAgentStart()
        {
            _isRunning = true;
            _aliveTime = 0;
        }

        private void OnReset()
        {
            _isRunning = false;
        }

        private void Update()
        {
            if (!_isRunning)
                return;
            
            _aliveTime += Time.deltaTime;
            generationLifeTimeText.text = _aliveTime.Seconds().ToString("hh':'mm':'ss");
        }
    }
}
