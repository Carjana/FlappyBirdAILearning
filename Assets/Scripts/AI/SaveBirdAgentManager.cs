using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JohaToolkit.UnityEngine.DataStructures;
using JohaToolkit.UnityEngine.SaveSystem;
using UnityEngine;

namespace AI
{
    public class SaveBirdAgentManager : MonoBehaviourSingleton<SaveBirdAgentManager>, ISaveGameUser
    {
        private void Start()
        {
            this.RegisterSaveGameUser();
        }

        public async Task LoadData(string saveName)
        {
            bool result;
            try
            {
                result = await SaveGameManager.LoadAsync(saveName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = false;
            }

            if (!result)
            {
                Debug.LogError($"Failed to load save game: {saveName}");
                return;
            }
        }

        public async Task SaveData(string saveName)
        {
            try
            {
                bool result = await SaveGameManager.SaveAsync(saveName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private SaveData GetSaveData()
        {
            (GameState[] states, float[] qValues) tableData = ConvertTableToSaveData(BirdAgentManager.Instance.QLearningManager.QTable);
            return new SaveData
            {
                Generation = BirdAgentManager.Instance.CurrentGeneration,
                GameStates = tableData.states,
                QValues = tableData.qValues
            };
        }

        private async Awaitable LoadSaveData(SaveData saveData)
        {
            await Awaitable.MainThreadAsync();
            Table<GameState, bool, float> table = GenerateTable(saveData.GameStates, saveData.QValues);
            BirdAgentManager.Instance.SetLearningData(saveData.Generation, table);
        }
        
        private Table<GameState, bool, float> GenerateTable(GameState[] gameStates, float[] qValues)
        {
            Table<GameState, bool, float> table = new();
            table.AddColumn(true, 0);
            table.AddColumn(false, 0);
            int i = 0;
            foreach (GameState gameState in gameStates)
            {
                table.AddRow(gameState, 0);
                table.SetValue(gameState, true, qValues[i]);
                table.SetValue(gameState, false, qValues[i+1]);
                i += 2;
            }

            return table;
        }
        
        private (GameState[] states, float[] qValues) ConvertTableToSaveData(Table<GameState, bool, float> table)
        {
            List<GameState> states = new();
            List<float> qValues = new();
            foreach (GameState gameState in table.Rows)
            {
                states.Add(gameState);
                table.TryGetValue(gameState, true, out float qValueTrue);
                table.TryGetValue(gameState, false, out float qValueFalse);
                
                qValues.Add(qValueTrue);
                qValues.Add(qValueFalse);
            }
            
            return (states.ToArray(), qValues.ToArray());
        }

        public void Save(SaveGame saveGame)
        {
            SaveData saveData = GetSaveData();
            if (!saveGame.SaveData.TryAdd("qTableSaveData", saveData))
            {
                saveGame.SaveData["qTableSaveData"] = saveData;
            }
        }

        public void Load(SaveGame saveGame)
        {
            if (!saveGame.SaveData.TryGetValue("qTableSaveData", out object saveData) || saveData is not SaveData qTableSaveData)
            {
                // Something went wrong, no save data found
                Debug.LogError("No save data found for 'qTableSaveData'.");
                return;
            }
            
            LoadSaveData(qTableSaveData);
        }
    }
    
    [Serializable]
    public struct SaveData
    {
        public int Generation;
        public GameState[] GameStates;
        public float[] QValues;
    }
}