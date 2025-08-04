using JohaToolkit.UnityEngine.ScriptableObjects.Variables;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UpdateTextWithIntVariable : MonoBehaviour
    {
        [SerializeField] private IntVariable intVariable;
        [SerializeField] private TextMeshProUGUI generationText;

        private void Awake()
        {
            intVariable.OnValueChanged += OnINTVariableChanged;
        }

        private void OnINTVariableChanged(int newValue)
        {
            generationText.text = newValue.ToString();
        }
    }
}
