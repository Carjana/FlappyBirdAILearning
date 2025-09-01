using System;
using System.Linq;
using UnityEngine;

namespace AI
{
    // TODO: Export and Set Table
    public class QLearningManager<TGameState, TAction>
    {
        public Table<TGameState, TAction, float> QTable { get; private set; }
        public float LearningRate { get; private set; }
        public float DiscountFactor { get; private set; }

        public QLearningManager(TAction[] tActions, float learningRate = 0.1f, float discountFactor = 0.9f)
        {
            QTable = new Table<TGameState, TAction, float>();
            SetLearningRate(learningRate);
            SetDiscountFactor(discountFactor);
            foreach (TAction tAction in tActions)
            {
                QTable.AddColumn(tAction, 0);
            }
        }
        
        public void SetQTable(Table<TGameState, TAction, float> qTable)
        {
            QTable = qTable;
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
        
        public void UpdateQValue(TGameState oldState, TAction oldAction, TGameState newState, float reward)
        {
            if (!QTable.TryGetValue(oldState, oldAction, out float currentQValue))
                return;
            
            float maxFutureQValue = GetColumnsOrdAddRow(newState).Select(kvp => kvp.Item2).Max();
            
            float newQValue = (1 - LearningRate) * currentQValue + LearningRate * (reward + DiscountFactor * maxFutureQValue);
            
            QTable.SetValue(oldState, oldAction, newQValue);
        }

        private (TAction, float)[] GetColumnsOrdAddRow(TGameState row)
        {
            (TAction, float)[] cols = QTable.GetColumns(row);
            if (cols.Length > 0)
                return cols;
            QTable.AddRow(row, 0);
            return QTable.GetColumns(row);
        }
        
        public (TAction action, float qValue)[] GetActions(TGameState state)
        {
            (TAction, float)[] actions = QTable.GetColumns(state);
            if (actions != null && actions.Length != 0)
                return actions;
            
            QTable.AddRow(state, 0);
            return Array.Empty<(TAction, float)>();
        }
    }
}