using System;
using System.Linq;
using UnityEngine;

namespace AI
{
    // TODO: Export and Set Table
    public class QLearningManager
    {
        public Table<Vector2, bool, float> QTable { get; private set; }
        public float LearningRate { get; private set; }
        public float DiscountFactor { get; private set; }

        public QLearningManager(float learningRate = 0.1f, float discountFactor = 0.9f)
        {
            QTable = new Table<Vector2, bool, float>();
            SetLearningRate(learningRate);
            SetDiscountFactor(discountFactor);
        }
        
        public void SetLearningRate(float learningRate)
        {
            if (learningRate is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(learningRate), "Learning rate must be between 0 and 1.");
            LearningRate = learningRate;
        }
        
        public void SetDiscountFactor(float discountFactor)
        {
            if (discountFactor is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(discountFactor), "Discount factor must be between 0 and 1.");
            DiscountFactor = discountFactor;
        }
        
        public void UpdateQValue(Vector2 oldState, bool oldAction, Vector2 newState, float reward)
        {
            if (!QTable.TryGetValue(oldState, oldAction, out float currentQValue))
                return;

            float maxFutureQValue = QTable.GetColumns(newState).Select(kvp => kvp.Item2).Max();
            
            float newQValue = (1 - LearningRate) * currentQValue + LearningRate * (reward + DiscountFactor * maxFutureQValue);
            
            QTable.SetValue(oldState, oldAction, newQValue);
        }
        
        public (bool action, float qValue)[] GetActions(Vector2 state)
        {
            (bool, float)[] actions = QTable.GetColumns(state);
            if (actions != null && actions.Length != 0)
                return actions;
            
            QTable.AddRow(state, 0);
            return Array.Empty<(bool, float)>();
        }
    }
}