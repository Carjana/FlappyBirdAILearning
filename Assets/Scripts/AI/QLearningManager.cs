using System;
using System.Linq;
using UnityEngine;

namespace AI
{
    // TODO: Export and Set Table
    public class QLearningManager
    {
        public Table<GameState, bool, float> QTable { get; private set; }
        public float LearningRate { get; private set; }
        public float DiscountFactor { get; private set; }

        public QLearningManager(float learningRate = 0.1f, float discountFactor = 0.9f)
        {
            QTable = new Table<GameState, bool, float>();
            SetLearningRate(learningRate);
            SetDiscountFactor(discountFactor);
            QTable.AddColumn(true, 0);
            QTable.AddColumn(false, 0);
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
        
        public void UpdateQValue(GameState oldState, bool oldAction, GameState newState, float reward)
        {
            if (!QTable.TryGetValue(oldState, oldAction, out float currentQValue))
                return;

            if (reward < 0)
            {
                int a = 1;
            }
            
            float maxFutureQValue = GetColumnsOrdAddRow(newState).Select(kvp => kvp.Item2).Max();
            
            float newQValue = (1 - LearningRate) * currentQValue + LearningRate * (reward + DiscountFactor * maxFutureQValue);
            
            QTable.SetValue(oldState, oldAction, newQValue);
        }

        private (bool, float)[] GetColumnsOrdAddRow(GameState row)
        {
            (bool, float)[] cols = QTable.GetColumns(row);
            if (cols.Length > 0)
                return cols;
            QTable.AddRow(row, 0);
            return QTable.GetColumns(row);
        }
        
        public (bool action, float qValue)[] GetActions(GameState state)
        {
            (bool, float)[] actions = QTable.GetColumns(state);
            if (actions != null && actions.Length != 0)
                return actions;
            
            QTable.AddRow(state, 0);
            return Array.Empty<(bool, float)>();
        }
    }
}