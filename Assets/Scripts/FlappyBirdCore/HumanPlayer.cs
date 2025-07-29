using UnityEngine;
using UnityEngine.InputSystem;

namespace FlappyBirdCore
{
    public class HumanPlayer : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputAction;

        [SerializeField] private Bird possessedBird;
        
        private void OnEnable()
        {
            inputAction.action.performed += _ => PerformJump();
        }

        private void PerformJump()
        {
            if(!possessedBird.HasStarted)
                possessedBird.StartBird();
            possessedBird.Jump();
        }
    }
}