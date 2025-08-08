using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public abstract class ActionChoosingStrategy
    {
        public abstract bool ChooseUnInformedAction();
        public abstract bool ChooseInformedAction((bool action, float qValue)[] actions);
    }
    
    [Serializable]
    public class LearningStrategy : ActionChoosingStrategy
    {
        [SerializeField] private float jumpProbability = 0.1f;
        [SerializeField] private float pickerBaseValue = 1f;
        
        public override bool ChooseUnInformedAction()
        {
            return false;
            return Random.Range(0f, 1f) < jumpProbability;
        }

        public override bool ChooseInformedAction((bool action, float qValue)[] actions)
        {
            WeightedPicker<bool> picker = new();
            foreach ((bool action, float qValue) in actions)
            {
                picker.Add(action, Mathf.Max(qValue + pickerBaseValue, 0));
            }
                
            return picker.Pick();
        }
    }
    
    [Serializable]
    public class BestStrategy : ActionChoosingStrategy
    {
        [SerializeField] private float jumpProbability = 0.1f;
        
        public override bool ChooseUnInformedAction()
        {
            return false;
            return Random.Range(0f, 1f) < jumpProbability;
        }

        public override bool ChooseInformedAction((bool action, float qValue)[] actions)
        {
            bool bestAction = false;
            float bestQValue = float.MinValue;
            foreach ((bool action, float qValue) in actions)
            {
                if (qValue < bestQValue)
                    continue;
                bestAction = action;
                bestQValue = qValue;
            }
            return bestAction;
        }
    }
}