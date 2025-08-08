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
        [SerializeField] private float mutateProbability = 0.001f;
        
        public override bool ChooseUnInformedAction()
        {
            return false;
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

            if (Random.Range(0f, 1f) < mutateProbability)
                return !bestAction;
            return bestAction;
        }
    }
    
    [Serializable]
    public class BestStrategy : ActionChoosingStrategy
    {
        [SerializeField] private float jumpProbability = 0.1f;
        
        public override bool ChooseUnInformedAction()
        {
            return false;
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