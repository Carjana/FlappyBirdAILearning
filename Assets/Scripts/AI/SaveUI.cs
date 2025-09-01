using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI
{
    public class SaveUI : MonoBehaviour
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] TMP_InputField saveNameInputField;
        [SerializeField] TMP_InputField loadNameInputField;

        private bool _isLoading;
        private bool _isSaving;
        
        private void Awake()
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
            loadButton.onClick.AddListener(OnLoadButtonClicked);
        }

        private void OnSaveButtonClicked()
        {
            if (_isSaving || _isLoading)
                return;
            
            _isSaving = true;
            _ = SaveAsync();
        }

        private void OnLoadButtonClicked()
        {
            if(_isSaving || _isLoading)
                return;
            
            _isLoading = true;
            _ = LoadAsync();
        }


        private async Task SaveAsync()
        {
            string saveName = saveNameInputField.text;
            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogWarning("Save name cannot be empty.");
                _isSaving = false;
                return;
            }
            
            // Implement saving logic here
            Debug.Log("Saving...");
            await SaveBirdAgentManager.Instance.SaveData(saveName);
            Debug.Log("Saving Completed!");
            _isSaving = false;
        }

        private async Task LoadAsync()
        {
            string loadName = loadNameInputField.text;
            if (string.IsNullOrEmpty(loadName))
            {
                Debug.LogWarning("Load name cannot be empty.");
                _isLoading = false;
                return;
            }
            
            // Implement saving logic here
            Debug.Log("Loading...");
            await SaveBirdAgentManager.Instance.LoadData(loadName);
            Debug.Log("Loading Completed!");
            _isLoading = false;
        }
    }
}